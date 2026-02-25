using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftCraft.Api.Models;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[Authorize]
public class TimeOffController : ControllerBase
{
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<TimeOffController> _logger;

    public TimeOffController(
        ITimeOffRepository timeOffRepository,
        IEmployeeRepository employeeRepository,
        ILogger<TimeOffController> logger)
    {
        _timeOffRepository = timeOffRepository;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<TimeOffDto>>> GetByEmployee(int employeeId, CancellationToken ct)
    {
        var items = await _timeOffRepository.GetByEmployeeAsync(employeeId, ct);
        return Ok(items.Select(MapToDto));
    }

    [HttpGet("business/{businessId}/pending")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<TimeOffDto>>> GetPendingByBusiness(int businessId, CancellationToken ct)
    {
        var items = await _timeOffRepository.GetPendingByBusinessAsync(businessId, ct);
        return Ok(items.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<TimeOffDto>> Create([FromBody] CreateTimeOffRequest request, CancellationToken ct)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, ct);
        if (employee == null)
            return BadRequest(new { message = "Çalışan bulunamadı" });

        var entity = new TimeOffRequest
        {
            EmployeeId = request.EmployeeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Reason = request.Reason,
            BusinessId = request.BusinessId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _timeOffRepository.AddAsync(entity, ct);
        _logger.LogInformation("Time off request created: {Id} for employee {EmployeeId}", created.Id, request.EmployeeId);

        return CreatedAtAction(nameof(GetByEmployee), new { employeeId = request.EmployeeId }, MapToDto(created));
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
    {
        var request = await _timeOffRepository.GetByIdAsync(id, ct);
        if (request == null)
            return NotFound();

        if (request.Status != "Pending")
            return BadRequest(new { message = "Bu talep zaten işlenmiş" });

        request.Status = "Approved";
        request.ResolvedAt = DateTime.UtcNow;
        request.ResolvedBy = User.Identity?.Name ?? "System";
        await _timeOffRepository.UpdateAsync(request, ct);

        _logger.LogInformation("Time off request {Id} approved", id);
        return NoContent();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectTimeOffRequest? body, CancellationToken ct)
    {
        var request = await _timeOffRepository.GetByIdAsync(id, ct);
        if (request == null)
            return NotFound();

        if (request.Status != "Pending")
            return BadRequest(new { message = "Bu talep zaten işlenmiş" });

        request.Status = "Rejected";
        request.ResponseNote = body?.ResponseNote;
        request.ResolvedAt = DateTime.UtcNow;
        request.ResolvedBy = User.Identity?.Name ?? "System";
        await _timeOffRepository.UpdateAsync(request, ct);

        _logger.LogInformation("Time off request {Id} rejected", id);
        return NoContent();
    }

    private static TimeOffDto MapToDto(TimeOffRequest t) => new()
    {
        Id = t.Id,
        EmployeeId = t.EmployeeId,
        EmployeeName = t.Employee?.Name ?? string.Empty,
        StartDate = t.StartDate,
        EndDate = t.EndDate,
        Type = t.Type,
        Status = t.Status,
        Reason = t.Reason,
        ResponseNote = t.ResponseNote,
        CreatedAt = t.CreatedAt,
        ResolvedAt = t.ResolvedAt,
        ResolvedBy = t.ResolvedBy,
        BusinessId = t.BusinessId
    };
}
