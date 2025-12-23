using Microsoft.Extensions.Logging;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Services;

public class ScheduleValidationService : IScheduleValidationService
{
    private readonly IRuleEngineService _ruleEngineService;
    private readonly IRuleViolationRepository _ruleViolationRepository;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ScheduleValidationService> _logger;

    public ScheduleValidationService(
        IRuleEngineService ruleEngineService,
        IRuleViolationRepository ruleViolationRepository,
        IPushNotificationService pushNotificationService,
        IUserRepository userRepository,
        ILogger<ScheduleValidationService> logger)
    {
        _ruleEngineService = ruleEngineService;
        _ruleViolationRepository = ruleViolationRepository;
        _pushNotificationService = pushNotificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<RuleViolation>> ValidateOnPublishAsync(int weeklyScheduleId, CancellationToken cancellationToken = default)
    {
        await _ruleViolationRepository.DeleteByWeeklyScheduleIdAsync(weeklyScheduleId, cancellationToken);
        var violations = await _ruleEngineService.ValidateScheduleAsync(weeklyScheduleId, cancellationToken);
        
        // Send push notifications to managers if violations detected
        if (violations.Any())
        {
            await NotifyManagersOfViolationsAsync(violations, cancellationToken);
        }
        
        return violations;
    }

    private async Task NotifyManagersOfViolationsAsync(IEnumerable<RuleViolation> violations, CancellationToken cancellationToken)
    {
        try
        {
            // Get all manager user IDs (in a real implementation, filter by business)
            var allUsers = await _userRepository.GetAllAsync(cancellationToken);
            var managerUserIds = allUsers
                .Where(u => u.Role == Domain.Enums.UserRole.Manager || u.Role == Domain.Enums.UserRole.Admin)
                .Where(u => u.IsActive)
                .Select(u => u.Id)
                .ToList();

            if (managerUserIds.Any())
            {
                foreach (var violation in violations.Take(5)) // Limit to first 5 violations
                {
                    await _pushNotificationService.SendViolationDetectedNotificationAsync(
                        violation.Id, managerUserIds, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send violation notifications");
        }
    }
}