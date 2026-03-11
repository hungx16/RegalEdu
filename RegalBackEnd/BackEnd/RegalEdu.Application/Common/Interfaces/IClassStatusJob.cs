using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enums;

public interface IClassStatusJob
{
    Task RollDailyStatusesAsync(CancellationToken ct);
}

public sealed class ClassStatusJob : IClassStatusJob
{
    private readonly IRegalEducationDbContext _db;
    private readonly ILogger<ClassStatusJob> _log;
    private readonly IStringLocalizer<ClassStatusJob> _l10n;

    public ClassStatusJob(
        IRegalEducationDbContext db,
        ILogger<ClassStatusJob> log,
        IStringLocalizer<ClassStatusJob> l10n)
    {
        _db = db;
        _log = log;
        _l10n = l10n;
    }

    public async Task RollDailyStatusesAsync(CancellationToken ct)
    {
        // Lấy thời gian hiện tại theo múi giờ VN
        var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date;

        _log.LogInformation(_l10n["JobStart", nowLocal.ToString("yyyy-MM-dd")]);

        using var tx = await _db.BeginTransactionAsync(ct);

        // 1️⃣ Plan -> InProgress
        var planClasses = await _db.Classes
            .Include(c => c.ClassType) // để lấy MinStudents
            .Where(c => c.ClassStatus == ClassStatus.Plan
                        && c.StartDate.Date == nowLocal
                        && c.TeacherId != null
                        && !c.IsDeleted)
            .ToListAsync(ct);

        foreach (var c in planClasses)
        {
            // Đếm số học sinh thực tế từ bảng Enrollments
            var studentCount = await _db.Enrollments
                .Where(e => e.ClassId == c.Id
                         && e.StudentCourseStatus == StudentCourseStatus.InProgress)
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync(ct);

            if (studentCount >= c.ClassType.MinStudents
                && c.TeacherId != null
                && c.StartDate.Date == nowLocal)
            {
                c.ClassStatus = ClassStatus.InProgress;
            }

        }

        // 2️⃣ InProgress -> Completed
        var inProgressClasses = await _db.Classes
            .Where(c => c.ClassStatus == ClassStatus.InProgress
                        && c.EndDate.HasValue
                        && c.EndDate.Value.Date < nowLocal// Ngày kết thúc nhỏ hơn ngày hiện tại
                        && !c.IsDeleted)
            .ToListAsync(ct);

        foreach (var c in inProgressClasses)
        {
            c.ClassStatus = ClassStatus.Completed;
            //Bổ sung chuyển trạng thái học viên đang học thành hoàn thành ở Enrrollment
            var enrollments = await _db.Enrollments
                .Where(e => e.ClassId == c.Id
                    && e.StudentCourseStatus == StudentCourseStatus.InProgress)
                .ToListAsync(ct);

            foreach (var e in enrollments)
            {
                e.StudentCourseStatus = StudentCourseStatus.Graduated;
                //e.EndDate = nowLocal; // Nếu bạn muốn cập nhật ngày hoàn thành
            }
        }

        // Lưu thay đổi
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        _log.LogInformation(_l10n["JobDone",
            planClasses.Count, inProgressClasses.Count, nowLocal.ToString("yyyy-MM-dd")]);
    }
}
