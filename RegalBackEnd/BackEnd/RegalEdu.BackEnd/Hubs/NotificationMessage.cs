using System;

namespace RegalEdu.Api.Hubs;

public sealed class NotificationMessage
{
    public string Title { get; set; } = string.Empty;
    public string? TitleVi { get; set; }
    public string? TitleEn { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? MessageVi { get; set; }
    public string? MessageEn { get; set; }
    public string? Payload { get; set; }
    public string? Type { get; set; }
    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
}
