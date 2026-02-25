using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class EmployeeAvailabilityRepository : Repository<EmployeeAvailability>, IEmployeeAvailabilityRepository
{
    public EmployeeAvailabilityRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<EmployeeAvailability>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(a => a.EmployeeId == employeeId)
            .OrderBy(a => a.DayOfWeek)
            .ToListAsync(ct);
    }

    public async Task DeleteByEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        var existing = await _dbSet
            .Where(a => a.EmployeeId == employeeId)
            .ToListAsync(ct);

        _dbSet.RemoveRange(existing);
        await _context.SaveChangesAsync(ct);
    }
}
