using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IWorkRuleRepository : IRepository<WorkRule>
{
    Task<WorkRule?> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default);
}