using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Entities;

public interface IClassScheduleStatusJob
{
    Task CompletePastSchedulesAsync(CancellationToken ct);
}

public sealed class ClassScheduleStatusJob : IClassScheduleStatusJob
{
    private readonly IRegalEducationDbContext _db;
    private readonly ILogger<ClassScheduleStatusJob> _log;
    private readonly IStringLocalizer<ClassScheduleStatusJob> _l10n;
    private readonly IClassScheduleUsableAmountJob _usableAmountJob;// Hải bổ sung

    public ClassScheduleStatusJob(
        IRegalEducationDbContext db,
        ILogger<ClassScheduleStatusJob> log,
        IStringLocalizer<ClassScheduleStatusJob> l10n,
        IClassScheduleUsableAmountJob usableAmountJob)// Hải bổ sung
    {
        _db = db;
        _log = log;
        _l10n = l10n;
        _usableAmountJob = usableAmountJob;//Hải bổ sung
    }

    /// <summary>
    /// Tự động chuyển các buổi học của ngày hôm qua trở về trước từ NotStarted sang Completed.
    /// Ngay sau khi hoàn thành cập nhật trạng thái, nó gọi job tính học phí usable amount
    /// </summary>
    public async Task CompletePastSchedulesAsync(CancellationToken ct)
    {
        // 1. Lấy thời gian hiện tại theo múi giờ VN (SE Asia Standard Time: UTC+7)
        var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        // nowLocal.Date chỉ lấy ngày hiện tại (n)
        var currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date;

        _log.LogInformation(_l10n["JobCompleteStart", currentDate.ToString("yyyy-MM-dd")]);

        using var tx = await _db.BeginTransactionAsync(ct);

        // 2. Truy vấn các buổi học cần được hoàn thành (NotStarted -> Completed)
        // Điều kiện:
        // - Trạng thái là NotStarted
        // - Ngày của buổi học phải là ngày hôm qua trở về trước (n-1 trở về trước)
        // - Buổi học không bị xóa
        var schedulesToComplete = await _db.ClassSchedule
            .Where(cs => cs.ClassScheduleStatus == ClassScheduleStatus.NotStarted
                        && cs.Date.Date < currentDate // Ngày học < Ngày hiện tại
                        && !cs.IsDeleted)
            .ToListAsync(ct);

        int countCompleted = 0;

        // 3. Thực hiện chuyển trạng thái
        foreach (var schedule in schedulesToComplete)
        {
            // Chuyển trạng thái sang Đã hoàn thành (Completed = 1)
            schedule.ClassScheduleStatus = ClassScheduleStatus.Completed;
            countCompleted++;
        }

        // 4. Lưu thay đổi và Commit Transaction
        if (countCompleted > 0)
        {
            await _db.SaveChangesAsync(ct);
        }

        await tx.CommitAsync(ct);

        _log.LogInformation(_l10n["JobCompleteDone",
            countCompleted, currentDate.ToString("yyyy-MM-dd")]);
        
        // 5. Bổ sung Kích hoạt job tính UsableAmount ngay sau khi hoàn thành (trong cùng process)
        try
        {
            _log.LogInformation(_l10n["JobTriggerUsableStart"]);
            // Thực thi trực tiếp; job tính tiền tự chịu transaction riêng
            await _usableAmountJob.CalculateUsableAmountAsync(ct);
            _log.LogInformation(_l10n["JobTriggerUsableDone"]);
        }
        catch (Exception ex)
        {
            // Ghi log nhưng không ném lỗi ra để tránh ảnh hưởng job chính
            _log.LogError(ex, _l10n["JobTriggerUsableFailed", ex.Message]);           
        }
    }
}