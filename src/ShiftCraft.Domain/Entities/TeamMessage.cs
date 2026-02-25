namespace ShiftCraft.Domain.Entities;

public class TeamMessage
{
    public int Id { get; set; }
    public int SenderUserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Channel { get; set; } = "general";
    public bool IsAnnouncement { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int BusinessId { get; set; }

    public User? Sender { get; set; }
}
