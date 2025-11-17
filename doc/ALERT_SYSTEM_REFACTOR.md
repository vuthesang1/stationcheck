# Hệ Thống Cảnh Báo Dựa Trên Email - Tài Liệu Triển Khai

## Tổng Quan

Hệ thống mới sử dụng **email từ đầu thu** làm nguồn dữ liệu chính cho việc phát hiện chuyển động và sinh cảnh báo.

### Luồng Hoạt Động

```
┌─────────────────────────────────────────────────────────────────┐
│                      LUỒNG HỆ THỐNG MỚI                         │
└─────────────────────────────────────────────────────────────────┘

1. ĐẦU THU GỬI EMAIL (Alarm Notification)
   ↓
2. EmailMonitoringService (Chạy mỗi 5 phút)
   - Kết nối email server
   - Đọc email mới
   - Parse thông tin alarm
   ↓
3. EmailService.ParseEmailToMotionEventAsync()
   - Extract StationId từ subject
   - Parse alarm details → JSON payload
   - Tạo MotionEvent
   ↓
4. EmailService.SaveMotionEventAsync()
   - Lưu MotionEvent vào database
   - **Update Station.LastMotionDetectedAt = NOW()**  ✅ KEY POINT
   ↓
5. AlertGenerationService (Chạy mỗi 1 giờ)
   - Query tất cả Station có IsActive = 1
   - Với mỗi Station:
     a. Tìm TimeFrame khớp với giờ hiện tại
     b. Kiểm tra: NOW() - LastMotionDetectedAt > FrequencyMinutes?
     c. Nếu YES → Sinh cảnh báo mới
     d. Nếu có motion mới → Auto-resolve cảnh báo cũ
```

## Chi Tiết Các Thay Đổi

### 1. Database Schema

#### 1.1. Thêm `LastMotionDetectedAt` vào bảng Stations

```sql
ALTER TABLE Stations ADD LastMotionDetectedAt datetime2 NULL;
```

**Mục đích**: Track thời điểm phát hiện chuyển động gần nhất để so sánh với `FrequencyMinutes` của TimeFrame.

**Migration**: `20251112140043_AddLastMotionDetectedAtToStations`

#### 1.2. Sử dụng bảng MotionEvents (Đã có sẵn)

Email được parse và lưu vào `MotionEvents` thay vì `EmailEvents`:

```sql
CREATE TABLE MotionEvents (
    Id uniqueidentifier PRIMARY KEY,
    StationId int NULL,  -- Link to Station
    EventType nvarchar(50) NOT NULL,
    EmailSubject nvarchar(200) NULL,
    Payload nvarchar(2000) NULL,  -- JSON with alarm details
    DetectedAt datetime2 NOT NULL,
    CreatedAt datetime2 NOT NULL,
    IsProcessed bit NOT NULL,
    CONSTRAINT FK_MotionEvents_Stations_StationId 
        FOREIGN KEY (StationId) REFERENCES Stations (Id)
);
```

### 2. Services & Background Jobs

#### 2.1. EmailMonitoringService

**Thay đổi chính**:
- Interval: `1 phút` → `5 phút`
- Return type: `List<EmailEvent>` → `List<MotionEvent>`

```csharp
private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);
```

**Chức năng**:
- Kết nối email server (POP3/IMAP) - TODO: implement với credentials thực
- Fetch emails mới
- Gọi EmailService để parse và lưu

#### 2.2. EmailService (REFACTORED)

**Interface mới**:

```csharp
public interface IEmailService
{
    Task<List<MotionEvent>> CheckAndProcessNewEmailsAsync();
    Task<MotionEvent?> ParseEmailToMotionEventAsync(string subject, string body, string from, DateTime receivedAt);
    Task SaveMotionEventAsync(MotionEvent motionEvent);
}
```

**ParseEmailToMotionEventAsync**:
- Extract `StationId` từ subject:
  - Format 1: `ST000001` (StationCode) → Lookup Station by StationCode
  - Format 2: Direct numeric ID → Lookup Station by Id
- Parse alarm details từ email body bằng Regex
- Tạo JSON payload với tất cả thông tin

