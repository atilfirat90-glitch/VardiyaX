using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ShiftCraft.Mobile.Services;

/// <summary>
/// Centralized API client implementation.
/// Handles base URL, authentication, error handling, and retries.
/// v1.1 - Technical debt elimination
/// </summary>
public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly string _baseUrl;
    private readonly int _retryCount;
    private readonly int _retryDelayMs;

    public ApiClient(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
        _baseUrl = ApiSettings.BaseUrl;
        _retryCount = 1;
        _retryDelayMs = 1000;
        
        // Note: Timeout is configured in MauiProgram.cs during HttpClient registration
        // Do NOT set Timeout here as HttpClient may already be in use
    }

    #region Public Methods

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        return await ExecuteWithRetry(async () =>
        {
            var response = await ExecuteRequest(() => _httpClient.GetAsync(BuildUrl(endpoint)));
            return await DeserializeResponse<T>(response);
        });
    }

    public async Task<T?> PostAsync<T>(string endpoint, object? data = null)
    {
        return await ExecuteWithRetry(async () =>
        {
            var response = await ExecuteRequest(() => 
                data != null 
                    ? _httpClient.PostAsJsonAsync(BuildUrl(endpoint), data)
                    : _httpClient.PostAsync(BuildUrl(endpoint), null));
            return await DeserializeResponse<T>(response);
        });
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        return await ExecuteWithRetry(async () =>
        {
            var response = await ExecuteRequest(() => 
                _httpClient.PutAsJsonAsync(BuildUrl(endpoint), data));
            return await DeserializeResponse<T>(response);
        });
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        return await ExecuteWithRetry(async () =>
        {
            var response = await ExecuteRequest(() => _httpClient.DeleteAsync(BuildUrl(endpoint)));
            return response.IsSuccessStatusCode;
        });
    }

    public async Task<bool> PostAsync(string endpoint, object? data = null)
    {
        return await ExecuteWithRetry(async () =>
        {
            var response = await ExecuteRequest(() => 
                data != null 
                    ? _httpClient.PostAsJsonAsync(BuildUrl(endpoint), data)
                    : _httpClient.PostAsync(BuildUrl(endpoint), null));
            return response.IsSuccessStatusCode;
        });
    }

    public async Task<bool> PutAsync(string endpoint, object data)
    {
        return await ExecuteWithRetry(async () =>
        {
            var response = await ExecuteRequest(() => 
                _httpClient.PutAsJsonAsync(BuildUrl(endpoint), data));
            return response.IsSuccessStatusCode;
        });
    }

    #endregion

    #region Private Helpers

    private string BuildUrl(string endpoint)
    {
        // Ensure no double slashes
        var cleanEndpoint = endpoint.TrimStart('/');
        return $"{_baseUrl}{cleanEndpoint}";
    }

    private void SetAuthHeader()
    {
        if (_authService.IsAuthenticated)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authService.Token);
        }
    }

    private async Task<HttpResponseMessage> ExecuteRequest(Func<Task<HttpResponseMessage>> requestFunc)
    {
        // Check token expiry before request
        if (_authService.IsAuthenticated && _authService.IsTokenExpired())
        {
            await HandleUnauthorized();
            throw new UnauthorizedAccessException("Oturum süresi doldu. Lütfen tekrar giriş yapın.");
        }

        SetAuthHeader();
        
        var response = await requestFunc();
        
        // Handle response status
        await HandleResponseStatus(response);
        
        return response;
    }

    private async Task HandleResponseStatus(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                await HandleUnauthorized();
                throw new UnauthorizedAccessException("Oturum süresi doldu.");

            case HttpStatusCode.Forbidden:
                throw new ApiException("Bu işlem için yetkiniz yok.", 403);

            case HttpStatusCode.NotFound:
                throw new ApiException("Kayıt bulunamadı.", 404);

            case HttpStatusCode.Conflict:
                throw new ApiException("Bu kayıt zaten mevcut.", 409);

            case HttpStatusCode.BadRequest:
                var content = await response.Content.ReadAsStringAsync();
                var message = content.Contains("password")
                    ? "Şifre en az 8 karakter ve 1 rakam içermelidir."
                    : "Geçersiz istek.";
                throw new ApiException(message, 400);

            case HttpStatusCode.InternalServerError:
                throw new ApiException("Sunucu hatası. Lütfen daha sonra tekrar deneyin.", 500);

            default:
                throw new ApiException($"Hata: {response.StatusCode}", (int)response.StatusCode);
        }
    }

    private async Task HandleUnauthorized()
    {
        await _authService.LogoutAsync();
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.GoToAsync("//login");
        });
    }

    private async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.NoContent)
            return default;

        try
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ApiClient] Deserialization error: {ex.Message}");
            return default;
        }
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> action)
    {
        Exception? lastException = null;
        
        for (int attempt = 0; attempt <= _retryCount; attempt++)
        {
            try
            {
                return await action();
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
                System.Diagnostics.Debug.WriteLine($"[ApiClient] Attempt {attempt + 1} failed: {ex.Message}");
                
                if (attempt < _retryCount)
                {
                    await Task.Delay(_retryDelayMs);
                }
            }
            catch (TaskCanceledException ex)
            {
                // Timeout - don't retry
                throw new ApiException("İstek zaman aşımına uğradı. Lütfen tekrar deneyin.", 0);
            }
            catch (UnauthorizedAccessException)
            {
                // Auth errors - don't retry
                throw;
            }
            catch (ApiException)
            {
                // Business errors - don't retry
                throw;
            }
        }

        throw new ApiException($"Sunucuya bağlanılamadı: {lastException?.Message}", 0);
    }

    #endregion
}

/// <summary>
/// Custom exception for API errors with status code.
/// </summary>
public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
