using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string username, string password);
    Task<bool> TryRestoreSessionAsync();
    Task LogoutAsync();
    bool IsAuthenticated { get; }
    bool IsTokenExpired();
    string? Token { get; }
    string? Username { get; }
    string? Role { get; }
    bool IsManager { get; }
    bool IsWorker { get; }
}
