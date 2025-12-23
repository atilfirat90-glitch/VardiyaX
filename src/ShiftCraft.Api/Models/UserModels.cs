using System.ComponentModel.DataAnnotations;

namespace ShiftCraft.Api.Models;

public class CreateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = "Worker";
    
    public int? BusinessId { get; set; }
}

public class UpdateUserRequest
{
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
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

public class UserContextDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
}

public class ResetPasswordResponse
{
    public string TemporaryPassword { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
