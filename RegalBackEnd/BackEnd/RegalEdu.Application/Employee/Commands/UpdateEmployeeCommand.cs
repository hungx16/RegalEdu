using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Shared;

namespace RegalEdu.Application.Employee.Commands
{
    public class UpdateEmployeeCommand : IRequest<Result>
    {
        public required EmployeeModel EmployeeModel { get; set; }
    }
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateEmployeeCommandHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy Employee từ DB
            var employeeEntity = await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == request.EmployeeModel.Id, cancellationToken);

            if (employeeEntity == null)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, "Employee"));
            }
            // 2. Lấy ApplicationUser từ DB qua ApplicationUserId (đã có trong Employee)
            var applicationUserEntity = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == employeeEntity.ApplicationUserId, cancellationToken);

            if (applicationUserEntity == null)
            {
                return Result.Failure(_localizer.Format(LocalizationKey.EntityNotFound, "ApplicationUser"));
            }

            // Capture original position/company to detect role changes.
            var oldPositionId = employeeEntity.PositionId;
            var oldCompanyId = employeeEntity.CompanyId;
            var newPositionId = request.EmployeeModel.PositionId;

            _mapper.Map(request.EmployeeModel.ApplicationUser, applicationUserEntity);

            _mapper.Map(request.EmployeeModel, employeeEntity);

            // Only recalc when position changes.
            if (oldPositionId != newPositionId)
            {
                // Determine sale flags for old vs new position.
                var oldPosFlags = await _context.Positions
                    .Where(p => p.Id == oldPositionId)
                    .Select(p => new { p.IsSale, p.IsSaleLead })
                    .FirstOrDefaultAsync(cancellationToken);
                var newPosFlags = await _context.Positions
                    .Where(p => p.Id == newPositionId)
                    .Select(p => new { p.IsSale, p.IsSaleLead })
                    .FirstOrDefaultAsync(cancellationToken);

                var wasSale = (oldPosFlags?.IsSale == true) || (oldPosFlags?.IsSaleLead == true);
                var isSaleNow = (newPosFlags?.IsSale == true) || (newPosFlags?.IsSaleLead == true);

                if (wasSale && !isSaleNow)
                {
                    // Managers keep managerial quota rules; skip sales recalculation.
                    var isManager = await _context.Companies.AnyAsync(c => c.ManagerId == employeeEntity.Id, cancellationToken)
                        || await _context.Regions.AnyAsync(r => r.ManagerId == employeeEntity.Id, cancellationToken);

                    if (!isManager)
                    {
                        // Recalculate within the current allocation month.
                        var now = DateTime.Now.Date;
                        var y = now.Year;
                        var m = now.Month;

                        // Load allocation company + region + employees for this month.
                        var aqCompany = await _context.AdmissionsQuotaCompanies
                            .Include(c => c.AdmissionsQuotaRegion)
                                .ThenInclude(r => r.AdmissionsQuota)
                            .Include(c => c.AdmissionsQuotaEmployees)
                            .FirstOrDefaultAsync(c =>
                                   c.CompanyId == oldCompanyId
                                && c.AdmissionsQuotaRegion != null
                                && c.AdmissionsQuotaRegion.AdmissionsQuota != null
                                && c.AdmissionsQuotaRegion.AdmissionsQuota.Year == y
                                && c.AdmissionsQuotaRegion.AdmissionsQuota.Month == m
                                && !c.IsDeleted, cancellationToken);

                        if (aqCompany != null)
                        {
                            // Find the quota entry for this employee (Sale/SalesLead).
                            var aqEmployee = aqCompany.AdmissionsQuotaEmployees?
                                .FirstOrDefault(e =>
                                    e.EmployeeId == employeeEntity.Id
                                    && (e.QuotaRole == QuotaRole.Sale
                                        || e.QuotaRole == QuotaRole.SalesLead
                                        || e.QuotaRole == QuotaRole.ProbationEmployee
                                        || e.QuotaRole == QuotaRole.LeavingEmployee));

                            if (aqEmployee != null)
                            {
                                // Build working range: month start (or join date) -> today.
                                var monthStart = new DateTime(y, m, 1);
                                var monthEnd = Functions.LastDayOfMonth(y, m);
                                var joinDate = employeeEntity.EmployeeStartedDate?.Date;

                                var startDate = monthStart;
                                if (joinDate.HasValue && Functions.IsWithinMonth(joinDate.Value, y, m))
                                    startDate = joinDate.Value;

                                var endDate = now > monthEnd ? monthEnd : now;
                                if (endDate < startDate)
                                    endDate = startDate;

                                var dStd = Functions.GetStandardWorkingDaysInMonth(y, m);
                                if (dStd > 0)
                                {
                                    // Apply probation rules if still within 26-day threshold.
                                    var dWork = Functions.CalculateDWork(startDate, endDate);
                                    var basePerSale = aqCompany.RevenuePerSale;

                                    decimal newQuota;
                                    if (joinDate.HasValue)
                                    {
                                        var dProb = Functions.CalculateDProb(joinDate.Value, endDate);
                                        if (dProb < 26)
                                            newQuota = Functions.CalculateProbationQuota(basePerSale, dStd, dWork, dProb);
                                        else
                                            newQuota = Functions.CalculateSalesQuota(basePerSale, dStd, dWork);
                                    }
                                    else
                                    {
                                        newQuota = Functions.CalculateSalesQuota(basePerSale, dStd, dWork);
                                    }

                                    newQuota = Math.Round(newQuota, 0, MidpointRounding.AwayFromZero);
                                    var oldQuota = aqEmployee.RevenuePerSale ?? 0m;
                                    var delta = newQuota - oldQuota;
                                    if (delta != 0m)
                                    {
                                        // Apply delta to employee and roll up to company/region/header totals.
                                        aqEmployee.RevenuePerSale = newQuota;
                                        aqCompany.TotalRevenue = aqCompany.TotalRevenue + delta;
                                        if (aqCompany.AdmissionsQuotaRegion != null)
                                        {
                                            aqCompany.AdmissionsQuotaRegion.TotalRevenue =
                                                aqCompany.AdmissionsQuotaRegion.TotalRevenue + delta;
                                            if (aqCompany.AdmissionsQuotaRegion.AdmissionsQuota != null)
                                            {
                                                aqCompany.AdmissionsQuotaRegion.AdmissionsQuota.TotalQuota =
                                                    aqCompany.AdmissionsQuotaRegion.AdmissionsQuota.TotalQuota + delta;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            var success = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (success)
                return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, _localizer["Employee"]));
            else
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Employee"]));
        }
    }
}
