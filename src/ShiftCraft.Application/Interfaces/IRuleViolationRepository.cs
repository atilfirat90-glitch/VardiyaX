using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IRuleViolationRepository : IRepository<RuleViolation>
{
    Task<IEnumerable<RuleViolation>> GetByWeeklyScheduleIdAsync(int weeklyScheduleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RuleViolation>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task DeleteByWeeklyScheduleIdAsync(int weeklyScheduleId, CancellationToken cancellationToken = default);
}