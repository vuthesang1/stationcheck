using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StationCheck.Models.Common;

namespace StationCheck.Models
{
    public enum AlertSeverity
    {
        Info = 0,
        Warning = 1,
        Critical = 2
    }

    public class MotionAlert : AuditableEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Station reference
        public Guid? StationId { get; set; }
        
        [ForeignKey(nameof(StationId))]
        public Station? Station { get; set; }

        [MaxLength(200)]
        public string? StationName { get; set; }

        // TimeFrame reference (configuration used when generating alert)
        public Guid? TimeFrameId { get; set; }
        
        [ForeignKey(nameof(TimeFrameId))]
        public TimeFrame? TimeFrame { get; set; }

        // Configuration snapshot (JSON) for audit trail
        [MaxLength(4000)]
        public string? ConfigurationSnapshot { get; set; }

        // Alert details
        public DateTime AlertTime { get; set; } = DateTime.Now;

        public AlertSeverity Severity { get; set; } = AlertSeverity.Warning;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public int ExpectedFrequencyMinutes { get; set; }

        public DateTime? LastMotionAt { get; set; }

        public int MinutesSinceLastMotion { get; set; }

        // Track which camera had last motion (for multi-camera stations)
        [MaxLength(50)]
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

        // Legacy fields (keep for backwards compatibility, mark as obsolete)
        [Obsolete("Use StationId instead. This field is kept for backwards compatibility.")]
        [MaxLength(50)]
        public string? CameraId { get; set; }

        [Obsolete("Use StationName instead. This field is kept for backwards compatibility.")]
        [MaxLength(200)]
        public string? CameraName { get; set; }
    }
}
