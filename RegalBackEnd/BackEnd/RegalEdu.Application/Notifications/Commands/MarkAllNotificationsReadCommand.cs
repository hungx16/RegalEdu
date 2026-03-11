using System;
using MediatR;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Notifications.Interfaces;

namespace RegalEdu.Application.Notifications.Commands;

public class MarkAllNotificationsReadCommand : IRequest<Result>
{
    public Guid UserId { get; set; }

    public class Handler : IRequestHandler<MarkAllNotificationsReadCommand, Result>
    {
        private readonly INotificationRepository _notificationRepository;

        public Handler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException (nameof (notificationRepository));
        }

        public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationRepository.MarkAllAsReadAsync (request.UserId, cancellationToken);
            return Result.Success ( );
        }
    }
}
