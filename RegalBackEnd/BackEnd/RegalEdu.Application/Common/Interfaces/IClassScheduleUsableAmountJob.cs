using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Entities;

public interface IClassScheduleUsableAmountJob
{
    /// <summary>
    /// Tính lại UsableAmount sau khi các buổi học đã Completed 
    /// và học viên được điểm danh (trừ tiền và hoàn tiền)
    /// </summary>
    Task CalculateUsableAmountAsync(CancellationToken ct);
}

public sealed class ClassScheduleUsableAmountJob : IClassScheduleUsableAmountJob
{
    private readonly IRegalEducationDbContext _db;
    private readonly ILogger<ClassScheduleUsableAmountJob> _log;
    private readonly IStringLocalizer<ClassScheduleUsableAmountJob> _l10n;

    public ClassScheduleUsableAmountJob(
        IRegalEducationDbContext db,
        ILogger<ClassScheduleUsableAmountJob> log,
        IStringLocalizer<ClassScheduleUsableAmountJob> l10n)
    {
        _db = db;
        _log = log;
        _l10n = l10n;
    }

    /// <summary>
    /// Tính toán và cập nhật UsableAmount cho học viên.
    /// Xử lý cả logic Trừ tiền (Present) và Hoàn tiền (Non-Present sau khi đã trừ).
    /// </summary>
    public async Task CalculateUsableAmountAsync(CancellationToken ct)
    {
        _log.LogInformation(_l10n["JobUsableStart"]);

        // Sử dụng Transaction để đảm bảo tính nguyên tố cho việc trừ/hoàn tiền và đánh dấu.
        using var tx = await _db.BeginTransactionAsync(ct);

        // 1. Tải TẤT CẢ các bản ghi điểm danh thuộc buổi học ĐÃ HOÀN THÀNH (Completed)
        var attendancesToReview = await _db.ClassAttendent
            .Include(a => a.ClassSchedule)
                .ThenInclude(s => s.Class)
                    .ThenInclude(c => c.ClassType) // Cần ClassType để tính toán giờ
            .Where(a =>
                a.ClassSchedule.ClassScheduleStatus == ClassScheduleStatus.Completed &&
                !a.IsDeleted)
            .ToListAsync(ct);

        if (!attendancesToReview.Any())
        {
            _log.LogInformation(_l10n["JobUsableNoData"]);
            return;
        }

        int countUpdated = 0;
        int countRefunded = 0;

        // Caching: Giảm thiểu truy vấn DB trong vòng lặp
        var enrollmentsMap = new Dictionary<Guid, Enrollment>();
        var tuitionsMap = new Dictionary<(Guid CourseId, Guid ClassTypeId), Tuition>();


        foreach (var att in attendancesToReview)
        {
            var schedule = att.ClassSchedule;
            var classEntity = schedule?.Class;
            var classType = classEntity?.ClassType;

            // Bỏ qua nếu dữ liệu Entity không đầy đủ
            if (classEntity == null || classType == null)
                continue;

            // 2. Tìm hoặc tải Enrollment (Dựa trên StudentId và ClassId)
            if (!enrollmentsMap.TryGetValue(att.StudentId, out var enrollment))
            {
                enrollment = await _db.Enrollments
                    .FirstOrDefaultAsync(e =>
                        e.StudentId == att.StudentId &&
                        e.ClassId == classEntity.Id &&
                        !e.IsDeleted,
                        ct);

                if (enrollment != null)
                    enrollmentsMap[att.StudentId] = enrollment;
            }

            if (enrollment == null || enrollment.FinalFee == null || enrollment.CourseId == null)
                continue;

            // 3. Tìm hoặc tải Tuition (Dựa trên CourseId và ClassTypeId)
            var tuitionKey = (enrollment.CourseId.Value, classType.Id);
            if (!tuitionsMap.TryGetValue(tuitionKey, out var tuition))
            {
                tuition = await _db.Tuition
                    .FirstOrDefaultAsync(t =>
                        t.CourseId == enrollment.CourseId &&
                        t.ClassTypeId == classType.Id &&
                        !t.IsDeleted,
                        ct);
                if (tuition != null)
                    tuitionsMap[tuitionKey] = tuition;
            }

            if (tuition == null)
                continue;

            // 4. Tính toán Học phí mỗi buổi (CostPerSession)
            int totalSessions = (int)Math.Ceiling(
                tuition.DurationHours / (decimal)classType.HoursPerSession
            );

            if (totalSessions <= 0) continue;

            decimal costPerSession = (decimal)enrollment.FinalFee.Value / totalSessions; // Sử dụng .Value vì đã kiểm tra FinalFee != null

            // ----------------------------------------------------
            // ⭐️ LOGIC TRỪ TIỀN VÀ HOÀN TIỀN
            // ----------------------------------------------------

            // Xác định xem trạng thái hiện tại CÓ PHẢI là trạng thái bị trừ tiền hay không
            bool isPresentOrDeductible = att.StudentParticipationStatus == StudentParticipationStatus.Present;

            if (isPresentOrDeductible)
            {
                // TRƯỜNG HỢP 1: CẦN TRỪ TIỀN (Chỉ trừ nếu chưa tính)
                if (att.IsTuitionCalculated == false)
                {
                    enrollment.UsableAmount -= (double)costPerSession;
                    att.IsTuitionCalculated = true;
                    countUpdated++;
                }
            }
            else
            {
                // TRƯỜNG HỢP 2: CẦN HOÀN TIỀN (Nếu trạng thái không bị trừ tiền VÀ đã từng trừ)
                if (att.IsTuitionCalculated == true)
                {
                    enrollment.UsableAmount += (double)costPerSession;
                    att.IsTuitionCalculated = false;
                    countRefunded++;
                }
            }
        }

        // 7. Lưu tất cả thay đổi (cả Enrollment và ClassAttendent) trong Transaction
        if (countUpdated > 0 || countRefunded > 0)
        {
            await _db.SaveChangesAsync(ct);
        }

        await tx.CommitAsync(ct);

        _log.LogInformation(_l10n["JobUsableDone", countUpdated, countRefunded]);
    }
}