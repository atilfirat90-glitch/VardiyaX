using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IScheduleValidationService
{
    Task<IEnumerable<RuleViolation>> ValidateOnPublishAsync(int weeklyScheduleId, CancellationToken cancellationToken = default);
}