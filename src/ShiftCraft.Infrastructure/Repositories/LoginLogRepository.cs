using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class LoginLogRepository : Repository<LoginLog>, ILoginLogRepository
{
    public LoginLogRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<LoginLog>> GetByBusinessIdAsync(
        int businessId,
        string? username = null,
        DateTime? from = null,
        DateTime? to = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(l => l.BusinessId == businessId);

        if (!string.IsNullOrEmpty(username))
            query = query.Where(l => l.Username.Contains(username));

        if (from.HasValue)
            query = query.Where(l => l.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(l => l.Timestamp <= to.Value);

        return await query
            .OrderByDescending(l => l.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByBusinessIdAsync(
        int businessId,
        string? username = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(l => l.BusinessId == businessId);

        if (!string.IsNullOrEmpty(username))
            query = query.Where(l => l.Username.Contains(username));

        if (from.HasValue)
            query = query.Where(l => l.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(l => l.Timestamp <= to.Value);

        return await query.CountAsync(cancellationToken);
    }
}
