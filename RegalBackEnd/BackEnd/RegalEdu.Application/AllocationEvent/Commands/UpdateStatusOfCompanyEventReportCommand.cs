using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Notifications.Commands;
using RegalEdu.Application.Notifications.Models;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Enums;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Commands
{
    // Command: Thêm AllocationEvent cùng AllocationDetailEvent
    public class UpdateStatusOfCompanyEventReportCommand : IRequest<Result>
    {
        public required ApproveCompanyEventReportModel ApproveCompanyEventReportModel { get; set; }
    }

    // Handler: Xử lý logic thêm AllocationEvent + AllocationDetailEvent
    public class UpdateStatusOfCompanyEventReportCommandHandler
        : IRequestHandler<UpdateStatusOfCompanyEventReportCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly IUserPermissionInfoService _permissionInfoService;
        private readonly IMediator _mediator;

        public UpdateStatusOfCompanyEventReportCommandHandler(
            IRegalEducationDbContext context,
            AutoMapper.IMapper mapper,
            ILocalizationService localizer,
            IUserPermissionInfoService permissionInfoService,
            IMediator mediator
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _permissionInfoService = permissionInfoService ?? throw new ArgumentNullException(nameof(permissionInfoService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Result> Handle(
            UpdateStatusOfCompanyEventReportCommand request,
            CancellationToken cancellationToken)
        {

            var companyEventReport = await _context.CompanyEventReports
                .Include(t => t.CompanyEvent)
                .ThenInclude(t => t.AllocationDetailEvent)
                .ThenInclude(t => t.Company)
                .Where(t => t.Id == request.ApproveCompanyEventReportModel.CompanyEventReportId)
                .FirstOrDefaultAsync();

            if (companyEventReport == null)
            {
                return Result.Failure("Event report not found");
            }
            companyEventReport.CompanyEventStatus = request.ApproveCompanyEventReportModel.ApproveStatus;
            List<Guid> approverIds = new();
            if (request.ApproveCompanyEventReportModel.ApproveStatus == CompanyEventProposalStatus.PendingApproval)
            {
                approverIds = (await _permissionInfoService.GetMarketingUserIdsAsync())
                    .Distinct()
                    .ToList();
            }

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
            {
                if (approverIds.Count > 0)
                {
                    var reportName = companyEventReport.CompanyEvent?.CompanyEventName
                        ?? companyEventReport.CompanyEventReportCode
                        ?? companyEventReport.Id.ToString();
                    var branchName = companyEventReport.CompanyEvent?.AllocationDetailEvent?.Company?.CompanyName;
                    if (string.IsNullOrWhiteSpace(branchName))
                    {
                        branchName = "unknown branch";
                    }
                    var titleEn = "Company Event Report pending approval";
                    var titleVi = "Báo cáo sự kiện đang chờ phê duyệt";
                    var messageEn = $"A company event report '{reportName}' for branch '{branchName}' is waiting for your approval.";
                    var messageVi = $"Báo cáo sự kiện '{reportName}' của chi nhánh '{branchName}' đang chờ bạn phê duyệt.";
                    foreach (var approverId in approverIds)
                    {
                        var notificationCommand = new CreateNotificationCommand
                        {
                            Payload = new NotificationPayload
                            {
                                RecipientId = approverId,
                                Title = titleVi,
                                TitleVi = titleVi,
                                TitleEn = titleEn,
                                Message = messageVi,
                                MessageVi = messageVi,
                                MessageEn = messageEn,
                                Type = "CompanyEventApproval",
                                Channel = NotificationChannel.SignalR
                            },
                            Persist = true
                        };
                        await _mediator.Send(notificationCommand, cancellationToken);
                    }
                }
                return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.CompanyEventReport));
            }
            else
            {
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.CompanyEventReport));
            }
        }
    }
}

