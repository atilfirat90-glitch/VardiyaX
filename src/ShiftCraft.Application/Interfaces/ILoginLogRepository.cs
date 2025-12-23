using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface ILoginLogRepository : IRepository<LoginLog>
{
    Task<IEnumerable<LoginLog>> GetByBusinessIdAsync(
        int businessId, 
        string? username = null,
        DateTime? from = null,
        DateTime? to = null,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default);
    
    Task<int> GetCountByBusinessIdAsync(
        int businessId,
        string? username = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
}
