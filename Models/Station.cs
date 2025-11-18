using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models
{
    public class Station : BaseAuditEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
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
        public ICollection<ApplicationUser> StationEmployees { get; set; } = new List<ApplicationUser>();
        public ICollection<TimeFrame> TimeFrames { get; set; } = new List<TimeFrame>();
    }

  
}
