using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public class TimeOffService : ITimeOffService
{
    private readonly IApiClient _apiClient;
    private readonly IToastService _toastService;

    public TimeOffService(IApiClient apiClient, IToastService toastService)
    {
        _apiClient = apiClient;
        _toastService = toastService;
    }

    public async Task<List<TimeOffRequestModel>> GetByEmployeeAsync(int employeeId)
    {
        try
        {
            var endpoint = $"{ApiSettings.Endpoints.TimeOffEmployee}/{employeeId}";
            var result = await _apiClient.GetAsync<List<TimeOffRequestModel>>(endpoint);
            return result ?? new List<TimeOffRequestModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TimeOffService] GetByEmployee error: {ex.Message}");
            await _toastService.ShowErrorAsync("İzin talepleri yüklenemedi");
            return new List<TimeOffRequestModel>();
        }
    }

    public async Task<List<TimeOffRequestModel>> GetPendingByBusinessAsync(int businessId)
    {
        try
        {
            var endpoint = $"{ApiSettings.Endpoints.TimeOffPending}/{businessId}/pending";
            var result = await _apiClient.GetAsync<List<TimeOffRequestModel>>(endpoint);
            return result ?? new List<TimeOffRequestModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TimeOffService] GetPending error: {ex.Message}");
            await _toastService.ShowErrorAsync("Bekleyen talepler yüklenemedi");
            return new List<TimeOffRequestModel>();
        }
    }

    public async Task<TimeOffRequestModel?> CreateAsync(CreateTimeOffRequestModel request)
    {
        try
        {
            var result = await _apiClient.PostAsync<TimeOffRequestModel>(ApiSettings.Endpoints.TimeOff, request);
            if (result != null)
                await _toastService.ShowSuccessAsync("İzin talebi oluşturuldu");
            return result;
        }
        catch (ApiException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TimeOffService] Create error: {ex.Message}");
            await _toastService.ShowErrorAsync("İzin talebi oluşturulamadı");
            return null;
        }
    }

    public async Task<bool> ApproveAsync(int id)
    {
        try
        {
            var result = await _apiClient.PostAsync($"{ApiSettings.Endpoints.TimeOff}/{id}/approve");
            if (result)
                await _toastService.ShowSuccessAsync("İzin talebi onaylandı");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TimeOffService] Approve error: {ex.Message}");
            await _toastService.ShowErrorAsync("Onaylama başarısız");
            return false;
        }
    }

    public async Task<bool> RejectAsync(int id, string? responseNote = null)
    {
        try
        {
            var result = await _apiClient.PostAsync($"{ApiSettings.Endpoints.TimeOff}/{id}/reject", new { responseNote });
            if (result)
                await _toastService.ShowSuccessAsync("İzin talebi reddedildi");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TimeOffService] Reject error: {ex.Message}");
            await _toastService.ShowErrorAsync("Reddetme başarısız");
            return false;
        }
    }
}
