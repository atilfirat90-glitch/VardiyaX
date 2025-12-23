using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IShiftAssignmentRepository : IRepository<ShiftAssignment>
{
    Task<IEnumerable<ShiftAssignment>> GetByScheduleDayIdAsync(int scheduleDayId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShiftAssignment>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShiftAssignment>> GetByEmployeeAndDateRangeAsync(int employeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShiftAssignment>> GetByWeeklyScheduleIdAsync(int weeklyScheduleId, CancellationToken cancellationToken = default);
}