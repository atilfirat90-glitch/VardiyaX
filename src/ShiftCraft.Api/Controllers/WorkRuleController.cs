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
public class WorkRuleController : ControllerBase
{
    private readonly IWorkRuleRepository _workRuleRepository;

    public WorkRuleController(IWorkRuleRepository workRuleRepository)
    {
        _workRuleRepository = workRuleRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkRule>>> GetAll(CancellationToken cancellationToken)
    {
        var rules = await _workRuleRepository.GetAllAsync(cancellationToken);
        return Ok(rules);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkRule>> GetById(int id, CancellationToken cancellationToken)
    {
        var rule = await _workRuleRepository.GetByIdAsync(id, cancellationToken);
        if (rule == null) return NotFound();
        return Ok(rule);
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<WorkRule>> GetByBusinessId(int businessId, CancellationToken cancellationToken)
    {
        var rule = await _workRuleRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        if (rule == null) return NotFound();
        return Ok(rule);
    }

    [HttpPost]
    public async Task<ActionResult<WorkRule>> Create(WorkRule rule, CancellationToken cancellationToken)
    {
        var created = await _workRuleRepository.AddAsync(rule, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, WorkRule rule, CancellationToken cancellationToken)
    {
        if (id != rule.Id) return BadRequest();
        await _workRuleRepository.UpdateAsync(rule, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var rule = await _workRuleRepository.GetByIdAsync(id, cancellationToken);
        if (rule == null) return NotFound();
        await _workRuleRepository.DeleteAsync(rule, cancellationToken);
        return NoContent();
    }
}