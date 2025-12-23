namespace ShiftCraft.Domain.Entities;

public class ShiftTemplate
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DurationMinutes { get; set; }

    public Business? Business { get; set; }
    public ICollection<ShiftAssignment> ShiftAssignments { get; set; } = new List<ShiftAssignment>();
    public ICollection<ShiftRequirement> ShiftRequirements { get; set; } = new List<ShiftRequirement>();
}