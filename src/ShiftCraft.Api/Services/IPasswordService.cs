namespace ShiftCraft.Api.Services;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    PasswordValidationResult ValidatePassword(string password);
    string GenerateTemporaryPassword();
}

public class PasswordValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static PasswordValidationResult Success() => new() { IsValid = true };
    public static PasswordValidationResult Failure(string message) => new() { IsValid = false, ErrorMessage = message };
}
