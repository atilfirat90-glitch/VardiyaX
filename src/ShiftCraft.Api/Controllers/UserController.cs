using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftCraft.Api.Models;
using ShiftCraft.Api.Services;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Domain.Enums;
using System.Security.Claims;

namespace ShiftCraft.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserRepository userRepository,
        IBusinessRepository businessRepository,
        IPasswordService passwordService,
        ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _businessRepository = businessRepository;
        _passwordService = passwordService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return Forbid();

        var businessId = GetCurrentUserBusinessId();
        if (businessId == null)
            return BadRequest(new { message = "Business context required" });

        var users = await _userRepository.GetByBusinessIdAsync(businessId.Value, cancellationToken);
        var dtos = users.Select(MapToDto);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return Forbid();

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var businessId = GetCurrentUserBusinessId();
        if (businessId != null && user.BusinessId != businessId)
            return Forbid();

        return Ok(MapToDto(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return Forbid();

        // Validate username uniqueness
        if (await _userRepository.UsernameExistsAsync(request.Username, cancellationToken))
            return BadRequest(new { code = "USERNAME_EXISTS", message = "Username already exists" });

        // Validate password
        var passwordValidation = _passwordService.ValidatePassword(request.Password);
        if (!passwordValidation.IsValid)
            return BadRequest(new { code = "INVALID_PASSWORD", message = passwordValidation.ErrorMessage });

        // Validate role
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            return BadRequest(new { code = "INVALID_ROLE", message = "Invalid role. Valid roles: Admin, Manager, Worker, Trainee" });

        var businessId = request.BusinessId ?? GetCurrentUserBusinessId();

        var user = new User
        {
            Username = request.Username,
            PasswordHash = _passwordService.HashPassword(request.Password),
            Role = role,
            IsActive = true,
            MustChangePassword = false,
            CreatedAt = DateTime.UtcNow,
            BusinessId = businessId
        };

        var created = await _userRepository.AddAsync(user, cancellationToken);
        _logger.LogInformation("User {Username} created by {Admin}", user.Username, User.Identity?.Name);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return Forbid();

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var businessId = GetCurrentUserBusinessId();
        if (businessId != null && user.BusinessId != businessId)
            return Forbid();

        if (request.Role != null)
        {
            if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
                return BadRequest(new { code = "INVALID_ROLE", message = "Invalid role" });
            user.Role = role;
        }

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        await _userRepository.UpdateAsync(user, cancellationToken);
        _logger.LogInformation("User {Username} updated by {Admin}", user.Username, User.Identity?.Name);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return Forbid();

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var businessId = GetCurrentUserBusinessId();
        if (businessId != null && user.BusinessId != businessId)
            return Forbid();

        // Don't allow deactivating yourself
        var currentUsername = User.Identity?.Name;
        if (user.Username.Equals(currentUsername, StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Cannot deactivate your own account" });

        user.IsActive = false;
        await _userRepository.UpdateAsync(user, cancellationToken);
        _logger.LogInformation("User {Username} deactivated by {Admin}", user.Username, User.Identity?.Name);

        return NoContent();
    }

    [HttpPost("{id}/reset-password")]
    public async Task<ActionResult<ResetPasswordResponse>> ResetPassword(int id, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return Forbid();

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var businessId = GetCurrentUserBusinessId();
        if (businessId != null && user.BusinessId != businessId)
            return Forbid();

        var temporaryPassword = _passwordService.GenerateTemporaryPassword();
        user.PasswordHash = _passwordService.HashPassword(temporaryPassword);
        user.MustChangePassword = true;

        await _userRepository.UpdateAsync(user, cancellationToken);
        _logger.LogInformation("Password reset for user {Username} by {Admin}", user.Username, User.Identity?.Name);

        return Ok(new ResetPasswordResponse
        {
            TemporaryPassword = temporaryPassword,
            Message = "Password has been reset. User must change password on next login."
        });
    }

    private bool IsAdmin()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;
        return role == "Admin";
    }

    private int? GetCurrentUserBusinessId()
    {
        var businessIdClaim = User.FindFirst("business_id")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : null;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            MustChangePassword = user.MustChangePassword,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            BusinessId = user.BusinessId,
            BusinessName = user.Business?.Name
        };
    }
}
