using System;
using MediatR;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Notifications.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Notifications.Commands;

public class MarkNotificationReadCommand : IRequest<Result>
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }

    public class Handler : IRequestHandler<MarkNotificationReadCommand, Result>
    {
        private readonly INotificationRepository _notificationRepository;

        public Handler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException (nameof (notificationRepository));
        }

        public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdAsync (request.NotificationId, cancellationToken);

            if (notification == null || notification.RecipientId != request.UserId)
            {
                return Result.Failure ("Notification not found.");
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTimeOffset.UtcNow;
                notification.Status = NotificationStatus.Read;
                await _notificationRepository.SaveChangesAsync (cancellationToken);
            }

            return Result.Success ( );
        }
    }
}
