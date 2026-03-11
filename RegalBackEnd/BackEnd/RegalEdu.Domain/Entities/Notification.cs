using System.ComponentModel.DataAnnotations.Schema;
using RegalEdu.Domain.Enumerations;

namespace RegalEdu.Domain.Entities
{
    [Table ("Notification")]
    public sealed class Notification : BaseEntity
    {
        public Guid? RecipientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TitleVi { get; set; }
        public string? TitleEn { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? MessageVi { get; set; }
        public string? MessageEn { get; set; }
        public string? Payload { get; set; }
        public string? Type { get; set; }
        public NotificationChannel Channel { get; set; } = NotificationChannel.SignalR;
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        public bool IsRead { get; set; }
        public DateTimeOffset? SentAt { get; set; }
        public DateTimeOffset? DeliveredAt { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
    }
}
