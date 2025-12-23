using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ShiftCraft.Mobile.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public UserService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiSettings.BaseUrl);
        _authService = authService;
    }

    private void SetAuthHeader()
    {
        if (_authService.IsAuthenticated)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authService.Token);
        }
    }

    private async Task HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedAccessException("Oturum süresi doldu.");
                case HttpStatusCode.Forbidden:
                    throw new Exception("Bu işlem için yetkiniz yok.");
                case HttpStatusCode.NotFound:
                    throw new Exception("Kullanıcı bulunamadı.");
                case HttpStatusCode.Conflict:
                    throw new Exception("Bu kullanıcı adı zaten kullanılıyor.");
                case HttpStatusCode.BadRequest:
                    throw new Exception(content.Contains("password") 
                        ? "Şifre en az 8 karakter ve 1 rakam içermelidir." 
                        : "Geçersiz istek.");
                default:
                    throw new Exception($"Hata: {response.StatusCode}");
            }
        }
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("user");
        await HandleResponse(response);
        return await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? new List<UserDto>();
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"user/{id}");
        await HandleResponse(response);
        return await response.Content.ReadFromJsonAsync<UserDto>() 
            ?? throw new Exception("Kullanıcı bulunamadı.");
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("user", request);
        await HandleResponse(response);
        return await response.Content.ReadFromJsonAsync<UserDto>()
            ?? throw new Exception("Kullanıcı oluşturulamadı.");
    }

    public async Task UpdateUserAsync(int id, UpdateUserRequest request)
    {
        SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"user/{id}", request);
        await HandleResponse(response);
    }

    public async Task DeactivateUserAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"user/{id}");
        await HandleResponse(response);
    }

    public async Task<string> ResetPasswordAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsync($"user/{id}/reset-password", null);
        await HandleResponse(response);
        var result = await response.Content.ReadFromJsonAsync<ResetPasswordResponse>();
        return result?.TemporaryPassword ?? "Şifre sıfırlandı";
    }
}

public class ResetPasswordResponse
{
    public string TemporaryPassword { get; set; } = string.Empty;
}
