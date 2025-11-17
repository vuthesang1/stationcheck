# =====================================================
# StationCheck Email Simulator - Quick Test Script
# =====================================================
# Usage: .\send-test-email.ps1 [StationId]
# Example: .\send-test-email.ps1 7
# Example: .\send-test-email.ps1 ST000007

param(
    [Parameter(Mandatory=$false)]
    [string]$StationId = ""
)

Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   StationCheck - Email Simulator (PowerShell)         ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# ⚠️ CẤU HÌNH EMAIL (THAY ĐỔI TẠI ĐÂY)
$SmtpServer = "smtp.gmail.com"
$SmtpPort = 587
$FromEmail = "huythucson01@gmail.com"          # ⚠️ THAY ĐỔI
$FromPassword = "dtfn erbe wcjg mkrg"           # ⚠️ THAY ĐỔI (Gmail App Password)
$ToEmail = "vuthesang4@gmail.com"            # Email hệ thống nhận

# Kiểm tra cấu hình
if ($FromEmail -eq "your-email@gmail.com" -or $FromPassword -eq "your-app-password") {
    Write-Host "❌ ERROR: Email credentials not configured!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please edit send-test-email.ps1 and update:" -ForegroundColor Yellow
    Write-Host "  - `$FromEmail: Your Gmail address" -ForegroundColor Yellow
    Write-Host "  - `$FromPassword: Your Gmail App Password (not regular password)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To create App Password:" -ForegroundColor Cyan
    Write-Host "  1. Enable 2-Factor Authentication on Gmail" -ForegroundColor Cyan
    Write-Host "  2. Visit: https://myaccount.google.com/apppasswords" -ForegroundColor Cyan
    Write-Host "  3. Create new App Password for 'Mail'" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}

# Nếu không có StationId, hỏi user
if ([string]::IsNullOrWhiteSpace($StationId)) {
    Write-Host "Enter Station ID (e.g., '7' or 'ST000007'):" -ForegroundColor Yellow
    $StationId = Read-Host "Station ID"
    Write-Host ""
}

if ([string]::IsNullOrWhiteSpace($StationId)) {
    Write-Host "❌ Station ID cannot be empty!" -ForegroundColor Red
    exit 1
}

# Tạo email content
$AlarmTime = Get-Date
$Subject = "[stm] $StationId"
$DeviceName = "NVR-SIMULATOR"
$ChannelName = "Camera-Test"
$IpAddress = "192.168.1.250"

$Body = @"
Alarm Type: Motion Detection Alarm

Alarm Event: Motion Detection

Alarm Input Channel No.: 1

Alarm Input Channel Name: $ChannelName

Alarm Start Time (D/M/Y H:M:S): $($AlarmTime.ToString("dd/MM/yyyy HH:mm:ss"))

Alarm Device Name: $DeviceName

Alarm Name: 

IP Address: $IpAddress

Alarm Details: 
Motion detection triggered at $($AlarmTime.ToString("HH:mm:ss"))
"@

Write-Host "[EMAIL] Sending test email..." -ForegroundColor Green
Write-Host "   From: $FromEmail" -ForegroundColor Gray
Write-Host "   To: $ToEmail" -ForegroundColor Gray
Write-Host "   Subject: $Subject" -ForegroundColor Gray
Write-Host "   Time: $($AlarmTime.ToString("yyyy-MM-dd HH:mm:ss"))" -ForegroundColor Gray
Write-Host ""

try {
    # Tao SMTP client
    $Credential = New-Object System.Management.Automation.PSCredential($FromEmail, (ConvertTo-SecureString $FromPassword -AsPlainText -Force))
    
    # Gui email
    Send-MailMessage `
        -From $FromEmail `
        -To $ToEmail `
        -Subject $Subject `
        -Body $Body `
        -SmtpServer $SmtpServer `
        -Port $SmtpPort `
        -UseSsl `
        -Credential $Credential `
        -Encoding UTF8

    Write-Host "[SUCCESS] Email sent successfully!" -ForegroundColor Green
    Write-Host "   Station ID: $StationId" -ForegroundColor Gray
    Write-Host "   Device: $DeviceName" -ForegroundColor Gray
    Write-Host "   Channel: $ChannelName" -ForegroundColor Gray
    Write-Host "   IP: $IpAddress" -ForegroundColor Gray
    Write-Host ""
    Write-Host "[WAIT] Email should arrive within 1-2 minutes..." -ForegroundColor Yellow
    Write-Host "   Check EmailMonitor logs for processing status" -ForegroundColor Yellow
}
catch {
    Write-Host "[ERROR] Failed to send email: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    
    if ($_.Exception.Message -like "*authentication*" -or $_.Exception.Message -like "*username*password*") {
        Write-Host "[HINT] Check your email credentials" -ForegroundColor Cyan
        Write-Host "   - Make sure you're using Gmail App Password, not regular password" -ForegroundColor Cyan
        Write-Host "   - Enable 2-Factor Authentication first" -ForegroundColor Cyan
    }
    elseif ($_.Exception.Message -like "*SSL*" -or $_.Exception.Message -like "*TLS*") {
        Write-Host "[HINT] SSL/TLS connection issue" -ForegroundColor Cyan
        Write-Host "   - Check your firewall/antivirus settings" -ForegroundColor Cyan
        Write-Host "   - Make sure port 587 is not blocked" -ForegroundColor Cyan
    }
}

Write-Host ""
Write-Host "------------------------------------------------------------" -ForegroundColor DarkGray
