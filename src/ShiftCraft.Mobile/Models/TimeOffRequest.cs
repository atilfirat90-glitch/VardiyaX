namespace ShiftCraft.Mobile.Models;

public class TimeOffRequestModel
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? ResponseNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public int BusinessId { get; set; }

    public string StatusText => Status switch
    {
        "Pending" => "Beklemede",
        "Approved" => "Onaylandı",
        "Rejected" => "Reddedildi",
        _ => Status
    };

    public string TypeText => Type switch
    {
        "PaidLeave" => "Ücretli İzin",
        "SickLeave" => "Hastalık İzni",
        "Unpaid" => "Ücretsiz İzin",
        _ => Type
    };

    public string DateRangeText => $"{StartDate:dd MMM} - {EndDate:dd MMM yyyy}";
    public string CreatedAtText => CreatedAt.ToString("dd MMM yyyy HH:mm");
}

public class CreateTimeOffRequestModel
{
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = "PaidLeave";
    public string? Reason { get; set; }
    public int BusinessId { get; set; }
}
