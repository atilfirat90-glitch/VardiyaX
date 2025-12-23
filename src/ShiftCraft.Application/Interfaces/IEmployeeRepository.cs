using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employee>> GetCoreStaffByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdWithRolesAsync(int id, CancellationToken cancellationToken = default);
}