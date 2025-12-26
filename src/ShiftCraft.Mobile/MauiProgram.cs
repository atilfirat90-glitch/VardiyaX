using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ShiftCraft.Mobile.Services;
using ShiftCraft.Mobile.ViewModels;
using ShiftCraft.Mobile.Views;

namespace ShiftCraft.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // HttpClient with timeout
        builder.Services.AddSingleton(sp => new HttpClient { Timeout = TimeSpan.FromSeconds(30) });
        
        // v1.1: Centralized API Client
        builder.Services.AddSingleton<IApiClient, ApiClient>();
        
        // Services (Singleton for auth state)
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IAuditService, AuditService>();
        builder.Services.AddSingleton<IPushNotificationHandler, PushNotificationHandler>();
        builder.Services.AddSingleton<ICacheService, CacheService>();
        builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
        builder.Services.AddSingleton<IOfflineModeService, OfflineModeService>();
        builder.Services.AddSingleton<IToastService, ToastService>();
        
        // v1.2: Employee Management Service
        builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
        
        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<EmployeesViewModel>();
        builder.Services.AddTransient<SchedulesViewModel>();
        builder.Services.AddTransient<ViolationsViewModel>();
        builder.Services.AddTransient<UsersViewModel>();
        builder.Services.AddTransient<UserDetailViewModel>();
        builder.Services.AddTransient<AuditLogsViewModel>();
        builder.Services.AddTransient<NotificationPreferencesViewModel>();
        
        // v1.2: Employee Management ViewModels
        builder.Services.AddTransient<EmployeeManageViewModel>();
        builder.Services.AddTransient<EmployeeEditViewModel>();
        
        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<EmployeesPage>();
        builder.Services.AddTransient<SchedulesPage>();
        builder.Services.AddTransient<ViolationsPage>();
        builder.Services.AddTransient<UsersPage>();
        builder.Services.AddTransient<UserDetailPage>();
        builder.Services.AddTransient<AuditLogsPage>();
        builder.Services.AddTransient<NotificationPreferencesPage>();
        
        // v1.2: Employee Management Pages
        builder.Services.AddTransient<EmployeeManagePage>();
        builder.Services.AddTransient<EmployeeEditPage>();
        
        // Shell
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
