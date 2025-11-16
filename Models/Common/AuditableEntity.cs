using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models.Common
{
    /// <summary>
    /// Base class for entities that require audit tracking
    /// Provides CreatedAt, CreatedBy, ModifiedAt, ModifiedBy fields
    /// </summary>
    public abstract class AuditableEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [MaxLength(50)]
        public string? CreatedBy { get; set; }
        
        public DateTime? ModifiedAt { get; set; }
        
        [MaxLength(50)]
        public string? ModifiedBy { get; set; }
    }
}
