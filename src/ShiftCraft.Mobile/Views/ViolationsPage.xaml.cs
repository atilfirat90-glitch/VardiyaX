using ShiftCraft.Mobile.ViewModels;
using ShiftCraft.Mobile.Helpers;

namespace ShiftCraft.Mobile.Views;

public partial class ViolationsPage : ContentPage
{
    private readonly ViolationsViewModel _viewModel;

    public ViolationsPage(ViolationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDataSafeAsync().SafeFireAndForgetAsync(ex => 
            System.Diagnostics.Debug.WriteLine($"[ViolationsPage] OnAppearing error: {ex}"));
    }

    private async Task LoadDataSafeAsync()
    {
        try
        {
            await _viewModel.LoadViolationsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ViolationsPage] Load error: {ex}");
            _viewModel.ErrorMessage = "İhlaller yüklenirken hata oluştu";
        }
    }
}
