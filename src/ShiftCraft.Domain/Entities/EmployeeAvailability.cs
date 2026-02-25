namespace ShiftCraft.Domain.Entities;

public class EmployeeAvailability
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan? AvailableFrom { get; set; }
    public TimeSpan? AvailableTo { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Employee? Employee { get; set; }
}
