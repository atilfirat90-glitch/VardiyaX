namespace ShiftCraft.Api.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class LoginResponseWithContext
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserContextDto User { get; set; } = new();
    public bool MustChangePassword { get; set; }
}

public class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
}
