namespace ShiftCraft.Mobile.Services;

public class ConnectivityService : IConnectivityService, IDisposable
{
    public event EventHandler<bool>? ConnectivityChanged;

    public ConnectivityService()
    {
        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    public bool IsOnline => Connectivity.NetworkAccess == NetworkAccess.Internet;

    public Task<bool> CheckConnectivityAsync()
    {
        return Task.FromResult(IsOnline);
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        var isOnline = e.NetworkAccess == NetworkAccess.Internet;
        ConnectivityChanged?.Invoke(this, isOnline);
    }

    public void Dispose()
    {
        Connectivity.ConnectivityChanged -= OnConnectivityChanged;
    }
}
