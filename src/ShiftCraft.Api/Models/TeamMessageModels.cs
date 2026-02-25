namespace ShiftCraft.Api.Models;

public class SendMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public string Channel { get; set; } = "general";
    public int BusinessId { get; set; }
}

public class SendAnnouncementRequest
{
    public string Content { get; set; } = string.Empty;
    public int BusinessId { get; set; }
}

public class TeamMessageDto
{
    public int Id { get; set; }
    public int SenderUserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public bool IsAnnouncement { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BusinessId { get; set; }
}
