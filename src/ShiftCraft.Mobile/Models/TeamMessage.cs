namespace ShiftCraft.Mobile.Models;

public class TeamMessageModel
{
    public int Id { get; set; }
    public int SenderUserId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public bool IsAnnouncement { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BusinessId { get; set; }

    public string CreatedAtText => CreatedAt.ToString("HH:mm");
    public string DateText => CreatedAt.ToString("dd MMM yyyy");
}

public class SendMessageModel
{
    public string Content { get; set; } = string.Empty;
    public string Channel { get; set; } = "general";
    public int BusinessId { get; set; }
}

public class SendAnnouncementModel
{
    public string Content { get; set; } = string.Empty;
    public int BusinessId { get; set; }
}
