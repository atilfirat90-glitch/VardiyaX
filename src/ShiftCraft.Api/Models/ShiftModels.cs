namespace ShiftCraft.Api.Models;

public class CreateShiftRequest
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int BusinessId { get; set; }
    public int? RoleId { get; set; }
}

public class ShiftResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = "Draft";
    public string ShiftTemplateName { get; set; } = string.Empty;
    public int? ShiftTemplateId { get; set; }
}
