using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Notifications.Commands;
using RegalEdu.Application.Notifications.Models;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;

namespace RegalEdu.Application.AllocationEvent.Commands
{
    // Command: Thêm AllocationEvent cùng AllocationDetailEvent
    public class UpdateStatusOfCompanyEventProposalCommand : IRequest<Result>
    {
        public required ApproveCompanyEventModel ApproveCompanyEventModel { get; set; }
    }

    // Handler: Xử lý logic thêm AllocationEvent + AllocationDetailEvent
    public class UpdateStatusOfCompanyEventProposalCommandHandler
        : IRequestHandler<UpdateStatusOfCompanyEventProposalCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly IUserPermissionInfoService _permissionInfoService;
        private readonly IMediator _mediator;

        public UpdateStatusOfCompanyEventProposalCommandHandler(
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
            UpdateStatusOfCompanyEventProposalCommand request,
            CancellationToken cancellationToken)
        {


            var companyEvent = await _context.CompanyEvents
                .Include(t => t.AllocationDetailEvent)
                .ThenInclude(t => t.Company)
                .Where(t => t.Id == request.ApproveCompanyEventModel.CompanyEventId)
                .FirstOrDefaultAsync();

            if (companyEvent == null)
            {
                return Result.Failure("Proposal not found");
            }
            companyEvent.CompanyEventStatus = request.ApproveCompanyEventModel.ApproveStatus;
            List<Guid> approverIds = new();
            if (request.ApproveCompanyEventModel.ApproveStatus == Domain.Enums.CompanyEventProposalStatus.PendingApproval)
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
                    var eventName = !string.IsNullOrWhiteSpace(companyEvent.CompanyEventName)
                        ? companyEvent.CompanyEventName
                        : !string.IsNullOrWhiteSpace(companyEvent.CompanyEventCode)
                            ? companyEvent.CompanyEventCode
                            : companyEvent.Id.ToString();
                    var branchName = companyEvent.AllocationDetailEvent?.Company?.CompanyName;
                    if (string.IsNullOrWhiteSpace(branchName))
                    {
                        branchName = "unknown branch";
                    }
                    var titleEn = "Company Event Proposal pending approval";
                    var titleVi = "Đề xuất sự kiện chờ phê duyệt";
                    var messageEn = $"A company event proposal '{eventName}' for branch '{branchName}' is waiting for your approval.";
                    var messageVi = $"Đề xuất sự kiện '{eventName}' thuộc chi nhánh '{branchName}' đang chờ bạn phê duyệt.";
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

                return Result.Success(_localizer.Format(LocalizationKey.MSG_UPDATE_SUCCESS, EntityName.CompanyEvent));
            }
            else
            {
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.CompanyEvent));
            }
        }
    }
}

