namespace StationCheck.Models
{
    /// <summary>
    /// Thông tin về camera/NVR
    /// </summary>
    public class CameraInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; } = 554; // RTSP default port
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RtspUrl { get; set; } = string.Empty;
        public bool IsConnected { get; set; }
        public DateTime? LastConnectedAt { get; set; }
        public Guid? StationId { get; set; }
    }
}
