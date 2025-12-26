using System.Windows.Input;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class NotificationPreferencesViewModel : BaseViewModel
{
    private readonly IPushNotificationHandler _pushHandler;
    private bool _scheduleNotificationsEnabled = true;
    private bool _violationNotificationsEnabled = true;
    private bool _shiftRemindersEnabled = true;
    private int _reminderHoursBefore = 24;
    private string _errorMessage = string.Empty;

    public NotificationPreferencesViewModel(IPushNotificationHandler pushHandler)
    {
        _pushHandler = pushHandler;
        Title = "Bildirim Ayarları";
        SaveCommand = new Command(async () => await SaveAsync());
    }

    public bool ScheduleNotificationsEnabled
    {
        get => _scheduleNotificationsEnabled;
        set => SetProperty(ref _scheduleNotificationsEnabled, value);
    }

    public bool ViolationNotificationsEnabled
    {
        get => _violationNotificationsEnabled;
        set => SetProperty(ref _violationNotificationsEnabled, value);
    }

    public bool ShiftRemindersEnabled
    {
        get => _shiftRemindersEnabled;
        set => SetProperty(ref _shiftRemindersEnabled, value);
    }

    public int ReminderHoursBefore
    {
        get => _reminderHoursBefore;
        set => SetProperty(ref _reminderHoursBefore, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public List<int> ReminderOptions { get; } = new() { 1, 2, 4, 12, 24, 48 };

    public ICommand SaveCommand { get; }

    public async Task LoadPreferencesAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        
        try
        {
            var prefs = await _pushHandler.GetPreferencesAsync();
            ScheduleNotificationsEnabled = prefs.ScheduleNotificationsEnabled;
            ViolationNotificationsEnabled = prefs.ViolationNotificationsEnabled;
            ShiftRemindersEnabled = prefs.ShiftRemindersEnabled;
            ReminderHoursBefore = prefs.ReminderHoursBefore;
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificationPreferencesViewModel] Network error: {ex}");
            ErrorMessage = "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificationPreferencesViewModel] Load error: {ex}");
            ErrorMessage = "Bildirim ayarları yüklenirken hata oluştu";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var prefs = new NotificationPreferences
            {
                ScheduleNotificationsEnabled = ScheduleNotificationsEnabled,
                ViolationNotificationsEnabled = ViolationNotificationsEnabled,
                ShiftRemindersEnabled = ShiftRemindersEnabled,
                ReminderHoursBefore = ReminderHoursBefore
            };

            await _pushHandler.UpdatePreferencesAsync(prefs);
            await Shell.Current.DisplayAlert("Başarılı", "Bildirim ayarları kaydedildi.", "Tamam");
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificationPreferencesViewModel] Network error: {ex}");
            ErrorMessage = "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificationPreferencesViewModel] Save error: {ex}");
            ErrorMessage = "Ayarlar kaydedilirken hata oluştu";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
