using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models
{
    public class ConfigurationAuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EntityType { get; set; } = string.Empty; // "TimeFrame", "Station", etc.

        [Required]
        public int EntityId { get; set; }

        [MaxLength(200)]
        public string? EntityName { get; set; }

        [Required]
        [MaxLength(50)]
        public string ActionType { get; set; } = string.Empty; // "Create", "Update", "Delete"

        public string? OldValue { get; set; } // JSON snapshot of old values

        public string? NewValue { get; set; } // JSON snapshot of new values

        [MaxLength(500)]
        public string? Changes { get; set; } // Human-readable summary of changes

        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(100)]
        public string ChangedBy { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }
    }
}
