namespace ShiftCraft.Mobile.Helpers;

public static class SafeFireAndForget
{
    public static async void SafeFireAndForgetAsync(this Task task, Action<Exception>? onException = null)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SafeFireAndForget] Exception: {ex}");
            onException?.Invoke(ex);
        }
    }

    public static async void SafeFireAndForgetAsync<T>(this Task<T> task, Action<Exception>? onException = null)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SafeFireAndForget] Exception: {ex}");
            onException?.Invoke(ex);
        }
    }
}
