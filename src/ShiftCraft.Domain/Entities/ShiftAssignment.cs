using ShiftCraft.Domain.Enums;

namespace ShiftCraft.Domain.Entities;

public class ShiftAssignment
{
    public int Id { get; set; }
    public int ScheduleDayId { get; set; }
    public int EmployeeId { get; set; }
    public int RoleId { get; set; }
    public int ShiftTemplateId { get; set; }
    public ShiftSource Source { get; set; }

    public ScheduleDay? ScheduleDay { get; set; }
    public Employee? Employee { get; set; }
    public Role? Role { get; set; }
    public ShiftTemplate? ShiftTemplate { get; set; }
}