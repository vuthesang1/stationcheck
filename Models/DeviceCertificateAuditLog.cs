using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationCheck.Models
{
    public enum DeviceAuditAction
    {
        Registered = 0,
        Approved = 1,
        Rejected = 2,
        Revoked = 3,
        LoginSuccess = 4,
        LoginFailed = 5,
        UserAssigned = 6,
        UserRemoved = 7
    }

    public class DeviceCertificateAuditLog : BaseAuditEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid DeviceId { get; set; }

        [MaxLength(50)]
        public string? UserId { get; set; }

        [Required]
        public DeviceAuditAction Action { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(200)]
        public string? IPAddress { get; set; }

        // Navigation
        [ForeignKey("DeviceId")]
        public UserDevice? Device { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
