using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class ShiftRequirementRepository : Repository<ShiftRequirement>, IShiftRequirementRepository
{
    public ShiftRequirementRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<ShiftRequirement>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(s => s.BusinessId == businessId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ShiftRequirement>> GetByDayTypeIdAsync(int dayTypeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(s => s.DayTypeId == dayTypeId).ToListAsync(cancellationToken);
    }
}