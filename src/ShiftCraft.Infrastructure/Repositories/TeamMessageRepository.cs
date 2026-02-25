using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class TeamMessageRepository : Repository<TeamMessage>, ITeamMessageRepository
{
    public TeamMessageRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<TeamMessage>> GetByChannelAsync(string channel, int businessId, int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(m => m.Channel == channel && m.BusinessId == businessId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TeamMessage>> GetAnnouncementsAsync(int businessId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(m => m.IsAnnouncement && m.BusinessId == businessId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(ct);
    }
}
