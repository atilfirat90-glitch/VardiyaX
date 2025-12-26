using ShiftCraft.Mobile.ViewModels;

namespace ShiftCraft.Mobile.Views;

public partial class EmployeeManagePage : ContentPage
{
    public EmployeeManagePage(EmployeeManageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is EmployeeManageViewModel vm)
        {
            await vm.LoadEmployeesAsync();
        }
    }
}
