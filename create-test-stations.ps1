# Script tạo 20 stations test với monitoring enabled
# Mỗi station có 1 khung giờ

$baseUrl = "https://localhost:7018"
$username = "admin"
$password = "Admin@123"

Write-Host "=== Tạo 20 Stations Test ===" -ForegroundColor Cyan

# 1. Login để lấy token
Write-Host "`n[1/3] Đăng nhập..." -ForegroundColor Yellow
$loginBody = @{
    username = $username
    password = $password
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
        -Method Post `
        -Body $loginBody `
        -ContentType "application/json" `
        -SkipCertificateCheck

    $token = $loginResponse.token
    Write-Host "✓ Đăng nhập thành công!" -ForegroundColor Green
} catch {
    Write-Host "✗ Lỗi đăng nhập: $_" -ForegroundColor Red
    exit 1
}

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# 2. Tạo 20 stations
Write-Host "`n[2/3] Tạo 20 stations..." -ForegroundColor Yellow

$stationIds = @()

for ($i = 1; $i -le 20; $i++) {
    $stationName = "Trạm Test $i"
    $stationBody = @{
        name = $stationName
        address = "Địa chỉ trạm test số $i, Quận $($i % 12 + 1), TP.HCM"
        latitude = 10.7769 + ($i * 0.01)
        longitude = 106.7009 + ($i * 0.01)
        contactPhone = "028$(Get-Random -Minimum 1000000 -Maximum 9999999)"
        contactEmail = "tram$i@test.com"
        nvrIpAddress = "192.168.$($i).100"
        nvrPort = 80
        nvrUsername = "admin"
        nvrPassword = "admin123"
        isActive = $true
    } | ConvertTo-Json

    try {
        $stationResponse = Invoke-RestMethod -Uri "$baseUrl/api/stations" `
            -Method Post `
            -Headers $headers `
            -Body $stationBody `
            -SkipCertificateCheck

        $stationIds += $stationResponse.id
        Write-Host "  ✓ Tạo '$stationName' - ID: $($stationResponse.id)" -ForegroundColor Green
    } catch {
        Write-Host "  ✗ Lỗi tạo '$stationName': $_" -ForegroundColor Red
    }
}

Write-Host "`n✓ Đã tạo $($stationIds.Count) stations!" -ForegroundColor Green

# 3. Tạo khung giờ cho mỗi station
Write-Host "`n[3/3] Tạo khung giờ monitoring cho từng station..." -ForegroundColor Yellow

foreach ($stationId in $stationIds) {
    # Tạo 1 khung giờ mặc định (6:00 - 22:00, tất cả các ngày trong tuần)
    $timeFrameBody = @{
        stationId = $stationId
        name = "Khung giờ mặc định"
        startTime = "06:00:00"
        endTime = "22:00:00"
        daysOfWeek = @(1, 2, 3, 4, 5, 6, 0)  # Thứ 2 - Chủ nhật
        isActive = $true
        motionSensitivity = 5
        minMotionInterval = 10
        maxAlertsPerHour = 10
    } | ConvertTo-Json

    try {
        $timeFrameResponse = Invoke-RestMethod -Uri "$baseUrl/api/monitoring/time-frames" `
            -Method Post `
            -Headers $headers `
            -Body $timeFrameBody `
            -SkipCertificateCheck

        Write-Host "  ✓ Tạo khung giờ cho Station ID: $stationId" -ForegroundColor Green
    } catch {
        Write-Host "  ✗ Lỗi tạo khung giờ cho Station ID $stationId : $_" -ForegroundColor Red
    }
}

Write-Host "`n=== HOÀN THÀNH ===" -ForegroundColor Cyan
Write-Host "✓ Đã tạo $($stationIds.Count) stations" -ForegroundColor Green
Write-Host "✓ Mỗi station có 1 khung giờ monitoring (6:00-22:00, tất cả các ngày)" -ForegroundColor Green
Write-Host "✓ Tất cả stations và khung giờ đều đã được ENABLED" -ForegroundColor Green
