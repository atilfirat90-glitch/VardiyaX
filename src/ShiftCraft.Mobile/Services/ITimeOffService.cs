using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public interface ITimeOffService
{
    Task<List<TimeOffRequestModel>> GetByEmployeeAsync(int employeeId);
    Task<List<TimeOffRequestModel>> GetPendingByBusinessAsync(int businessId);
    Task<TimeOffRequestModel?> CreateAsync(CreateTimeOffRequestModel request);
    Task<bool> ApproveAsync(int id);
    Task<bool> RejectAsync(int id, string? responseNote = null);
}
