using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models
{
    /// <summary>
    /// System-wide configuration settings
    /// </summary>
    public class SystemConfiguration
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Unique configuration key (e.g., "EmailMonitorInterval", "AlertGenerationInterval")
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Configuration value (stored as string, parse as needed)
        /// </summary>
        [Required]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Display name for UI
        /// </summary>
        [MaxLength(200)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Description of what this configuration does
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Data type hint: "int", "string", "bool", "timespan"
        /// </summary>
        [MaxLength(50)]
        public string? ValueType { get; set; }

        /// <summary>
        /// UI category grouping
        /// </summary>
        [MaxLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Whether this setting can be edited by admin
        /// </summary>
        public bool IsEditable { get; set; } = true;

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "System";
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
