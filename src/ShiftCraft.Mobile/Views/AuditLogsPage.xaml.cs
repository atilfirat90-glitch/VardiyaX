using ShiftCraft.Mobile.ViewModels;
using ShiftCraft.Mobile.Helpers;

namespace ShiftCraft.Mobile.Views;

public partial class AuditLogsPage : ContentPage
{
    private readonly AuditLogsViewModel _viewModel;

    public AuditLogsPage(AuditLogsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDataSafeAsync().SafeFireAndForgetAsync(ex => 
            System.Diagnostics.Debug.WriteLine($"[AuditLogsPage] OnAppearing error: {ex}"));
    }

    private async Task LoadDataSafeAsync()
    {
        try
        {
            await _viewModel.LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AuditLogsPage] Load error: {ex}");
            _viewModel.ErrorMessage = "Denetim günlükleri yüklenirken hata oluştu";
        }
    }

    private void OnLoginTabClicked(object sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 0;
    }

    private void OnPublishTabClicked(object sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 1;
    }

    private void OnViolationsTabClicked(object sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 2;
    }
}
