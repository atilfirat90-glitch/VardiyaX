using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IShiftRequirementRepository : IRepository<ShiftRequirement>
{
    Task<IEnumerable<ShiftRequirement>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShiftRequirement>> GetByDayTypeIdAsync(int dayTypeId, CancellationToken cancellationToken = default);
}