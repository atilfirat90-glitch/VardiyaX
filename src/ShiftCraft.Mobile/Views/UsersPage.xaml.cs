using ShiftCraft.Mobile.ViewModels;
using ShiftCraft.Mobile.Helpers;

namespace ShiftCraft.Mobile.Views;

public partial class UsersPage : ContentPage
{
    private readonly UsersViewModel _viewModel;

    public UsersPage(UsersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDataSafeAsync().SafeFireAndForgetAsync(ex => 
            System.Diagnostics.Debug.WriteLine($"[UsersPage] OnAppearing error: {ex}"));
    }

    private async Task LoadDataSafeAsync()
    {
        try
        {
            await _viewModel.LoadUsersAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UsersPage] Load error: {ex}");
            _viewModel.ErrorMessage = "Kullanıcılar yüklenirken hata oluştu";
        }
    }
}
