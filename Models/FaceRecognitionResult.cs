namespace StationCheck.Models
{
    /// <summary>
    /// Kết quả nhận diện khuôn mặt
    /// </summary>
    public class FaceRecognitionResult
    {
        public bool FaceDetected { get; set; }
        public bool FaceRecognized { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public float Confidence { get; set; } // 0.0 to 1.0
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? ErrorMessage { get; set; }
        
        // Thông tin vị trí khuôn mặt trong ảnh
        public FaceLocation? Location { get; set; }
    }
    
    public class FaceLocation
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
