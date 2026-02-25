using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public class ShiftSwapService : IShiftSwapService
{
    private readonly IApiClient _apiClient;
    private readonly IToastService _toastService;

    public ShiftSwapService(IApiClient apiClient, IToastService toastService)
    {
        _apiClient = apiClient;
        _toastService = toastService;
    }

    public async Task<List<ShiftSwapRequestModel>> GetPendingByBusinessAsync(int businessId)
    {
        try
        {
            var endpoint = $"{ApiSettings.Endpoints.ShiftSwapBusiness}/{businessId}";
            var result = await _apiClient.GetAsync<List<ShiftSwapRequestModel>>(endpoint);
            return result ?? new List<ShiftSwapRequestModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftSwapService] GetPendingByBusiness error: {ex.Message}");
            await _toastService.ShowErrorAsync("Takas talepleri yüklenemedi");
            return new List<ShiftSwapRequestModel>();
        }
    }

    public async Task<List<ShiftSwapRequestModel>> GetByEmployeeAsync(int employeeId)
    {
        try
        {
            var endpoint = $"{ApiSettings.Endpoints.ShiftSwapEmployee}/{employeeId}";
            var result = await _apiClient.GetAsync<List<ShiftSwapRequestModel>>(endpoint);
            return result ?? new List<ShiftSwapRequestModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftSwapService] GetByEmployee error: {ex.Message}");
            await _toastService.ShowErrorAsync("Takas talepleri yüklenemedi");
            return new List<ShiftSwapRequestModel>();
        }
    }

    public async Task<ShiftSwapRequestModel?> CreateSwapAsync(CreateSwapRequestModel request)
    {
        try
        {
            var result = await _apiClient.PostAsync<ShiftSwapRequestModel>(ApiSettings.Endpoints.ShiftSwap, request);
            if (result != null)
                await _toastService.ShowSuccessAsync("Takas talebi oluşturuldu");
            return result;
        }
        catch (ApiException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftSwapService] CreateSwap error: {ex.Message}");
            await _toastService.ShowErrorAsync("Takas talebi oluşturulamadı");
            return null;
        }
    }

    public async Task<bool> ApproveAsync(int id)
    {
        try
        {
            var result = await _apiClient.PostAsync($"{ApiSettings.Endpoints.ShiftSwap}/{id}/approve");
            if (result)
                await _toastService.ShowSuccessAsync("Takas talebi onaylandı");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftSwapService] Approve error: {ex.Message}");
            await _toastService.ShowErrorAsync("Onaylama başarısız");
            return false;
        }
    }

    public async Task<bool> RejectAsync(int id)
    {
        try
        {
            var result = await _apiClient.PostAsync($"{ApiSettings.Endpoints.ShiftSwap}/{id}/reject");
            if (result)
                await _toastService.ShowSuccessAsync("Takas talebi reddedildi");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftSwapService] Reject error: {ex.Message}");
            await _toastService.ShowErrorAsync("Reddetme başarısız");
            return false;
        }
    }

    public async Task<bool> CancelAsync(int id)
    {
        try
        {
            var result = await _apiClient.PostAsync($"{ApiSettings.Endpoints.ShiftSwap}/{id}/cancel");
            if (result)
                await _toastService.ShowSuccessAsync("Takas talebi iptal edildi");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftSwapService] Cancel error: {ex.Message}");
            await _toastService.ShowErrorAsync("İptal başarısız");
            return false;
        }
    }
}
