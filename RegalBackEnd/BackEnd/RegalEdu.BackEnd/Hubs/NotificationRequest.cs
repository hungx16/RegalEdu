using System;
using System.ComponentModel.DataAnnotations;
using RegalEdu.Application.Notifications.Models;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Api.Hubs;

public sealed class NotificationRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? TitleVi { get; set; }

    public string? TitleEn { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? MessageVi { get; set; }

    public string? MessageEn { get; set; }

    public Guid? RecipientId { get; set; }

    public string? Payload { get; set; }

    public string? Type { get; set; }

    public NotificationChannel Channel { get; set; } = NotificationChannel.SignalR;

    public bool Persist { get; set; } = true;

    public NotificationPayload ToPayload ( )
        => new ()
        {
            Title = Title,
            TitleVi = TitleVi,
            TitleEn = TitleEn,
            Message = Message,
            MessageVi = MessageVi,
            MessageEn = MessageEn,
            RecipientId = RecipientId,
            Payload = Payload,
            Type = Type,
            Channel = Channel
        };
}
