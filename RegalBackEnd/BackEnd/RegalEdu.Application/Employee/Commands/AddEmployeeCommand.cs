using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Request;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Shared;

namespace RegalEdu.Application.Employee.Commands
{
    public class AddEmployeeCommand : IRequest<Result>
    {
        public required EmployeeModel EmployeeModel { get; set; }
    }
    public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        public AddEmployeeCommandHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer,
            IIdentityService identityService,
            IEmailService emailService,
            IEmailTemplateService templateService)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            _localizer = localizer ?? throw new ArgumentNullException (nameof (localizer));
            _identityService = identityService ?? throw new ArgumentNullException (nameof (identityService));
            _emailService = emailService ?? throw new ArgumentNullException (nameof (emailService));
            _templateService = templateService ?? throw new ArgumentNullException (nameof (templateService));
        }

        public async Task<Result> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.EmployeeModel?.ApplicationUser == null)
            {
                return Result.Failure (_localizer.Format (LocalizationKey.ActionException, _localizer["Create"], _localizer["Employee"], "ApplicationUser is null."));
            }
            var newPassword = Functions.GenerateSecurePassword ( );
            var info = AutoCodeConfig.Get (AutoCodeType.Employee);

            // 1. Map và thêm ApplicationUser
            var appUserEntity = _mapper.Map<ApplicationUser> (request.EmployeeModel.ApplicationUser);

            // 2. Sử dụng AutoCodeHelper để sinh UserCode và đảm bảo không trùng
            Result userResult = await AutoCodeHelper.CreateWithAutoCodeRetryAsync<Result> (
                info,
                async (code) =>
                {
                    appUserEntity.UserCode = code;

                    // Sinh mới Guid cho ApplicationUser.Id, đảm bảo không trùng (hiếm gặp)
                    do
                    {
                        appUserEntity.Id = Guid.NewGuid ( );
                    }
                    while (await _context.ApplicationUsers.AnyAsync (x => x.Id == appUserEntity.Id, cancellationToken));


                    // Dùng IdentityService để tạo user (đúng chuẩn Identity)
                    Result result = await _identityService.CreateUserAsync (appUserEntity, newPassword);
                    return result;
                },
                (DbContext)_context // Ép kiểu sang DbContext nếu cần cho AutoCodeHelper
            );

            if (!userResult.Succeeded)
            {
                return Result.Failure (_localizer.Format (LocalizationKey.ActionException, _localizer["Create"], _localizer["Employee"], userResult.Errors));
            }

            appUserEntity = await _context.ApplicationUsers.Where (t => t.Email == appUserEntity.Email).FirstOrDefaultAsync ( );
            if (appUserEntity == null)
            {
                return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Employee"]));
            }
            var employeeEntity = _mapper.Map<Domain.Entities.Employee> (request.EmployeeModel);
            employeeEntity.ApplicationUserId = appUserEntity.Id;
            employeeEntity.ApplicationUser = appUserEntity;
            await _context.Employees.AddAsync (employeeEntity, cancellationToken);

            var success = await _context.SaveChangesAsync (cancellationToken) > 0;
            if (success)
            {
                // ===== NEW: Auto-assign quota for new Sales =====
                try
                {
                    await AssignNewSalesQuotaIfNeededAsync (employeeEntity, cancellationToken);
                    await _context.SaveChangesAsync (cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException (_localizer.Format (LocalizationKey.ERR_SEND_EMAIL, _localizer["Employee"], Functions.GetFullExceptionMessage (ex)));
                }
                var emailModel = new UserModelRequest
                {
                    FullName = employeeEntity.ApplicationUser.FullName,
                    Email = employeeEntity.ApplicationUser.Email,
                    Password = newPassword // Mật khẩu mới được sinh ngẫu nhiên
                };
                try
                {
                    var body = await _templateService.RenderTemplateAsync ("CreateNewEmployee", emailModel);

                    await _emailService.SendEmailAsync (employeeEntity.ApplicationUser.Email, _localizer[LocalizationKey.CreateNewEmployee], body);
                }
                catch (Exception ex)
                {
                    // Log lỗi gửi email nếu cần
                    Console.WriteLine ($"Error sending email: {Functions.GetFullExceptionMessage (ex)}");
                    throw new ApplicationException (_localizer.Format (LocalizationKey.ERR_SEND_EMAIL, _localizer["Employee"], Functions.GetFullExceptionMessage (ex)));
                }
                return Result.Success (_localizer.Format (LocalizationKey.MSG_CREATE_SUCCESS, _localizer["Employee"]));
            }
            else
                return Result.Failure (_localizer.Format (LocalizationKey.ERR_SAVE_NO_EFFECT, _localizer["Employee"]));
        }

        private async Task AssignNewSalesQuotaIfNeededAsync(Domain.Entities.Employee emp, CancellationToken ct)
        {

            var isSale = false;
            isSale = (bool)await _context.Positions
                .Where (p => p.Id == emp.PositionId)
                .Select (p => p.IsSale)
                .FirstOrDefaultAsync (ct);
            if (!isSale) return;

            // Cần có Company và StartedDate
            if (emp.EmployeeStartedDate == null)
                return;

            var joinDate = emp.EmployeeStartedDate.Value.Date;
            var probationEnd = emp.EmployeeNewEndDate?.Date;

            // Tháng/năm phân bổ = theo Ngày bắt đầu (yêu cầu của anh: "của tháng đó")
            var y = joinDate.Year;
            var m = joinDate.Month;

            // Tìm AQC của chi nhánh trong tháng/năm đó
            var aqCompany = await _context.AdmissionsQuotaCompanies
                .Include (c => c.AdmissionsQuotaRegion)
                    .ThenInclude (r => r.AdmissionsQuota)
                .Include (c => c.AdmissionsQuotaEmployees)
                .FirstOrDefaultAsync (c =>
                       c.CompanyId == emp.CompanyId
                    && c.AdmissionsQuotaRegion != null
                    && c.AdmissionsQuotaRegion.AdmissionsQuota != null
                    && c.AdmissionsQuotaRegion.AdmissionsQuota.Year == y
                    && c.AdmissionsQuotaRegion.AdmissionsQuota.Month == m
                    && !c.IsDeleted, ct);

            if (aqCompany == null)
            {
                // Không có AQC cho tháng/năm đó -> bỏ qua nhẹ nhàng
                return;
            }

            var basePerSale = aqCompany.RevenuePerSale; // decimal(18,2)
            if (basePerSale <= 0) return;

            // Tính công chuẩn & số ngày làm việc (bỏ Chủ nhật)
            var stdDays = Functions.CountWorkingDaysInMonth (y, m);
            if (stdDays == 0) return;

            var monthStart = new DateTime (y, m, 1);
            var monthEnd = Functions.LastDayOfMonth (y, m);

            // Xác định TH1 / TH2
            decimal quota = 0m;
            var startInMonth = Functions.IsWithinMonth (joinDate, y, m);
            var probationEndInMonth = probationEnd.HasValue && Functions.IsWithinMonth (probationEnd.Value, y, m);
            var leavingInMonth = emp.EmployeeEndDate.HasValue && Functions.IsWithinMonth (emp.EmployeeEndDate.Value, y, m);
            var quotaRole = probationEnd.HasValue ? QuotaRole.ProbationEmployee : QuotaRole.Sale;
            if (leavingInMonth)
                quotaRole = QuotaRole.LeavingEmployee;
            if (startInMonth && !probationEndInMonth)
            {
                // TH1: chỉ có ngày bắt đầu thuộc tháng
                var daysProb = Functions.CountWorkingDaysBetween (joinDate, monthEnd);
                quota = Math.Round (0.6m * basePerSale * (daysProb / (decimal)stdDays), 0, MidpointRounding.AwayFromZero);

            }
            else if (startInMonth && probationEndInMonth)
            {
                // TH2: cả bắt đầu và kết thúc (kết thúc học việc) đều trong tháng
                var daysProb = Functions.CountWorkingDaysBetween (joinDate, probationEnd!.Value);
                var daysOfficial = Functions.CountWorkingDaysBetween (probationEnd.Value.AddDays (1), monthEnd);

                var partProb = 0.6m * basePerSale * (daysProb / (decimal)stdDays);
                var partOfficial = 1.0m * basePerSale * (daysOfficial / (decimal)stdDays);
                quota = Math.Round (partProb + partOfficial, 0, MidpointRounding.AwayFromZero);
            }
            else
            {
                return;
            }

            // Tạo employee quota trong AQC (role = Sale)
            var nextOrder = (aqCompany.AdmissionsQuotaEmployees?.Where (e => e.CompanyId == emp.CompanyId).Max (e => (int?)e.OrderIndex) ?? 0) + 1;

            var aqe = new Domain.Entities.AdmissionsQuotaEmployee
            {
                AdmissionsQuotaId = aqCompany.AdmissionsQuotaId,
                AdmissionsQuotaRegionId = aqCompany.AdmissionsQuotaRegionId,
                AdmissionsQuotaCompanyId = aqCompany.Id,
                EmployeeId = emp.Id,
                CompanyId = emp.CompanyId,
                RegionId = aqCompany?.AdmissionsQuotaRegion?.RegionId ?? null,
                PositionId = emp.PositionId,
                QuotaRole = quotaRole,
                RevenuePerSale = quota,
                JoinedAt = joinDate,
                AllocationStartAt = joinDate,
                AllocationEndAt = leavingInMonth ? emp.EmployeeEndDate : null,
                OrderIndex = nextOrder,
                Status = Domain.Enums.StatusType.Active,
            };

            await _context.AdmissionsQuotaEmployees.AddAsync (aqe, ct);

            // Cập nhật số sale hiện tại của Company và Region
            aqCompany.CurrentSales = (aqCompany.CurrentSales ?? 0) + 1;
            aqCompany.TotalRevenue = aqCompany.TotalRevenue + quota;
            if (aqCompany.AdmissionsQuotaRegion != null)
            {
                aqCompany.AdmissionsQuotaRegion.CurrentSales =
                    aqCompany.AdmissionsQuotaRegion.CurrentSales + 1;
                aqCompany.AdmissionsQuotaRegion.TotalRevenue =
                    aqCompany.AdmissionsQuotaRegion.TotalRevenue + quota;

                // Nếu cần cập nhật CurrentSales ở header tháng đó:
                if (aqCompany.AdmissionsQuotaRegion.AdmissionsQuota != null)
                {
                    aqCompany.AdmissionsQuotaRegion.AdmissionsQuota.CurrentSales =
                        (aqCompany.AdmissionsQuotaRegion.AdmissionsQuota.CurrentSales ?? 0) + 1;
                    aqCompany.AdmissionsQuotaRegion.AdmissionsQuota.TotalQuota =
                        aqCompany.AdmissionsQuotaRegion.AdmissionsQuota.TotalQuota + quota;
                }
            }
        }

    }
}
