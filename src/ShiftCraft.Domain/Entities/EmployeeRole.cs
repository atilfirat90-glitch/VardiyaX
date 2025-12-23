namespace ShiftCraft.Domain.Entities;

public class EmployeeRole
{
    public int EmployeeId { get; set; }
    public int RoleId { get; set; }

    public Employee? Employee { get; set; }
    public Role? Role { get; set; }
}