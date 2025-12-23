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
        try
        {
            var user = await _userService.GetUserAsync(id);
            Username = user.Username;
            SelectedRole = user.Role;
            IsActive = user.IsActive;
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

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Kullanıcı adı gereklidir.";
            return;
        }

        if (!_isEditMode && string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Şifre gereklidir.";
            return;
        }

        if (!_isEditMode && (Password.Length < 8 || !Password.Any(char.IsDigit)))
        {
            ErrorMessage = "Şifre en az 8 karakter ve 1 rakam içermelidir.";
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
}
