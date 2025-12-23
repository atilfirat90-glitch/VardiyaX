using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class ShiftTemplateRepository : Repository<ShiftTemplate>, IShiftTemplateRepository
{
    public ShiftTemplateRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<ShiftTemplate>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(s => s.BusinessId == businessId).ToListAsync(cancellationToken);
    }
}