using ShiftCraft.Mobile.ViewModels;
using ShiftCraft.Mobile.Helpers;

namespace ShiftCraft.Mobile.Views;

public partial class NotificationPreferencesPage : ContentPage
{
    private readonly NotificationPreferencesViewModel _viewModel;

    public NotificationPreferencesPage(NotificationPreferencesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDataSafeAsync().SafeFireAndForgetAsync(ex => 
            System.Diagnostics.Debug.WriteLine($"[NotificationPreferencesPage] OnAppearing error: {ex}"));
    }

    private async Task LoadDataSafeAsync()
    {
        try
        {
            await _viewModel.LoadPreferencesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NotificationPreferencesPage] Load error: {ex}");
            _viewModel.ErrorMessage = "Bildirim ayarları yüklenirken hata oluştu";
        }
    }
}
