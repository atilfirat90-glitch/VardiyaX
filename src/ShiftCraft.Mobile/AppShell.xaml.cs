using ShiftCraft.Mobile.Services;
using ShiftCraft.Mobile.Views;

namespace ShiftCraft.Mobile;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;

    public AppShell(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("employees", typeof(EmployeesPage));
        Routing.RegisterRoute("schedules", typeof(SchedulesPage));
        Routing.RegisterRoute("violations", typeof(ViolationsPage));
        Routing.RegisterRoute("users", typeof(UsersPage));
        Routing.RegisterRoute("userdetail", typeof(UserDetailPage));
        Routing.RegisterRoute("auditlogs", typeof(AuditLogsPage));
        Routing.RegisterRoute("notifications", typeof(NotificationPreferencesPage));
        
        // v1.2: Employee Management routes
        Routing.RegisterRoute("employeemanage", typeof(EmployeeManagePage));
        Routing.RegisterRoute("employeeedit", typeof(EmployeeEditPage));
        
        // Update header when navigating
        Navigated += OnNavigated;
    }

    private void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        UpdateUserInfo();
    }

    private void UpdateUserInfo()
    {
        if (_authService.IsAuthenticated)
        {
            UserNameLabel.Text = _authService.Username ?? "VardiyaX";
            UserRoleLabel.Text = _authService.IsManager ? "Yönetici" : "Çalışan";
        }
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Çıkış", "Çıkış yapmak istediğinize emin misiniz?", "Evet", "Hayır");
        if (confirm)
        {
            await _authService.LogoutAsync();
            await GoToAsync("//login");
        }
    }
}
