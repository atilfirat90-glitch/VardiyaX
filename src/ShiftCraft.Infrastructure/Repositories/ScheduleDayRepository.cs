using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class ScheduleDayRepository : Repository<ScheduleDay>, IScheduleDayRepository
{
    public ScheduleDayRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<ScheduleDay>> GetByWeeklyScheduleIdAsync(int weeklyScheduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.WeeklyScheduleId == weeklyScheduleId)
            .Include(s => s.DayType)
            .OrderBy(s => s.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<ScheduleDay?> GetByIdWithAssignmentsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.ShiftAssignments)
            .ThenInclude(a => a.Employee)
            .Include(s => s.ShiftAssignments)
            .ThenInclude(a => a.ShiftTemplate)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}