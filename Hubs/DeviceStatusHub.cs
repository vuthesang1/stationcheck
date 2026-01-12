using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StationCheck.Hubs;

/// <summary>
/// SignalR Hub for real-time device status notifications
/// Notifies employees when their device is removed, disabled, or revoked
/// </summary>
[Authorize]
public class DeviceStatusHub : Hub
{
    private readonly ILogger<DeviceStatusHub> _logger;

    public DeviceStatusHub(ILogger<DeviceStatusHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = Context.User?.Identity?.Name;
        
        if (!string.IsNullOrEmpty(userId))
        {
            // Add user to their personal group for targeted notifications
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("[DeviceStatusHub] User {Username} ({UserId}) connected. ConnectionId: {ConnectionId}", 
                username, userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = Context.User?.Identity?.Name;
        
        _logger.LogInformation("[DeviceStatusHub] User {Username} ({UserId}) disconnected. ConnectionId: {ConnectionId}", 
            username, userId, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
}
