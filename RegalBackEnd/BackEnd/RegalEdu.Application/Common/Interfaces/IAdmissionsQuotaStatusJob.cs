using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

public interface IAdmissionsQuotaStatusJob
{
    Task RollMonthlyStatusesAsync(CancellationToken ct);
}

public sealed class AdmissionsQuotaStatusJob : IAdmissionsQuotaStatusJob
{
    private readonly IRegalEducationDbContext _db;
    private readonly ILogger<AdmissionsQuotaStatusJob> _log;
    private readonly IStringLocalizer<AdmissionsQuotaStatusJob> _l10n;

    public AdmissionsQuotaStatusJob(
        IRegalEducationDbContext db,
        ILogger<AdmissionsQuotaStatusJob> log,
        IStringLocalizer<AdmissionsQuotaStatusJob> l10n)
    {
        _db = db;
        _log = log;
        _l10n = l10n;
    }

    public async Task RollMonthlyStatusesAsync(CancellationToken ct)
    {
        // Lấy thời gian theo VN
        var tz = TimeZoneInfo.FindSystemTimeZoneById ("SE Asia Standard Time");
        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc (DateTime.UtcNow, tz);
        var y = nowLocal.Year;
        var m = nowLocal.Month;

        var prev = nowLocal.AddMonths (-1);
        var py = prev.Year;
        var pm = prev.Month;

        _log.LogInformation (_l10n["AdmissionsQuotaStatusJob", nowLocal.ToString ("yyyy-MM-dd HH:mm:ss")]); // vi/en trong resx

        using var tx = await _db.BeginTransactionAsync (ct);

        // 1) Tháng hiện tại: Allocated -> InProgress
        var currentUpdated = await _db.AdmissionsQuotas
            .Where (q => q.Year == y && q.Month == m && q.QuotaStage == QuotaStatus.Allocated).ToListAsync (ct);
        foreach (var quota in currentUpdated)
            quota.QuotaStage = QuotaStatus.InProgress;


        // 2) Tháng trước: InProgress -> Completed
        var prevCompleted = await _db.AdmissionsQuotas
            .Where (q => q.Year == py && q.Month == pm && q.QuotaStage == QuotaStatus.InProgress).ToListAsync (ct);
        foreach (var quota in prevCompleted)
            quota.QuotaStage = QuotaStatus.Completed;
        await _db.SaveChangesAsync (ct);
        await tx.CommitAsync (ct);

        _log.LogInformation (_l10n["AdmissionsQuotaStatusJob",
            currentUpdated, prevCompleted, y, m, py, pm]);
    }
}
