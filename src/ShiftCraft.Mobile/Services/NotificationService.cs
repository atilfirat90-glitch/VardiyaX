using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public class NotificationService : INotificationService
{
    private readonly IApiClient _apiClient;

    public NotificationService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<NotificationListResponse?> GetNotificationsAsync(int page = 1, int pageSize = 20)
    {
        return await _apiClient.GetAsync<NotificationListResponse>($"notification?page={page}&pageSize={pageSize}");
    }

    public async Task<IEnumerable<NotificationItem>> GetUnreadNotificationsAsync()
    {
        var result = await _apiClient.GetAsync<List<NotificationItem>>("notification/unread");
        return result ?? Enumerable.Empty<NotificationItem>();
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var result = await _apiClient.GetAsync<UnreadCountResponse>("notification/unread/count");
        return result?.Count ?? 0;
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        await _apiClient.PostAsync($"notification/{notificationId}/read");
    }

    public async Task MarkAllAsReadAsync()
    {
        await _apiClient.PostAsync("notification/read-all");
    }
}
