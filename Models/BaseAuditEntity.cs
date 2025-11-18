using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models
{
    public abstract class BaseAuditEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [MaxLength(50)]
        public string? ModifiedBy { get; set; }

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [MaxLength(50)]
        public string? DeletedBy { get; set; }
    }
}
