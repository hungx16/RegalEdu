using System;
using MediatR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Application.Notifications.Interfaces;
using RegalEdu.Application.Notifications.Models;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Notifications.Commands;

public class CreateNotificationCommand : IRequest<Result>
{
    public NotificationPayload Payload { get; set; } = new ();
    public bool Persist { get; set; } = true;

    public class Handler : IRequestHandler<CreateNotificationCommand, Result>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IRealTimeNotificationPublisher _publisher;

        public Handler(INotificationRepository notificationRepository, IRealTimeNotificationPublisher publisher)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException (nameof (notificationRepository));
            _publisher = publisher ?? throw new ArgumentNullException (nameof (publisher));
        }

        public async Task<Result> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            if (request.Payload == null)
            {
                return Result.Failure ("Notification payload is required.");
            }

            var title = request.Payload.Title?.Trim ( );
            var titleVi = request.Payload.TitleVi?.Trim ( );
            var titleEn = request.Payload.TitleEn?.Trim ( );
            var message = request.Payload.Message ?? string.Empty;
            var messageVi = request.Payload.MessageVi;
            var messageEn = request.Payload.MessageEn;

            if (string.IsNullOrWhiteSpace (title))
            {
                title = !string.IsNullOrWhiteSpace (titleVi)
                    ? titleVi
                    : titleEn;
            }

            if (string.IsNullOrWhiteSpace (title))
            {
                return Result.Failure ("Notification must include a title.");
            }

            if (string.IsNullOrWhiteSpace (message))
            {
                message = !string.IsNullOrWhiteSpace (messageVi)
                    ? messageVi
                    : messageEn ?? string.Empty;
            }

            var notification = new Notification
            {
                RecipientId = request.Payload.RecipientId,
                Title = title,
                TitleVi = !string.IsNullOrWhiteSpace (titleVi) ? titleVi : title,
                TitleEn = !string.IsNullOrWhiteSpace (titleEn) ? titleEn : title,
                Message = message,
                MessageVi = !string.IsNullOrWhiteSpace (messageVi) ? messageVi : message,
                MessageEn = !string.IsNullOrWhiteSpace (messageEn) ? messageEn : message,
                Payload = request.Payload.Payload,
                Type = request.Payload.Type,
                Channel = request.Payload.Channel,
                SentAt = DateTimeOffset.UtcNow,
                Status = NotificationStatus.Pending
            };

            if (request.Persist)
            {
                await _notificationRepository.AddAsync (notification, cancellationToken);
            }

            await _publisher.PublishAsync (request.Payload, cancellationToken);

            if (request.Persist)
            {
                notification.DeliveredAt = DateTimeOffset.UtcNow;
                notification.Status = NotificationStatus.Delivered;
                await _notificationRepository.SaveChangesAsync (cancellationToken);
            }

            return Result.Success ( );
        }
    }
}
