using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models
{
    public class MonitoringProfile : BaseAuditEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // ✅ Version control for profile changes
        [Required]
        public int Version { get; set; } = 1;

        // Navigation property for time frames
        public ICollection<TimeFrame> TimeFrames { get; set; } = new List<TimeFrame>();

        // ✅ Navigation property for historical versions
        public ICollection<MonitoringProfileHistory> History { get; set; } = new List<MonitoringProfileHistory>();
    }
}
