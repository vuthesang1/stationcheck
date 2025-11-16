using System.ComponentModel.DataAnnotations;
using StationCheck.Models.Common;

namespace StationCheck.Models
{
    public class TimeFrame : AuditableEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Link directly to Station (nullable for backward compatibility)
        public Guid? StationId { get; set; }
        public Station? Station { get; set; }

        // Keep ProfileId for backward compatibility (will be deprecated)
        [Obsolete("Use StationId instead. This field is kept for backwards compatibility.")]
        public int? ProfileId { get; set; }
        
        [Obsolete("Use Station instead. This navigation is kept for backwards compatibility.")]
        public MonitoringProfile? Profile { get; set; }

        // Time frame configuration
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        // Frequency in minutes (e.g., 5 = check every 5 minutes)
        [Required]
        public int FrequencyMinutes { get; set; } = 5;

        // Buffer time in minutes (tolerance for early/late check-ins)
        // Example: If buffer is 15 minutes and frequency is 60 minutes (1 hour),
        // then check at 10:00 will accept motion from 09:45 to 10:15
        public int BufferMinutes { get; set; } = 0;

        // Days of week (JSON array or comma-separated: "1,2,3,4,5" for Mon-Fri)
        [MaxLength(50)]
        public string? DaysOfWeek { get; set; } = "1,2,3,4,5,6,7"; // All days by default

        public bool IsEnabled { get; set; } = true;
    }
}
