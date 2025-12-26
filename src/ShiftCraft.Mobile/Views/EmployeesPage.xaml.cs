using ShiftCraft.Mobile.ViewModels;
using ShiftCraft.Mobile.Helpers;

namespace ShiftCraft.Mobile.Views;

public partial class EmployeesPage : ContentPage
{
    private readonly EmployeesViewModel _viewModel;

    public EmployeesPage(EmployeesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDataSafeAsync().SafeFireAndForgetAsync(ex => 
            System.Diagnostics.Debug.WriteLine($"[EmployeesPage] OnAppearing error: {ex}"));
    }

    private async Task LoadDataSafeAsync()
    {
        try
        {
            await _viewModel.LoadEmployeesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EmployeesPage] Load error: {ex}");
            _viewModel.ErrorMessage = "Çalışanlar yüklenirken hata oluştu";
        }
    }
}
