namespace ShiftCraft.Mobile.Models;

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
    
    public string TimeRange => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    public string DateText => Date.ToString("dd MMM yyyy, dddd");
}

public class ShiftGroup : List<ShiftResponseDto>
{
    public string DateHeader { get; }
    public ShiftGroup(string dateHeader, IEnumerable<ShiftResponseDto> shifts) : base(shifts) { DateHeader = dateHeader; }
}
