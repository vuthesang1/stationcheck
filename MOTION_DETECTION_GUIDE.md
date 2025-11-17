# StationCheck - Motion Detection Monitoring System

## Overview
Hệ thống giám sát phát hiện chuyển động (Motion Detection Monitoring) tích hợp với camera Dahua NVR, tự động tạo cảnh báo khi không phát hiện chuyển động trong khoảng thời gian quy định.

## Architecture

### Database Tables
- **MotionEvents**: Lưu trữ các sự kiện phát hiện chuyển động từ camera
- **MotionAlerts**: Lưu trữ các cảnh báo khi không có chuyển động
- **AlertRules**: Cấu hình quy tắc cảnh báo theo khung giờ
- **Cameras**: Thông tin camera (từ bảng cũ)

### Services
- **MotionDetectionService**: Xử lý logic phát hiện chuyển động và tạo cảnh báo
- **MotionMonitoringService**: Background service chạy mỗi 60 giây để kiểm tra và tạo cảnh báo
- **MotionController**: API endpoints để nhận webhook từ Dahua NVR và quản lý alerts

### UI Components
- **MotionAlertsPanel**: Hiển thị danh sách cảnh báo chưa giải quyết với tính năng auto-refresh mỗi 30 giây
- **MotionStatsPanel**: Hiển thị thống kê số lượng motion events hôm nay theo từng camera

## Alert Rules Configuration

### Seed Data (Camera CAM001 - Main Entrance)
```
RULE001: 08:00 - 17:00 (Giờ hành chính) → Kiểm tra mỗi 60 phút
RULE002: 17:00 - 22:00 (Chiều tối) → Kiểm tra mỗi 120 phút  
RULE003: 22:00 - 08:00 (Ban đêm) → Kiểm tra mỗi 180 phút
```

### Alert Severity Levels
- **Critical**: Quá 3x khoảng thời gian quy định
- **Error**: Quá 2x khoảng thời gian quy định
- **Warning**: Quá khoảng thời gian quy định
- **Info**: Thông tin chung

## API Endpoints

### 1. Motion Event Webhook (Dahua NVR)
```http
POST /api/motion/event
Content-Type: application/json

{
  "cameraId": "CAM001",
  "cameraName": "Main Entrance",
  "eventType": "VideoMotion",
  "detectedAt": "2025-10-28T19:30:00",
  "payload": "{\"Channel\":1,\"Action\":\"Start\",\"RegionName\":\"Region1\"}"
}
```

**Response**: 
```json
{
  "success": true,
  "message": "Motion event processed successfully",
  "eventId": "ME-20251028193000-ABC123"
}
```

### 2. Get Unresolved Alerts
```http
GET /api/motion/alerts
```

**Response**:
```json
[
  {
    "id": "ALERT-20251028193000-XYZ",
    "cameraId": "CAM001",
    "cameraName": "Main Entrance",
    "alertTime": "2025-10-28T19:30:00",
    "severity": 2,
    "message": "No motion detected for 120 minutes (expected 60 minutes)",
    "expectedIntervalMinutes": 60,
    "lastMotionAt": "2025-10-28T17:30:00",
    "minutesSinceLastMotion": 120
  }
]
```

### 3. Resolve Alert
```http
POST /api/motion/alerts/{alertId}/resolve
Content-Type: application/json

{
  "resolvedBy": "Admin User",
  "notes": "False alarm - camera was offline for maintenance"
}
```

### 4. Get Motion Events
```http
GET /api/motion/events?cameraId=CAM001&from=2025-10-28T00:00:00&top=50
```

### 5. Get Today's Statistics
```http
GET /api/motion/stats/today
```

**Response**:
```json
{
  "Main Entrance": 145,
  "Office Area": 89,
  "Parking Lot": 203
}
```

## Background Monitoring

### MotionMonitoringService
- Chạy mỗi 60 giây
- Kiểm tra tất cả cameras
- Tự động tạo alert khi:
  - Không có motion event nào được ghi nhận
  - Hoặc thời gian từ motion event cuối cùng > interval quy định
- Tự động resolve alert khi phát hiện motion mới

### Workflow
```
1. Timer (60s) → MotionMonitoringService.ExecuteAsync()
2. Lấy danh sách cameras từ database
3. Với mỗi camera:
   a. CheckAndCreateAlertsAsync()
   b. Lấy AlertRule hiện tại (dựa trên thời gian hiện tại)
   c. Lấy MotionEvent cuối cùng
   d. Tính thời gian từ motion event cuối → hiện tại
   e. So sánh với interval trong AlertRule
   f. Nếu vượt quá → Tạo MotionAlert mới
   g. Nếu có motion mới → Auto-resolve alerts cũ
```

## Setup Instructions

### 1. Database Setup
Database sẽ tự động migrate khi chạy ứng dụng:
```bash
dotnet run --project StationCheck.csproj
```

Migration `AddMotionDetection` sẽ tạo:
- 3 tables: MotionEvents, MotionAlerts, AlertRules
- Indexes để tối ưu performance
- Seed data: 3 alert rules cho CAM001

