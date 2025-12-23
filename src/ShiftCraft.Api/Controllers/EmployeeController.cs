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
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetById(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdWithRolesAsync(id, cancellationToken);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetByBusinessId(int businessId, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        return Ok(employees);
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> Create(Employee employee, CancellationToken cancellationToken)
    {
        var created = await _employeeRepository.AddAsync(employee, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Employee employee, CancellationToken cancellationToken)
    {
        if (id != employee.Id) return BadRequest();
        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
        if (employee == null) return NotFound();
        await _employeeRepository.DeleteAsync(employee, cancellationToken);
        return NoContent();
    }
}