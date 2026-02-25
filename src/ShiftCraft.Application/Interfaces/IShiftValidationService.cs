namespace ShiftCraft.Application.Interfaces;

public interface IShiftValidationService
{
    Task<List<string>> ValidateShiftAsync(int employeeId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeAssignmentId = null, CancellationToken cancellationToken = default);
}
