using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")] // Backward compatibility
[Authorize]
public class RuleViolationController : ControllerBase
{
    private readonly IRuleViolationRepository _ruleViolationRepository;

    public RuleViolationController(IRuleViolationRepository ruleViolationRepository)
    {
        _ruleViolationRepository = ruleViolationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RuleViolation>>> GetAll(CancellationToken cancellationToken)
    {
        var violations = await _ruleViolationRepository.GetAllAsync(cancellationToken);
        return Ok(violations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RuleViolation>> GetById(int id, CancellationToken cancellationToken)
    {
        var violation = await _ruleViolationRepository.GetByIdAsync(id, cancellationToken);
        if (violation == null) return NotFound();
        return Ok(violation);
    }

    [HttpGet("weeklyschedule/{weeklyScheduleId}")]
    public async Task<ActionResult<IEnumerable<RuleViolation>>> GetByWeeklyScheduleId(int weeklyScheduleId, CancellationToken cancellationToken)
    {
        var violations = await _ruleViolationRepository.GetByWeeklyScheduleIdAsync(weeklyScheduleId, cancellationToken);
        return Ok(violations);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<RuleViolation>>> GetByEmployeeId(int employeeId, CancellationToken cancellationToken)
    {
        var violations = await _ruleViolationRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);
        return Ok(violations);
    }
}