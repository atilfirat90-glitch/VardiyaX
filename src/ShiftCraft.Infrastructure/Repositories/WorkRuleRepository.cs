using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class WorkRuleRepository : Repository<WorkRule>, IWorkRuleRepository
{
    public WorkRuleRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<WorkRule?> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.BusinessId == businessId, cancellationToken);
    }
}