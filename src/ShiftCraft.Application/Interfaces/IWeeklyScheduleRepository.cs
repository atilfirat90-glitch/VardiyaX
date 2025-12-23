using ShiftCraft.Domain.Entities;
using ShiftCraft.Domain.Enums;

namespace ShiftCraft.Application.Interfaces;

public interface IWeeklyScheduleRepository : IRepository<WeeklySchedule>
{
    Task<IEnumerable<WeeklySchedule>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WeeklySchedule>> GetByStatusAsync(ScheduleStatus status, CancellationToken cancellationToken = default);
    Task<WeeklySchedule?> GetByIdWithDaysAsync(int id, CancellationToken cancellationToken = default);
}