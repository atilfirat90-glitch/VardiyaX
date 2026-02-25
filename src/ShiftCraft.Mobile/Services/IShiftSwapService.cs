using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public interface IShiftSwapService
{
    Task<List<ShiftSwapRequestModel>> GetPendingByBusinessAsync(int businessId);
    Task<List<ShiftSwapRequestModel>> GetByEmployeeAsync(int employeeId);
    Task<ShiftSwapRequestModel?> CreateSwapAsync(CreateSwapRequestModel request);
    Task<bool> ApproveAsync(int id);
    Task<bool> RejectAsync(int id);
    Task<bool> CancelAsync(int id);
}
