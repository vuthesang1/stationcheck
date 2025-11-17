using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationCheck.Models
{
    public class MotionEvent
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Legacy camera reference (keep for backwards compatibility)
        [MaxLength(50)]
        public string? CameraId { get; set; }

        [MaxLength(200)]
        public string? CameraName { get; set; }

        // ✅ NEW: Station reference (primary for monitoring)
        public int? StationId { get; set; }
        
        [ForeignKey(nameof(StationId))]
        public Station? Station { get; set; }

        // ✅ NEW: Email source tracking (for email-based detection)
        [MaxLength(500)]
        public string? EmailMessageId { get; set; }  // Gmail MessageId for duplicate checking

        [MaxLength(500)]
        public string? EmailSubject { get; set; }

        // ✅ NEW: Snapshot storage path
        [MaxLength(500)]
        public string? SnapshotPath { get; set; }

        // Event details
        [Required]
        [MaxLength(50)]
        public string EventType { get; set; } = "Motion";

        [MaxLength(2000)]
        public string? Payload { get; set; }

        public DateTime DetectedAt { get; set; } = DateTime.Now;

        public bool IsProcessed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
