namespace ShiftCraft.Domain.Entities;

public class ShiftRequirement
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int DayTypeId { get; set; }
    public int RoleId { get; set; }
    public int ShiftTemplateId { get; set; }
    public int RequiredCount { get; set; }

    public Business? Business { get; set; }
    public DayType? DayType { get; set; }
    public Role? Role { get; set; }
    public ShiftTemplate? ShiftTemplate { get; set; }
}