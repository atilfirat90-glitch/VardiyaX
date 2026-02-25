namespace ShiftCraft.Mobile.Models;

public class EmployeeAvailabilityModel
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeSpan? AvailableFrom { get; set; }
    public TimeSpan? AvailableTo { get; set; }
    public bool IsAvailable { get; set; } = true;

    public string DayName => DayOfWeek switch
    {
        0 => "Pazar",
        1 => "Pazartesi",
        2 => "Salı",
        3 => "Çarşamba",
        4 => "Perşembe",
        5 => "Cuma",
        6 => "Cumartesi",
        _ => string.Empty
    };

    public string TimeRangeText => AvailableFrom.HasValue && AvailableTo.HasValue
        ? $"{AvailableFrom.Value:hh\\:mm} - {AvailableTo.Value:hh\\:mm}"
        : "Tüm gün";
}

public class UpdateAvailabilityModel
{
    public int DayOfWeek { get; set; }
    public TimeSpan? AvailableFrom { get; set; }
    public TimeSpan? AvailableTo { get; set; }
    public bool IsAvailable { get; set; } = true;
}
