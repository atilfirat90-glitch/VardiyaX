using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class PublishLogRepository : Repository<PublishLog>, IPublishLogRepository
{
    public PublishLogRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<PublishLog>> GetByBusinessIdAsync(
        int businessId,
        string? publisher = null,
        DateTime? from = null,
        DateTime? to = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.WeeklySchedule)
            .Where(p => p.BusinessId == businessId);

        if (!string.IsNullOrEmpty(publisher))
            query = query.Where(p => p.PublisherUsername.Contains(publisher));

        if (from.HasValue)
            query = query.Where(p => p.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(p => p.Timestamp <= to.Value);

        return await query
            .OrderByDescending(p => p.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByBusinessIdAsync(
        int businessId,
        string? publisher = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(p => p.BusinessId == businessId);

        if (!string.IsNullOrEmpty(publisher))
            query = query.Where(p => p.PublisherUsername.Contains(publisher));

        if (from.HasValue)
            query = query.Where(p => p.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(p => p.Timestamp <= to.Value);

        return await query.CountAsync(cancellationToken);
    }
}
