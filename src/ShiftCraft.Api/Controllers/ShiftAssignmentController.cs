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
public class ShiftAssignmentController : ControllerBase
{
    private readonly IShiftAssignmentRepository _shiftAssignmentRepository;

    public ShiftAssignmentController(IShiftAssignmentRepository shiftAssignmentRepository)
    {
        _shiftAssignmentRepository = shiftAssignmentRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftAssignment>>> GetAll(CancellationToken cancellationToken)
    {
        var assignments = await _shiftAssignmentRepository.GetAllAsync(cancellationToken);
        return Ok(assignments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftAssignment>> GetById(int id, CancellationToken cancellationToken)
    {
        var assignment = await _shiftAssignmentRepository.GetByIdAsync(id, cancellationToken);
        if (assignment == null) return NotFound();
        return Ok(assignment);
    }

    [HttpGet("scheduleday/{scheduleDayId}")]
    public async Task<ActionResult<IEnumerable<ShiftAssignment>>> GetByScheduleDayId(int scheduleDayId, CancellationToken cancellationToken)
    {
        var assignments = await _shiftAssignmentRepository.GetByScheduleDayIdAsync(scheduleDayId, cancellationToken);
        return Ok(assignments);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<ShiftAssignment>>> GetByEmployeeId(int employeeId, CancellationToken cancellationToken)
    {
        var assignments = await _shiftAssignmentRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);
        return Ok(assignments);
    }

    [HttpPost]
    public async Task<ActionResult<ShiftAssignment>> Create(ShiftAssignment assignment, CancellationToken cancellationToken)
    {
        var created = await _shiftAssignmentRepository.AddAsync(assignment, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ShiftAssignment assignment, CancellationToken cancellationToken)
    {
        if (id != assignment.Id) return BadRequest();
        await _shiftAssignmentRepository.UpdateAsync(assignment, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var assignment = await _shiftAssignmentRepository.GetByIdAsync(id, cancellationToken);
        if (assignment == null) return NotFound();
        await _shiftAssignmentRepository.DeleteAsync(assignment, cancellationToken);
        return NoContent();
    }
}