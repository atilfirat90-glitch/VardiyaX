namespace ShiftCraft.Domain.Entities;

public class DeviceRegistration
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string DeviceToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty; // Android, iOS, Windows
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastActiveAt { get; set; }
    public bool IsActive { get; set; } = true;

    public User? User { get; set; }
}
