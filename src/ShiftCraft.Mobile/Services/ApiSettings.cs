namespace ShiftCraft.Mobile.Services;

public static class ApiSettings
{
    // Development: Use Android emulator loopback (10.0.2.2 = host localhost)
    // Production: Use Azure API
#if DEBUG
    public const string BaseUrl = "http://10.0.2.2:5184/api/";
#else
    public const string BaseUrl = "https://shiftcraft-api-prod.azurewebsites.net/api/v1/";
#endif
    
    public static class Endpoints
    {
        public const string Login = "auth/login";
        public const string Employees = "employee";
        public const string WeeklySchedules = "weeklyschedule";
        public const string RuleViolations = "ruleviolation";
    }
}
