using ShiftCraft.Mobile.ViewModels;

namespace ShiftCraft.Mobile.Views;

public partial class ScheduleViewPage : ContentPage
{
    private readonly ScheduleViewViewModel _viewModel;

    public ScheduleViewPage(ScheduleViewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadShiftsAsync();
    }
}
