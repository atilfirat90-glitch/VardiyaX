using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IRuleEngineService
{
    Task<IEnumerable<RuleViolation>> ValidateMaxDailyMinutesAsync(int weeklyScheduleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RuleViolation>> ValidateWeeklyOffDaysAsync(int weeklyScheduleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RuleViolation>> ValidateCoreStaffMinimumAsync(int weeklyScheduleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RuleViolation>> ValidateScheduleAsync(int weeklyScheduleId, CancellationToken cancellationToken = default);
}