using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IScheduleDayRepository : IRepository<ScheduleDay>
{
    Task<IEnumerable<ScheduleDay>> GetByWeeklyScheduleIdAsync(int weeklyScheduleId, CancellationToken cancellationToken = default);
    Task<ScheduleDay?> GetByIdWithAssignmentsAsync(int id, CancellationToken cancellationToken = default);
}