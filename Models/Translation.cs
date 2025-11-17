using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationCheck.Models
{
    public class Translation
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string LanguageCode { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(200)]
        public string Key { get; set; } = string.Empty; // e.g. "menu.dashboard", "button.save"
        
        [Required]
        public string Value { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Category { get; set; } // menu, button, label, message, etc.
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
        
        // Navigation
        [ForeignKey(nameof(LanguageCode))]
        public virtual Language Language { get; set; } = null!;
    }
}
