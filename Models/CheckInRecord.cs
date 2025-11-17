namespace StationCheck.Models;

public class CheckInRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; } = DateTime.Now;
    public string? CameraId { get; set; }
    public string? CameraName { get; set; }
    public double? Confidence { get; set; } // Độ tin cậy của việc nhận diện (0-1)
    public string? SnapshotUrl { get; set; } // Ảnh chụp lúc check-in
    public CheckInStatus Status { get; set; } = CheckInStatus.Success;
    public string? Notes { get; set; }
}

public enum CheckInStatus
{
    Success,
    LowConfidence,
    Failed,
    ManualOverride
}
