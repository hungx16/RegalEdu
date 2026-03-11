using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Domain.Models;

public sealed class NotificationModel
{
    public Guid Id { get; set; }
    public Guid? RecipientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleVi { get; set; }
    public string? TitleEn { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? MessageVi { get; set; }
    public string? MessageEn { get; set; }
    public string? Payload { get; set; }
    public string? Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class NotificationListModel
{
    public List<NotificationModel> Items { get; set; } = new ();
    public int TotalCount { get; set; }
}
