using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IDayTypeRepository : IRepository<DayType>
{
    Task<DayType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}