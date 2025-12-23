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
public class EmployeeRoleController : ControllerBase
{
    private readonly IEmployeeRoleRepository _employeeRoleRepository;

    public EmployeeRoleController(IEmployeeRoleRepository employeeRoleRepository)
    {
        _employeeRoleRepository = employeeRoleRepository;
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<EmployeeRole>>> GetByEmployeeId(int employeeId, CancellationToken cancellationToken)
    {
        var roles = await _employeeRoleRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);
        return Ok(roles);
    }

    [HttpGet("role/{roleId}")]
    public async Task<ActionResult<IEnumerable<EmployeeRole>>> GetByRoleId(int roleId, CancellationToken cancellationToken)
    {
        var employees = await _employeeRoleRepository.GetByRoleIdAsync(roleId, cancellationToken);
        return Ok(employees);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeRole>> Create(EmployeeRole employeeRole, CancellationToken cancellationToken)
    {
        var created = await _employeeRoleRepository.AddAsync(employeeRole, cancellationToken);
        return Ok(created);
    }

    [HttpDelete("{employeeId}/{roleId}")]
    public async Task<IActionResult> Delete(int employeeId, int roleId, CancellationToken cancellationToken)
    {
        var employeeRole = await _employeeRoleRepository.GetAsync(employeeId, roleId, cancellationToken);
        if (employeeRole == null) return NotFound();
        await _employeeRoleRepository.DeleteAsync(employeeRole, cancellationToken);
        return NoContent();
    }
}