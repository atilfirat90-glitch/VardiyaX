using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class ShiftSwapViewModel : BaseViewModel
{
    private readonly IShiftSwapService _swapService;
    private readonly IAuthService _authService;

    public ShiftSwapViewModel(IShiftSwapService swapService, IAuthService authService)
    {
        _swapService = swapService;
        _authService = authService;
        Title = "Vardiya Takas";
        SwapRequests = new ObservableCollection<ShiftSwapRequestModel>();
        LoadCommand = new Command(async () => await LoadAsync());
        ApproveCommand = new Command<int>(async (id) => await ApproveAsync(id));
        RejectCommand = new Command<int>(async (id) => await RejectAsync(id));
        CancelCommand = new Command<int>(async (id) => await CancelAsync(id));
    }

    public ICommand LoadCommand { get; }
    public ICommand ApproveCommand { get; }
    public ICommand RejectCommand { get; }
    public ICommand CancelCommand { get; }
    public ObservableCollection<ShiftSwapRequestModel> SwapRequests { get; }

    public bool IsManager => _authService.IsManager;

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            SwapRequests.Clear();
            var requests = await _swapService.GetPendingByBusinessAsync(1);
            foreach (var req in requests)
            {
                SwapRequests.Add(req);
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftSwapViewModel] Load error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ApproveAsync(int id)
    {
        var confirm = await Shell.Current.DisplayAlert("Onay", "Bu takas talebini onaylamak istiyor musunuz?", "Evet", "Hayır");
        if (!confirm) return;

        var result = await _swapService.ApproveAsync(id);
        if (result) await LoadAsync();
    }

    private async Task RejectAsync(int id)
    {
        var confirm = await Shell.Current.DisplayAlert("Reddet", "Bu takas talebini reddetmek istiyor musunuz?", "Evet", "Hayır");
        if (!confirm) return;

        var result = await _swapService.RejectAsync(id);
        if (result) await LoadAsync();
    }

    private async Task CancelAsync(int id)
    {
        var confirm = await Shell.Current.DisplayAlert("İptal", "Bu takas talebini iptal etmek istiyor musunuz?", "Evet", "Hayır");
        if (!confirm) return;

        var result = await _swapService.CancelAsync(id);
        if (result) await LoadAsync();
    }
}
