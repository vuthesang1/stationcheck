# Test Motion Detection API Endpoints
# Run this script to test the motion detection webhook and other endpoints

# Configuration
$baseUrl = "http://localhost:55704/api/motion"

Write-Host "=== Testing Motion Detection API ===" -ForegroundColor Cyan
Write-Host ""

# Test 1: Send Motion Event (Webhook from Dahua NVR)
Write-Host "[1] Testing POST /api/motion/event (Webhook)" -ForegroundColor Yellow
$motionEvent = @{
    CameraId = "CAM001"
    CameraName = "Main Entrance"
    EventType = "VideoMotion"
    DetectedAt = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    Payload = @{
        Channel = 1
        Action = "Start"
        RegionName = "Region1"
    } | ConvertTo-Json
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/event" -Method Post -Body $motionEvent -ContentType "application/json"
    Write-Host "✅ Success: Motion event created" -ForegroundColor Green
    Write-Host "Response: $($response | ConvertTo-Json)" -ForegroundColor Gray
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Start-Sleep -Seconds 2

# Test 2: Get Unresolved Alerts
Write-Host "[2] Testing GET /api/motion/alerts" -ForegroundColor Yellow
try {
    $alerts = Invoke-RestMethod -Uri "$baseUrl/alerts" -Method Get
    Write-Host "✅ Success: Retrieved $($alerts.Count) alert(s)" -ForegroundColor Green
    $alerts | ForEach-Object {
        Write-Host "  - Alert: $($_.cameraName) - $($_.message) - Severity: $($_.severity)" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Start-Sleep -Seconds 2

# Test 3: Get Motion Events
Write-Host "[3] Testing GET /api/motion/events?cameraId=CAM001" -ForegroundColor Yellow
try {
    $events = Invoke-RestMethod -Uri "$baseUrl/events?cameraId=CAM001" -Method Get
    Write-Host "✅ Success: Retrieved $($events.Count) event(s)" -ForegroundColor Green
    $events | Select-Object -First 5 | ForEach-Object {
        Write-Host "  - Event: $($_.cameraName) at $($_.detectedAt)" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Start-Sleep -Seconds 2

# Test 4: Get Today's Stats
Write-Host "[4] Testing GET /api/motion/stats/today" -ForegroundColor Yellow
try {
    $stats = Invoke-RestMethod -Uri "$baseUrl/stats/today" -Method Get
    Write-Host "✅ Success: Retrieved motion stats" -ForegroundColor Green
    $stats.PSObject.Properties | ForEach-Object {
        Write-Host "  - $($_.Name): $($_.Value) events" -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Open browser: https://localhost:55703" -ForegroundColor White
Write-Host "2. View motion alerts panel on the dashboard" -ForegroundColor White
Write-Host "3. Configure Dahua NVR to send webhooks to: http://localhost:55704/api/motion/event" -ForegroundColor White
