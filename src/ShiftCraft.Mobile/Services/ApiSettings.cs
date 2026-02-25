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
        public const string Notifications = "notification";
        public const string NotificationUnread = "notification/unread";
        public const string NotificationUnreadCount = "notification/unread/count";
        public const string NotificationReadAll = "notification/read-all";
        public const string NotificationPreferences = "notification/preferences";
        
        // v1.4: Shift Assignment endpoints
        public const string ShiftAssignments = "shiftassignment";
        public const string ShiftCreate = "shiftassignment/create";
        public const string ShiftDateRange = "shiftassignment/date-range";

        // v1.5: Shift Swap endpoints
        public const string ShiftSwap = "shiftswap";
        public const string ShiftSwapBusiness = "shiftswap/business";
        public const string ShiftSwapEmployee = "shiftswap/employee";

        // v1.5: Time Off endpoints
        public const string TimeOff = "timeoff";
        public const string TimeOffEmployee = "timeoff/employee";
        public const string TimeOffPending = "timeoff/business";

        // v1.5: Team Message endpoints
        public const string TeamMessage = "teammessage";
        public const string TeamMessageAnnouncements = "teammessage/announcements";
        public const string TeamMessageAnnouncement = "teammessage/announcement";

        // v1.5: Availability endpoints
        public const string Availability = "availability/employee";

        // v1.5: Dashboard endpoint
        public const string Dashboard = "dashboard/business";
    }
}
