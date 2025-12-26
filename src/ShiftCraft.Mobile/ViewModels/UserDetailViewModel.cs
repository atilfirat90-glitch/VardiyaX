using System.Windows.Input;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

[QueryProperty(nameof(UserId), "userId")]
public class UserDetailViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private int? _userId;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _selectedRole = "Worker";
    private bool _isActive = true;
    private string _errorMessage = string.Empty;
    private bool _isEditMode;
    private string _temporaryPassword = string.Empty;

    public UserDetailViewModel(IUserService userService)
    {
        _userService = userService;
        Title = "Yeni Kullanıcı";
        SaveCommand = new Command(async () => await SaveAsync());
        ResetPasswordCommand = new Command(async () => await ResetPasswordAsync());
        DeactivateCommand = new Command(async () => await DeactivateAsync());
    }

    public string UserId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _userId = id;
                _isEditMode = true;
                Title = "Kullanıcı Düzenle";
                _ = LoadUserAsync(id);
            }
        }
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

    public string SelectedRole
    {
        get => _selectedRole;
        set => SetProperty(ref _selectedRole, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public string TemporaryPassword
    {
        get => _temporaryPassword;
        set => SetProperty(ref _temporaryPassword, value);
    }

    public List<string> Roles { get; } = new() { "Admin", "Manager", "Worker", "Trainee" };

    public ICommand SaveCommand { get; }
    public ICommand ResetPasswordCommand { get; }
    public ICommand DeactivateCommand { get; }

    private async Task LoadUserAsync(int id)
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            System.Diagnostics.Debug.WriteLine($"[UserDetailViewModel] Loading user {id}...");
            var user = await _userService.GetUserAsync(id);
            System.Diagnostics.Debug.WriteLine($"[UserDetailViewModel] User loaded: {user.Username}");
            Username = user.Username;
            SelectedRole = user.Role;
            IsActive = user.IsActive;
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UserDetailViewModel] Network error: {ex}");
            ErrorMessage = "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UserDetailViewModel] Load error: {ex}");
            ErrorMessage = $"Kullanıcı yüklenirken hata: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        // #region agent log
        LogDebug("H1", "SaveAsync start", new Dictionary<string, object?>
        {
            { "isEditMode", _isEditMode },
            { "userId", _userId },
            { "username", Username },
            { "selectedRole", SelectedRole },
            { "passwordLength", Password?.Length ?? 0 }
        });
        // #endregion

        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Kullanıcı adı gereklidir.";
            // #region agent log
            LogDebug("H2", "Username validation failed", new Dictionary<string, object?>
            {
                { "username", Username }
            });
            // #endregion
            return;
        }

        if (!_isEditMode && string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Şifre gereklidir.";
            // #region agent log
            LogDebug("H2", "Password empty validation failed", new Dictionary<string, object?>
            {
                { "username", Username }
            });
            // #endregion
            return;
        }

        // v1.2: Updated validation to match API (5 chars minimum)
        if (!_isEditMode && Password.Length < 5)
        {
            ErrorMessage = "Şifre en az 5 karakter olmalı.";
            // #region agent log
            LogDebug("H2", "Password length validation failed", new Dictionary<string, object?>
            {
                { "username", Username },
                { "passwordLength", Password.Length }
            });
            // #endregion
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            if (_isEditMode && _userId.HasValue)
            {
                await _userService.UpdateUserAsync(_userId.Value, new UpdateUserRequest
                {
                    Role = SelectedRole,
                    IsActive = IsActive
                });
            }
            else
            {
                await _userService.CreateUserAsync(new CreateUserRequest
                {
                    Username = Username,
                    Password = Password,
                    Role = SelectedRole
                });
            }

            // #region agent log
            LogDebug("H1", "SaveAsync success", new Dictionary<string, object?>
            {
                { "isEditMode", _isEditMode },
                { "userId", _userId },
                { "username", Username }
            });
            // #endregion

            await Shell.Current.GoToAsync("..");
        }
        catch (ApiException ex)
        {
            // v1.2: Display specific API error message
            ErrorMessage = ex.Message;
            // #region agent log
            LogDebug("H3", "ApiException", new Dictionary<string, object?>
            {
                { "statusCode", ex.StatusCode },
                { "message", ex.Message }
            });
            // #endregion
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
            // #region agent log
            LogDebug("H4", "HttpRequestException", new Dictionary<string, object?>
            {
                { "username", Username }
            });
            // #endregion
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            // #region agent log
            LogDebug("H5", "Unexpected exception", new Dictionary<string, object?>
            {
                { "message", ex.Message },
                { "username", Username }
            });
            // #endregion
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ResetPasswordAsync()
    {
        if (!_userId.HasValue) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var tempPassword = await _userService.ResetPasswordAsync(_userId.Value);
            TemporaryPassword = tempPassword;
            await Shell.Current.DisplayAlert("Şifre Sıfırlandı", 
                $"Geçici şifre: {tempPassword}", "Tamam");
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

    private async Task DeactivateAsync()
    {
        if (!_userId.HasValue) return;

        var confirm = await Shell.Current.DisplayAlert("Onay", 
            "Bu kullanıcıyı devre dışı bırakmak istediğinizden emin misiniz?", 
            "Evet", "Hayır");

        if (!confirm) return;

        IsBusy = true;
        try
        {
            await _userService.DeactivateUserAsync(_userId.Value);
            await Shell.Current.GoToAsync("..");
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

    // #region agent log
    private static readonly HttpClient _logClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(2)
    };

    private void LogDebug(string hypothesisId, string message, Dictionary<string, object?> data)
    {
        try
        {
            var payload = new
            {
                sessionId = "debug-session",
                runId = "pre-fix",
                hypothesisId,
                location = "UserDetailViewModel.cs",
                message,
                data,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);

            // Preferred: send to host via emulator loopback (10.0.2.2). Fallback to host loopback in dev.
            _ = _logClient.PostAsync("http://10.0.2.2:7242/ingest/7a8f70c6-57ed-41e7-bf68-3f80ee8d777a",
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            _ = _logClient.PostAsync("http://127.0.0.1:7242/ingest/7a8f70c6-57ed-41e7-bf68-3f80ee8d777a",
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

            // Best-effort local append (device-side path for adb pull if needed)
            try
            {
                System.IO.File.AppendAllText("/sdcard/Download/debug.log", json + Environment.NewLine);
            }
            catch { /* ignore secondary failure */ }
        }
        catch
        {
            // avoid throwing from logging
        }
    }
    // #endregion
}
