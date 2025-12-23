namespace ShiftCraft.Domain.Entities;

public class Business
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<WeeklySchedule> WeeklySchedules { get; set; } = new List<WeeklySchedule>();
    public ICollection<ShiftTemplate> ShiftTemplates { get; set; } = new List<ShiftTemplate>();
    public ICollection<ShiftRequirement> ShiftRequirements { get; set; } = new List<ShiftRequirement>();
    public ICollection<CoreStaffRule> CoreStaffRules { get; set; } = new List<CoreStaffRule>();
    public ICollection<WorkRule> WorkRules { get; set; } = new List<WorkRule>();
}