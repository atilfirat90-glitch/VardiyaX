using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public interface INotificationService
{
    Task<NotificationListResponse?> GetNotificationsAsync(int page = 1, int pageSize = 20);
    Task<IEnumerable<NotificationItem>> GetUnreadNotificationsAsync();
    Task<int> GetUnreadCountAsync();
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync();
}
