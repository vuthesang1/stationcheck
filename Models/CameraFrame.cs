namespace StationCheck.Models;

public class CameraFrame
{
    public string CameraId { get; set; } = string.Empty;
    public string CameraName { get; set; } = string.Empty;
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int Width { get; set; }
    public int Height { get; set; }
}

public class CameraInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public int Port { get; set; } = 554;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string StreamUrl { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public CameraType Type { get; set; } = CameraType.Dahua;
}

public enum CameraType
{
    Dahua,
    Hikvision,
    Generic
}
