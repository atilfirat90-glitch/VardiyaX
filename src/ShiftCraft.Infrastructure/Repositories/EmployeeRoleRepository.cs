using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class EmployeeRoleRepository : IEmployeeRoleRepository
{
    private readonly ShiftCraftDbContext _context;

    public EmployeeRoleRepository(ShiftCraftDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeRole?> GetAsync(int employeeId, int roleId, CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeRoles
            .FirstOrDefaultAsync(er => er.EmployeeId == employeeId && er.RoleId == roleId, cancellationToken);
    }

    public async Task<IEnumerable<EmployeeRole>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeRoles
            .Where(er => er.EmployeeId == employeeId)
            .Include(er => er.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EmployeeRole>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeRoles
            .Where(er => er.RoleId == roleId)
            .Include(er => er.Employee)
            .ToListAsync(cancellationToken);
    }

    public async Task<EmployeeRole> AddAsync(EmployeeRole entity, CancellationToken cancellationToken = default)
    {
        await _context.EmployeeRoles.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(EmployeeRole entity, CancellationToken cancellationToken = default)
    {
        _context.EmployeeRoles.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}