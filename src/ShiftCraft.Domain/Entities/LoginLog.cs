namespace ShiftCraft.Domain.Entities;

public class LoginLog
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Action { get; set; } = string.Empty; // Login, Logout, FailedLogin
    public string? DeviceInfo { get; set; }
    public string? FailureReason { get; set; }
    public string? IpAddress { get; set; }
    public int? BusinessId { get; set; }

    public Business? Business { get; set; }
}
