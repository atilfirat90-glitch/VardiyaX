using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IPublishLogRepository : IRepository<PublishLog>
{
    Task<IEnumerable<PublishLog>> GetByBusinessIdAsync(
        int businessId,
        string? publisher = null,
        DateTime? from = null,
        DateTime? to = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
    
    Task<int> GetCountByBusinessIdAsync(
        int businessId,
        string? publisher = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
}
