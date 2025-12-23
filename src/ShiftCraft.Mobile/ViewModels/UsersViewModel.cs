using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class UsersViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private string _errorMessage = string.Empty;

    public UsersViewModel(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
        Title = "Kullanıcılar";
        Users = new ObservableCollection<UserDto>();
        RefreshCommand = new Command(async () => await LoadUsersAsync());
        AddUserCommand = new Command(async () => await NavigateToAddUser());
        EditUserCommand = new Command<UserDto>(async (user) => await NavigateToEditUser(user));
    }

    public ObservableCollection<UserDto> Users { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AddUserCommand { get; }
    public ICommand EditUserCommand { get; }

    public bool IsAdmin => _authService.Role == "Admin";

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public async Task LoadUsersAsync()
    {
        if (IsBusy) return;

        // Admin-only access check
        if (!IsAdmin)
        {
            ErrorMessage = "Bu sayfaya erişim yetkiniz yok.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            Users.Clear();
            var users = await _userService.GetUsersAsync();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
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

    private async Task NavigateToAddUser()
    {
        await Shell.Current.GoToAsync("userdetail");
    }

    private async Task NavigateToEditUser(UserDto user)
    {
        await Shell.Current.GoToAsync($"userdetail?userId={user.Id}");
    }
}
