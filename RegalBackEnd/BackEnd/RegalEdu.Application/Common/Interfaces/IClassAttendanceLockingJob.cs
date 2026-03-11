using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces; // Chứa IRegalEducationDbContext
using RegalEdu.Domain.Enums; // Chứa SessionAttendanceLockStatus

/// <summary>
/// Định nghĩa Job tự động khóa trạng thái điểm danh cho các buổi học đã qua.
/// </summary>
public interface IClassAttendanceLockingJob
{   
    Task LockPastScheduleAttendanceAsync(CancellationToken ct);
}

// ----------------------------------------------------------------------
// I M P L E M E N T A T I O N
// ----------------------------------------------------------------------

public sealed class ClassAttendanceLockingJob : IClassAttendanceLockingJob
{
    private readonly IRegalEducationDbContext _db;
    private readonly ILogger<ClassAttendanceLockingJob> _log;
    private readonly IStringLocalizer<ClassAttendanceLockingJob> _l10n;

    public ClassAttendanceLockingJob(
        IRegalEducationDbContext db,
        ILogger<ClassAttendanceLockingJob> log,
        IStringLocalizer<ClassAttendanceLockingJob> l10n)
    {
        _db = db;
        _log = log;
        _l10n = l10n;
    }
    public async Task LockPastScheduleAttendanceAsync(CancellationToken ct)
    {
        // 1. Lấy thời gian hiện tại theo múi giờ VN (SE Asia Standard Time: UTC+7)
        var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        // currentDate chỉ lấy ngày (yyyy-MM-dd)
        var currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date;

        _log.LogInformation(_l10n["JobLockStart", currentDate.ToString("yyyy-MM-dd")]);

        // Bắt đầu Transaction để đảm bảo tính toàn vẹn dữ liệu
        using var tx = await _db.BeginTransactionAsync(ct);

        // 2. Truy vấn các buổi học cần được khóa điểm danh
        // Điều kiện:
        // - Ngày của buổi học phải là ngày hôm qua trở về trước (cs.Date.Date < currentDate)
        // - Trạng thái khóa điểm danh hiện tại là Unlocked
        // - Buổi học không bị xóa
        var schedulesToLock = await _db.ClassSchedule
            .Where(cs => cs.SessionAttendanceLockStatus == SessionAttendanceLockStatus.Unlocked
                         && cs.Date.Date < currentDate
                         && !cs.IsDeleted)
            .ToListAsync(ct);
        int countLocked = 0;
        // 3. Thực hiện khóa trạng thái điểm danh
        foreach (var schedule in schedulesToLock)
        {
            // Chuyển trạng thái khóa điểm danh sang Khóa (Locked)
            schedule.SessionAttendanceLockStatus = SessionAttendanceLockStatus.Locked;
            countLocked++;
        }

        // 4. Lưu thay đổi và Commit Transaction
        if (countLocked > 0)
        {
            await _db.SaveChangesAsync(ct);
        }

        await tx.CommitAsync(ct);

        _log.LogInformation(_l10n["JobLockDone",
            countLocked, currentDate.ToString("yyyy-MM-dd")]);
    }
}