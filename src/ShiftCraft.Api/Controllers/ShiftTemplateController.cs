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
public class ShiftTemplateController : ControllerBase
{
    private readonly IShiftTemplateRepository _shiftTemplateRepository;

    public ShiftTemplateController(IShiftTemplateRepository shiftTemplateRepository)
    {
        _shiftTemplateRepository = shiftTemplateRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftTemplate>>> GetAll(CancellationToken cancellationToken)
    {
        var templates = await _shiftTemplateRepository.GetAllAsync(cancellationToken);
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftTemplate>> GetById(int id, CancellationToken cancellationToken)
    {
        var template = await _shiftTemplateRepository.GetByIdAsync(id, cancellationToken);
        if (template == null) return NotFound();
        return Ok(template);
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<IEnumerable<ShiftTemplate>>> GetByBusinessId(int businessId, CancellationToken cancellationToken)
    {
        var templates = await _shiftTemplateRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        return Ok(templates);
    }

    [HttpPost]
    public async Task<ActionResult<ShiftTemplate>> Create(ShiftTemplate template, CancellationToken cancellationToken)
    {
        var created = await _shiftTemplateRepository.AddAsync(template, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ShiftTemplate template, CancellationToken cancellationToken)
    {
        if (id != template.Id) return BadRequest();
        await _shiftTemplateRepository.UpdateAsync(template, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var template = await _shiftTemplateRepository.GetByIdAsync(id, cancellationToken);
        if (template == null) return NotFound();
        await _shiftTemplateRepository.DeleteAsync(template, cancellationToken);
        return NoContent();
    }
}