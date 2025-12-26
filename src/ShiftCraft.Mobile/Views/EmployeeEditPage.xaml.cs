using ShiftCraft.Mobile.ViewModels;

namespace ShiftCraft.Mobile.Views;

[QueryProperty(nameof(EmployeeId), "id")]
public partial class EmployeeEditPage : ContentPage
{
    private readonly EmployeeEditViewModel _viewModel;

    public EmployeeEditPage(EmployeeEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public string? EmployeeId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _viewModel.EmployeeId = id;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
