using ShiftCraft.Mobile.ViewModels;

namespace ShiftCraft.Mobile.Views;

public partial class ShiftCreatePage : ContentPage
{
    private readonly ShiftCreateViewModel _viewModel;

    public ShiftCreatePage(ShiftCreateViewModel viewModel)
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
