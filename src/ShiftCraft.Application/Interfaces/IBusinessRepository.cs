using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IBusinessRepository : IRepository<Business>
{
    Task<Business?> GetByIdWithEmployeesAsync(int id, CancellationToken cancellationToken = default);
}