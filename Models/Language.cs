using System.ComponentModel.DataAnnotations;
using StationCheck.Models.Common;

namespace StationCheck.Models
{
    public class Language : AuditableEntity
    {
        [Key]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty; // vi, en, etc.
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Vietnamese, English
        
        [MaxLength(100)]
        public string NativeName { get; set; } = string.Empty; // Tiếng Việt, English
        
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        
        [MaxLength(10)]
        public string? FlagIcon { get; set; } // vi, us, gb
        
        // Navigation
        public virtual ICollection<Translation> Translations { get; set; } = new List<Translation>();
    }
}
