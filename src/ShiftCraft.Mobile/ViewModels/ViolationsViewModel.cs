using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class ViolationsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private string _errorMessage = string.Empty;

    public ViolationsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Kural İhlalleri";
        Violations = new ObservableCollection<RuleViolation>();
        RefreshCommand = new Command(async () => await LoadViolationsAsync());
    }

    public ObservableCollection<RuleViolation> Violations { get; }
    public ICommand RefreshCommand { get; }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public async Task LoadViolationsAsync()
    {
        if (IsBusy) return;
        
        IsBusy = true;
        ErrorMessage = string.Empty;
        
        try
        {
            Violations.Clear();
            var violations = await _apiService.GetRuleViolationsAsync();
            foreach (var v in violations)
            {
                Violations.Add(v);
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ViolationsViewModel] Network error: {ex}");
            ErrorMessage = "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ViolationsViewModel] Load error: {ex}");
            ErrorMessage = "İhlaller yüklenirken hata oluştu";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