### 2. Configure Dahua NVR

#### Bước 1: Đăng nhập vào Dahua NVR Web Interface
```
URL: http://<NVR_IP_ADDRESS>
Username/Password: (admin credentials)
```

#### Bước 2: Setup Event Subscription
```
Menu: Setup → Event → Subscription
- Enable: HTTP Subscription
- URL: http://<YOUR_SERVER_IP>:55704/api/motion/event
- Method: POST
- Content-Type: application/json
```

#### Bước 3: Configure Motion Detection
```
Menu: Setup → Event → Video Detection → Motion Detection
- Channel: Select camera channel
- Enable: Motion Detection
- Sensitivity: Medium/High
- Region: Draw detection area
- Schedule: 24/7 or custom schedule
```

#### Bước 4: Setup HTTP Action
```
Menu: Setup → Event → Action
Event Type: Motion Detection
Action: HTTP Notification
URL Template: Use default or custom JSON payload
```

### 3. Test the System

#### Test với PowerShell Script
```powershell
# Chạy server (Terminal 1)
cd d:\stationcheck
dotnet run --project StationCheck.csproj

# Chạy test script (Terminal 2)
cd d:\stationcheck
.\test-motion-api.ps1
```

#### Test với curl
```bash
# Send motion event
curl -X POST http://localhost:55704/api/motion/event \
  -H "Content-Type: application/json" \
  -d '{
    "cameraId": "CAM001",
    "cameraName": "Main Entrance",
    "eventType": "VideoMotion",
    "detectedAt": "2025-10-28T19:30:00"
  }'

# Get alerts
curl http://localhost:55704/api/motion/alerts

# Get stats
curl http://localhost:55704/api/motion/stats/today
```

### 4. Access Web Dashboard
```
https://localhost:55703
```

Dashboard hiển thị:
- **MotionAlertsPanel**: Danh sách alerts chưa giải quyết (auto-refresh 30s)
- **MotionStatsPanel**: Thống kê motion events hôm nay
- **CameraView**: Live camera feed (existing)
- **EmployeeList**: Danh sách nhân viên (existing)
- **CheckInHistory**: Lịch sử check-in (existing)

## Production Deployment

### 1. Update Connection String
Cập nhật `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PRODUCTION_SERVER;Database=StationCheckDb;User Id=sa;Password=xxx;TrustServerCertificate=True;"
  }
}
```

### 2. Add Authentication
Thêm authentication cho API endpoints:
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });

// MotionController.cs
[Authorize]
public class MotionController : ControllerBase { ... }
```

### 3. Configure Logging
Cập nhật `appsettings.Production.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "StationCheck.BackgroundServices": "Information",
      "StationCheck.Controllers": "Information"
    },
    "EventLog": {
      "LogLevel": {
        "Default": "Warning"
      }
    }
  }
}
```

### 4. Enable HTTPS for Webhook
Cấu hình Dahua NVR gửi webhook qua HTTPS:
```
URL: https://<YOUR_DOMAIN>/api/motion/event
```

### 5. Setup Monitoring
- Application Insights hoặc Serilog cho centralized logging
- Health check endpoints
- Performance monitoring

## Troubleshooting

### Background Service không chạy
Kiểm tra logs:
```
info: StationCheck.BackgroundServices.MotionMonitoringService[0]
      [MotionMonitor] Service started
```

Nếu không thấy → Kiểm tra Program.cs:
```csharp
builder.Services.AddHostedService<MotionMonitoringService>();
```

### API trả về 404
Kiểm tra Controllers đã được map:
```csharp
app.MapControllers(); // Phải có dòng này trong Program.cs
```

### DbContext concurrency errors
Đảm bảo:
- Tất cả services sử dụng DbContext là `Scoped`
- Background service sử dụng `IServiceScopeFactory` để tạo scope mới
- Render mode là `Server` (không phải `ServerPrerendered`)

### Alerts không tự động resolve
Kiểm tra logic trong `MotionDetectionService.ProcessMotionEventAsync()`:
```csharp
// Phải có đoạn này
var unresolvedAlerts = await _context.MotionAlerts
    .Where(a => a.CameraId == evt.CameraId && !a.IsResolved)
    .ToListAsync();

foreach (var alert in unresolvedAlerts)
{
    alert.IsResolved = true;
    alert.ResolvedAt = DateTime.Now;
    alert.Notes = "Auto-resolved: Motion detected";
}
```

## Next Steps

1. **Implement Real Face Recognition**: Thay thế `MockFaceRecognitionService` bằng AI model thực (OpenCV, Face Recognition API)
2. **Add Notification System**: Email/SMS khi có alert mới
3. **Implement Report Generation**: Báo cáo hàng ngày/tuần/tháng về motion events và alerts
4. **Add Camera Health Check**: Kiểm tra camera online/offline status
5. **Implement Alert Escalation**: Tự động escalate alerts chưa được giải quyết sau X phút
6. **Add Multi-tenancy**: Hỗ trợ nhiều công ty/chi nhánh

## License
Internal use only - Company Confidential
