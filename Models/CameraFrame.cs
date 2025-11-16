namespace StationCheck.Models
{
    /// <summary>
    /// Frame từ camera (một ảnh tĩnh)
    /// </summary>
    public class CameraFrame
    {
        public string CameraId { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Format { get; set; } = "JPEG"; // JPEG, PNG, etc.
    }
}
