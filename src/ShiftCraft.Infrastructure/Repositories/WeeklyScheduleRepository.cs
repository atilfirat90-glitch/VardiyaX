using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Domain.Enums;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class WeeklyScheduleRepository : Repository<WeeklySchedule>, IWeeklyScheduleRepository
{
    public WeeklyScheduleRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<WeeklySchedule>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(w => w.BusinessId == businessId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WeeklySchedule>> GetByStatusAsync(ScheduleStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(w => w.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<WeeklySchedule?> GetByIdWithDaysAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(w => w.ScheduleDays)
            .ThenInclude(d => d.ShiftAssignments)
            .ThenInclude(a => a.Employee)
            .Include(w => w.ScheduleDays)
            .ThenInclude(d => d.ShiftAssignments)
            .ThenInclude(a => a.ShiftTemplate)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }
}