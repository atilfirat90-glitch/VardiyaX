using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftCraft.Api.Models;
using ShiftCraft.Application.Interfaces;
using System.Security.Claims;

namespace ShiftCraft.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly ILoginLogRepository _loginLogRepository;
    private readonly IPublishLogRepository _publishLogRepository;
    private readonly IRuleViolationRepository _ruleViolationRepository;
    private readonly ILogger<AuditController> _logger;

    public AuditController(
        ILoginLogRepository loginLogRepository,
        IPublishLogRepository publishLogRepository,
        IRuleViolationRepository ruleViolationRepository,
        ILogger<AuditController> logger)
    {
        _loginLogRepository = loginLogRepository;
        _publishLogRepository = publishLogRepository;
        _ruleViolationRepository = ruleViolationRepository;
        _logger = logger;
    }

    [HttpGet("login")]
    public async Task<ActionResult<PagedResult<LoginLogDto>>> GetLoginLogs(
        [FromQuery] string? username,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (!IsAdminOrManager())
            return Forbid();

        var businessId = GetCurrentUserBusinessId();
        if (businessId == null)
            return BadRequest(new { message = "Business context required" });

        var skip = (page - 1) * pageSize;
        var logs = await _loginLogRepository.GetByBusinessIdAsync(
            businessId.Value, username, from, to, skip, pageSize, cancellationToken);
        var totalCount = await _loginLogRepository.GetCountByBusinessIdAsync(
            businessId.Value, username, from, to, cancellationToken);

        return Ok(new PagedResult<LoginLogDto>
        {
            Items = logs.Select(l => new LoginLogDto
            {
                Id = l.Id,
                Username = l.Username,
                Timestamp = l.Timestamp,
                Action = l.Action,
                DeviceInfo = l.DeviceInfo,
                FailureReason = l.FailureReason,
                IpAddress = l.IpAddress
            }).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("publish")]
    public async Task<ActionResult<PagedResult<PublishLogDto>>> GetPublishLogs(
        [FromQuery] string? publisher,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (!IsAdminOrManager())
            return Forbid();

        var businessId = GetCurrentUserBusinessId();
        if (businessId == null)
            return BadRequest(new { message = "Business context required" });

        var skip = (page - 1) * pageSize;
        var logs = await _publishLogRepository.GetByBusinessIdAsync(
            businessId.Value, publisher, from, to, skip, pageSize, cancellationToken);
        var totalCount = await _publishLogRepository.GetCountByBusinessIdAsync(
            businessId.Value, publisher, from, to, cancellationToken);

        return Ok(new PagedResult<PublishLogDto>
        {
            Items = logs.Select(p => new PublishLogDto
            {
                Id = p.Id,
                PublisherUsername = p.PublisherUsername,
                Timestamp = p.Timestamp,
                WeekStartDate = p.WeeklySchedule?.WeekStartDate ?? DateTime.MinValue,
                AffectedEmployeeCount = p.AffectedEmployeeCount
            }).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("violations")]
    public async Task<ActionResult<PagedResult<ViolationLogDto>>> GetViolationHistory(
        [FromQuery] int? employeeId,
        [FromQuery] string? ruleType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (!IsAdminOrManager())
            return Forbid();

        var businessId = GetCurrentUserBusinessId();
        if (businessId == null)
            return BadRequest(new { message = "Business context required" });

        var allViolations = await _ruleViolationRepository.GetAllAsync(cancellationToken);
        
        var query = allViolations
            .Where(v => v.WeeklySchedule?.BusinessId == businessId);

        if (employeeId.HasValue)
            query = query.Where(v => v.EmployeeId == employeeId.Value);

        if (!string.IsNullOrEmpty(ruleType))
            query = query.Where(v => v.RuleCode.ToString() == ruleType);

        if (from.HasValue)
            query = query.Where(v => v.ViolationDate >= from.Value);

        if (to.HasValue)
            query = query.Where(v => v.ViolationDate <= to.Value);

        var totalCount = query.Count();
        var skip = (page - 1) * pageSize;

        var violations = query
            .OrderByDescending(v => v.ViolationDate)
            .Skip(skip)
            .Take(pageSize)
            .ToList();

        return Ok(new PagedResult<ViolationLogDto>
        {
            Items = violations.Select(v => new ViolationLogDto
            {
                Id = v.Id,
                ViolationDate = v.ViolationDate,
                EmployeeName = v.Employee?.Name ?? "Unknown",
                EmployeeId = v.EmployeeId,
                RuleType = v.RuleCode.ToString(),
                IsAcknowledged = v.IsAcknowledged,
                AcknowledgedAt = v.AcknowledgedAt,
                AcknowledgedBy = v.AcknowledgedBy
            }).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("violations/trends")]
    public async Task<ActionResult<ViolationTrendsDto>> GetViolationTrends(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken = default)
    {
        if (!IsAdminOrManager())
            return Forbid();

        var businessId = GetCurrentUserBusinessId();
        if (businessId == null)
            return BadRequest(new { message = "Business context required" });

        var allViolations = await _ruleViolationRepository.GetAllAsync(cancellationToken);
        
        var violations = allViolations
            .Where(v => v.WeeklySchedule?.BusinessId == businessId)
            .Where(v => v.ViolationDate >= from && v.ViolationDate <= to)
            .ToList();

        // Weekly trends
        var weeklyTrends = violations
            .GroupBy(v => GetWeekStart(v.ViolationDate))
            .Select(g => new TrendPoint { Period = g.Key, Count = g.Count() })
            .OrderBy(t => t.Period)
            .ToList();

        // Monthly trends
        var monthlyTrends = violations
            .GroupBy(v => new DateTime(v.ViolationDate.Year, v.ViolationDate.Month, 1))
            .Select(g => new TrendPoint { Period = g.Key, Count = g.Count() })
            .OrderBy(t => t.Period)
            .ToList();

        return Ok(new ViolationTrendsDto
        {
            WeeklyTrends = weeklyTrends,
            MonthlyTrends = monthlyTrends
        });
    }

    [HttpPost("violations/{id}/acknowledge")]
    public async Task<IActionResult> AcknowledgeViolation(int id, CancellationToken cancellationToken)
    {
        if (!IsAdminOrManager())
            return Forbid();

        var violation = await _ruleViolationRepository.GetByIdAsync(id, cancellationToken);
        if (violation == null)
            return NotFound(new { message = "Violation not found" });

        var businessId = GetCurrentUserBusinessId();
        if (businessId != null && violation.WeeklySchedule?.BusinessId != businessId)
            return Forbid();

        violation.IsAcknowledged = true;
        violation.AcknowledgedAt = DateTime.UtcNow;
        violation.AcknowledgedBy = User.Identity?.Name;

        await _ruleViolationRepository.UpdateAsync(violation, cancellationToken);
        _logger.LogInformation("Violation {ViolationId} acknowledged by {User}", id, User.Identity?.Name);

        return NoContent();
    }

    private bool IsAdminOrManager()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;
        return role == "Admin" || role == "Manager";
    }

    private int? GetCurrentUserBusinessId()
    {
        var businessIdClaim = User.FindFirst("business_id")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : null;
    }

    private static DateTime GetWeekStart(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }
}
