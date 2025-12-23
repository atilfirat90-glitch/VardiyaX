namespace ShiftCraft.Domain.Entities;

public class CoreStaffRule
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int DayTypeId { get; set; }
    public int MinCoreStaffCount { get; set; }

    public Business? Business { get; set; }
    public DayType? DayType { get; set; }
}