using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface ICoreStaffRuleRepository : IRepository<CoreStaffRule>
{
    Task<IEnumerable<CoreStaffRule>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default);
    Task<CoreStaffRule?> GetByBusinessAndDayTypeAsync(int businessId, int dayTypeId, CancellationToken cancellationToken = default);
}