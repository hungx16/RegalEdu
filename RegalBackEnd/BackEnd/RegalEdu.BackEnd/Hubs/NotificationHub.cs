using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RegalEdu.Api.Hubs;

[Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class NotificationHub : Hub<INotificationClient>
{
    internal static string GetGroupName (string userId) => $"notifications-{userId}";

    public override async Task OnConnectedAsync ( )
    {
        var userId = Context.User?.FindFirst (ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst ("sub")?.Value;

        if (!string.IsNullOrWhiteSpace (userId))
        {
            await Groups.AddToGroupAsync (Context.ConnectionId, GetGroupName (userId));
        }

        await base.OnConnectedAsync ( );
    }

    public override async Task OnDisconnectedAsync (Exception? exception)
    {
        var userId = Context.User?.FindFirst (ClaimTypes.NameIdentifier)?.Value
                     ?? Context.User?.FindFirst ("sub")?.Value;

        if (!string.IsNullOrWhiteSpace (userId))
        {
            await Groups.RemoveFromGroupAsync (Context.ConnectionId, GetGroupName (userId));
        }

        await base.OnDisconnectedAsync (exception);
    }
}

public interface INotificationClient
{
    Task ReceiveNotification (NotificationMessage notification);
}
