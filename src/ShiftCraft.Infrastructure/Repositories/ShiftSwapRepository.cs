using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class ShiftSwapRepository : Repository<ShiftSwapRequest>, IShiftSwapRepository
{
    public ShiftSwapRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<ShiftSwapRequest>> GetPendingByBusinessAsync(int businessId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(s => s.BusinessId == businessId && s.Status == "Pending")
            .Include(s => s.Requester)
            .Include(s => s.RequesterShift)
            .Include(s => s.TargetEmployee)
            .Include(s => s.TargetShift)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ShiftSwapRequest>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(s => s.RequesterId == employeeId || s.TargetEmployeeId == employeeId)
            .Include(s => s.Requester)
            .Include(s => s.RequesterShift)
            .Include(s => s.TargetEmployee)
            .Include(s => s.TargetShift)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }
}
