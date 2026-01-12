using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationCheck.Models
{
    public class DeviceUserAssignment : BaseAuditEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid DeviceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? AssignedBy { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        [ForeignKey("DeviceId")]
        public UserDevice? Device { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
