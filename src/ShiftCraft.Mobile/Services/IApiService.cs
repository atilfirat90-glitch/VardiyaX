using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public interface IApiService
{
    Task<List<Employee>> GetEmployeesAsync();
    Task<List<WeeklySchedule>> GetWeeklySchedulesAsync();
    Task<WeeklySchedule?> GetWeeklyScheduleAsync(int id);
    Task<List<RuleViolation>> PublishScheduleAsync(int scheduleId);
    Task<List<RuleViolation>> GetRuleViolationsAsync();
}
