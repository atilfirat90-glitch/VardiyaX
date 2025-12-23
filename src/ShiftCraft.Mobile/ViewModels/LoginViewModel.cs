using System.Windows.Input;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isOffline;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Giriş";
        LoginCommand = new Command(async () => await LoginAsync());
        CheckConnectivityCommand = new Command(async () => await CheckConnectivityAsync());
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsOffline
    {
        get => _isOffline;
        set => SetProperty(ref _isOffline, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand CheckConnectivityCommand { get; }

    public async Task InitializeAsync()
    {
        // Try to restore previous session
        if (await _authService.TryRestoreSessionAsync())
        {
            await Shell.Current.GoToAsync("//main");
        }
        
        await CheckConnectivityAsync();
    }

    private async Task CheckConnectivityAsync()
    {
        var current = Connectivity.Current.NetworkAccess;
        IsOffline = current != NetworkAccess.Internet;
        
        if (IsOffline)
        {
            ErrorMessage = "İnternet bağlantısı yok";
        }
        else
        {
            ErrorMessage = string.Empty;
        }
        
        await Task.CompletedTask;
    }

    private async Task LoginAsync()
    {
        if (IsBusy) return;
        
        // Validate inputs
        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Kullanıcı adı gerekli";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Şifre gerekli";
            return;
        }

        // Check connectivity
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            ErrorMessage = "İnternet bağlantısı yok";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _authService.LoginAsync(Username, Password);
            if (result != null)
            {
                // Clear password from memory
                Password = string.Empty;
                
                // Navigate to main page
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                ErrorMessage = "Geçersiz kullanıcı adı veya şifre";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
