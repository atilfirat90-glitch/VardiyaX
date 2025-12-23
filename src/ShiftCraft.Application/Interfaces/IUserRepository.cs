using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task UpdateLastLoginAsync(int userId, CancellationToken cancellationToken = default);
}
