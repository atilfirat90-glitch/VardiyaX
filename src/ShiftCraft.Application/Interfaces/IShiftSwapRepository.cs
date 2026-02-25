using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IShiftSwapRepository : IRepository<ShiftSwapRequest>
{
    Task<IEnumerable<ShiftSwapRequest>> GetPendingByBusinessAsync(int businessId, CancellationToken ct = default);
    Task<IEnumerable<ShiftSwapRequest>> GetByEmployeeAsync(int employeeId, CancellationToken ct = default);
}
