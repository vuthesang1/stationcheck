using StationCheck.Models;

namespace StationCheck.Interfaces;

/// <summary>
/// Interface cho Face Recognition service - nhận diện khuôn mặt từ ảnh
/// </summary>
public interface IFaceRecognitionService
{
    /// <summary>
    /// Phát hiện và nhận diện khuôn mặt từ frame camera
    /// </summary>
    Task<FaceRecognitionResult> RecognizeFaceAsync(CameraFrame frame);
    
    /// <summary>
    /// Phát hiện và nhận diện khuôn mặt từ byte array
    /// </summary>
    Task<FaceRecognitionResult> RecognizeFaceAsync(byte[] imageData);
    
    /// <summary>
    /// Đăng ký khuôn mặt mới cho nhân viên
    /// </summary>
    Task<bool> RegisterFaceAsync(string employeeId, byte[] imageData);
    
    /// <summary>
    /// Xóa dữ liệu khuôn mặt của nhân viên
    /// </summary>
    Task<bool> RemoveFaceAsync(string employeeId);
    
    /// <summary>
    /// Cập nhật ngưỡng confidence tối thiểu để nhận diện
    /// </summary>
    void SetConfidenceThreshold(double threshold);
}
