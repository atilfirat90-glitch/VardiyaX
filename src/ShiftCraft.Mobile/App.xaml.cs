using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private static IToastService? _toastService;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _toastService = serviceProvider.GetService<IToastService>();
        
        // Global exception handlers
        SetupGlobalExceptionHandling();
    }

    private void SetupGlobalExceptionHandling()
    {
        // Handle exceptions from async void methods on main thread
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        
        // Handle exceptions from Task-based async methods
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        System.Diagnostics.Debug.WriteLine($"[FATAL] Unhandled Exception: {exception}");
        
        // Try to show toast on main thread
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await (_toastService?.ShowErrorAsync("Beklenmeyen bir hata oluştu") ?? Task.CompletedTask);
            }
            catch { /* Ignore toast errors */ }
        });
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR] Unobserved Task Exception: {e.Exception}");
        e.SetObserved(); // Prevent app crash
        
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await (_toastService?.ShowErrorAsync("İşlem sırasında hata oluştu") ?? Task.CompletedTask);
            }
            catch { /* Ignore toast errors */ }
        });
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        return new Window(shell);
    }
}
