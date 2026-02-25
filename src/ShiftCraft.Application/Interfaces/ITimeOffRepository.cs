using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface ITimeOffRepository : IRepository<TimeOffRequest>
{
    Task<IEnumerable<TimeOffRequest>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default);
    Task<IEnumerable<TimeOffRequest>> GetPendingByBusinessAsync(int businessId, CancellationToken ct = default);
    Task<IEnumerable<TimeOffRequest>> GetByDateRangeAsync(int businessId, DateTime start, DateTime end, CancellationToken ct = default);
}
