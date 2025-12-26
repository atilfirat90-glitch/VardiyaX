namespace ShiftCraft.Mobile.Models;

public class Employee
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    
    // API uses single Name field
    public string Name { get; set; } = string.Empty;
    
    // Legacy fields for backward compatibility with existing views
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    
    // Use Name if available, otherwise combine FirstName/LastName
    public string FullName => !string.IsNullOrEmpty(Name) ? Name : $"{FirstName} {LastName}".Trim();
}
