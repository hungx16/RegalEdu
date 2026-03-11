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
    public class ApproveCompanyEventReportCommand : IRequest<Result>
    {
        public required ApproveCompanyEventReportModel ApproveCompanyEventReportModel { get; set; }
    }

    // Handler: Xử lý logic thêm AllocationEvent + AllocationDetailEvent
    public class ApproveCompanyEventReportCommandHandler
        : IRequestHandler<ApproveCompanyEventReportCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILocalizationService _localizer;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;
        public ApproveCompanyEventReportCommandHandler(
            IRegalEducationDbContext context,
            AutoMapper.IMapper mapper,
            ILocalizationService localizer
,
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
            ApproveCompanyEventReportCommand request,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_currentUser.EmployeeId, out var employeeId))
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.ApproveCompanyEventReportModel));
            var approveCompanyEvent = _mapper.Map<Domain.Entities.ApproveCompanyEventReport>(request.ApproveCompanyEventReportModel);

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
            approveCompanyEvent.EmployeeId = employeeId;
            companyEventReport.CompanyEventStatus = approveCompanyEvent.ApproveStatus;
            await _context.ApproveCompanyEventReports.AddAsync(approveCompanyEvent, cancellationToken);
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
            {
                if (!string.IsNullOrWhiteSpace(companyEventReport.CreatedBy))
                {
                    var createdBy = companyEventReport.CreatedBy;
                    var proposer = await _context.ApplicationUsers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            u => u.UserName == createdBy
                              || u.Email == createdBy
                              || u.UserCode == createdBy,
                            cancellationToken);

                    if (proposer != null)
                    {
                        var isApproved = request.ApproveCompanyEventReportModel.ApproveStatus == CompanyEventProposalStatus.Approved;
                        var statusTextEn = isApproved ? "approved" : "rejected";
                        var statusTextVi = isApproved ? "đã được phê duyệt" : "đã bị từ chối";
                        var reportName = companyEventReport.CompanyEvent?.CompanyEventName
                            ?? companyEventReport.CompanyEventReportCode
                            ?? companyEventReport.Id.ToString();
                        var branchName = companyEventReport.CompanyEvent?.AllocationDetailEvent?.Company?.CompanyName;
                        if (string.IsNullOrWhiteSpace(branchName))
                        {
                            branchName = "unknown branch";
                        }
                        var titleEn = $"Company Event Report {statusTextEn}";
                        var titleVi = $"Báo cáo sự kiện {statusTextVi}";
                        var messageEn = $"Your company event report '{reportName}' for branch '{branchName}' has been {statusTextEn}.";
                        var messageVi = $"Báo cáo sự kiện '{reportName}' thuộc chi nhánh '{branchName}' của bạn {statusTextVi}.";

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

                if (request.ApproveCompanyEventReportModel.ApproveStatus == CompanyEventProposalStatus.Approved)
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_APPROVE_CREATE_SUCCESS, _localizer[LocalizationKey.APPROVED]));
                else
                    return Result.Success(_localizer.Format(LocalizationKey.MSG_APPROVE_CREATE_SUCCESS, _localizer[LocalizationKey.REJECTED]));
            }
            else
            {
                return Result.Failure(_localizer.Format(LocalizationKey.ERR_SAVE_NO_EFFECT, EntityName.ApproveCompanyEventReportModel));
            }
        }
    }
}
