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
public class ShiftRequirementController : ControllerBase
{
    private readonly IShiftRequirementRepository _shiftRequirementRepository;

    public ShiftRequirementController(IShiftRequirementRepository shiftRequirementRepository)
    {
        _shiftRequirementRepository = shiftRequirementRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftRequirement>>> GetAll(CancellationToken cancellationToken)
    {
        var requirements = await _shiftRequirementRepository.GetAllAsync(cancellationToken);
        return Ok(requirements);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftRequirement>> GetById(int id, CancellationToken cancellationToken)
    {
        var requirement = await _shiftRequirementRepository.GetByIdAsync(id, cancellationToken);
        if (requirement == null) return NotFound();
        return Ok(requirement);
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<IEnumerable<ShiftRequirement>>> GetByBusinessId(int businessId, CancellationToken cancellationToken)
    {
        var requirements = await _shiftRequirementRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        return Ok(requirements);
    }

    [HttpPost]
    public async Task<ActionResult<ShiftRequirement>> Create(ShiftRequirement requirement, CancellationToken cancellationToken)
    {
        var created = await _shiftRequirementRepository.AddAsync(requirement, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ShiftRequirement requirement, CancellationToken cancellationToken)
    {
        if (id != requirement.Id) return BadRequest();
        await _shiftRequirementRepository.UpdateAsync(requirement, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var requirement = await _shiftRequirementRepository.GetByIdAsync(id, cancellationToken);
        if (requirement == null) return NotFound();
        await _shiftRequirementRepository.DeleteAsync(requirement, cancellationToken);
        return NoContent();
    }
}