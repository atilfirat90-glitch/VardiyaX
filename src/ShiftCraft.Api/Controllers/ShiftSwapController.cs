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
public class ShiftSwapController : ControllerBase
{
    private readonly IShiftSwapRepository _swapRepository;
    private readonly IShiftAssignmentRepository _shiftAssignmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ShiftSwapController> _logger;

    public ShiftSwapController(
        IShiftSwapRepository swapRepository,
        IShiftAssignmentRepository shiftAssignmentRepository,
        IEmployeeRepository employeeRepository,
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        ILogger<ShiftSwapController> logger)
    {
        _swapRepository = swapRepository;
        _shiftAssignmentRepository = shiftAssignmentRepository;
        _employeeRepository = employeeRepository;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<IEnumerable<ShiftSwapDto>>> GetPendingByBusiness(int businessId, CancellationToken ct)
    {
        var swaps = await _swapRepository.GetPendingByBusinessAsync(businessId, ct);
        var dtos = swaps.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<ShiftSwapDto>>> GetByEmployee(int employeeId, CancellationToken ct)
    {
        var swaps = await _swapRepository.GetByEmployeeAsync(employeeId, ct);
        var dtos = swaps.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult<ShiftSwapDto>> Create([FromBody] CreateSwapRequest request, CancellationToken ct)
    {
        var shift = await _shiftAssignmentRepository.GetByIdAsync(request.RequesterShiftId, ct);
        if (shift == null)
            return BadRequest(new { message = "Vardiya bulunamadı" });

        var employee = await _employeeRepository.GetByIdAsync(shift.EmployeeId, ct);
        if (employee == null)
            return BadRequest(new { message = "Çalışan bulunamadı" });

        var swap = new ShiftSwapRequest
        {
            RequesterId = shift.EmployeeId,
            RequesterShiftId = request.RequesterShiftId,
            TargetEmployeeId = request.TargetEmployeeId,
            Reason = request.Reason,
            BusinessId = employee.BusinessId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _swapRepository.AddAsync(swap, ct);
        _logger.LogInformation("Shift swap request created: {Id} by employee {EmployeeId}", created.Id, shift.EmployeeId);

        return CreatedAtAction(nameof(GetByEmployee), new { employeeId = shift.EmployeeId }, MapToDto(created));
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
    {
        var swap = await _swapRepository.GetByIdAsync(id, ct);
        if (swap == null)
            return NotFound();

        if (swap.Status != "Pending")
            return BadRequest(new { message = "Bu talep zaten işlenmiş" });

        var requesterShift = await _shiftAssignmentRepository.GetByIdAsync(swap.RequesterShiftId, ct);
        if (requesterShift == null)
            return BadRequest(new { message = "Talep eden vardiya bulunamadı" });

        if (swap.TargetShiftId.HasValue && swap.TargetEmployeeId.HasValue)
        {
            var targetShift = await _shiftAssignmentRepository.GetByIdAsync(swap.TargetShiftId.Value, ct);
            if (targetShift == null)
                return BadRequest(new { message = "Hedef vardiya bulunamadı" });

            var tempEmployeeId = requesterShift.EmployeeId;
            requesterShift.EmployeeId = targetShift.EmployeeId;
            targetShift.EmployeeId = tempEmployeeId;

            await _shiftAssignmentRepository.UpdateAsync(requesterShift, ct);
            await _shiftAssignmentRepository.UpdateAsync(targetShift, ct);
        }

        swap.Status = "Approved";
        swap.ResolvedAt = DateTime.UtcNow;
        swap.ResolvedBy = User.Identity?.Name ?? "System";
        await _swapRepository.UpdateAsync(swap, ct);

        await SendSwapNotificationAsync(swap, "Takas talebiniz onaylandı", ct);

        _logger.LogInformation("Shift swap {Id} approved", id);
        return NoContent();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Reject(int id, CancellationToken ct)
    {
        var swap = await _swapRepository.GetByIdAsync(id, ct);
        if (swap == null)
            return NotFound();

        if (swap.Status != "Pending")
            return BadRequest(new { message = "Bu talep zaten işlenmiş" });

        swap.Status = "Rejected";
        swap.ResolvedAt = DateTime.UtcNow;
        swap.ResolvedBy = User.Identity?.Name ?? "System";
        await _swapRepository.UpdateAsync(swap, ct);

        await SendSwapNotificationAsync(swap, "Takas talebiniz reddedildi", ct);

        _logger.LogInformation("Shift swap {Id} rejected", id);
        return NoContent();
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var swap = await _swapRepository.GetByIdAsync(id, ct);
        if (swap == null)
            return NotFound();

        if (swap.Status != "Pending")
            return BadRequest(new { message = "Bu talep zaten işlenmiş" });

        swap.Status = "Cancelled";
        swap.ResolvedAt = DateTime.UtcNow;
        await _swapRepository.UpdateAsync(swap, ct);

        _logger.LogInformation("Shift swap {Id} cancelled", id);
        return NoContent();
    }

    private async Task SendSwapNotificationAsync(ShiftSwapRequest swap, string message, CancellationToken ct)
    {
        try
        {
            var users = await _userRepository.GetAllAsync(ct);
            var requesterUser = users.FirstOrDefault(u => u.Username != null);

            if (requesterUser != null)
            {
                var notification = new Notification
                {
                    UserId = requesterUser.Id,
                    Title = "Vardiya Takas",
                    Body = message,
                    Type = "ShiftSwap",
                    Action = "navigate_swaps",
                    BusinessId = swap.BusinessId,
                    CreatedAt = DateTime.UtcNow
                };
                await _notificationRepository.AddAsync(notification, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send swap notification for swap {SwapId}", swap.Id);
        }
    }

    private static ShiftSwapDto MapToDto(ShiftSwapRequest s) => new()
    {
        Id = s.Id,
        RequesterId = s.RequesterId,
        RequesterName = s.Requester?.Name ?? string.Empty,
        RequesterShiftId = s.RequesterShiftId,
        TargetEmployeeId = s.TargetEmployeeId,
        TargetEmployeeName = s.TargetEmployee?.Name,
        TargetShiftId = s.TargetShiftId,
        Status = s.Status,
        Reason = s.Reason,
        CreatedAt = s.CreatedAt,
        ResolvedAt = s.ResolvedAt,
        ResolvedBy = s.ResolvedBy,
        BusinessId = s.BusinessId
    };
}
