namespace ShiftCraft.Mobile.Services;

public interface IUserService
{
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserAsync(int id);
    Task<UserDto> CreateUserAsync(CreateUserRequest request);
    Task UpdateUserAsync(int id, UpdateUserRequest request);
    Task DeactivateUserAsync(int id);
    Task<string> ResetPasswordAsync(int id);
}

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool MustChangePassword { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int? BusinessId { get; set; }
    public string? BusinessName { get; set; }
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Worker";
}

public class UpdateUserRequest
{
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
