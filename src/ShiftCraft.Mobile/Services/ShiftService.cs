using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

/// <summary>
/// Shift management service implementation.
/// v1.4 - Shift Create &amp; Schedule View
/// </summary>
public class ShiftService : IShiftService
{
    private readonly IApiClient _apiClient;
    private readonly IToastService _toastService;

    public ShiftService(IApiClient apiClient, IToastService toastService)
    {
        _apiClient = apiClient;
        _toastService = toastService;
    }

    public async Task<ShiftResponseDto?> CreateShiftAsync(CreateShiftRequest request)
    {
        try
        {
            var created = await _apiClient.PostAsync<ShiftResponseDto>(ApiSettings.Endpoints.ShiftCreate, request);
            
            if (created != null)
            {
                await _toastService.ShowSuccessAsync("Vardiya oluşturuldu");
            }
            
            return created;
        }
        catch (ApiException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftService] CreateShiftAsync error: {ex.Message}");
            await _toastService.ShowErrorAsync("Vardiya oluşturulamadı");
            return null;
        }
    }

    public async Task<List<ShiftResponseDto>> GetShiftsByDateRangeAsync(int employeeId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var endpoint = $"{ApiSettings.Endpoints.ShiftDateRange}?employeeId={employeeId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            var shifts = await _apiClient.GetAsync<List<ShiftResponseDto>>(endpoint);
            return shifts ?? new List<ShiftResponseDto>();
        }
        catch (ApiException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
            return new List<ShiftResponseDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftService] GetShiftsByDateRangeAsync error: {ex.Message}");
            await _toastService.ShowErrorAsync("Vardiyalar yüklenemedi");
            return new List<ShiftResponseDto>();
        }
    }

    public async Task<List<ShiftResponseDto>> GetBusinessShiftsByDateAsync(int businessId, DateTime date)
    {
        try
        {
            var endpoint = $"{ApiSettings.Endpoints.ShiftAssignments}?businessId={businessId}&date={date:yyyy-MM-dd}";
            var shifts = await _apiClient.GetAsync<List<ShiftResponseDto>>(endpoint);
            return shifts ?? new List<ShiftResponseDto>();
        }
        catch (ApiException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
            return new List<ShiftResponseDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftService] GetBusinessShiftsByDateAsync error: {ex.Message}");
            await _toastService.ShowErrorAsync("Vardiyalar yüklenemedi");
            return new List<ShiftResponseDto>();
        }
    }
}
