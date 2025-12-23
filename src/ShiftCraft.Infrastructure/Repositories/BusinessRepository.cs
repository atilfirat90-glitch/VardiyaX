using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class BusinessRepository : Repository<Business>, IBusinessRepository
{
    public BusinessRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<Business?> GetByIdWithEmployeesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Employees)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}