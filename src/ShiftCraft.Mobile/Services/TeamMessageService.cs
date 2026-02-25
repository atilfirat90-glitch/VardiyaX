using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public class TeamMessageService : ITeamMessageService
{
    private readonly IApiClient _apiClient;
    private readonly IToastService _toastService;

    public TeamMessageService(IApiClient apiClient, IToastService toastService)
    {
        _apiClient = apiClient;
        _toastService = toastService;
    }

    public async Task<List<TeamMessageModel>> GetByChannelAsync(string channel, int businessId, int page = 1)
    {
        try
        {
            var endpoint = $"{ApiSettings.Endpoints.TeamMessage}/{channel}?businessId={businessId}&page={page}";
            var result = await _apiClient.GetAsync<List<TeamMessageModel>>(endpoint);
            return result ?? new List<TeamMessageModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TeamMessageService] GetByChannel error: {ex.Message}");
            await _toastService.ShowErrorAsync("Mesajlar yüklenemedi");
            return new List<TeamMessageModel>();
        }
    }

    public async Task<List<TeamMessageModel>> GetAnnouncementsAsync(int businessId)
    {
        try
        {
            var endpoint = $"{ApiSettings.Endpoints.TeamMessageAnnouncements}?businessId={businessId}";
            var result = await _apiClient.GetAsync<List<TeamMessageModel>>(endpoint);
            return result ?? new List<TeamMessageModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TeamMessageService] GetAnnouncements error: {ex.Message}");
            await _toastService.ShowErrorAsync("Duyurular yüklenemedi");
            return new List<TeamMessageModel>();
        }
    }

    public async Task<TeamMessageModel?> SendMessageAsync(SendMessageModel request)
    {
        try
        {
            var result = await _apiClient.PostAsync<TeamMessageModel>(ApiSettings.Endpoints.TeamMessage, request);
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TeamMessageService] SendMessage error: {ex.Message}");
            await _toastService.ShowErrorAsync("Mesaj gönderilemedi");
            return null;
        }
    }

    public async Task<TeamMessageModel?> SendAnnouncementAsync(SendAnnouncementModel request)
    {
        try
        {
            var result = await _apiClient.PostAsync<TeamMessageModel>(ApiSettings.Endpoints.TeamMessageAnnouncement, request);
            if (result != null)
                await _toastService.ShowSuccessAsync("Duyuru gönderildi");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TeamMessageService] SendAnnouncement error: {ex.Message}");
            await _toastService.ShowErrorAsync("Duyuru gönderilemedi");
            return null;
        }
    }
}
