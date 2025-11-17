using StationCheck.Interfaces;
using StationCheck.Models;

namespace StationCheck.Services;

/// <summary>
/// Mock implementation của NVR Service
/// Interface sẵn sàng để tích hợp với Dahua NVR khi có device thật
/// </summary>
public class MockNvrService : INvrService
{
    private readonly Dictionary<string, CameraInfo> _connectedCameras = new();
    private readonly Dictionary<string, CancellationTokenSource> _streamingTasks = new();
    private readonly Random _random = new();

    public Task<bool> ConnectAsync(CameraInfo cameraInfo)
    {
        // TODO: Implement real connection to Dahua NVR using HTTP API
        // Reference: DAHUA_IPC_HTTP_API document
        // Example: GET http://{ip}:{port}/cgi-bin/magicBox.cgi?action=getDeviceType
        
        // Mock: Giả lập kết nối thành công
        cameraInfo.IsConnected = true;
        _connectedCameras[cameraInfo.Id] = cameraInfo;
        
        Console.WriteLine($"[Mock] Connected to camera: {cameraInfo.Name} ({cameraInfo.IpAddress})");
        return Task.FromResult(true);
    }

    public Task DisconnectAsync(string cameraId)
    {
        if (_streamingTasks.ContainsKey(cameraId))
        {
            _streamingTasks[cameraId].Cancel();
            _streamingTasks.Remove(cameraId);
        }

        if (_connectedCameras.ContainsKey(cameraId))
        {
            _connectedCameras[cameraId].IsConnected = false;
            _connectedCameras.Remove(cameraId);
            Console.WriteLine($"[Mock] Disconnected camera: {cameraId}");
        }

        return Task.CompletedTask;
    }

    public Task<List<CameraInfo>> GetConnectedCamerasAsync()
    {
        return Task.FromResult(_connectedCameras.Values.ToList());
    }

    public Task<CameraFrame?> GetCurrentFrameAsync(string cameraId)
    {
        // TODO: Implement real frame capture from Dahua camera
        // Example: GET http://{ip}:{port}/cgi-bin/snapshot.cgi
        
        if (!_connectedCameras.ContainsKey(cameraId))
        {
            return Task.FromResult<CameraFrame?>(null);
        }

        // Mock: Tạo frame giả
        var frame = new CameraFrame
        {
            CameraId = cameraId,
            CameraName = _connectedCameras[cameraId].Name,
            ImageData = GenerateMockImageData(),
            Timestamp = DateTime.Now,
            Width = 1920,
            Height = 1080
        };

        return Task.FromResult<CameraFrame?>(frame);
    }

    public async Task<bool> StartStreamAsync(string cameraId, Action<CameraFrame> onFrameReceived)
    {
        // TODO: Implement real RTSP stream handling
        // Example: rtsp://{username}:{password}@{ip}:{port}/cam/realmonitor?channel=1&subtype=0
        
        if (!_connectedCameras.ContainsKey(cameraId))
        {
            return false;
        }

        // Stop existing stream if any
        if (_streamingTasks.ContainsKey(cameraId))
        {
            await StopStreamAsync(cameraId);
        }

        var cts = new CancellationTokenSource();
        _streamingTasks[cameraId] = cts;

        // Mock: Giả lập stream với 10 FPS
        _ = Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var frame = await GetCurrentFrameAsync(cameraId);
                    if (frame != null)
                    {
                        onFrameReceived(frame);
                    }
                    await Task.Delay(100, cts.Token); // ~10 FPS
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }, cts.Token);

        Console.WriteLine($"[Mock] Started streaming from camera: {cameraId}");
        return true;
    }

    public Task StopStreamAsync(string cameraId)
    {
        if (_streamingTasks.ContainsKey(cameraId))
        {
            _streamingTasks[cameraId].Cancel();
            _streamingTasks.Remove(cameraId);
            Console.WriteLine($"[Mock] Stopped streaming from camera: {cameraId}");
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsConnectedAsync(string cameraId)
    {
        var isConnected = _connectedCameras.ContainsKey(cameraId) && 
                         _connectedCameras[cameraId].IsConnected;
        return Task.FromResult(isConnected);
    }

    private byte[] GenerateMockImageData()
    {
        // Mock: Tạo dữ liệu ảnh giả (1KB)
        var data = new byte[1024];
        _random.NextBytes(data);
        return data;
    }
}
