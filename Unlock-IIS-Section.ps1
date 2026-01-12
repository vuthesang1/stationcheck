# Unlock IIS Security Access Section
# Run this script AS ADMINISTRATOR to unlock the access section in applicationHost.config

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Unlock IIS Security Access Section" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host "Right-click PowerShell and select 'Run as Administrator'" -ForegroundColor Yellow
    exit 1
}

$configPath = "$env:SystemRoot\System32\inetsrv\config\applicationHost.config"

Write-Host "Backing up applicationHost.config..." -ForegroundColor Yellow
try {
    $backupPath = "$configPath.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    Copy-Item $configPath $backupPath -ErrorAction Stop
    Write-Host "Backup created: $backupPath" -ForegroundColor Green
} catch {
    Write-Host "WARNING: Could not create backup" -ForegroundColor Yellow
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Gray
}
Write-Host ""

Write-Host "Reading applicationHost.config..." -ForegroundColor Yellow
try {
    $content = Get-Content $configPath -Raw -ErrorAction Stop
    
    # Check if already unlocked
    if ($content -match '<section name="access"[^>]*overrideModeDefault="Allow"[^>]*/>') {
        Write-Host "Section 'access' is already unlocked!" -ForegroundColor Green
        Write-Host ""
        Write-Host "You can now run: .\Configure-IIS-ClientCert.ps1 -SiteName 'station'" -ForegroundColor White
        exit 0
    }
    
    # Unlock access section
    Write-Host "Unlocking 'access' section..." -ForegroundColor Yellow
    $originalPattern = '<section name="access"([^>]*)overrideModeDefault="Deny"([^>]*)/>'
    $replacement = '<section name="access"$1overrideModeDefault="Allow"$2/>'
    
    if ($content -match $originalPattern) {
        $newContent = $content -replace $originalPattern, $replacement
        
        # Save changes
        Set-Content -Path $configPath -Value $newContent -ErrorAction Stop
        Write-Host "Successfully unlocked 'access' section!" -ForegroundColor Green
    } else {
        Write-Host "WARNING: Could not find 'access' section with Deny mode" -ForegroundColor Yellow
        Write-Host "The section may already be unlocked or have different format" -ForegroundColor Gray
    }
    
} catch {
    Write-Host "ERROR: Failed to modify configuration" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Manual steps:" -ForegroundColor Yellow
    Write-Host "1. Open: $configPath" -ForegroundColor Gray
    Write-Host "2. Find: <section name=`"access`" overrideModeDefault=`"Deny`" />" -ForegroundColor Gray
    Write-Host "3. Change to: <section name=`"access`" overrideModeDefault=`"Allow`" />" -ForegroundColor Gray
    Write-Host "4. Save and run: iisreset" -ForegroundColor Gray
    exit 1
}
Write-Host ""

Write-Host "Restarting IIS..." -ForegroundColor Yellow
try {
    iisreset | Out-Null
    Write-Host "IIS restarted successfully!" -ForegroundColor Green
} catch {
    Write-Host "WARNING: Could not restart IIS automatically" -ForegroundColor Yellow
    Write-Host "Please run: iisreset" -ForegroundColor Gray
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Unlock Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next step:" -ForegroundColor White
Write-Host "  .\Configure-IIS-ClientCert.ps1 -SiteName 'station'" -ForegroundColor Gray
Write-Host ""
