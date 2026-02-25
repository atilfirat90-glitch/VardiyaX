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
public class AvailabilityController : ControllerBase
{
    private readonly IEmployeeAvailabilityRepository _availabilityRepository;
    private readonly ILogger<AvailabilityController> _logger;

    public AvailabilityController(
        IEmployeeAvailabilityRepository availabilityRepository,
        ILogger<AvailabilityController> logger)
    {
        _availabilityRepository = availabilityRepository;
        _logger = logger;
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetByEmployee(int employeeId, CancellationToken ct)
    {
        var items = await _availabilityRepository.GetByEmployeeAsync(employeeId, ct);
        var dtos = items.Select(a => new AvailabilityDto
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            DayOfWeek = a.DayOfWeek,
            AvailableFrom = a.AvailableFrom,
            AvailableTo = a.AvailableTo,
            IsAvailable = a.IsAvailable
        });
        return Ok(dtos);
    }

    [HttpPut("employee/{employeeId}")]
    public async Task<IActionResult> UpdateByEmployee(
        int employeeId,
        [FromBody] List<UpdateAvailabilityRequest> request,
        CancellationToken ct)
    {
        await _availabilityRepository.DeleteByEmployeeAsync(employeeId, ct);

        foreach (var item in request)
        {
            var entity = new EmployeeAvailability
            {
                EmployeeId = employeeId,
                DayOfWeek = item.DayOfWeek,
                AvailableFrom = item.AvailableFrom,
                AvailableTo = item.AvailableTo,
                IsAvailable = item.IsAvailable
            };
            await _availabilityRepository.AddAsync(entity, ct);
        }

        _logger.LogInformation("Availability updated for employee {EmployeeId}", employeeId);
        return NoContent();
    }
}
