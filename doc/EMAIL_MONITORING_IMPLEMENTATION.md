# Email Event Monitoring System - Implementation Summary

## Tổng Quan
Hệ thống tự động check email từ đầu thu gửi về, parse thông tin alarm và lưu vào database.

## Các Thay Đổi Đã Thực Hiện

### 1. Database Schema

#### 1.1. Thêm StationCode vào bảng Stations
- **Field**: `StationCode` (nvarchar(10), required)
- **Format**: `ST` + 6 chữ số (VD: ST000001, ST000002)
- **Purpose**: Dùng để nhận diện station từ email subject

```sql
ALTER TABLE Stations ADD StationCode nvarchar(10) NOT NULL DEFAULT N'';

-- Generate station codes for existing records
UPDATE Stations 
SET StationCode = 'ST' + RIGHT('000000' + CAST(Id AS VARCHAR(6)), 6)
WHERE StationCode = '' OR StationCode IS NULL;
```

#### 1.2. Tạo bảng EmailEvents
Bảng lưu trữ thông tin từ email của đầu thu:

```sql
CREATE TABLE EmailEvents (
    Id int NOT NULL IDENTITY PRIMARY KEY,
    StationCode nvarchar(10) NOT NULL,
    StationId int NULL,
    AlarmEvent nvarchar(200) NULL,
    AlarmInputChannelNo int NULL,
    AlarmInputChannelName nvarchar(200) NULL,
    AlarmStartTime datetime2 NULL,
    AlarmDeviceName nvarchar(200) NULL,
    AlarmName nvarchar(200) NULL,
    IpAddress nvarchar(50) NULL,
    AlarmDetails nvarchar(2000) NULL,
    EmailSubject nvarchar(200) NULL,
    EmailFrom nvarchar(500) NULL,
    EmailReceivedAt datetime2 NOT NULL,
    RawEmailBody nvarchar(4000) NULL,
    CreatedAt datetime2 NOT NULL,
    IsProcessed bit NOT NULL,
    CONSTRAINT FK_EmailEvents_Stations_StationId FOREIGN KEY (StationId) 
        REFERENCES Stations (Id) ON DELETE SET NULL
);

-- Indexes for performance
CREATE INDEX IX_EmailEvents_StationCode ON EmailEvents (StationCode);
CREATE INDEX IX_EmailEvents_StationId ON EmailEvents (StationId);
CREATE INDEX IX_EmailEvents_EmailReceivedAt ON EmailEvents (EmailReceivedAt);
CREATE INDEX IX_EmailEvents_IsProcessed ON EmailEvents (IsProcessed);
```

### 2. Models

#### 2.1. Station.cs
```csharp
public class Station
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string StationCode { get; set; } = string.Empty; // ST000001

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    // ... các fields khác
}
```

#### 2.2. EmailEvent.cs
```csharp
public class EmailEvent
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string StationCode { get; set; } = string.Empty; // From email subject

    public int? StationId { get; set; }
    public Station? Station { get; set; }

    // Alarm information from email body
    [MaxLength(200)]
    public string? AlarmEvent { get; set; } // Motion Detection

    public int? AlarmInputChannelNo { get; set; }
    [MaxLength(200)]
    public string? AlarmInputChannelName { get; set; } // IPC

    public DateTime? AlarmStartTime { get; set; }
    [MaxLength(200)]
    public string? AlarmDeviceName { get; set; } // NVR-6C39

    [MaxLength(200)]
    public string? AlarmName { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; } // 192.168.1.200

    [MaxLength(2000)]
    public string? AlarmDetails { get; set; }

    // Email metadata
    [MaxLength(200)]
    public string? EmailSubject { get; set; }
    [MaxLength(500)]
    public string? EmailFrom { get; set; }
    public DateTime EmailReceivedAt { get; set; }
    [MaxLength(4000)]
    public string? RawEmailBody { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsProcessed { get; set; } = false;
}
```

### 3. Services

#### 3.1. IEmailService.cs
```csharp
public interface IEmailService
{
    Task<List<EmailEvent>> CheckAndProcessNewEmailsAsync();
    Task<EmailEvent> ParseEmailToEventAsync(string subject, string body, string from, DateTime receivedAt);
    Task SaveEmailEventAsync(EmailEvent emailEvent);
}
```

