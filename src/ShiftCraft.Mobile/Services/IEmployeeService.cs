namespace ShiftCraft.Mobile.Services;

/// <summary>
/// Service for employee management operations.
/// v1.2 - Operational MVP
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Gets all employees.
    /// </summary>
    Task<List<Models.Employee>> GetAllAsync();

    /// <summary>
    /// Gets only active employees (for shift assignment picker).
    /// </summary>
    Task<List<Models.Employee>> GetActiveAsync();

    /// <summary>
    /// Gets an employee by ID.
    /// </summary>
    Task<Models.Employee?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new employee with IsActive=true.
    /// </summary>
    /// <param name="name">Full name of the employee</param>
    /// <returns>Created employee</returns>
    Task<Models.Employee?> CreateAsync(string name);

    /// <summary>
    /// Updates an employee's name and active status.
    /// </summary>
    Task<bool> UpdateAsync(int id, string name, bool isActive);
}
