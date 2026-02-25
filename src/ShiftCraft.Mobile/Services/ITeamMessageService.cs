using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public interface ITeamMessageService
{
    Task<List<TeamMessageModel>> GetByChannelAsync(string channel, int businessId, int page = 1);
    Task<List<TeamMessageModel>> GetAnnouncementsAsync(int businessId);
    Task<TeamMessageModel?> SendMessageAsync(SendMessageModel request);
    Task<TeamMessageModel?> SendAnnouncementAsync(SendAnnouncementModel request);
}
