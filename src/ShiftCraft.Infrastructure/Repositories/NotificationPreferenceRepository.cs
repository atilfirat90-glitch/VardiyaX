using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;

namespace ShiftCraft.Infrastructure.Repositories;

public class NotificationPreferenceRepository : Repository<NotificationPreference>, INotificationPreferenceRepository
{
    public NotificationPreferenceRepository(ShiftCraftDbContext context) : base(context) { }

    public async Task<NotificationPreference?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(n => n.UserId == userId, cancellationToken);
    }

    public async Task<NotificationPreference> GetOrCreateByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var preference = await GetByUserIdAsync(userId, cancellationToken);
        if (preference == null)
        {
            preference = new NotificationPreference
            {
                UserId = userId,
                ScheduleNotificationsEnabled = true,
                ViolationNotificationsEnabled = true,
                ShiftRemindersEnabled = true,
                ReminderHoursBefore = 24
            };
            await AddAsync(preference, cancellationToken);
        }
        return preference;
    }
}