**Email Body Format**:
```
Alarm Event: Motion Detection
Alarm Input Channel No.: 2
Alarm Input Channel Name: IPC
Alarm Start Time (D/M/Y H:M:S): 12/11/2025 16:03:57
Alarm Device Name: NVR-6C39
Alarm Name:
IP Address: 192.168.1.200
Alarm Details:
```

**SaveMotionEventAsync** (QUAN TRỌNG):
```csharp
public async Task SaveMotionEventAsync(MotionEvent motionEvent)
{
    // 1. Save MotionEvent
    _context.MotionEvents.Add(motionEvent);
    
    // 2. ✅ Update Station.LastMotionDetectedAt
    if (motionEvent.StationId.HasValue)
    {
        var station = await _context.Stations.FindAsync(motionEvent.StationId.Value);
        if (station != null)
        {
            station.LastMotionDetectedAt = motionEvent.DetectedAt;
            _context.Stations.Update(station);
        }
    }
    
    await _context.SaveChangesAsync();
}
```

#### 2.3. AlertGenerationService (MỚI)

**Background service mới chạy mỗi 1 giờ**:

```csharp
private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);
```

**Luồng xử lý**:

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        await GenerateAlertsAsync(stoppingToken);
        await Task.Delay(_checkInterval, stoppingToken);
    }
}

