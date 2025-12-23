using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ShiftCraft.Mobile.Services;

public class PushNotificationHandler : IPushNotificationHandler
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private const string PreferencesKey = "notification_preferences";

    public PushNotificationHandler(HttpClient httpClient, IAuthService authService)
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

    public async Task RegisterDeviceAsync(string deviceToken)
    {
        SetAuthHeader();
        
        var platform = DeviceInfo.Platform.ToString();
        var request = new DeviceRegistrationRequest
        {
            DeviceToken = deviceToken,
            Platform = platform
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("notification/device", request);
            response.EnsureSuccessStatusCode();
            
            // Store token locally
            await SecureStorage.SetAsync("device_token", deviceToken);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PushNotification] Register failed: {ex.Message}");
        }
    }

    public async Task UnregisterDeviceAsync()
    {
        SetAuthHeader();
        
        try
        {
            var token = await SecureStorage.GetAsync("device_token");
            if (!string.IsNullOrEmpty(token))
            {
                await _httpClient.DeleteAsync($"notification/device/{token}");
                SecureStorage.Remove("device_token");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PushNotification] Unregister failed: {ex.Message}");
        }
    }

    public async Task<NotificationPreferences> GetPreferencesAsync()
    {
        try
        {
            var json = await SecureStorage.GetAsync(PreferencesKey);
            if (!string.IsNullOrEmpty(json))
            {
                return System.Text.Json.JsonSerializer.Deserialize<NotificationPreferences>(json) 
                    ?? new NotificationPreferences();
            }
        }
        catch
        {
            // Ignore errors
        }
        
        return new NotificationPreferences();
    }

    public async Task UpdatePreferencesAsync(NotificationPreferences preferences)
    {
        SetAuthHeader();
        
        try
        {
            // Save locally
            var json = System.Text.Json.JsonSerializer.Serialize(preferences);
            await SecureStorage.SetAsync(PreferencesKey, json);
            
            // Sync to server
            await _httpClient.PutAsJsonAsync("notification/preferences", preferences);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PushNotification] Update preferences failed: {ex.Message}");
        }
    }

    public void HandleNotificationReceived(PushNotificationPayload payload)
    {
        // Show local notification or update UI
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.DisplayAlert(payload.Title, payload.Body, "Tamam");
        });
    }

    public void HandleNotificationTapped(PushNotificationPayload payload)
    {
        // Navigate based on action
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            switch (payload.Action)
            {
                case "navigate_schedule":
                    await Shell.Current.GoToAsync("//main/schedules");
                    break;
                case "navigate_violations":
                    await Shell.Current.GoToAsync("//main/violations");
                    break;
                default:
                    // Default navigation
                    break;
            }
        });
    }
}
