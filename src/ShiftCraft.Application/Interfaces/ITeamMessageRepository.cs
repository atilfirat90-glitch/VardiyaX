using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface ITeamMessageRepository : IRepository<TeamMessage>
{
    Task<IEnumerable<TeamMessage>> GetByChannelAsync(string channel, int businessId, int page = 1, int pageSize = 50, CancellationToken ct = default);
    Task<IEnumerable<TeamMessage>> GetAnnouncementsAsync(int businessId, CancellationToken ct = default);
}
