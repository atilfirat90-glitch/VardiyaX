namespace ShiftCraft.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // SchedulePublished, ViolationDetected, ShiftReminder
    public string Action { get; set; } = string.Empty; // navigate_schedule, navigate_violations
    public string? DataJson { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
    public int? BusinessId { get; set; }

    public User? User { get; set; }
}
