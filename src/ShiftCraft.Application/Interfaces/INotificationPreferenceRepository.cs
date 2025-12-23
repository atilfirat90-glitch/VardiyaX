using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface INotificationPreferenceRepository : IRepository<NotificationPreference>
{
    Task<NotificationPreference?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<NotificationPreference> GetOrCreateByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}
