namespace StationCheck.Models;

public class FaceRecognitionResult
{
    public bool Success { get; set; }
    public string? EmployeeId { get; set; }
    public double Confidence { get; set; } // 0-1
    public string? ErrorMessage { get; set; }
    public List<FaceDetection> DetectedFaces { get; set; } = new();
}

public class FaceDetection
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double Confidence { get; set; }
    public byte[]? FaceEmbedding { get; set; }
}
