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
public class ScheduleDayController : ControllerBase
{
    private readonly IScheduleDayRepository _scheduleDayRepository;

    public ScheduleDayController(IScheduleDayRepository scheduleDayRepository)
    {
        _scheduleDayRepository = scheduleDayRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleDay>>> GetAll(CancellationToken cancellationToken)
    {
        var days = await _scheduleDayRepository.GetAllAsync(cancellationToken);
        return Ok(days);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScheduleDay>> GetById(int id, CancellationToken cancellationToken)
    {
        var day = await _scheduleDayRepository.GetByIdWithAssignmentsAsync(id, cancellationToken);
        if (day == null) return NotFound();
        return Ok(day);
    }

    [HttpGet("weeklyschedule/{weeklyScheduleId}")]
    public async Task<ActionResult<IEnumerable<ScheduleDay>>> GetByWeeklyScheduleId(int weeklyScheduleId, CancellationToken cancellationToken)
    {
        var days = await _scheduleDayRepository.GetByWeeklyScheduleIdAsync(weeklyScheduleId, cancellationToken);
        return Ok(days);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleDay>> Create(ScheduleDay day, CancellationToken cancellationToken)
    {
        var created = await _scheduleDayRepository.AddAsync(day, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ScheduleDay day, CancellationToken cancellationToken)
    {
        if (id != day.Id) return BadRequest();
        await _scheduleDayRepository.UpdateAsync(day, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var day = await _scheduleDayRepository.GetByIdAsync(id, cancellationToken);
        if (day == null) return NotFound();
        await _scheduleDayRepository.DeleteAsync(day, cancellationToken);
        return NoContent();
    }
}