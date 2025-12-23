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
public class BusinessController : ControllerBase
{
    private readonly IBusinessRepository _businessRepository;

    public BusinessController(IBusinessRepository businessRepository)
    {
        _businessRepository = businessRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Business>>> GetAll(CancellationToken cancellationToken)
    {
        var businesses = await _businessRepository.GetAllAsync(cancellationToken);
        return Ok(businesses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Business>> GetById(int id, CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetByIdAsync(id, cancellationToken);
        if (business == null) return NotFound();
        return Ok(business);
    }

    [HttpPost]
    public async Task<ActionResult<Business>> Create(Business business, CancellationToken cancellationToken)
    {
        var created = await _businessRepository.AddAsync(business, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Business business, CancellationToken cancellationToken)
    {
        if (id != business.Id) return BadRequest();
        await _businessRepository.UpdateAsync(business, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetByIdAsync(id, cancellationToken);
        if (business == null) return NotFound();
        await _businessRepository.DeleteAsync(business, cancellationToken);
        return NoContent();
    }
}