#### 3.2. EmailService.cs
Service để:
- Check email mới từ mail server (TODO: implement POP3/IMAP)
- Parse email body theo format của đầu thu
- Extract StationCode từ email subject
- Lưu EmailEvent vào database

**Email Body Format được parse:**
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

**Parsing Logic:**
- Sử dụng Regex để extract từng field
- Extract StationCode từ subject (format: `ST\d{6}`)
- Parse datetime theo format `dd/MM/yyyy HH:mm:ss`
- Link với Station record dựa vào StationCode

### 4. Background Service

#### 4.1. EmailMonitoringService.cs
- **Interval**: Chạy mỗi 1 phút
- **Function**: 
  1. Gọi `EmailService.CheckAndProcessNewEmailsAsync()`
  2. Parse các email mới
  3. Lưu vào database
  4. Log kết quả

```csharp
private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    _logger.LogInformation("[EmailMonitor] Service started");
    
    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Initial delay
    
    while (!stoppingToken.IsCancellationRequested)
    {
        await CheckEmailsAsync(stoppingToken);
        await Task.Delay(_checkInterval, stoppingToken);
    }
}
```

### 5. Registration trong Program.cs

```csharp
// Register EmailService
builder.Services.AddScoped<IEmailService, EmailService>();

// Register Background Services
builder.Services.AddHostedService<MotionMonitoringService>();
builder.Services.AddHostedService<EmailMonitoringService>();  // NEW
```

## Cách Hoạt Động

### Flow Tổng Quát:
```
1. Đầu thu gửi email alarm 
   → Subject: "ST000001 - Alarm Notification"
   → Body: Alarm Event details

2. EmailMonitoringService (Background) 
   → Chạy mỗi 1 phút
   → Gọi EmailService.CheckAndProcessNewEmailsAsync()

3. EmailService
   → Fetch emails từ mail server (TODO: implement)
   → Parse email subject → Extract StationCode (ST000001)
   → Parse email body → Extract alarm details
   → Lookup Station by StationCode
   → Create EmailEvent object

4. Save to Database
   → EmailEvents table
   → Link với Station record nếu tìm thấy

5. Process EmailEvent
   → Có thể trigger alert
   → Có thể update MotionEvent
   → Mark IsProcessed = true
```

## Testing

### Test Email Parsing:
```csharp
// Sample email
var subject = "ST000001 - Alarm Notification";
var body = @"
Alarm Event: Motion Detection
Alarm Input Channel No.: 2
Alarm Input Channel Name: IPC
Alarm Start Time (D/M/Y H:M:S): 12/11/2025 16:03:57
Alarm Device Name: NVR-6C39
Alarm Name:
IP Address: 192.168.1.200
Alarm Details:
";

var emailEvent = await emailService.ParseEmailToEventAsync(
    subject, body, "nvr@monitoring.local", DateTime.Now
);

// Result:
// StationCode: ST000001
// AlarmEvent: Motion Detection
// AlarmInputChannelNo: 2
// AlarmDeviceName: NVR-6C39
// IpAddress: 192.168.1.200
// AlarmStartTime: 2025-11-12 16:03:57
```

### Test SQL Script:
Chạy `test_email_service.sql` để:
1. Update StationCode cho stations hiện tại
2. Insert test EmailEvent record
3. Verify data

```sql
-- Update station codes
UPDATE Stations 
SET StationCode = 'ST' + RIGHT('000000' + CAST(Id AS VARCHAR(6)), 6);

-- Insert test email event
INSERT INTO EmailEvents (...) VALUES (...);

-- Query results
SELECT * FROM EmailEvents ORDER BY EmailReceivedAt DESC;
```

## TODO: Email Fetching Implementation

Để hoàn thiện hệ thống, cần implement email fetching:

