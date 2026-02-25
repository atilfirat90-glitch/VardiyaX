using ShiftCraft.Application.Interfaces;

namespace ShiftCraft.Application.Services;

public class ShiftValidationService : IShiftValidationService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IShiftAssignmentRepository _shiftAssignmentRepository;

    public ShiftValidationService(
        IEmployeeRepository employeeRepository,
        IShiftAssignmentRepository shiftAssignmentRepository)
    {
        _employeeRepository = employeeRepository;
        _shiftAssignmentRepository = shiftAssignmentRepository;
    }

    public async Task<List<string>> ValidateShiftAsync(
        int employeeId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        int? excludeAssignmentId = null,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        if (endTime <= startTime)
        {
            errors.Add("Bitiş saati başlangıçtan sonra olmalı");
        }

        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee == null || !employee.IsActive)
        {
            errors.Add("Çalışan bulunamadı veya aktif değil");
        }

        var existingAssignments = await _shiftAssignmentRepository
            .GetByEmployeeAndDateRangeAsync(employeeId, date, date, cancellationToken);

        foreach (var assignment in existingAssignments)
        {
            if (excludeAssignmentId.HasValue && assignment.Id == excludeAssignmentId.Value)
                continue;

            if (assignment.ShiftTemplate == null)
                continue;

            var existingStart = assignment.ShiftTemplate.StartTime;
            var existingEnd = assignment.ShiftTemplate.EndTime;

            if (startTime < existingEnd && endTime > existingStart)
            {
                errors.Add("Bu çalışanın bu saatlerde başka vardiyası var");
                break;
            }
        }

        return errors;
    }
}
