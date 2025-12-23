using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}