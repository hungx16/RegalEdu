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
    public class ApproveCompanyEventProposalCommand : IRequest<Result>
    {
        public required ApproveCompanyEventModel ApproveCompanyEventModel { get; set; }
    }

    // Handler: Xử lý logic thêm AllocationEvent + AllocationDetailEvent
    public class ApproveCompanyEventProposalCommandHandler
        : IRequestHandler<ApproveCompanyEventProposalCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;
        public ApproveCompanyEventProposalCommandHandler(
            IRegalEducationDbContext context,
            AutoMapper.IMapper mapper,
            ILocalizationService localizer,
            ICurrentUserService currentUser,
            IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Result> Handle(
            ApproveCompanyEventProposalCommand request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.EmployeeId, out var employeeId))
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.ApproveCompanyEventModel));
            var approveCompanyEvent = _mapper.Map<Domain.Entities.ApproveCompanyEvent>(request.ApproveCompanyEventModel);

            var companyEvent = await _context.CompanyEvents
                .Include(t => t.AllocationDetailEvent)
                .ThenInclude(t => t.Company)
                .Where(t => t.Id == request.ApproveCompanyEventModel.CompanyEventId)
                .FirstOrDefaultAsync();

            if (companyEvent == null)
            {
                return Result.Failure("Proposal not found");
            }
            approveCompanyEvent.ApprovedBy = employeeId;
            companyEvent.CompanyEventStatus = approveCompanyEvent.ApproveStatus;
            await _context.ApproveCompanyEvents.AddAsync(approveCompanyEvent, cancellationToken);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
            {
                // Send notification to the proposer
                if (!string.IsNullOrWhiteSpace(companyEvent.CreatedBy))
                {
                    var createdBy = companyEvent.CreatedBy;
                    var proposer = await _context.ApplicationUsers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            u => u.UserName == createdBy
                              || u.Email == createdBy
                              || u.UserCode == createdBy,
                            cancellationToken);

                    if (proposer != null)
                    {
                        var isApproved = request.ApproveCompanyEventModel.ApproveStatus == CompanyEventProposalStatus.Approved;
                        var statusTextEn = isApproved ? "approved" : "rejected";
                        var statusTextVi = isApproved ? "được phê duyệt" : "bị từ chối";
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
                        var titleEn = $"Company Event Proposal {statusTextEn}";
                        var titleVi = $"Đề xuất sự kiện {statusTextVi}";
                        var messageEn = $"Your company event proposal '{eventName}' for branch '{branchName}' has been {statusTextEn}.";
                        var messageVi = $"Đề xuất sự kiện '{eventName}' thuộc chi nhánh '{branchName}' của bạn {statusTextVi}.";

                        var notificationCommand = new CreateNotificationCommand
                        {
                            Payload = new NotificationPayload
                            {
                                RecipientId = proposer.Id,
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

                if (request.ApproveCompanyEventModel.ApproveStatus == CompanyEventProposalStatus.Approved)
                {

                    return Result.Success(_localizer.Format(LocalizationKey.MSG_APPROVE_CREATE_SUCCESS, _localizer[LocalizationKey.APPROVED]));

                }
                else
                {
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_APPROVE_CREATE_SUCCESS, _localizer[LocalizationKey.REJECTED]));
                }
            }
            else
            {
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.ApproveCompanyEventModel));
            }
        }
    }
}

