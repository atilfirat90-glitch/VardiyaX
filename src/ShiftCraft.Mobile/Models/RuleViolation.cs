namespace ShiftCraft.Mobile.Models;

public class RuleViolation
{
    public int Id { get; set; }
    public int WeeklyScheduleId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime ViolationDate { get; set; }
    public int RuleCode { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? AcknowledgedBy { get; set; }
    
    // Computed properties for UI
    public string RuleType => GetRuleType(RuleCode);
    public string Description => GetDescription(RuleCode);
    public int Severity => RuleCode >= 100 ? 1 : 0; // 100+ = Error, else Warning
    public string SeverityText => Severity == 0 ? "Warning" : "Error";
    public DateTime DetectedAt => ViolationDate;
    
    private static string GetRuleType(int code) => code switch
    {
        1 => "Haftalık Çalışma Süresi",
        2 => "Günlük Çalışma Süresi",
        3 => "Dinlenme Süresi",
        4 => "Çekirdek Kadro",
        100 => "Kritik İhlal",
        _ => $"Kural #{code}"
    };
    
    private static string GetDescription(int code) => code switch
    {
        1 => "Haftalık maksimum çalışma süresi aşıldı",
        2 => "Günlük maksimum çalışma süresi aşıldı",
        3 => "Minimum dinlenme süresi sağlanmadı",
        4 => "Çekirdek kadro gereksinimi karşılanmadı",
        100 => "Kritik kural ihlali tespit edildi",
        _ => "Kural ihlali tespit edildi"
    };
}
