using ShiftCraft.Mobile.ViewModels;
using ShiftCraft.Mobile.Helpers;

namespace ShiftCraft.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDataSafeAsync().SafeFireAndForgetAsync(ex => 
            System.Diagnostics.Debug.WriteLine($"[LoginPage] OnAppearing error: {ex}"));
    }

    private async Task LoadDataSafeAsync()
    {
        try
        {
            await _viewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[LoginPage] Initialize error: {ex}");
        }
    }
}
