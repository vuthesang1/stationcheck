namespace StationCheck.Models;

/// <summary>
/// Quy tắc cảnh báo cho từng camera theo khung giờ (LEGACY - sẽ bị thay thế bởi MonitoringConfiguration + TimeFrame)
/// </summary>
public class AlertRule
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CameraId { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; } // Ví dụ: 08:00
    public TimeSpan EndTime { get; set; }   // Ví dụ: 17:00
    public int IntervalMinutes { get; set; } = 60; // 60, 120, 180 phút
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Tracking last motion detected cho mỗi camera
/// </summary>
public class CameraMotionStatus
{
    public string CameraId { get; set; } = string.Empty;
    public DateTime? LastMotionDetectedAt { get; set; }
    public int TotalMotionEventsToday { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
}
