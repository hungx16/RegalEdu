using RegalEdu.Application.Notifications.Models;

namespace RegalEdu.Application.Common.Interfaces;

public interface IRealTimeNotificationPublisher
{
    Task PublishAsync(NotificationPayload payload, CancellationToken cancellationToken = default);
}
