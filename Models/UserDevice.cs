using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationCheck.Models
{
    /// <summary>
    /// Device status enum for MAC address authentication
    /// </summary>
    public enum DeviceStatus
    {
        PendingApproval = 0,
        Approved = 1,
        Rejected = 2
    }

    public class UserDevice : BaseAuditEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// MAC Address in format XX:XX:XX:XX:XX:XX (primary device identifier)
        /// Will replace certificate-based authentication
        /// </summary>
        [MaxLength(17)]
        public string? MacAddress { get; set; }

        /// <summary>
        /// Device approval status (will replace IsApproved boolean)
        /// </summary>
        public DeviceStatus DeviceStatus { get; set; } = DeviceStatus.PendingApproval;

        // Certificate fields - keep for backward compatibility during migration
        [Required]
        [MaxLength(200)]
        public string CertificateThumbprint { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string CertificateSubject { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? CertificateIssuer { get; set; }

        public DateTime CertificateValidFrom { get; set; }

        public DateTime CertificateValidTo { get; set; }

        // Comma-separated EKU OIDs, e.g., "1.3.6.1.5.5.7.3.2" for Client Authentication
        [MaxLength(500)]
        public string? EKUOids { get; set; }

        public bool IsApproved { get; set; } = false;

        public bool IsRevoked { get; set; } = false;

        [MaxLength(50)]
        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [MaxLength(200)]
        public string? IPAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        public DateTime? LastUsedAt { get; set; }

        // Navigation
        // Device ownership/assignment is managed via DeviceUserAssignments table
        public ICollection<DeviceUserAssignment>? Assignments { get; set; }
        public ICollection<DeviceCertificateAuditLog>? AuditLogs { get; set; }
    }
}
