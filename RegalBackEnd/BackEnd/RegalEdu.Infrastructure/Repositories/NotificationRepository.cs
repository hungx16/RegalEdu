using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Notifications.Interfaces;
using RegalEdu.Domain.Entities;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Infrastructure.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly IRegalEducationDbContext _context;

    public NotificationRepository(IRegalEducationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException (nameof (context));
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync (notification, cancellationToken);
    }

    public async Task<List<Notification>> GetNotificationsForRecipientAsync(Guid recipientId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where (x => !x.IsRead && (x.RecipientId == null || x.RecipientId == recipientId))
            .OrderByDescending (x => x.CreatedAt)
            .Skip (skip)
            .Take (take)
            .AsNoTracking ( )
            .ToListAsync (cancellationToken);
    }

    public async Task<int> CountNotificationsForRecipientAsync(Guid recipientId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where (x => !x.IsRead && (x.RecipientId == null || x.RecipientId == recipientId))
            .CountAsync (cancellationToken);
    }

    public async Task<Notification?> GetByIdAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications.FindAsync (new object[] { notificationId }, cancellationToken);
    }

    public async Task<int> MarkAllAsReadAsync(Guid recipientId, CancellationToken cancellationToken = default)
    {
        var notifications = await _context.Notifications
            .Where (x => x.RecipientId == recipientId && !x.IsRead)
            .ToListAsync (cancellationToken);

        if (notifications.Count == 0)
        {
            return 0;
        }

        var readAt = DateTimeOffset.UtcNow;
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = readAt;
            notification.Status = NotificationStatus.Read;
        }

        return await _context.SaveChangesAsync (cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync (cancellationToken);
    }
}
