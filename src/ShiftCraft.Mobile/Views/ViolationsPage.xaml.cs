using ShiftCraft.Mobile.ViewModels;

namespace ShiftCraft.Mobile.Views;

public partial class ViolationsPage : ContentPage
{
    private readonly ViolationsViewModel _viewModel;

    public ViolationsPage(ViolationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadViolationsAsync();
    }
}
