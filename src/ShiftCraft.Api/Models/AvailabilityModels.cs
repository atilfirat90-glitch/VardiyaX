namespace ShiftCraft.Api.Models;

public class AvailabilityDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan? AvailableFrom { get; set; }
    public TimeSpan? AvailableTo { get; set; }
    public bool IsAvailable { get; set; }
}

public class UpdateAvailabilityRequest
{
    public int DayOfWeek { get; set; }
    public TimeSpan? AvailableFrom { get; set; }
    public TimeSpan? AvailableTo { get; set; }
    public bool IsAvailable { get; set; } = true;
}