private async Task GenerateAlertsAsync(CancellationToken cancellationToken)
{
    // 1. Lấy tất cả Station có IsActive = 1
    var activeStations = await _context.Stations
        .Where(s => s.IsActive)
        .ToListAsync(cancellationToken);
    
    foreach (var station in activeStations)
    {
        // 2. Tìm TimeFrame khớp với giờ hiện tại
        var matchingTimeFrames = await GetMatchingTimeFramesAsync(
            context, 
            station.Id, 
            now, 
            cancellationToken
        );
        
        if (!matchingTimeFrames.Any()) continue;
        
        var timeFrame = matchingTimeFrames.First();
        
        // 3. Kiểm tra điều kiện sinh cảnh báo
        if (ShouldGenerateAlert(station, timeFrame, now))
        {
            await CreateAlertAsync(context, station, timeFrame, now, cancellationToken);
        }
        
        // 4. Auto-resolve cảnh báo nếu có motion mới
        if (station.LastMotionDetectedAt.HasValue)
        {
            await AutoResolveAlertsAsync(context, station, now, cancellationToken);
        }
    }
}
```

**GetMatchingTimeFramesAsync**: Tìm TimeFrame phù hợp

```csharp
private async Task<List<TimeFrame>> GetMatchingTimeFramesAsync(
    ApplicationDbContext context,
    int stationId,
    DateTime now,
    CancellationToken cancellationToken)
{
    var currentTime = now.TimeOfDay;
    var currentDayOfWeek = ((int)now.DayOfWeek == 0 ? 7 : (int)now.DayOfWeek).ToString();

    var timeFrames = await context.TimeFrames
        .Where(tf => tf.StationId == stationId && tf.IsEnabled)
        .ToListAsync(cancellationToken);

    return timeFrames
        .Where(tf => 
            currentTime >= tf.StartTime && 
            currentTime <= tf.EndTime &&
            (string.IsNullOrEmpty(tf.DaysOfWeek) || tf.DaysOfWeek.Contains(currentDayOfWeek))
        )
        .ToList();
}
```

**ShouldGenerateAlert**: Kiểm tra điều kiện cảnh báo

```csharp
private bool ShouldGenerateAlert(Station station, TimeFrame timeFrame, DateTime now)
{
    // Case 1: Chưa có motion nào
    if (!station.LastMotionDetectedAt.HasValue)
    {
        return true;
    }
    
    // Case 2: Quá lâu không có motion
    var minutesSinceLastMotion = (now - station.LastMotionDetectedAt.Value).TotalMinutes;
    
    if (minutesSinceLastMotion > timeFrame.FrequencyMinutes)
    {
        return true;
    }
    
    return false;
}
```

**CreateAlertAsync**: Sinh cảnh báo

```csharp
private async Task CreateAlertAsync(
    ApplicationDbContext context,
    Station station,
    TimeFrame timeFrame,
    DateTime now,
    CancellationToken cancellationToken)
{
    // Check xem đã có alert chưa (tránh duplicate)
    var existingAlert = await context.MotionAlerts
        .Where(a => a.StationId == station.Id && !a.IsResolved)
        .FirstOrDefaultAsync(cancellationToken);

    if (existingAlert != null) return;

    // Tạo configuration snapshot (audit trail)
    var configSnapshot = new
    {
        TimeFrameId = timeFrame.Id,
        TimeFrameName = timeFrame.Name,
        StartTime = timeFrame.StartTime.ToString(),
        EndTime = timeFrame.EndTime.ToString(),
        FrequencyMinutes = timeFrame.FrequencyMinutes,
        DaysOfWeek = timeFrame.DaysOfWeek,
        CheckedAt = now
    };

    var minutesSinceLastMotion = station.LastMotionDetectedAt.HasValue
        ? (int)(now - station.LastMotionDetectedAt.Value).TotalMinutes
        : int.MaxValue;

    var alert = new MotionAlert
    {
        StationId = station.Id,
        StationName = station.Name,
        TimeFrameId = timeFrame.Id,
        ConfigurationSnapshot = JsonSerializer.Serialize(configSnapshot),
        AlertTime = now,
        Severity = minutesSinceLastMotion > timeFrame.FrequencyMinutes * 2 
            ? AlertSeverity.Critical 
            : AlertSeverity.Warning,
        Message = $"Không phát hiện chuyển động tại {station.Name} trong {minutesSinceLastMotion} phút (mong đợi: {timeFrame.FrequencyMinutes} phút)",
        ExpectedFrequencyMinutes = timeFrame.FrequencyMinutes,
        LastMotionAt = station.LastMotionDetectedAt,
        MinutesSinceLastMotion = minutesSinceLastMotion,
        IsResolved = false
    };

    context.MotionAlerts.Add(alert);
    await context.SaveChangesAsync(cancellationToken);
}
```

**AutoResolveAlertsAsync**: Tự động resolve cảnh báo

```csharp
private async Task<int> AutoResolveAlertsAsync(
    ApplicationDbContext context,
    Station station,
    DateTime now,
    CancellationToken cancellationToken)
{
    var activeAlerts = await context.MotionAlerts
        .Where(a => a.StationId == station.Id && !a.IsResolved)
        .ToListAsync(cancellationToken);

    if (!activeAlerts.Any()) return 0;

    int resolvedCount = 0;

    foreach (var alert in activeAlerts)
    {
        // Chỉ resolve nếu có motion sau khi alert được tạo
        if (station.LastMotionDetectedAt.HasValue && 
            alert.AlertTime < station.LastMotionDetectedAt.Value)
        {
            alert.IsResolved = true;
            alert.ResolvedAt = now;
            alert.ResolvedBy = "System (Auto-resolved by motion detection)";
            alert.Notes = $"Phát hiện chuyển động lúc {station.LastMotionDetectedAt.Value:dd/MM/yyyy HH:mm:ss}";
            
            resolvedCount++;
        }
    }

    if (resolvedCount > 0)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    return resolvedCount;
}
```

### 3. Program.cs Registration

```csharp
// Services
builder.Services.AddScoped<IEmailService, EmailService>();

// Background Services
builder.Services.AddHostedService<MotionMonitoringService>();  // Existing (every 1 min)
builder.Services.AddHostedService<EmailMonitoringService>();   // Check emails every 5 min
builder.Services.AddHostedService<AlertGenerationService>();   // Generate alerts every 1 hour
```

## Testing

### Test Script: `test_alert_system_flow.sql`

Script bao gồm 4 test cases:

1. **TEST 1**: Tạo MotionEvent từ email → Update LastMotionDetectedAt
2. **TEST 2**: Kiểm tra TimeFrame matching logic
3. **TEST 3**: Sinh cảnh báo khi quá lâu không có motion
4. **TEST 4**: Auto-resolve cảnh báo khi có motion mới

### Chạy Test

```bash
# 1. Run application
dotnet run

