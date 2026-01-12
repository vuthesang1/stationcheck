using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Hubs;
using StationCheck.Models;

namespace StationCheck.BackgroundServices;

/// <summary>
/// Background service that monitors device status changes and notifies affected users in real-time
/// Detects when devices are removed, disabled, revoked, or user assignments are removed
/// </summary>
public class DeviceStatusMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeviceStatusMonitorService> _logger;
    private readonly IHubContext<DeviceStatusHub> _hubContext;
    private readonly Dictionary<string, DeviceStatusSnapshot> _lastKnownStatus = new();
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10); // Check every 10 seconds

    public DeviceStatusMonitorService(
        IServiceProvider serviceProvider,
        ILogger<DeviceStatusMonitorService> logger,
        IHubContext<DeviceStatusHub> hubContext)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[DeviceStatusMonitor] ✅ Service started");

        // Wait a bit for app to fully start
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        
        _logger.LogInformation("[DeviceStatusMonitor] 🔄 Starting monitoring loop...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("[DeviceStatusMonitor] 🔍 Checking device status changes...");
                await CheckDeviceStatusChangesAsync(stoppingToken);
                _logger.LogInformation("[DeviceStatusMonitor] ✅ Check completed. Devices tracked: {Count}", _lastKnownStatus.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DeviceStatusMonitor] ❌ Error checking device status: {Message}", ex.Message);
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("[DeviceStatusMonitor] ⏹️ Service stopped");
    }

    private async Task CheckDeviceStatusChangesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Get current status of all active devices with assignments
        var currentStatus = await dbContext.UserDevices
            .Where(d => !d.IsDeleted)
            .Include(d => d.Assignments)
            .Select(d => new DeviceStatusSnapshot
            {
                DeviceId = d.Id.ToString(), // Convert Guid to string
                MacAddress = d.MacAddress ?? string.Empty,
                DeviceStatus = d.DeviceStatus,
                IsRevoked = d.IsRevoked,
                AssignedUserIds = d.Assignments!
                    .Where(a => a.IsActive && !a.IsDeleted)
                    .Select(a => a.UserId)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        // Check for changes
        foreach (var current in currentStatus)
        {
            var key = current.DeviceId;

            if (_lastKnownStatus.TryGetValue(key, out var previous))
            {
                // Device was approved, now it's not
                if (previous.DeviceStatus == DeviceStatus.Approved && current.DeviceStatus != DeviceStatus.Approved)
                {
                    _logger.LogWarning("[DeviceStatusMonitor] ⚠️ Device {DeviceId} status changed from {OldStatus} to {NewStatus}", 
                        current.DeviceId, previous.DeviceStatus, current.DeviceStatus);
                    
                    await NotifyUsersAsync(current.AssignedUserIds, 
                        "Thiết bị của bạn đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.",
                        "DEVICE_DISABLED");
                }

                // Device was not revoked, now it is
                if (!previous.IsRevoked && current.IsRevoked)
                {
                    _logger.LogWarning("[DeviceStatusMonitor] ⚠️ Device {DeviceId} has been revoked", current.DeviceId);
                    
                    await NotifyUsersAsync(current.AssignedUserIds, 
                        "Thiết bị của bạn đã bị thu hồi. Vui lòng liên hệ quản trị viên.",
                        "DEVICE_REVOKED");
                }

                // Check for removed user assignments
                var removedUsers = previous.AssignedUserIds.Except(current.AssignedUserIds).ToList();
                if (removedUsers.Any())
                {
                    _logger.LogWarning("[DeviceStatusMonitor] ⚠️ Users removed from device {DeviceId}: {UserIds}", 
                        current.DeviceId, string.Join(", ", removedUsers));
                    
                    await NotifyUsersAsync(removedUsers, 
                        "Bạn đã bị gỡ quyền truy cập khỏi thiết bị này. Vui lòng liên hệ quản trị viên.",
                        "USER_REMOVED");
                }
            }

            _lastKnownStatus[key] = current;
        }

        // Check for deleted devices
        var deletedDeviceIds = _lastKnownStatus.Keys.Except(currentStatus.Select(c => c.DeviceId)).ToList();
        foreach (var deletedId in deletedDeviceIds)
        {
            var deleted = _lastKnownStatus[deletedId];
            _logger.LogWarning("[DeviceStatusMonitor] ⚠️ Device {DeviceId} has been deleted", deletedId);
            
            await NotifyUsersAsync(deleted.AssignedUserIds, 
                "Thiết bị của bạn đã bị xóa khỏi hệ thống. Vui lòng liên hệ quản trị viên.",
                "DEVICE_DELETED");

            _lastKnownStatus.Remove(deletedId);
        }
    }

    private async Task NotifyUsersAsync(List<string> userIds, string message, string reason)
    {
        foreach (var userId in userIds)
        {
            try
            {
                var groupName = $"user_{userId}";
                
                _logger.LogInformation("[DeviceStatusMonitor] 📢 Notifying user {UserId}: {Message} (Reason: {Reason})", 
                    userId, message, reason);

                await _hubContext.Clients.Group(groupName).SendAsync("DeviceStatusChanged", new
                {
                    message = message,
                    reason = reason,
                    requiresLogout = true,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DeviceStatusMonitor] ❌ Failed to notify user {UserId}", userId);
            }
        }
    }

    private class DeviceStatusSnapshot
    {
        public string DeviceId { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public DeviceStatus DeviceStatus { get; set; }
        public bool IsRevoked { get; set; }
        public List<string> AssignedUserIds { get; set; } = new();
    }
}
