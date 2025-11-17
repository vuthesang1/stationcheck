# Email Simulator - Quick Start Guide

## ✅ Setup Complete!

Email simulator đã được cấu hình và test thành công.

## 🚀 How to Send Test Email

### Method 1: Quick Script (Recommended)
```powershell
cd d:\stationcheck
.\test-email.ps1 7
.\test-email.ps1 ST000007
```

### Method 2: Direct C# Program
```powershell
cd d:\stationcheck\EmailSimulator\EmailSimulator
dotnet run 7                    # Send single email for Station 7
dotnet run                      # Interactive mode
```

## 📧 Email Configuration

**Already configured:**
- SMTP Server: smtp.gmail.com:587
- From: huythucson01@gmail.com
- To: vuthesang4@gmail.com
- Password: ✅ Set (Gmail App Password)

## 📋 Testing Workflow

### 1. Send Test Email
```powershell
.\test-email.ps1 7
```

Expected output:
```
✅ Email sent successfully!
   Station ID: 7
   Device: NVR-TEST
   Channel: IPC-TEST
   IP: 192.168.1.100
```

### 2. Start StationCheck App (if not running)
```powershell
dotnet run
```

### 3. Check Email Processing Logs
```powershell
# View today's log
Get-Content "Logs\app-$(Get-Date -Format 'yyyyMMdd').txt" -Tail 50 -Wait
```

Look for:
```
[EmailService] Found 1 UNREAD email(s)
[EmailService] Processing NEW email: Subject='[stm] 7', MessageId='...'
[EmailService] Parsed email: StationId=7, DetectedAt=2025-11-14 11:39:31
[EmailService] Saved MotionEvent for Station 7
[EmailService] Auto-resolved 1 active alert(s)
[EmailService] Updated Station 7 LastMotionDetectedAt
```

### 4. Verify in Database
```sql
-- Check MotionEvent created
SELECT TOP 1 * FROM MotionEvents 
WHERE StationId = 7 
ORDER BY DetectedAt DESC;

-- Check Alert auto-resolved
SELECT * FROM MotionAlerts 
WHERE StationId = 7 AND IsResolved = 1 
ORDER BY ResolvedAt DESC;

-- Check Station updated
SELECT Id, Name, LastMotionDetectedAt 
FROM Stations 
WHERE Id = 7;
```

## 🧪 Test Scenarios

### Scenario 1: Auto-Resolve Alert
1. Create active alert for Station 7 (wait for no motion)
2. Send test email: `.\test-email.ps1 7`
3. Check alert auto-resolved in database

### Scenario 2: Buffer Time Tolerance
1. Configure TimeFrame: 09:00-17:00, frequency 60 min, buffer 15 min
2. Send email at 09:50 (10 min early)
3. Alert check at 10:00 → Should NOT generate alert (within 75-min window)

### Scenario 3: Bulk Email Test
```powershell
cd EmailSimulator\EmailSimulator
dotnet run

# Choose option 2 (Bulk emails)
# Enter Station ID: 7
# Sends 5 emails with 2-second delay
```

## 📊 What Happens When Email Received?

1. **EmailService polls Gmail** (every 30 seconds)
2. **Finds UNREAD email** with subject `[stm] 7`
3. **Parses email:**
   - StationId: 7
   - DetectedAt: 2025-11-14 11:39:31
   - Device: NVR-TEST
   - Channel: IPC-TEST
4. **Creates MotionEvent** record
5. **Auto-resolves active alerts** for Station 7
6. **Updates Station.LastMotionDetectedAt**
7. **Marks email as READ** (via MessageId tracking)

## 🔍 Troubleshooting

### Email Not Received
- Check spam/junk folder in vuthesang4@gmail.com
- Wait 1-2 minutes for delivery
- Check email sent: Login to huythucson01@gmail.com → Sent folder

### Email Not Processed
- Make sure StationCheck app is running
- Check EmailMonitor background service is enabled
- Check logs for errors: `Logs\app-YYYYMMDD.txt`
- Verify Station 7 exists in database

### Authentication Failed
- Using Gmail App Password (not regular password) ✅
- 2FA enabled on Gmail ✅
- Check password not expired

## 📝 Email Format Reference

**Subject:**
```
[stm] 7
```

**Body:**
```
Alarm Type: Motion Detection Alarm

Alarm Event: Motion Detection

Alarm Input Channel No.: 1

Alarm Input Channel Name: IPC-TEST

Alarm Start Time (D/M/Y H:M/S): 14/11/2025 11:39:31

Alarm Device Name: NVR-TEST

Alarm Name: 

IP Address: 192.168.1.100

Alarm Details: 
Motion detection triggered at 11:39:31
```

## 🎯 Quick Commands

```powershell
# Send test email
.\test-email.ps1 7

# Run app
dotnet run

# Watch logs
Get-Content "Logs\app-$(Get-Date -Format 'yyyyMMdd').txt" -Tail 50 -Wait

# Build simulator
cd EmailSimulator\EmailSimulator
dotnet build

# Interactive mode
dotnet run
```

## ✨ Features Tested

- ✅ Email sending via Gmail SMTP
- ✅ Dahua NVR format compatible
- ✅ StationId extraction from subject
- ✅ Email parsing and MotionEvent creation
- ⏳ Auto-resolve functionality (waiting for app to process)
- ⏳ Station.LastMotionDetectedAt update
- ⏳ Duplicate detection (MessageId)

## 🚀 Next: Start StationCheck and Process Email

```powershell
# Terminal 1: Run app
cd d:\stationcheck
dotnet run

# Terminal 2: Watch logs
Get-Content "Logs\app-$(Get-Date -Format 'yyyyMMdd').txt" -Tail 50 -Wait

# Terminal 3: Send test
.\test-email.ps1 7
```