# 2. In another terminal, run SQL test script
sqlcmd -S localhost -d StationCheckDb -i test_alert_system_flow.sql
```

### Expected Behavior

**Email Monitoring Logs**:
```
[EmailMonitor] Service started
[EmailMonitor] Running check at 2025-11-12 21:00:00
[EmailService] Checking for new emails...
[EmailService] Parsed email to MotionEvent: StationId=3, StationCode=ST000003, AlarmEvent=Motion Detection
[EmailService] Updated Station 3 LastMotionDetectedAt to 2025-11-12 21:00:00
[EmailService] Saved MotionEvent ID=abc-123 for StationId=3
```

**Alert Generation Logs** (Chạy mỗi 1 giờ):
```
[AlertGeneration] Service started
[AlertGeneration] Running check at 2025-11-12 22:00:00
[AlertGeneration] Found 2 active stations
[AlertGeneration] Station 3 has no matching timeframes for current time
[AlertGeneration] Station 1 exceeds interval. Last motion: 2025-11-12 21:30:00, Minutes: 30, Expected: 10
[AlertGeneration] Created alert abc-456 for Station 1 (Trạm A)
[AlertGeneration] Check completed. Alerts generated: 1, Alerts resolved: 0
```

**Auto-Resolve Logs** (Khi có motion mới):
```
[AlertGeneration] Running check at 2025-11-12 23:00:00
[AlertGeneration] Found 2 active stations
[AlertGeneration] Auto-resolved alert abc-456 for Station 1. Motion detected at 2025-11-12 22:45:00
[AlertGeneration] Check completed. Alerts generated: 0, Alerts resolved: 1
```

## So Sánh Với Hệ Thống Cũ

| Khía Cạnh | Hệ Thống Cũ | Hệ Thống Mới |
|-----------|-------------|--------------|
| **Nguồn dữ liệu** | API webhook từ NVR | Email từ đầu thu |
| **Check motion** | Mỗi 1 phút (MotionMonitoringService) | Mỗi 5 phút (EmailMonitoringService) |
| **Lưu trữ** | EmailEvents (17 fields) | MotionEvents (existing table) |
| **Alert logic** | Based on MonitoringConfiguration | Based on TimeFrame + LastMotionDetectedAt |
| **Alert interval** | Real-time (same service) | Every 1 hour (separate service) |
| **Tracking** | Per-camera (CameraMotionStatus) | Per-station (Station.LastMotionDetectedAt) |
| **Config snapshot** | Không có | JSON trong MotionAlert.ConfigurationSnapshot |
| **Auto-resolve** | Thủ công | Tự động khi có motion mới |

## Điểm Mạnh Của Hệ Thống Mới

### 1. Decoupling Services
- **EmailMonitoringService**: Chỉ lo fetch và lưu MotionEvent
- **AlertGenerationService**: Chỉ lo sinh và resolve cảnh báo
- Dễ test, dễ maintain, dễ scale

### 2. Audit Trail
- `ConfigurationSnapshot` lưu lại cấu hình tại thời điểm sinh cảnh báo
- Có thể review lại lý do cảnh báo được sinh ra

### 3. Flexibility
- Có thể thay đổi interval của AlertGeneration mà không ảnh hưởng EmailMonitoring
- Có thể disable/enable từng service độc lập

### 4. Performance
- Chỉ check alert mỗi 1 giờ thay vì realtime → Giảm database queries
- Sử dụng `LastMotionDetectedAt` thay vì query MotionEvents table

### 5. Auto-Resolve
- Alert tự động resolve khi có motion mới
- Giảm công việc thủ công cho admin

## Workflow Example

### Scenario: Trạm A - Giờ làm việc 8:00-18:00, Check mỗi 10 phút

**Timeline**:

```
08:00 - AlertGenerationService check
        → No alert (trong giờ làm việc, chưa vượt quá 10 phút)

08:05 - Email từ đầu thu → MotionEvent created
        → Station.LastMotionDetectedAt = 08:05

08:10 - Email từ đầu thu → MotionEvent created
        → Station.LastMotionDetectedAt = 08:10

