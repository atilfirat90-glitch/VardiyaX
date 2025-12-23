using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Interfaces;

public interface IShiftTemplateRepository : IRepository<ShiftTemplate>
{
    Task<IEnumerable<ShiftTemplate>> GetByBusinessIdAsync(int businessId, CancellationToken cancellationToken = default);
}