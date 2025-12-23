using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class CoreStaffRuleRepository : Repository<CoreStaffRule>, ICoreStaffRuleRepository
{
    public CoreStaffRuleRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<CoreStaffRule>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(c => c.BusinessId == businessId).ToListAsync(cancellationToken);
    }

    public async Task<CoreStaffRule?> GetByBusinessAndDayTypeAsync(int businessId, int dayTypeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.BusinessId == businessId && c.DayTypeId == dayTypeId, cancellationToken);
    }
}