using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Interfaces;
using StationCheck.Models;

namespace StationCheck.Services;

public class MotionDetectionService : IMotionDetectionService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<MotionDetectionService> _logger;
    private readonly IMonitoringService _monitoringService;

    public MotionDetectionService(
        IDbContextFactory<ApplicationDbContext> contextFactory, 
        ILogger<MotionDetectionService> logger,
        IMonitoringService monitoringService)
    {
        _contextFactory = contextFactory;
        _logger = logger;
        _monitoringService = monitoringService;
    }

    public async Task<MotionEvent> ProcessMotionEventAsync(string cameraId, string eventType, string? payload = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        // var camera = await context.Cameras.FindAsync(cameraId);
        
        // Find station(s) that this camera belongs to
        // TODO: Implement camera-station relationship when Cameras table added back
        // var cameraStations = await context.CameraStations
        //     .Where(cs => cs.CameraId == cameraId)
        //     .Select(cs => cs.StationId)
        //     .ToListAsync();
        
        Guid? stationId = null; // TODO: Implement camera-station relationship
        
        var motionEvent = new MotionEvent
        {
            CameraId = cameraId,
            CameraName = cameraId, // TODO: Get camera name from relationship
            StationId = stationId, // Store StationId
            EventType = eventType,
            Payload = payload,
            DetectedAt = DateTime.UtcNow,
            IsProcessed = true
        };

        context.MotionEvents.Add(motionEvent);
        await context.SaveChangesAsync();

        _logger.LogInformation($"[MotionEvent] Camera {cameraId} ({motionEvent.CameraName}) detected motion at {motionEvent.DetectedAt:HH:mm:ss} - Station: {stationId}");

        // Auto-resolve station alerts when motion detected
        if (stationId.HasValue && stationId.Value != Guid.Empty)
        {
            await _monitoringService.ResolveStationAlertsAsync(stationId.Value, $"Camera {motionEvent.CameraName}");
        }

        return motionEvent;
    }

    public async Task CheckAndCreateAlertsAsync()
    {
        // TODO: Refactor to use Stations instead of Cameras
        return;
        
        /* COMMENTED OUT - Cameras table removed
        using var context = await _contextFactory.CreateDbContextAsync();
        var cameras = await context.Cameras.ToListAsync();
        var now = DateTime.UtcNow;
        var currentTime = now.TimeOfDay;

        foreach (var camera in cameras)
        {
            // Lấy rule hiện tại
            var currentRule = await GetCurrentAlertRuleAsync(camera.Id, currentTime);
            if (currentRule == null || !currentRule.IsActive)
                continue;

            // Lấy motion event cuối cùng
            var lastMotion = await context.MotionEvents
                .Where(e => e.CameraId == camera.Id)
                .OrderByDescending(e => e.DetectedAt)
                .FirstOrDefaultAsync();

            if (lastMotion == null)
            {
                _logger.LogWarning($"[Alert] No motion detected yet for camera {camera.Id}");
                continue;
            }

            var minutesSinceLastMotion = (int)(now - lastMotion.DetectedAt).TotalMinutes;

            // Kiểm tra có vượt quá interval không
            if (minutesSinceLastMotion > currentRule.IntervalMinutes)
            {
                // Kiểm tra đã có alert chưa resolve cho camera này chưa
                var existingAlert = await context.MotionAlerts
                    .Where(a => a.CameraId == camera.Id && !a.IsResolved)
                    .OrderByDescending(a => a.AlertTime)
                    .FirstOrDefaultAsync();

                if (existingAlert == null)
                {
                    // Tạo alert mới
                    var alert = new MotionAlert
                    {
                        CameraId = camera.Id,
                        CameraName = camera.Name,
                        AlertTime = now,
                        Severity = minutesSinceLastMotion > currentRule.IntervalMinutes * 2 
                            ? AlertSeverity.Critical 
                            : AlertSeverity.Warning,
                        Message = $"Không phát hiện nhấn nút trong {minutesSinceLastMotion} phút (mong đợi: {currentRule.IntervalMinutes} phút)",
                        // TODO: Sau khi refactor sang station-based, thay StationId và thêm ConfigurationSnapshot
                        // ExpectedIntervalMinutes removed - use TimeFrame logic instead
                        LastMotionAt = lastMotion.DetectedAt,
                        MinutesSinceLastMotion = minutesSinceLastMotion
                    };

                    context.MotionAlerts.Add(alert);
                    await context.SaveChangesAsync();

                    _logger.LogWarning($"[Alert] Created alert for camera {camera.Id} - {minutesSinceLastMotion} minutes since last motion");
                }
            }
            else
            {
                // Nếu đã có motion trong khoảng thời gian cho phép, resolve các alert cũ
                var unresolvedAlerts = await context.MotionAlerts
                    .Where(a => a.CameraId == camera.Id && !a.IsResolved)
                    .ToListAsync();

                foreach (var alert in unresolvedAlerts)
                {
                    alert.IsResolved = true;
                    alert.ResolvedAt = now;
                    alert.ResolvedBy = "System Auto-Resolve";
                    alert.IsDeleted = true; // Hide resolved alert from display
                    alert.Notes = "Motion detected, alert auto-resolved";
                }

                if (unresolvedAlerts.Count > 0)
                {
                    await context.SaveChangesAsync();
                    _logger.LogInformation($"[Alert] Auto-resolved {unresolvedAlerts.Count} alerts for camera {camera.Id}");
                }
            }
        }
        */
    }

    public async Task<List<MotionAlert>> GetUnresolvedAlertsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.MotionAlerts
            .Include(a => a.TimeFrame) // Include TimeFrame for BufferMinutes
            .Where(a => !a.IsResolved)
            .OrderByDescending(a => a.AlertTime)
            .ToListAsync();
    }

    public async Task<bool> ResolveAlertAsync(string alertId, string resolvedBy, string? notes = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var alert = await context.MotionAlerts.FindAsync(alertId);
        if (alert == null)
            return false;

        alert.IsResolved = true;
        alert.ResolvedAt = DateTime.UtcNow;
        alert.ResolvedBy = resolvedBy;
        // Keep alert visible - don't mark as deleted
        alert.Notes = notes;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<MotionEvent>> GetMotionEventsAsync(string? cameraId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.MotionEvents.AsQueryable();

        if (!string.IsNullOrEmpty(cameraId))
            query = query.Where(e => e.CameraId == cameraId);

        if (fromDate.HasValue)
            query = query.Where(e => e.DetectedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.DetectedAt <= toDate.Value);

        return await query
            .OrderByDescending(e => e.DetectedAt)
            .Take(100)
            .ToListAsync();
    }

    public async Task<AlertRule?> GetCurrentAlertRuleAsync(string cameraId, TimeSpan currentTime)
    {
        // TODO: Refactor to use TimeFrames instead of AlertRules
        return null;
        
        /* COMMENTED OUT - AlertRules table removed
        using var context = await _contextFactory.CreateDbContextAsync();
        var rules = await context.AlertRules
            .Where(r => r.CameraId == cameraId && r.IsActive)
            .ToListAsync();

        foreach (var rule in rules)
        {
            // Handle overnight rules (e.g., 22:00 to 08:00)
            if (rule.StartTime > rule.EndTime)
            {
                if (currentTime >= rule.StartTime || currentTime < rule.EndTime)
                    return rule;
            }
            else
            {
                if (currentTime >= rule.StartTime && currentTime < rule.EndTime)
                    return rule;
            }
        }

        return null;
        */
    }

    public async Task<Dictionary<string, int>> GetTodayMotionStatsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var stats = await context.MotionEvents
            .Where(e => e.DetectedAt >= today && e.DetectedAt < tomorrow && e.CameraId != null)
            .GroupBy(e => e.CameraId)
            .Select(g => new { CameraId = g.Key!, Count = g.Count() })
            .ToDictionaryAsync(x => x.CameraId, x => x.Count);

        return stats;
    }
}
