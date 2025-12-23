using ShiftCraft.Mobile.ViewModels;

namespace ShiftCraft.Mobile.Views;

public partial class AuditLogsPage : ContentPage
{
    private readonly AuditLogsViewModel _viewModel;

    public AuditLogsPage(AuditLogsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }

    private void OnLoginTabClicked(object sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 0;
    }

    private void OnPublishTabClicked(object sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 1;
    }

    private void OnViolationsTabClicked(object sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 2;
    }
}