09:00 - AlertGenerationService check
        → LastMotionDetectedAt = 08:10
        → NOW - 08:10 = 50 minutes > 10 minutes
        → ✅ CREATE ALERT: "Không phát hiện chuyển động trong 50 phút"

09:15 - Email từ đầu thu → MotionEvent created
        → Station.LastMotionDetectedAt = 09:15

10:00 - AlertGenerationService check
        → LastMotionDetectedAt = 09:15
        → Alert created at 09:00 < LastMotionDetectedAt (09:15)
        → ✅ AUTO-RESOLVE ALERT: "Phát hiện chuyển động lúc 09:15"

18:00 - Hết giờ làm việc
        → AlertGenerationService check
        → No matching TimeFrame
        → Skip alert generation
```

## TODO: Integration

### 1. Email Server Connection

Hiện tại `EmailService.CheckAndProcessNewEmailsAsync()` đang return empty list (placeholder).

Cần implement:

```csharp
// Option 1: POP3
using MailKit.Net.Pop3;

public async Task<List<MotionEvent>> CheckAndProcessNewEmailsAsync()
{
    var processedEvents = new List<MotionEvent>();
    
    using var client = new Pop3Client();
    await client.ConnectAsync("pop.gmail.com", 995, true);
    await client.AuthenticateAsync("monitoring@company.com", "password");
    
    for (int i = 0; i < client.Count; i++)
    {
        var message = await client.GetMessageAsync(i);
        
        var motionEvent = await ParseEmailToMotionEventAsync(
            message.Subject,
            message.TextBody,
            message.From.ToString(),
            message.Date.DateTime
        );
        
        if (motionEvent != null)
        {
            await SaveMotionEventAsync(motionEvent);
            processedEvents.Add(motionEvent);
        }
    }
    
    await client.DisconnectAsync(true);
    return processedEvents;
}
```

### 2. Configuration

Add to `appsettings.json`:

```json
{
  "EmailSettings": {
    "Server": "imap.gmail.com",
    "Port": 993,
    "Username": "monitoring@company.com",
    "Password": "your-app-password",
    "UseSsl": true,
    "MarkAsRead": true,
    "DeleteAfterProcess": false
  },
  "BackgroundServices": {
    "EmailMonitoring": {
      "IntervalMinutes": 5,
      "Enabled": true
    },
    "AlertGeneration": {
      "IntervalMinutes": 60,
      "Enabled": true
    }
  }
}
```

### 3. Monitoring & Alerts

- Add logging to Seq/Application Insights
- Add Prometheus metrics:
  - `email_monitoring_check_count`
  - `motion_events_created_count`
  - `alerts_generated_count`
  - `alerts_resolved_count`
- Setup alert notifications (email, SMS, Slack) khi có cảnh báo mới

## Files Modified/Created

### Modified:
1. `Models/Station.cs` - Added `LastMotionDetectedAt`
2. `Interfaces/IEmailService.cs` - Changed interface to return MotionEvent
3. `Services/EmailService.cs` - Complete refactor to work with MotionEvent
4. `BackgroundServices/EmailMonitoringService.cs` - Changed interval to 5 minutes
5. `Program.cs` - Registered AlertGenerationService

### Created:
1. `BackgroundServices/AlertGenerationService.cs` - New background service (1 hour interval)
2. `Migrations/20251112140043_AddLastMotionDetectedAtToStations.cs` - Database migration
3. `test_alert_system_flow.sql` - Complete test script
4. `ALERT_SYSTEM_REFACTOR.md` - This document

## Summary

✅ **Completed**:
- EmailMonitoringService chạy mỗi 5 phút, lưu MotionEvent
- Station.LastMotionDetectedAt được update tự động
- AlertGenerationService chạy mỗi 1 giờ, check TimeFrame
- Sinh cảnh báo khi `NOW - LastMotionDetectedAt > FrequencyMinutes`
- Auto-resolve cảnh báo khi có motion mới
- Migration applied successfully
- Test script hoàn chỉnh

⚠️ **TODO**:
- Implement actual email server connection (POP3/IMAP)
- Add configuration for email credentials
- Add monitoring & alerting
- Test with real email data from NVR devices
