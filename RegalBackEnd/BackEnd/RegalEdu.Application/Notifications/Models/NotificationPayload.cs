using System.ComponentModel.DataAnnotations;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Application.Notifications.Models;

public sealed class NotificationPayload
{
    public Guid? RecipientId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? TitleVi { get; set; }

    public string? TitleEn { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? MessageVi { get; set; }

    public string? MessageEn { get; set; }

    public string? Payload { get; set; }

    public string? Type { get; set; }

    public NotificationChannel Channel { get; set; } = NotificationChannel.SignalR;
}
