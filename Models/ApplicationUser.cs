using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models
{
    public enum UserRole
    {
        StationEmployee = 0,  // Nhân viên trạm - chỉ xem được trạm của mình
        Manager = 1,          // Quản lý - cấu hình lịch, xem báo cáo tất cả trạm
        Admin = 2             // Quản trị - quản lý trạm, gán camera
    }

    public class ApplicationUser
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.StationEmployee;

        public bool IsActive { get; set; } = true;

        // Station assignment for StationEmployee role
        public int? StationId { get; set; }
        public Station? Station { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLoginAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        [MaxLength(50)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }

    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsRevoked { get; set; } = false;

        public DateTime? RevokedAt { get; set; }

        // Navigation property
        public ApplicationUser? User { get; set; }
    }
}
