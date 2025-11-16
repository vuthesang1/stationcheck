using System.ComponentModel.DataAnnotations;

namespace StationCheck.Models;

public class Employee
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public byte[]? FaceEmbedding { get; set; } // Vector nhúng của khuôn mặt để so sánh
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(50)]
    public string? CreatedBy { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
    
    [MaxLength(50)]
    public string? ModifiedBy { get; set; }
}
