using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Notifications.Models;

namespace RegalEdu.Api.Hubs;

public sealed class NotificationHubService : IRealTimeNotificationPublisher
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public NotificationHubService (IHubContext<NotificationHub, INotificationClient> hubContext)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException (nameof (hubContext));
    }

    public Task PublishAsync (NotificationPayload payload, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull (payload);
        var message = new NotificationMessage
        {
            Title = payload.Title,
            TitleVi = payload.TitleVi,
            TitleEn = payload.TitleEn,
            Message = payload.Message,
            MessageVi = payload.MessageVi,
            MessageEn = payload.MessageEn,
            Payload = payload.Payload,
            Type = payload.Type,
            SentAt = DateTimeOffset.UtcNow
        };

        if (payload.RecipientId.HasValue)
        {
            var groupName = NotificationHub.GetGroupName (payload.RecipientId.Value.ToString ( ));
            return _hubContext.Clients.Group (groupName).ReceiveNotification (message);
        }

        return _hubContext.Clients.All.ReceiveNotification (message);
    }
}
