using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
        
        public bool RememberMe { get; set; } = false;

        /// <summary>
        /// Device MAC address for device validation (StationEmployee only)
        /// </summary>
        [MaxLength(17)]
        public string? MacAddress { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public UserInfo? User { get; set; }
        public bool RequiresDeviceRegistration { get; set; } = false;
    }

    public class RefreshTokenRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.StationEmployee;
    }

    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public int DeviceCount { get; set; } = 0;
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UpdateUserRequest
    {
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public bool IsActive { get; set; }
    }

    // Device Registration Models
    public class DeviceRegistrationRequest
    {
        // For Desktop Login App registration
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? MacAddress { get; set; }

        // Legacy fields for DeviceInstaller
        [MaxLength(200)]
        public string? DeviceName { get; set; }

        [MaxLength(200)]
        public string? CertificateThumbprint { get; set; }

        [MaxLength(500)]
        public string? CertificateSubject { get; set; }

        [MaxLength(500)]
        public string? CertificateIssuer { get; set; }

        public DateTime? CertificateValidFrom { get; set; }

        public DateTime? CertificateValidTo { get; set; }

        [MaxLength(500)]
        public string? EKUOids { get; set; }
    }

    public class DeviceRegistrationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? DeviceId { get; set; }
        public DeviceStatus Status { get; set; }
    }

    public class DeviceStatusResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DeviceStatus Status { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUserAssigned { get; set; }
        public string? DeviceName { get; set; }
        public string? MacAddress { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
    }

    public class DeviceApprovalRequest
    {
        [Required]
        public Guid DeviceId { get; set; }

        public bool Approve { get; set; } = true;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class UserDeviceDto
    {
        public Guid Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string CertificateThumbprint { get; set; } = string.Empty;
        public string CertificateSubject { get; set; } = string.Empty;
        public DateTime CertificateValidFrom { get; set; }
        public DateTime CertificateValidTo { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PendingDeviceDto
    {
        public Guid Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string CertificateThumbprint { get; set; } = string.Empty;
        public string CertificateSubject { get; set; } = string.Empty;
        public DateTime CertificateValidFrom { get; set; }
        public DateTime CertificateValidTo { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DeviceAssignmentDetailsDto
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string OwnerUserId { get; set; } = string.Empty;
        public string OwnerUsername { get; set; } = string.Empty;
        public int AssignedUsersCount { get; set; }
        public DateTime? LastUsedAt { get; set; }
    }

    public class AssignedUserDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class AvailableUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class AssignUserRequest
    {
        [Required]
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for desktop app device login
    /// </summary>
    public class DeviceLoginRequest
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(17)]
        [RegularExpression(@"^([0-9A-F]{2}:){5}[0-9A-F]{2}$", ErrorMessage = "MAC Address must be in format XX:XX:XX:XX:XX:XX")]
        public string MacAddress { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? DeviceName { get; set; }
    }

    /// <summary>
    /// Response model for device login
    /// </summary>
    public class DeviceLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DeviceStatus Status { get; set; }
        public Guid? DeviceId { get; set; }
    }
}

