using ShiftCraft.Mobile.ViewModels;

namespace ShiftCraft.Mobile.Views;

public partial class EmployeesPage : ContentPage
{
    private readonly EmployeesViewModel _viewModel;

    public EmployeesPage(EmployeesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadEmployeesAsync();
    }
}
