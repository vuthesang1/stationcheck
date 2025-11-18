using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationCheck.Models
{
    public enum AlertSeverity
    {
        Info = 0,
        Warning = 1,
        Critical = 2
    }

    public class MotionAlert
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // ✅ NEW: Station-based (PRIMARY - replaces Camera)
        public Guid? StationId { get; set; }
        
        [ForeignKey(nameof(StationId))]
        public Station? Station { get; set; }

        [MaxLength(200)]
        public string? StationName { get; set; }

        // ✅ NEW: Configuration references for audit trail
        public Guid? MonitoringConfigurationId { get; set; }
        
        [ForeignKey(nameof(MonitoringConfigurationId))]
        public MonitoringConfiguration? MonitoringConfiguration { get; set; }

        public Guid? TimeFrameId { get; set; }
        
        [ForeignKey(nameof(TimeFrameId))]
        public TimeFrame? TimeFrame { get; set; }

        // ✅ NEW: Link to specific profile version that generated this alert
        public Guid? ProfileHistoryId { get; set; }
        
        [ForeignKey(nameof(ProfileHistoryId))]
        public MonitoringProfileHistory? ProfileHistory { get; set; }

        // ✅ NEW: Link to specific timeframe version that generated this alert
        public Guid? TimeFrameHistoryId { get; set; }
        
        [ForeignKey(nameof(TimeFrameHistoryId))]
        public TimeFrameHistory? TimeFrameHistorySnapshot { get; set; }

        // ✅ NEW: Configuration snapshot (JSON) for audit
        [MaxLength(4000)]
        public string? ConfigurationSnapshot { get; set; }

        // Alert details
        public DateTime AlertTime { get; set; } = DateTime.UtcNow;

        public AlertSeverity Severity { get; set; } = AlertSeverity.Warning;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public int ExpectedFrequencyMinutes { get; set; }

        public DateTime? LastMotionAt { get; set; }

        public int MinutesSinceLastMotion { get; set; }

        // ✅ NEW: Track which camera had last motion (for multi-camera stations)
        [MaxLength(200)]  // ✅ Updated from 50 to 200 to match CameraId length
        public string? LastMotionCameraId { get; set; }

        [MaxLength(200)]
        public string? LastMotionCameraName { get; set; }

        // Resolution
        public bool IsResolved { get; set; } = false;

        public DateTime? ResolvedAt { get; set; }

        [MaxLength(100)]
        public string? ResolvedBy { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        // Legacy fields (keep for backwards compatibility, mark as obsolete)
        [Obsolete("Use StationId instead. This field is kept for backwards compatibility.")]
        [MaxLength(50)]
        public string? CameraId { get; set; }

        [Obsolete("Use StationName instead. This field is kept for backwards compatibility.")]
        [MaxLength(200)]
        public string? CameraName { get; set; }
    }
}
