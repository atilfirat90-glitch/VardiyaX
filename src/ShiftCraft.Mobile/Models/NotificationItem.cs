namespace ShiftCraft.Mobile.Models;

public class NotificationItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? DataJson { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public string TimeAgo
    {
        get
        {
            var diff = DateTime.UtcNow - CreatedAt;
            if (diff.TotalMinutes < 1) return "Az önce";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} dk önce";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} saat önce";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} gün önce";
            return CreatedAt.ToString("dd.MM.yyyy");
        }
    }

    public string TypeIcon => Type switch
    {
        "SchedulePublished" => "📅",
        "ViolationDetected" => "⚠️",
        "ShiftReminder" => "🔔",
        _ => "📬"
    };
}

public class NotificationListResponse
{
    public IEnumerable<NotificationItem> Notifications { get; set; } = Enumerable.Empty<NotificationItem>();
    public int UnreadCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class UnreadCountResponse
{
    public int Count { get; set; }
}
