namespace ShiftCraft.Domain.Entities;

public class PublishLog
{
    public int Id { get; set; }
    public int WeeklyScheduleId { get; set; }
    public string PublisherUsername { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int AffectedEmployeeCount { get; set; }
    public int? BusinessId { get; set; }

    public WeeklySchedule? WeeklySchedule { get; set; }
    public Business? Business { get; set; }
}
