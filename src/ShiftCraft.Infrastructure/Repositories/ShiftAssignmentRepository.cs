using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class ShiftAssignmentRepository : Repository<ShiftAssignment>, IShiftAssignmentRepository
{
    public ShiftAssignmentRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<ShiftAssignment>> GetByScheduleDayIdAsync(int scheduleDayId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.ScheduleDayId == scheduleDayId)
            .Include(s => s.Employee)
            .Include(s => s.ShiftTemplate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ShiftAssignment>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.EmployeeId == employeeId)
            .Include(s => s.ScheduleDay)
            .Include(s => s.ShiftTemplate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ShiftAssignment>> GetByEmployeeAndDateRangeAsync(int employeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.EmployeeId == employeeId && s.ScheduleDay.Date >= startDate && s.ScheduleDay.Date <= endDate)
            .Include(s => s.ScheduleDay)
            .Include(s => s.ShiftTemplate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ShiftAssignment>> GetByWeeklyScheduleIdAsync(int weeklyScheduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.ScheduleDay != null && s.ScheduleDay.WeeklyScheduleId == weeklyScheduleId)
            .Include(s => s.Employee)
            .ToListAsync(cancellationToken);
    }
}