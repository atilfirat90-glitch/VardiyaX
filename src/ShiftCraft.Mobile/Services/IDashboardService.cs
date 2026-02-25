using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public interface IDashboardService
{
    Task<DashboardData?> GetDashboardAsync(int businessId);
}
