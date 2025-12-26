using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class AuditLogsViewModel : BaseViewModel
{
    private readonly IAuditService _auditService;
    private readonly IAuthService _authService;
    private string _errorMessage = string.Empty;
    private int _selectedTabIndex;
    private string _filterUsername = string.Empty;
    private DateTime _filterFrom;
    private DateTime _filterTo;

    public AuditLogsViewModel(IAuditService auditService, IAuthService authService)
    {
        _auditService = auditService;
        _authService = authService;
        Title = "Denetim Günlükleri";
        
        LoginLogs = new ObservableCollection<LoginLogDto>();
        PublishLogs = new ObservableCollection<PublishLogDto>();
        ViolationLogs = new ObservableCollection<ViolationLogDto>();
        
        _filterFrom = DateTime.Today.AddDays(-30);
        _filterTo = DateTime.Today;
        
        RefreshCommand = new Command(async () => await LoadDataAsync());
        LoadMoreCommand = new Command(async () => await LoadMoreAsync());
        AcknowledgeCommand = new Command<ViolationLogDto>(async (v) => await AcknowledgeAsync(v));
        ApplyFilterCommand = new Command(async () => await LoadDataAsync());
    }

    public ObservableCollection<LoginLogDto> LoginLogs { get; }
    public ObservableCollection<PublishLogDto> PublishLogs { get; }
    public ObservableCollection<ViolationLogDto> ViolationLogs { get; }

    public ICommand RefreshCommand { get; }
    public ICommand LoadMoreCommand { get; }
    public ICommand AcknowledgeCommand { get; }
    public ICommand ApplyFilterCommand { get; }

    public bool IsManager => _authService.IsManager;

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (SetProperty(ref _selectedTabIndex, value))
            {
                _ = LoadDataAsync();
            }
        }
    }

    public string FilterUsername
    {
        get => _filterUsername;
        set => SetProperty(ref _filterUsername, value);
    }

    public DateTime FilterFrom
    {
        get => _filterFrom;
        set => SetProperty(ref _filterFrom, value);
    }

    public DateTime FilterTo
    {
        get => _filterTo;
        set => SetProperty(ref _filterTo, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private int _currentPage = 1;
    private bool _hasMoreData = true;

    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        _currentPage = 1;
        _hasMoreData = true;

        try
        {
            switch (_selectedTabIndex)
            {
                case 0:
                    await LoadLoginLogsAsync(true);
                    break;
                case 1:
                    await LoadPublishLogsAsync(true);
                    break;
                case 2:
                    await LoadViolationLogsAsync(true);
                    break;
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuditLogsViewModel] Network error: {ex}");
            ErrorMessage = "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuditLogsViewModel] Load error: {ex}");
            ErrorMessage = "Denetim günlükleri yüklenirken hata oluştu";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMoreAsync()
    {
        if (IsBusy || !_hasMoreData) return;

        _currentPage++;
        IsBusy = true;

        try
        {
            switch (_selectedTabIndex)
            {
                case 0:
                    await LoadLoginLogsAsync(false);
                    break;
                case 1:
                    await LoadPublishLogsAsync(false);
                    break;
                case 2:
                    await LoadViolationLogsAsync(false);
                    break;
            }
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuditLogsViewModel] Network error: {ex}");
            ErrorMessage = "Bağlantı hatası";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuditLogsViewModel] LoadMore error: {ex}");
            ErrorMessage = "Daha fazla veri yüklenirken hata oluştu";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadLoginLogsAsync(bool clear)
    {
        var filter = new LoginLogFilter
        {
            Username = string.IsNullOrWhiteSpace(FilterUsername) ? null : FilterUsername,
            From = FilterFrom,
            To = FilterTo,
            Page = _currentPage
        };

        var result = await _auditService.GetLoginLogsAsync(filter);
        
        if (clear) LoginLogs.Clear();
        
        foreach (var log in result.Items)
        {
            LoginLogs.Add(log);
        }

        _hasMoreData = _currentPage < result.TotalPages;
    }

    private async Task LoadPublishLogsAsync(bool clear)
    {
        var filter = new PublishLogFilter
        {
            Publisher = string.IsNullOrWhiteSpace(FilterUsername) ? null : FilterUsername,
            From = FilterFrom,
            To = FilterTo,
            Page = _currentPage
        };

        var result = await _auditService.GetPublishLogsAsync(filter);
        
        if (clear) PublishLogs.Clear();
        
        foreach (var log in result.Items)
        {
            PublishLogs.Add(log);
        }

        _hasMoreData = _currentPage < result.TotalPages;
    }

    private async Task LoadViolationLogsAsync(bool clear)
    {
        var filter = new ViolationFilter
        {
            From = FilterFrom,
            To = FilterTo,
            Page = _currentPage
        };

        var result = await _auditService.GetViolationHistoryAsync(filter);
        
        if (clear) ViolationLogs.Clear();
        
        foreach (var log in result.Items)
        {
            ViolationLogs.Add(log);
        }

        _hasMoreData = _currentPage < result.TotalPages;
    }

    private async Task AcknowledgeAsync(ViolationLogDto? violation)
    {
        if (violation == null || violation.IsAcknowledged) return;

        try
        {
            await _auditService.AcknowledgeViolationAsync(violation.Id);
            violation.IsAcknowledged = true;
            violation.AcknowledgedAt = DateTime.Now;
            
            // Refresh the list to update UI
            var index = ViolationLogs.IndexOf(violation);
            if (index >= 0)
            {
                ViolationLogs.RemoveAt(index);
                ViolationLogs.Insert(index, violation);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuditLogsViewModel] Acknowledge error: {ex}");
            ErrorMessage = "İşlem sırasında hata oluştu";
        }
    }
}
