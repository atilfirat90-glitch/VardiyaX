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
public class CoreStaffRuleController : ControllerBase
{
    private readonly ICoreStaffRuleRepository _coreStaffRuleRepository;

    public CoreStaffRuleController(ICoreStaffRuleRepository coreStaffRuleRepository)
    {
        _coreStaffRuleRepository = coreStaffRuleRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoreStaffRule>>> GetAll(CancellationToken cancellationToken)
    {
        var rules = await _coreStaffRuleRepository.GetAllAsync(cancellationToken);
        return Ok(rules);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoreStaffRule>> GetById(int id, CancellationToken cancellationToken)
    {
        var rule = await _coreStaffRuleRepository.GetByIdAsync(id, cancellationToken);
        if (rule == null) return NotFound();
        return Ok(rule);
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<IEnumerable<CoreStaffRule>>> GetByBusinessId(int businessId, CancellationToken cancellationToken)
    {
        var rules = await _coreStaffRuleRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        return Ok(rules);
    }

    [HttpPost]
    public async Task<ActionResult<CoreStaffRule>> Create(CoreStaffRule rule, CancellationToken cancellationToken)
    {
        var created = await _coreStaffRuleRepository.AddAsync(rule, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CoreStaffRule rule, CancellationToken cancellationToken)
    {
        if (id != rule.Id) return BadRequest();
        await _coreStaffRuleRepository.UpdateAsync(rule, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var rule = await _coreStaffRuleRepository.GetByIdAsync(id, cancellationToken);
        if (rule == null) return NotFound();
        await _coreStaffRuleRepository.DeleteAsync(rule, cancellationToken);
        return NoContent();
    }
}