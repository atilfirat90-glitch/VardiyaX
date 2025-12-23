using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftCraft.Api.Models;
using ShiftCraft.Api.Services;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using System.Security.Claims;

namespace ShiftCraft.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")] // Backward compatibility
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly ILoginLogRepository _loginLogRepository;
    private readonly IPasswordService _passwordService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtService jwtService, 
        IUserRepository userRepository,
        ILoginLogRepository loginLogRepository,
        IPasswordService passwordService,
        IConfiguration configuration, 
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _loginLogRepository = loginLogRepository;
        _passwordService = passwordService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var deviceInfo = GetDeviceInfo();
        var ipAddress = GetClientIpAddress();

        // Try database user first
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        
        if (user != null)
        {
            // Database user authentication
            if (!user.IsActive)
            {
                await LogLoginAttempt(request.Username, "FailedLogin", user.BusinessId, deviceInfo, ipAddress, "User deactivated", cancellationToken);
                _logger.LogWarning("Login attempt for deactivated user: {Username}", request.Username);
                return Unauthorized(new { code = "USER_DEACTIVATED", message = "User account is deactivated" });
            }

            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                await LogLoginAttempt(request.Username, "FailedLogin", user.BusinessId, deviceInfo, ipAddress, "Invalid password", cancellationToken);
                _logger.LogWarning("Failed login attempt for user: {Username}", request.Username);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Update last login
            await _userRepository.UpdateLastLoginAsync(user.Id, cancellationToken);
            
            // Log successful login
            await LogLoginAttempt(user.Username, "Login", user.BusinessId, deviceInfo, ipAddress, null, cancellationToken);

            var token = _jwtService.GenerateToken(user.Username, user.Role.ToString(), user.BusinessId);
            var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);

            _logger.LogInformation("User {Username} logged in successfully", user.Username);

            return Ok(new LoginResponseWithContext
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                User = new UserContextDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Role = user.Role.ToString(),
                    BusinessId = user.BusinessId ?? 0,
                    BusinessName = user.Business?.Name ?? ""
                },
                MustChangePassword = user.MustChangePassword
            });
        }

        // Fallback to config-based auth (for backward compatibility)
        var validUsername = _configuration["Auth:Username"] ?? "admin";
        var validPassword = _configuration["Auth:Password"] ?? "ShiftCraft2024!";

        if (request.Username != validUsername || request.Password != validPassword)
        {
            await LogLoginAttempt(request.Username, "FailedLogin", 1, deviceInfo, ipAddress, "Invalid credentials", cancellationToken);
            _logger.LogWarning("Failed login attempt for user: {Username}", request.Username);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Log successful login (config auth)
        await LogLoginAttempt(request.Username, "Login", 1, deviceInfo, ipAddress, null, cancellationToken);

        var fallbackToken = _jwtService.GenerateToken(request.Username, "Admin", 1);
        var fallbackExpirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);

        _logger.LogInformation("User {Username} logged in successfully (config auth)", request.Username);

        return Ok(new LoginResponseWithContext
        {
            Token = fallbackToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(fallbackExpirationMinutes),
            User = new UserContextDto
            {
                UserId = 0,
                Username = request.Username,
                Role = "Admin",
                BusinessId = 1,
                BusinessName = "Default Business"
            },
            MustChangePassword = false
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var username = User.Identity?.Name ?? "unknown";
        var businessIdClaim = User.FindFirst("business_id")?.Value;
        var businessId = int.TryParse(businessIdClaim, out var bid) ? bid : (int?)null;
        
        await LogLoginAttempt(username, "Logout", businessId, GetDeviceInfo(), GetClientIpAddress(), null, cancellationToken);
        _logger.LogInformation("User {Username} logged out", username);
        
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("refresh")]
    [Authorize]
    public IActionResult Refresh()
    {
        var username = User.Identity?.Name ?? "unknown";
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Worker";
        var businessIdClaim = User.FindFirst("business_id")?.Value;
        var businessId = int.TryParse(businessIdClaim, out var bid) ? bid : (int?)null;
        
        var token = _jwtService.GenerateToken(username, role, businessId);
        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);

        _logger.LogInformation("Token refreshed for user: {Username}", username);

        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Username = username
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? User.FindFirst("role")?.Value;
        var businessId = User.FindFirst("business_id")?.Value;
        
        return Ok(new UserContextDto
        {
            UserId = 0,
            Username = User.Identity?.Name ?? "",
            Role = role ?? "Worker",
            BusinessId = int.TryParse(businessId, out var bid) ? bid : 0,
            BusinessName = ""
        });
    }

    private async Task LogLoginAttempt(
        string username, 
        string action, 
        int? businessId, 
        string? deviceInfo, 
        string? ipAddress, 
        string? failureReason,
        CancellationToken cancellationToken)
    {
        var log = new LoginLog
        {
            Username = username,
            Timestamp = DateTime.UtcNow,
            Action = action,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            FailureReason = failureReason,
            BusinessId = businessId
        };

        await _loginLogRepository.AddAsync(log, cancellationToken);
    }

    private string? GetDeviceInfo()
    {
        return Request.Headers.UserAgent.ToString();
    }

    private string? GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
