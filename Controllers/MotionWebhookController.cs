using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Models;
using System.Text.Json;

namespace StationCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotionWebhookController : ControllerBase
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILogger<MotionWebhookController> _logger;

        public MotionWebhookController(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            ILogger<MotionWebhookController> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        /// <summary>
        /// Nhận webhook từ NVR khi phát hiện chuyển động
        /// POST /api/motionwebhook/motion
        /// </summary>
        [HttpPost("motion")]
        public async Task<IActionResult> ReceiveMotion([FromBody] MotionWebhookPayload payload)
        {
            try
            {
                _logger.LogInformation(
                    "[MotionWebhook] Received motion event for StationCode: {StationCode}, Camera: {CameraChannel}",
                    payload.StationCode,
                    payload.CameraChannel
                );

                using var context = await _contextFactory.CreateDbContextAsync();

                // Tìm trạm theo mã
                var station = await context.Stations
                    .FirstOrDefaultAsync(s => s.StationCode == payload.StationCode);

                if (station == null)
                {
                    _logger.LogWarning(
                        "[MotionWebhook] Station not found: {StationCode}",
                        payload.StationCode
                    );
                    return NotFound(new { error = $"Station {payload.StationCode} not found" });
                }

                // Tạo MotionEvent mới
                var motionEvent = new MotionEvent
                {
                    StationId = station.Id,
                    CameraId = payload.CameraChannel,
                    CameraName = payload.CameraName ?? $"Camera {payload.CameraChannel}",
                    EventType = payload.EventType ?? "Motion Detected",
                    DetectedAt = payload.DetectedAt ?? DateTime.Now,
                    IsProcessed = false,
                    CreatedAt = DateTime.Now
                };

                context.MotionEvents.Add(motionEvent);

                // Cập nhật LastMotionDetectedAt cho trạm
                station.LastMotionDetectedAt = motionEvent.DetectedAt;
                station.ModifiedAt = DateTime.Now;

                // Tự động resolve các alert đang active cho trạm này
                var activeAlerts = await context.MotionAlerts
                    .Where(a => a.StationId == station.Id && !a.IsResolved)
                    .ToListAsync();

                foreach (var alert in activeAlerts)
                {
                    alert.IsResolved = true;
                    alert.ResolvedAt = DateTime.Now;
                    alert.ResolvedBy = "System (Auto)";
                    alert.Notes = $"Motion detected at {motionEvent.DetectedAt:yyyy-MM-dd HH:mm:ss}";
                }

                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "[MotionWebhook] Motion event saved for {StationCode}, resolved {AlertCount} alerts",
                    payload.StationCode,
                    activeAlerts.Count
                );

                return Ok(new
                {
                    success = true,
                    message = "Motion event recorded successfully",
                    eventId = motionEvent.Id,
                    alertsResolved = activeAlerts.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MotionWebhook] Error processing motion event");
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        /// <summary>
        /// Nhận webhook dạng form-data (cho một số NVR cũ)
        /// POST /api/motionwebhook/motion-form
        /// </summary>
        [HttpPost("motion-form")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ReceiveMotionForm([FromForm] MotionWebhookFormPayload payload)
        {
            var jsonPayload = new MotionWebhookPayload
            {
                StationCode = payload.station_code,
                CameraChannel = payload.camera_channel,
                CameraName = payload.camera_name,
                EventType = payload.event_type,
                DetectedAt = DateTime.TryParse(payload.detected_at, out var dt) ? dt : DateTime.Now,
                Confidence = double.TryParse(payload.confidence, out var conf) ? conf : null
            };

            return await ReceiveMotion(jsonPayload);
        }

        /// <summary>
        /// Test endpoint để kiểm tra API
        /// GET /api/motionwebhook/test
        /// </summary>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                status = "OK",
                timestamp = DateTime.Now,
                message = "Motion Webhook API is running"
            });
        }
    }

    /// <summary>
    /// Model cho webhook payload (JSON)
    /// </summary>
    public class MotionWebhookPayload
    {
        public string StationCode { get; set; } = string.Empty;
        public string CameraChannel { get; set; } = string.Empty;
        public string? CameraName { get; set; }
        public string? EventType { get; set; }
        public DateTime? DetectedAt { get; set; }
        public double? Confidence { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }

    /// <summary>
    /// Model cho webhook payload (Form data)
    /// </summary>
    public class MotionWebhookFormPayload
    {
        public string station_code { get; set; } = string.Empty;
        public string camera_channel { get; set; } = string.Empty;
        public string? camera_name { get; set; }
        public string? event_type { get; set; }
        public string? detected_at { get; set; }
        public string? confidence { get; set; }
    }
}
