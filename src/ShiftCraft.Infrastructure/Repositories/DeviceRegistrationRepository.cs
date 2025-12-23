using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class DeviceRegistrationRepository : Repository<DeviceRegistration>, IDeviceRegistrationRepository
{
    public DeviceRegistrationRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<IEnumerable<DeviceRegistration>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.UserId == userId && d.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DeviceRegistration>> GetActiveByUserIdsAsync(IEnumerable<int> userIds, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => userIds.Contains(d.UserId) && d.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeviceRegistration?> GetByDeviceTokenAsync(string deviceToken, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.DeviceToken == deviceToken, cancellationToken);
    }

    public async Task DeactivateByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var devices = await _dbSet.Where(d => d.UserId == userId).ToListAsync(cancellationToken);
        foreach (var device in devices)
        {
            device.IsActive = false;
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}
