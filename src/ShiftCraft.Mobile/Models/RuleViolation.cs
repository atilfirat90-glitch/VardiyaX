namespace ShiftCraft.Mobile.Models;

public class RuleViolation
{
    public int Id { get; set; }
    public int WeeklyScheduleId { get; set; }
    public int? EmployeeId { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Severity { get; set; } // 0=Warning, 1=Error
    public DateTime DetectedAt { get; set; }
    
    public string SeverityText => Severity == 0 ? "Warning" : "Error";
}
