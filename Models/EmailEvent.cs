using System.ComponentModel.DataAnnotations;
using StationCheck.Models.Common;

namespace StationCheck.Models
{
    public class EmailEvent : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string StationCode { get; set; } = string.Empty; // ST000001

        public Guid? StationId { get; set; }
        public Station? Station { get; set; }

        [MaxLength(200)]
        public string? AlarmEvent { get; set; } // Motion Detection

        public int? AlarmInputChannelNo { get; set; }

        [MaxLength(200)]
        public string? AlarmInputChannelName { get; set; } // IPC

        public DateTime? AlarmStartTime { get; set; } // Parsed from email

        [MaxLength(200)]
        public string? AlarmDeviceName { get; set; } // NVR-6C39

        [MaxLength(200)]
        public string? AlarmName { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; } // 192.168.1.200

        [MaxLength(2000)]
        public string? AlarmDetails { get; set; }

        // Email metadata
        [MaxLength(500)]
        public string? EmailMessageId { get; set; } // Unique identifier from email header
        
        [MaxLength(200)]
        public string? EmailSubject { get; set; }

        [MaxLength(500)]
        public string? EmailFrom { get; set; }

        public DateTime EmailReceivedAt { get; set; }

        [MaxLength(4000)]
        public string? RawEmailBody { get; set; }

        public bool IsProcessed { get; set; } = false;
    }
}
