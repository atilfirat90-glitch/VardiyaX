namespace ShiftCraft.Mobile.Services;

/// <summary>
/// v1.1: Refactored to use IApiClient for centralized HTTP handling.
/// </summary>
public class UserService : IUserService
{
    private readonly IApiClient _apiClient;

    public UserService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _apiClient.GetAsync<List<UserDto>>("user") ?? new List<UserDto>();
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        return await _apiClient.GetAsync<UserDto>($"user/{id}") 
            ?? throw new ApiException("Kullanıcı bulunamadı.", 404);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
    {
        return await _apiClient.PostAsync<UserDto>("user", request)
            ?? throw new ApiException("Kullanıcı oluşturulamadı.", 500);
    }

    public async Task UpdateUserAsync(int id, UpdateUserRequest request)
    {
        await _apiClient.PutAsync($"user/{id}", request);
    }

    public async Task DeactivateUserAsync(int id)
    {
        await _apiClient.DeleteAsync($"user/{id}");
    }

    public async Task<string> ResetPasswordAsync(int id)
    {
        var result = await _apiClient.PostAsync<ResetPasswordResponse>($"user/{id}/reset-password");
        return result?.TemporaryPassword ?? "Şifre sıfırlandı";
    }
}

public class ResetPasswordResponse
{
    public string TemporaryPassword { get; set; } = string.Empty;
}
