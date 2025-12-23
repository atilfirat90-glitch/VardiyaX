namespace ShiftCraft.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsCoreStaff { get; set; }
    public int WeeklyMaxMinutes { get; set; }
    public bool IsActive { get; set; }

    public Business? Business { get; set; }
    public ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();
    public ICollection<ShiftAssignment> ShiftAssignments { get; set; } = new List<ShiftAssignment>();
    public ICollection<RuleViolation> RuleViolations { get; set; } = new List<RuleViolation>();
}