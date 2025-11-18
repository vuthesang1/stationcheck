using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StationCheck.Models
{
    /// <summary>
    /// Audit log và version history cho TimeFrame
    /// Mỗi lần create/update/delete TimeFrame sẽ tạo 1 record mới
    /// </summary>
    public class TimeFrameHistory
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Reference tới TimeFrame (có thể null nếu TimeFrame đã bị xóa)
        /// </summary>
        public Guid? TimeFrameId { get; set; }

        /// <summary>
        /// Reference tới Station
        /// </summary>
        [Required]
        public Guid? StationId { get; set; }

        /// <summary>
        /// Version number - tăng dần cho mỗi TimeFrame
        /// </summary>
        [Required]
        public int Version { get; set; }

        /// <summary>
        /// Action type: Create, Update, Delete
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Snapshot của TimeFrame configuration tại thời điểm này
        /// JSON format
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ConfigurationSnapshot { get; set; } = string.Empty;

        /// <summary>
        /// Thông tin thay đổi (optional)
        /// VD: "Changed FrequencyMinutes from 60 to 30"
        /// </summary>
        [MaxLength(1000)]
        public string? ChangeDescription { get; set; }

        /// <summary>
        /// User thực hiện thay đổi
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string ChangedBy { get; set; } = string.Empty;

        /// <summary>
        /// Thời điểm thay đổi
        /// </summary>
        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual TimeFrame? TimeFrame { get; set; }
        public virtual Station Station { get; set; } = null!;
    }
}
