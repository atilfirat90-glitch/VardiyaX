using CommunityToolkit.Maui.Core;

namespace ShiftCraft.Mobile.Services;

public interface IToastService
{
    Task ShowToastAsync(string message, ToastDuration duration = ToastDuration.Short);
    Task ShowErrorAsync(string message);
    Task ShowSuccessAsync(string message);
}
