using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ShiftCraft.Mobile.Services;

public class ToastService : IToastService
{
    public async Task ShowToastAsync(string message, ToastDuration duration = ToastDuration.Short)
    {
        try
        {
            var toast = Toast.Make(message, duration, 14);
            await toast.Show();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Toast error: {ex.Message}");
        }
    }

    public async Task ShowErrorAsync(string message)
    {
        await ShowToastAsync($"❌ {message}", ToastDuration.Long);
    }

    public async Task ShowSuccessAsync(string message)
    {
        await ShowToastAsync($"✓ {message}", ToastDuration.Short);
    }
}
