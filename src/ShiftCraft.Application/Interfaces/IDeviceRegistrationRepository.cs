using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IDeviceRegistrationRepository : IRepository<DeviceRegistration>
{
    Task<IEnumerable<DeviceRegistration>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DeviceRegistration>> GetActiveByUserIdsAsync(IEnumerable<int> userIds, CancellationToken cancellationToken = default);
    Task<DeviceRegistration?> GetByDeviceTokenAsync(string deviceToken, CancellationToken cancellationToken = default);
    Task DeactivateByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
