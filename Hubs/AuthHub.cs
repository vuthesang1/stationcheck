using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace StationCheck.Hubs;

/// <summary>
/// SignalR Hub for real-time device and authentication notifications
/// </summary>
[Authorize]
public class AuthHub : Hub
{
    private readonly ILogger<AuthHub> _logger;

    public AuthHub(ILogger<AuthHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var macAddress = Context.User?.FindFirst("MacAddress")?.Value;

        _logger.LogInformation($"[AuthHub] User connected. ConnectionId: {Context.ConnectionId}, User: {username}, MAC: {macAddress}");

        // Add to user-specific group for targeted notifications
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        // Add to device-specific group
        if (!string.IsNullOrEmpty(macAddress))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"device_{macAddress}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        _logger.LogInformation($"[AuthHub] User disconnected. ConnectionId: {Context.ConnectionId}, User: {username}");
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Client calls this to register for notifications
    /// </summary>
    public async Task Register()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        
        _logger.LogInformation($"[AuthHub] User registered for notifications. User: {username}");
        await Clients.Caller.SendAsync("RegistrationConfirmed", new { userId, username });
    }
}
