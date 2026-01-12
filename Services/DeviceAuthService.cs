using Microsoft.AspNetCore.SignalR;
using StationCheck.Hubs;
using StationCheck.Data;
using Microsoft.EntityFrameworkCore;

namespace StationCheck.Services;

public interface IDeviceAuthService
{
    Task ForceLogoutUserAsync(string userId, string reason, string message);
    Task ForceLogoutDeviceAsync(string macAddress, string reason, string message);
    Task NotifyDeviceRevokedAsync(Guid deviceId);
    Task NotifyUserRemovedFromDeviceAsync(Guid deviceId, string userId);
}

public class DeviceAuthService : IDeviceAuthService
{
    private readonly IHubContext<AuthHub> _hubContext;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ILogger<DeviceAuthService> _logger;

    public DeviceAuthService(
        IHubContext<AuthHub> hubContext,
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        ILogger<DeviceAuthService> logger)
    {
        _hubContext = hubContext;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Force logout a specific user across all devices
    /// </summary>
    public async Task ForceLogoutUserAsync(string userId, string reason, string message)
    {
        _logger.LogWarning($"[ForceLogout] Forcing logout for user: {userId}, Reason: {reason}");

        await _hubContext.Clients.Group($"user_{userId}").SendAsync("ForceLogout", new
        {
            reason,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Force logout all users on a specific device
    /// </summary>
    public async Task ForceLogoutDeviceAsync(string macAddress, string reason, string message)
    {
        _logger.LogWarning($"[ForceLogout] Forcing logout for device: {macAddress}, Reason: {reason}");

        await _hubContext.Clients.Group($"device_{macAddress}").SendAsync("ForceLogout", new
        {
            reason,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Notify when a device is revoked
    /// </summary>
    public async Task NotifyDeviceRevokedAsync(Guid deviceId)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var device = await context.UserDevices
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == deviceId);

        if (device == null)
        {
            _logger.LogWarning($"[NotifyDeviceRevoked] Device not found: {deviceId}");
            return;
        }

        _logger.LogWarning($"[NotifyDeviceRevoked] Device revoked: {device.MacAddress}");

        if (!string.IsNullOrEmpty(device.MacAddress))
        {
            await ForceLogoutDeviceAsync(
                device.MacAddress,
                "DeviceRevoked",
                "Thiết bị của bạn đã bị thu hồi quyền truy cập. Vui lòng liên hệ quản trị viên."
            );
        }
    }

    /// <summary>
    /// Notify when a user is removed from a device
    /// </summary>
    public async Task NotifyUserRemovedFromDeviceAsync(Guid deviceId, string userId)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();

        var device = await context.UserDevices
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == deviceId);

        if (device == null)
        {
            _logger.LogWarning($"[NotifyUserRemoved] Device not found: {deviceId}");
            return;
        }

        _logger.LogWarning($"[NotifyUserRemoved] User {userId} removed from device {device.MacAddress}");

        // Send to specific user on specific device
        await _hubContext.Clients.Group($"user_{userId}").SendAsync("ForceLogout", new
        {
            reason = "UserRemovedFromDevice",
            message = "Bạn đã bị gỡ khỏi thiết bị này. Vui lòng liên hệ quản trị viên.",
            deviceMacAddress = device.MacAddress,
            timestamp = DateTime.UtcNow
        });
    }
}
