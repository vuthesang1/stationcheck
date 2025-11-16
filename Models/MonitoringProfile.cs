using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models
{
    /// <summary>
    /// Monitoring Profile - Deprecated, kept for backward compatibility
    /// Use Station directly instead
    /// </summary>
    [Obsolete("Use Station directly. This class is kept for backward compatibility only.")]
    public class MonitoringProfile
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        public Guid? StationId { get; set; }
        public Station? Station { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
