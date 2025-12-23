using ShiftCraft.Mobile.ViewModels;

namespace ShiftCraft.Mobile.Views;

public partial class UserDetailPage : ContentPage
{
    public UserDetailPage(UserDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
