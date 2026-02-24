using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class NotificationsViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private string _errorMessage = string.Empty;
    private int _unreadCount;
    private int _currentPage = 1;
    private bool _hasMorePages = true;

    public NotificationsViewModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
        Title = "Bildirimler";
        Notifications = new ObservableCollection<NotificationItem>();
        RefreshCommand = new Command(async () => await LoadNotificationsAsync());
        LoadMoreCommand = new Command(async () => await LoadMoreAsync());
        MarkAllReadCommand = new Command(async () => await MarkAllAsReadAsync());
        NotificationTappedCommand = new Command<NotificationItem>(async (n) => await OnNotificationTapped(n));
    }

    public ObservableCollection<NotificationItem> Notifications { get; }
    public ICommand RefreshCommand { get; }
    public ICommand LoadMoreCommand { get; }
    public ICommand MarkAllReadCommand { get; }
    public ICommand NotificationTappedCommand { get; }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public int UnreadCount
    {
        get => _unreadCount;
        set
        {
            if (SetProperty(ref _unreadCount, value))
                OnPropertyChanged(nameof(HasUnread));
        }
    }

    public bool HasUnread => UnreadCount > 0;
    public bool HasMorePages => _hasMorePages;

    public async Task LoadNotificationsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        _currentPage = 1;

        try
        {
            var response = await _notificationService.GetNotificationsAsync(_currentPage, 20);
            Notifications.Clear();

            if (response?.Notifications != null)
            {
                foreach (var notification in response.Notifications)
                {
                    Notifications.Add(notification);
                }
                UnreadCount = response.UnreadCount;
                _hasMorePages = Notifications.Count >= 20;
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificationsViewModel] Load error: {ex}");
            ErrorMessage = "Bildirimler yüklenirken hata oluştu";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMoreAsync()
    {
        if (IsBusy || !_hasMorePages) return;

        IsBusy = true;
        _currentPage++;

        try
        {
            var response = await _notificationService.GetNotificationsAsync(_currentPage, 20);
            if (response?.Notifications != null)
            {
                foreach (var notification in response.Notifications)
                {
                    Notifications.Add(notification);
                }
                _hasMorePages = response.Notifications.Count() >= 20;
            }
            else
            {
                _hasMorePages = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificationsViewModel] LoadMore error: {ex}");
            _currentPage--;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task MarkAllAsReadAsync()
    {
        try
        {
            await _notificationService.MarkAllAsReadAsync();
            foreach (var notification in Notifications)
            {
                notification.IsRead = true;
            }
            UnreadCount = 0;
            await LoadNotificationsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificationsViewModel] MarkAllRead error: {ex}");
        }
    }

    private async Task OnNotificationTapped(NotificationItem? notification)
    {
        if (notification == null) return;

        if (!notification.IsRead)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(notification.Id);
                notification.IsRead = true;
                UnreadCount = Math.Max(0, UnreadCount - 1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NotificationsViewModel] MarkRead error: {ex}");
            }
        }

        switch (notification.Action)
        {
            case "navigate_schedule":
                await Shell.Current.GoToAsync("//main/schedules");
                break;
            case "navigate_violations":
                await Shell.Current.GoToAsync("//main/violations");
                break;
        }
    }
}
