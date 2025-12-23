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
public class RoleController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;

    public RoleController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Role>> GetById(int id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null) return NotFound();
        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<Role>> Create(Role role, CancellationToken cancellationToken)
    {
        var created = await _roleRepository.AddAsync(role, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Role role, CancellationToken cancellationToken)
    {
        if (id != role.Id) return BadRequest();
        await _roleRepository.UpdateAsync(role, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null) return NotFound();
        await _roleRepository.DeleteAsync(role, cancellationToken);
        return NoContent();
    }
}