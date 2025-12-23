using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class DayTypeRepository : Repository<DayType>, IDayTypeRepository
{
    public DayTypeRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<DayType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
    }
}