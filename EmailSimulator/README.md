# Email Simulator for StationCheck

## Purpose
Test tool to simulate motion detection emails from Dahua NVR without needing real hardware.

## Quick Start

### Method 1: PowerShell Script (Easiest)
```powershell
# From d:\stationcheck directory:
.\send-test-email.ps1 7
.\send-test-email.ps1ST000007
```

### Method 2: C# Console App
```powershell
# From d:\stationcheck directory:
cd EmailSimulator\EmailSimulator
dotnet run 7                    # Quick test
dotnet run                      # Interactive mode
```

## Configuration

**Already configured in `send-test-email.ps1`:**
- From: huythucson01@gmail.com
- Password: (App Password already set)
- To: vuthesang4@gmail.com

**To change C# app settings:**
Edit `EmailSimulator\EmailSimulator\Program.cs` line ~249:
```csharp
var simulator = new EmailSimulator(
    fromEmail: "your-email@gmail.com",
    fromPassword: "your-app-password",
    toEmail: "vuthesang4@gmail.com"
);
```

## Email Format

Mimics Dahua NVR exactly:

**Subject:** `[stm] 7`

**Body:**
```
Alarm Type: Motion Detection Alarm

Alarm Event: Motion Detection

Alarm Input Channel No.: 1

Alarm Input Channel Name: IPC-TEST

Alarm Start Time (D/M/Y H:M:S): 14/11/2025 15:30:45

Alarm Device Name: NVR-TEST

IP Address: 192.168.1.100
```

## Testing Workflow

1. **Send test email:**
   ```powershell
   .\send-test-email.ps1 7
   ```

2. **Check logs:**
   ```
   Logs\app-20251114.txt
   ```
   Look for:
   - `[EmailService] Found 1 UNREAD email(s)`
   - `[EmailService] Parsed email: StationId=7`
   - `[EmailService] Saved MotionEvent`
   - `[EmailService] Auto-resolved X alert(s)`

3. **Check database:**
   - `MotionEvents` table → New record with EmailMessageId
   - `MotionAlerts` table → Alert auto-resolved (IsResolved=true)
   - `Stations` table → LastMotionDetectedAt updated

## Features

- ✅ Single email test
- ✅ Bulk email test (5 emails with 2s delay)
- ✅ Interactive console menu
- ✅ Command-line quick test
- ✅ Dahua NVR format compatible
- ✅ Gmail SMTP support

## Troubleshooting

**"Authentication failed":**
- Make sure you're using Gmail App Password (not regular password)
- Enable 2-Factor Authentication: https://myaccount.google.com/security
- Create App Password: https://myaccount.google.com/apppasswords

**"Email not received":**
- Check spam/junk folder
- Wait 1-2 minutes for delivery
- Check EmailMonitor is running (polls every 30 seconds)

**"Email received but not processed":**
- Check `Logs\app-YYYYMMDD.txt` for errors
- Verify subject format: `[stm] {StationId}`
- Check Station exists in database
