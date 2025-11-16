using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationCheck.Models
{
    /// <summary>
    /// Lịch sử thay đổi cấu hình của Station (Audit Log)
    /// </summary>
    public class StationHistory
    {
        [Key]
        public int Id { get; set; }

        // Reference to Station (nullable - station might be deleted)
        public Guid? StationId { get; set; }
        
        [ForeignKey(nameof(StationId))]
        public Station? Station { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // Created, Updated, Deleted

        [Required]
        [MaxLength(200)]
        public string StationCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string StationName { get; set; } = string.Empty;

        // Snapshot of configuration at the time of change (JSON)
        [Required]
        public string ConfigurationSnapshot { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? ChangeDescription { get; set; }

        // Who made the change (User ID)
        [Required]
        [MaxLength(50)]
        public string ChangedBy { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; } = DateTime.Now;

        // Old and new values for specific fields (optional, for detailed tracking)
        [MaxLength(4000)]
        public string? OldValues { get; set; }

        [MaxLength(4000)]
        public string? NewValues { get; set; }
    }
}
