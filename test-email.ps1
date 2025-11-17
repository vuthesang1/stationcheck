# Quick Email Test Script
# This script sends a test email using the C# EmailSimulator

param(
    [Parameter(Mandatory=$true)]
    [string]$StationId
)

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " StationCheck Email Simulator - Quick Test" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

$ProjectPath = "d:\stationcheck\EmailSimulator\EmailSimulator"

Write-Host "[INFO] Sending test email for Station: $StationId" -ForegroundColor Yellow
Write-Host ""

Set-Location $ProjectPath
dotnet run $StationId

Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " Next Steps:" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Check email inbox (vuthesang4@gmail.com)" -ForegroundColor Green
Write-Host "2. Make sure StationCheck app is running to process email" -ForegroundColor Green
Write-Host "3. Check logs: Logs\app-$(Get-Date -Format 'yyyyMMdd').txt" -ForegroundColor Green
Write-Host "4. Check MotionEvents table in database" -ForegroundColor Green
Write-Host ""
