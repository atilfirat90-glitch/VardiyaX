using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<Employee>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.BusinessId == businessId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Employee>> GetCoreStaffByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.BusinessId == businessId && e.IsCoreStaff).ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdWithRolesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.EmployeeRoles)
            .ThenInclude(er => er.Role)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}