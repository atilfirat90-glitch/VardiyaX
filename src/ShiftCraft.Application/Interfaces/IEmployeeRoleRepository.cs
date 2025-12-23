using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IEmployeeRoleRepository
{
    Task<EmployeeRole?> GetAsync(int employeeId, int roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeRole>> GetByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeRole>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
    Task<EmployeeRole> AddAsync(EmployeeRole entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(EmployeeRole entity, CancellationToken cancellationToken = default);
}