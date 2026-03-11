using RegalEdu.Domain.Entities;

namespace RegalEdu.Application.Notifications.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetNotificationsForRecipientAsync(Guid recipientId, int skip, int take, CancellationToken cancellationToken = default);
    Task<int> CountNotificationsForRecipientAsync(Guid recipientId, CancellationToken cancellationToken = default);
    Task<Notification?> GetByIdAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<int> MarkAllAsReadAsync(Guid recipientId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
