using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class RuleViolationRepository : Repository<RuleViolation>, IRuleViolationRepository
{
    public RuleViolationRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<RuleViolation>> GetByWeeklyScheduleIdAsync(int weeklyScheduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.WeeklyScheduleId == weeklyScheduleId)
            .Include(r => r.Employee)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RuleViolation>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.EmployeeId == employeeId)
            .Include(r => r.WeeklySchedule)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteByWeeklyScheduleIdAsync(int weeklyScheduleId, CancellationToken cancellationToken = default)
    {
        var violations = await _dbSet.Where(r => r.WeeklyScheduleId == weeklyScheduleId).ToListAsync(cancellationToken);
        _dbSet.RemoveRange(violations);
        await _context.SaveChangesAsync(cancellationToken);
    }
}