### Option 1: POP3 (Simple)
```csharp
// Cần NuGet package: MailKit
using MailKit.Net.Pop3;
using MimeKit;

public async Task<List<EmailMessage>> FetchEmailsAsync()
{
    using var client = new Pop3Client();
    await client.ConnectAsync("pop.gmail.com", 995, true);
    await client.AuthenticateAsync("username", "password");
    
    var messages = new List<EmailMessage>();
    for (int i = 0; i < client.Count; i++)
    {
        var message = await client.GetMessageAsync(i);
        messages.Add(new EmailMessage
        {
            Subject = message.Subject,
            Body = message.TextBody,
            From = message.From.ToString(),
            ReceivedDate = message.Date.DateTime
        });
    }
    
    await client.DisconnectAsync(true);
    return messages;
}
```

### Option 2: IMAP (Advanced)
```csharp
// Cần NuGet package: MailKit
using MailKit.Net.Imap;

public async Task<List<EmailMessage>> FetchEmailsAsync()
{
    using var client = new ImapClient();
    await client.ConnectAsync("imap.gmail.com", 993, true);
    await client.AuthenticateAsync("username", "password");
    
    var inbox = client.Inbox;
    await inbox.OpenAsync(FolderAccess.ReadOnly);
    
    var messages = new List<EmailMessage>();
    var uids = await inbox.SearchAsync(SearchQuery.NotSeen);
    
    foreach (var uid in uids)
    {
        var message = await inbox.GetMessageAsync(uid);
        messages.Add(new EmailMessage
        {
            Subject = message.Subject,
            Body = message.TextBody,
            From = message.From.ToString(),
            ReceivedDate = message.Date.DateTime
        });
    }
    
    await client.DisconnectAsync(true);
    return messages;
}
```

### Configuration trong appsettings.json:
```json
{
  "EmailSettings": {
    "Server": "imap.gmail.com",
    "Port": 993,
    "Username": "monitoring@company.com",
    "Password": "your-app-password",
    "UseSsl": true,
    "CheckIntervalMinutes": 1
  }
}
```

## Migration Applied

**Migration Name**: `20251112133624_AddStationCodeAndEmailEvents`

**Changes**:
1. Added `StationCode` column to Stations table
2. Created EmailEvents table with all fields and indexes
3. Updated seed data timestamps

**Apply Migration**:
```bash
dotnet ef database update --context ApplicationDbContext
```

## Monitoring & Logs

Background service logs:
```
[EmailMonitor] Service started
[EmailMonitor] Running check at 2025-11-12 20:40:59
[EmailService] Checking for new emails...
[EmailService] No new emails found.
```

Khi có email mới:
```
[EmailService] Checking for new emails...
[EmailService] Parsed email: StationCode=ST000001, AlarmEvent=Motion Detection, Device=NVR-6C39
[EmailService] Saved EmailEvent ID=1 for Station=ST000001
[EmailMonitor] Processed 1 new emails
[EmailMonitor] Email: Station=ST000001, Event=Motion Detection, Device=NVR-6C39, Time=12/11/2025 16:03:57
```

## Files Created/Modified

### Created:
- `Models/EmailEvent.cs` - Email event model
- `Interfaces/IEmailService.cs` - Email service interface  
- `Services/EmailService.cs` - Email parsing & storage service
- `BackgroundServices/EmailMonitoringService.cs` - Background job (1 min interval)
- `Migrations/20251112133624_AddStationCodeAndEmailEvents.cs` - Database migration
- `update_station_codes.sql` - Script to update existing stations
- `test_email_service.sql` - Test script

### Modified:
- `Models/Station.cs` - Added StationCode field
- `Data/ApplicationDbContext.cs` - Added EmailEvents DbSet & configuration
- `Program.cs` - Registered EmailService & EmailMonitoringService

## Next Steps

1. **Implement Email Fetching**: 
   - Install MailKit NuGet package
   - Add POP3/IMAP email fetching logic
   - Configure email server settings in appsettings.json

2. **Email Processing Logic**:
   - Link EmailEvent with MotionEvent
   - Auto-create MotionAlert based on EmailEvent
   - Mark emails as processed after handling

3. **Error Handling**:
   - Retry failed email parsing
   - Dead letter queue for unparseable emails
   - Alert admin on email service errors

4. **UI Dashboard**:
   - Page to view EmailEvents
   - Filter by station, date range
   - Mark as processed manually
   - Resend/reprocess failed events

5. **Testing**:
   - Unit tests for email parsing regex
   - Integration tests with mock email server
   - Load testing with bulk emails
