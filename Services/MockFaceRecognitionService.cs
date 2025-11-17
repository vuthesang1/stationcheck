using StationCheck.Interfaces;
using StationCheck.Models;

namespace StationCheck.Services;

/// <summary>
/// Mock implementation của Face Recognition Service
/// Interface sẵn sàng để tích hợp với AI model thật (OpenCV, Azure Face API, AWS Rekognition, etc.)
/// </summary>
public class MockFaceRecognitionService : IFaceRecognitionService
{
    private readonly Random _random = new();
    private double _confidenceThreshold = 0.75; // Ngưỡng tin cậy tối thiểu

    // Mock: Dictionary lưu face embeddings của employees
    private readonly Dictionary<string, byte[]> _faceDatabase = new();
    
    // Mock: Hardcoded employee IDs for testing
    private readonly List<string> _mockEmployeeIds = new() { "EMP001", "EMP002", "EMP003" };

    public MockFaceRecognitionService()
    {
        InitializeMockFaceData();
    }

    public async Task<FaceRecognitionResult> RecognizeFaceAsync(CameraFrame frame)
    {
        return await RecognizeFaceAsync(frame.ImageData);
    }

    public async Task<FaceRecognitionResult> RecognizeFaceAsync(byte[] imageData)
    {
        // TODO: Implement real face recognition
        // Options:
        // 1. Azure Face API: https://azure.microsoft.com/services/cognitive-services/face/
        // 2. AWS Rekognition: https://aws.amazon.com/rekognition/
        // 3. OpenCV + dlib: Local face recognition
        // 4. Face-api.js: Browser-based recognition
        
        await Task.Delay(100); // Giả lập thời gian xử lý

        // Mock: Random nhận diện
        var shouldDetect = _random.NextDouble() > 0.3; // 70% tỷ lệ phát hiện khuôn mặt

        if (!shouldDetect)
        {
            return new FaceRecognitionResult
            {
                Success = false,
                ErrorMessage = "No face detected",
                DetectedFaces = new List<FaceDetection>()
            };
        }

        // Mock: Tạo face detection
        var faceDetection = new FaceDetection
        {
            X = _random.Next(100, 500),
            Y = _random.Next(100, 400),
            Width = _random.Next(200, 400),
            Height = _random.Next(200, 400),
            Confidence = _random.NextDouble() * 0.3 + 0.7, // 0.7-1.0
            FaceEmbedding = new byte[128] // Mock embedding vector
        };

        // Mock: Random chọn employee từ hardcoded list
        if (_mockEmployeeIds.Count == 0)
        {
            return new FaceRecognitionResult
            {
                Success = false,
                ErrorMessage = "No employees in database",
                DetectedFaces = new List<FaceDetection> { faceDetection }
            };
        }

        var recognizedEmployeeId = _mockEmployeeIds[_random.Next(_mockEmployeeIds.Count)];
        var confidence = _random.NextDouble() * 0.25 + 0.7; // 0.7-0.95

        return new FaceRecognitionResult
        {
            Success = confidence >= _confidenceThreshold,
            EmployeeId = recognizedEmployeeId,
            Confidence = confidence,
            DetectedFaces = new List<FaceDetection> { faceDetection },
            ErrorMessage = confidence < _confidenceThreshold ? "Confidence too low" : null
        };
    }

    public Task<bool> RegisterFaceAsync(string employeeId, byte[] imageData)
    {
        // TODO: Implement real face registration
        // 1. Detect face in image
        // 2. Extract face embedding/features
        // 3. Store in database
        
        // Mock: Tạo và lưu face embedding giả
        var faceEmbedding = new byte[128];
        _random.NextBytes(faceEmbedding);
        _faceDatabase[employeeId] = faceEmbedding;

        Console.WriteLine($"[Mock] Registered face for employee: {employeeId}");
        return Task.FromResult(true);
    }

    public Task<bool> RemoveFaceAsync(string employeeId)
    {
        if (_faceDatabase.ContainsKey(employeeId))
        {
            _faceDatabase.Remove(employeeId);
            Console.WriteLine($"[Mock] Removed face data for employee: {employeeId}");
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public void SetConfidenceThreshold(double threshold)
    {
        if (threshold >= 0 && threshold <= 1)
        {
            _confidenceThreshold = threshold;
            Console.WriteLine($"[Mock] Updated confidence threshold to: {threshold:P0}");
        }
    }

    private void InitializeMockFaceData()
    {
        // Mock: Tạo face embeddings giả cho các employee mẫu
        var employeeIds = new[] { "EMP001", "EMP002", "EMP003" };
        foreach (var empId in employeeIds)
        {
            var embedding = new byte[128];
            _random.NextBytes(embedding);
            _faceDatabase[empId] = embedding;
        }
    }
}
