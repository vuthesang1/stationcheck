using StationCheck.Models;

namespace StationCheck.Interfaces;

/// <summary>
/// Interface cho NVR/Camera service - kết nối và lấy video stream từ camera
/// </summary>
public interface INvrService
{
    /// <summary>
    /// Kết nối tới NVR/Camera
    /// </summary>
    Task<bool> ConnectAsync(CameraInfo cameraInfo);
    
    /// <summary>
    /// Ngắt kết nối
    /// </summary>
    Task DisconnectAsync(string cameraId);
    
    /// <summary>
    /// Lấy danh sách camera đã kết nối
    /// </summary>
    Task<List<CameraInfo>> GetConnectedCamerasAsync();
    
    /// <summary>
    /// Lấy frame hiện tại từ camera
    /// </summary>
    Task<CameraFrame?> GetCurrentFrameAsync(string cameraId);
    
    /// <summary>
    /// Bắt đầu stream video từ camera
    /// </summary>
    Task<bool> StartStreamAsync(string cameraId, Action<CameraFrame> onFrameReceived);
    
    /// <summary>
    /// Dừng stream video
    /// </summary>
    Task StopStreamAsync(string cameraId);
    
    /// <summary>
    /// Kiểm tra trạng thái kết nối
    /// </summary>
    Task<bool> IsConnectedAsync(string cameraId);
}
