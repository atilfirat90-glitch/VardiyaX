using ShiftCraft.Mobile.ViewModels;

namespace ShiftCraft.Mobile.Views;

public partial class SchedulesPage : ContentPage
{
    private readonly SchedulesViewModel _viewModel;

    public SchedulesPage(SchedulesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSchedulesAsync();
    }
}
