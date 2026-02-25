using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IEmployeeAvailabilityRepository : IRepository<EmployeeAvailability>
{
    Task<IEnumerable<EmployeeAvailability>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default);
    Task DeleteByEmployeeAsync(int employeeId, CancellationToken ct = default);
}
