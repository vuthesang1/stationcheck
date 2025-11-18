using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models
{
    public class MonitoringConfiguration : BaseAuditEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsEnabled { get; set; } = true;

        public Guid? StationId { get; set; }
        public Station? Station { get; set; }

        public Guid? ProfileId { get; set; }
        public MonitoringProfile? Profile { get; set; }
    }
}
