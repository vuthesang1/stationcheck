using System.ComponentModel.DataAnnotations;
using StationCheck.Models.Common;

namespace StationCheck.Models
{
    public class Station : AuditableEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(10)]
        public string StationCode { get; set; } = string.Empty; // ST000001

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; } = true;

        // ✅ Track last motion detection time for alert generation
        public DateTime? LastMotionDetectedAt { get; set; }

        // Navigation properties
        public ICollection<TimeFrame> TimeFrames { get; set; } = new List<TimeFrame>();
    }
}
