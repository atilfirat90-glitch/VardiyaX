using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ShiftCraft.Api.Services;

public class PasswordService : IPasswordService
{
    private const int MinLength = 8;
    private const int WorkFactor = 11;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    public PasswordValidationResult ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return PasswordValidationResult.Failure("Password is required");

        if (password.Length < MinLength)
            return PasswordValidationResult.Failure($"Password must be at least {MinLength} characters");

        if (!Regex.IsMatch(password, @"\d"))
            return PasswordValidationResult.Failure("Password must contain at least one digit");

        return PasswordValidationResult.Success();
    }

    public string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
        var random = RandomNumberGenerator.Create();
        var bytes = new byte[12];
        random.GetBytes(bytes);
        
        var result = new char[12];
        for (int i = 0; i < 12; i++)
        {
            result[i] = chars[bytes[i] % chars.Length];
        }
        
        // Ensure at least one digit
        result[^1] = "23456789"[bytes[^1] % 8];
        
        return new string(result);
    }
}
