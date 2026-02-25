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
public class TeamMessageController : ControllerBase
{
    private readonly ITeamMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TeamMessageController> _logger;

    public TeamMessageController(
        ITeamMessageRepository messageRepository,
        IUserRepository userRepository,
        ILogger<TeamMessageController> logger)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet("{channel}")]
    public async Task<ActionResult<IEnumerable<TeamMessageDto>>> GetByChannel(
        string channel,
        [FromQuery] int businessId = 1,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var messages = await _messageRepository.GetByChannelAsync(channel, businessId, page, pageSize, ct);
        return Ok(messages.Select(MapToDto));
    }

    [HttpGet("announcements")]
    public async Task<ActionResult<IEnumerable<TeamMessageDto>>> GetAnnouncements(
        [FromQuery] int businessId = 1,
        CancellationToken ct = default)
    {
        var messages = await _messageRepository.GetAnnouncementsAsync(businessId, ct);
        return Ok(messages.Select(MapToDto));
    }

    [HttpPost]
    public async Task<ActionResult<TeamMessageDto>> SendMessage([FromBody] SendMessageRequest request, CancellationToken ct)
    {
        var username = User.Identity?.Name ?? "anonymous";
        var user = await _userRepository.GetByUsernameAsync(username, ct);

        var message = new TeamMessage
        {
            SenderUserId = user?.Id ?? 0,
            SenderName = username,
            Content = request.Content,
            Channel = request.Channel,
            BusinessId = request.BusinessId,
            IsAnnouncement = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _messageRepository.AddAsync(message, ct);
        _logger.LogInformation("Message sent to {Channel} by {User}", request.Channel, username);

        return CreatedAtAction(nameof(GetByChannel), new { channel = request.Channel }, MapToDto(created));
    }

    [HttpPost("announcement")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<TeamMessageDto>> SendAnnouncement([FromBody] SendAnnouncementRequest request, CancellationToken ct)
    {
        var username = User.Identity?.Name ?? "anonymous";
        var user = await _userRepository.GetByUsernameAsync(username, ct);

        var message = new TeamMessage
        {
            SenderUserId = user?.Id ?? 0,
            SenderName = username,
            Content = request.Content,
            Channel = "announcement",
            BusinessId = request.BusinessId,
            IsAnnouncement = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _messageRepository.AddAsync(message, ct);
        _logger.LogInformation("Announcement sent by {User}", username);

        return CreatedAtAction(nameof(GetAnnouncements), null, MapToDto(created));
    }

    private static TeamMessageDto MapToDto(TeamMessage m) => new()
    {
        Id = m.Id,
        SenderUserId = m.SenderUserId,
        SenderName = m.SenderName,
        Content = m.Content,
        Channel = m.Channel,
        IsAnnouncement = m.IsAnnouncement,
        CreatedAt = m.CreatedAt,
        BusinessId = m.BusinessId
    };
}
