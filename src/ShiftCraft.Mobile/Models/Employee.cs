namespace ShiftCraft.Mobile.Models;

public class Employee
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
}
