namespace ShiftCraft.Mobile.Services;

public static class ApiSettings
{
    // Production Azure API
    public const string BaseUrl = "https://shiftcraft-api-prod.azurewebsites.net/api/v1/";
    
    public static class Endpoints
    {
        public const string Login = "auth/login";
        public const string Employees = "employee";
        public const string WeeklySchedules = "weeklyschedule";
        public const string RuleViolations = "ruleviolation";
    }
}
