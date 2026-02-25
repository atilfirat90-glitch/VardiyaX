using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public class DashboardService : IDashboardService
{
    private readonly IApiClient _apiClient;
    private readonly IToastService _toastService;

    public DashboardService(IApiClient apiClient, IToastService toastService)
    {
        _apiClient = apiClient;
        _toastService = toastService;
    }

    public async Task<DashboardData?> GetDashboardAsync(int businessId)
    {
        try
        {
            var endpoint = $"{ApiSettings.Endpoints.Dashboard}/{businessId}";
            return await _apiClient.GetAsync<DashboardData>(endpoint);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DashboardService] GetDashboard error: {ex.Message}");
            await _toastService.ShowErrorAsync("Panel verileri yüklenemedi");
            return null;
        }
    }
}
