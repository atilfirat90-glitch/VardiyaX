namespace ShiftCraft.Mobile.Services;

/// <summary>
/// v1.1: Refactored to use IApiClient for centralized HTTP handling.
/// </summary>
public class PushNotificationHandler : IPushNotificationHandler
{
    private readonly IApiClient _apiClient;
    private const string PreferencesKey = "notification_preferences";

    public PushNotificationHandler(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task RegisterDeviceAsync(string deviceToken)
    {
        var platform = DeviceInfo.Platform.ToString();
        var request = new DeviceRegistrationRequest
        {
            DeviceToken = deviceToken,
            Platform = platform
        };

        try
        {
            await _apiClient.PostAsync("notification/device", request);
            
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
        try
        {
            var token = await SecureStorage.GetAsync("device_token");
            if (!string.IsNullOrEmpty(token))
            {
                await _apiClient.DeleteAsync($"notification/device/{token}");
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
        try
        {
            // Save locally
            var json = System.Text.Json.JsonSerializer.Serialize(preferences);
            await SecureStorage.SetAsync(PreferencesKey, json);
            
            // Sync to server
            await _apiClient.PutAsync("notification/preferences", preferences);
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
