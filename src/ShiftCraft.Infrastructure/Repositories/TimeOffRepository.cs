using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class TimeOffRepository : Repository<TimeOffRequest>, ITimeOffRepository
{
    public TimeOffRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<TimeOffRequest>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(t => t.EmployeeId == employeeId)
            .Include(t => t.Employee)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TimeOffRequest>> GetPendingByBusinessAsync(int businessId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(t => t.BusinessId == businessId && t.Status == "Pending")
            .Include(t => t.Employee)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<TimeOffRequest>> GetByDateRangeAsync(int businessId, DateTime start, DateTime end, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(t => t.BusinessId == businessId && t.StartDate <= end && t.EndDate >= start)
            .Include(t => t.Employee)
            .OrderBy(t => t.StartDate)
            .ToListAsync(ct);
    }
}
