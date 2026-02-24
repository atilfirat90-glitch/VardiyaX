using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public interface IShiftService
{
    Task<ShiftResponseDto?> CreateShiftAsync(CreateShiftRequest request);
    Task<List<ShiftResponseDto>> GetShiftsByDateRangeAsync(int employeeId, DateTime startDate, DateTime endDate);
    Task<List<ShiftResponseDto>> GetBusinessShiftsByDateAsync(int businessId, DateTime date);
}
