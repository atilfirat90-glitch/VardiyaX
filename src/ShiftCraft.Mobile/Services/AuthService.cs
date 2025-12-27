using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string? _token;
    private string? _username;
    private string? _role;
    private DateTime? _tokenExpiry;

    // TEST MODE FLAG - Set to true to auto-login with admin/admin credentials
    private const bool TEST_MODE = true;
    private bool _testModeInitialized = false;
    
    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseUrl = ApiSettings.BaseUrl;
        // v1.1: Removed BaseAddress assignment - using full URL pattern
        
        // TEST MODE: Will auto-login on first API call
        if (TEST_MODE)
        {
            System.Diagnostics.Debug.WriteLine("[AuthService] TEST MODE ACTIVE - Will auto-login on first use");
        }
    }
    
    private async Task EnsureTestModeLoginAsync()
    {
        if (!TEST_MODE || _testModeInitialized) return;
        
        try
        {
            System.Diagnostics.Debug.WriteLine("[AuthService] TEST MODE - Auto-login with admin/admin");
            var result = await LoginAsync("admin", "admin");
            if (result != null)
            {
                _testModeInitialized = true;
                System.Diagnostics.Debug.WriteLine("[AuthService] TEST MODE - Auto-login successful");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthService] TEST MODE - Auto-login failed: {ex.Message}");
            // Fallback to hardcoded token if API is not available
            _token = "test-token-fallback";
            _username = "admin";
            _role = "Admin";
            _tokenExpiry = DateTime.UtcNow.AddYears(1);
            _testModeInitialized = true;
        }
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token) && _tokenExpiry > DateTime.UtcNow;
    public string? Token => _token;
    public string? Username => _username;
    public string? Role => _role;
    public bool IsManager => _role == "Admin" || _role == "Manager";
    public bool IsWorker => _role == "Worker" || _role == "Trainee";

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        try
        {
            var request = new LoginRequest { Username = username, Password = password };
            
            // v1.1: Using full URL pattern for consistency
            var loginUrl = $"{_baseUrl}{ApiSettings.Endpoints.Login}";
            System.Diagnostics.Debug.WriteLine($"[AuthService] POST to: {loginUrl}");
            System.Diagnostics.Debug.WriteLine($"[AuthService] Username: {username}");
            
            var response = await _httpClient.PostAsJsonAsync(loginUrl, request);
            
            // Debug: Log response status
            System.Diagnostics.Debug.WriteLine($"[AuthService] Response Status: {response.StatusCode}");
            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[AuthService] Response Body: {responseContent}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent, 
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                if (result != null)
                {
                    _token = result.Token;
                    _tokenExpiry = result.ExpiresAt;
                    _username = result.Username;
                    
                    // Parse JWT to get role
                    ParseJwtClaims(result.Token);
                    
                    // Store securely
                    await SecureStorage.SetAsync("auth_token", _token);
                    await SecureStorage.SetAsync("auth_username", _username);
                    await SecureStorage.SetAsync("auth_role", _role ?? "Worker");
                    await SecureStorage.SetAsync("auth_expiry", _tokenExpiry?.ToString("O") ?? "");
                }
                return result;
            }
            
            // Debug: Show actual error from server
            System.Diagnostics.Debug.WriteLine($"[AuthService] Login failed: {responseContent}");
            return null;
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthService] HttpRequestException: {ex.Message}");
            throw new Exception("Sunucuya bağlanılamadı. İnternet bağlantınızı kontrol edin.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuthService] Exception: {ex.Message}");
            throw new Exception($"Giriş hatası: {ex.Message}");
        }
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        // TEST MODE: Auto-login with admin/admin
        if (TEST_MODE)
        {
            await EnsureTestModeLoginAsync();
            return IsAuthenticated;
        }
        
        try
        {
            var token = await SecureStorage.GetAsync("auth_token");
            var expiry = await SecureStorage.GetAsync("auth_expiry");
            var username = await SecureStorage.GetAsync("auth_username");
            var role = await SecureStorage.GetAsync("auth_role");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(expiry))
            {
                var expiryDate = DateTime.Parse(expiry);
                if (expiryDate > DateTime.UtcNow)
                {
                    _token = token;
                    _tokenExpiry = expiryDate;
                    _username = username;
                    _role = role;
                    return true;
                }
            }
        }
        catch
        {
            // Ignore errors during restore
        }
        
        return false;
    }

    public async Task LogoutAsync()
    {
        // TEST MODE: Don't actually logout - just show message
        if (TEST_MODE)
        {
            System.Diagnostics.Debug.WriteLine("[AuthService] TEST MODE - Logout disabled");
            await Task.CompletedTask;
            return;
        }
        
        _token = null;
        _username = null;
        _role = null;
        _tokenExpiry = null;
        
        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("auth_username");
        SecureStorage.Remove("auth_role");
        SecureStorage.Remove("auth_expiry");
        
        await Task.CompletedTask;
    }

    public bool IsTokenExpired()
    {
        return _tokenExpiry == null || _tokenExpiry <= DateTime.UtcNow;
    }

    private void ParseJwtClaims(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            _role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value ?? "Worker";
            _username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "unique_name")?.Value ?? _username;
        }
        catch
        {
            _role = "Worker";
        }
    }
}
