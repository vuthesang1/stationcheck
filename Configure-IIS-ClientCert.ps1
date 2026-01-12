# Configure IIS SSL Settings for Client Certificate Authentication
# Run this script AS ADMINISTRATOR on the Plesk IIS server

param(
    [Parameter(Mandatory=$true)]
    [string]$SiteName,
    
    [Parameter(Mandatory=$false)]
    [string]$ApplicationPath = ""
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "IIS Client Certificate Configuration" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host "Right-click PowerShell and select 'Run as Administrator'" -ForegroundColor Yellow
    exit 1
}

# Import WebAdministration module
Import-Module WebAdministration -ErrorAction SilentlyContinue
if (-not (Get-Module WebAdministration)) {
    Write-Host "ERROR: WebAdministration module not found!" -ForegroundColor Red
    Write-Host "Please install IIS Management Scripts and Tools" -ForegroundColor Yellow
    exit 1
}

# Build IIS path
$iisPath = "IIS:\Sites\$SiteName"
if ($ApplicationPath) {
    $iisPath = "$iisPath\$ApplicationPath"
}

Write-Host "Target: $iisPath" -ForegroundColor White
Write-Host ""

# Check if site exists
if (-not (Test-Path $iisPath)) {
    Write-Host "ERROR: Site not found: $SiteName" -ForegroundColor Red
    Write-Host ""
    Write-Host "Available sites:" -ForegroundColor Yellow
    Get-ChildItem IIS:\Sites | ForEach-Object {
        Write-Host "  - $($_.Name)" -ForegroundColor Gray
    }
    exit 1
}

Write-Host "Step 1: Checking current SSL settings..." -ForegroundColor Yellow
try {
    $currentSettings = Get-WebConfigurationProperty -PSPath $iisPath -Filter "system.webServer/security/access" -Name "sslFlags" -ErrorAction Stop
    Write-Host "Current SSL Flags: $($currentSettings.Value)" -ForegroundColor Gray
} catch {
    Write-Host "WARNING: Cannot read current SSL settings" -ForegroundColor Yellow
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Gray
}
Write-Host ""

Write-Host "Step 2: Configuring SSL settings..." -ForegroundColor Yellow
try {
    # Set SSL flags: Ssl (require HTTPS) + SslNegotiateCert (request client cert but don't require validation)
    # IMPORTANT: Use "SslNegotiateCert" NOT "SslRequireCert" to allow users without certificates to download DeviceInstaller
    Set-WebConfigurationProperty -PSPath $iisPath -Filter "system.webServer/security/access" -Name "sslFlags" -Value "Ssl,SslNegotiateCert" -ErrorAction Stop
    Write-Host "SSL Flags set to: Ssl, SslNegotiateCert" -ForegroundColor Green
    Write-Host "  (Certificate is REQUESTED but NOT REQUIRED - allows DeviceInstaller download)" -ForegroundColor Gray
} catch {
    Write-Host "Failed to configure SSL settings" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "This may be because the section is locked in applicationHost.config" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To unlock, edit: C:\Windows\System32\inetsrv\config\applicationHost.config" -ForegroundColor Yellow
    Write-Host 'Find: <section name="access" overrideModeDefault="Deny" />' -ForegroundColor Gray
    Write-Host 'Change to: <section name="access" overrideModeDefault="Allow" />' -ForegroundColor Gray
    Write-Host ""
    Write-Host "Then restart IIS: iisreset" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

Write-Host "Step 3: Verifying configuration..." -ForegroundColor Yellow
try {
    $newSettings = Get-WebConfigurationProperty -PSPath $iisPath -Filter "system.webServer/security/access" -Name "sslFlags" -ErrorAction Stop
    Write-Host "New SSL Flags: $($newSettings.Value)" -ForegroundColor Green
    
    if ($newSettings.Value -like "*SslNegotiateCert*") {
        Write-Host "Client certificate negotiation enabled" -ForegroundColor Green
    } else {
        Write-Host "Warning: SslNegotiateCert not set" -ForegroundColor Yellow
    }
} catch {
    Write-Host "Could not verify settings" -ForegroundColor Yellow
}
Write-Host ""

Write-Host "Step 4: Restarting application pool..." -ForegroundColor Yellow
try {
    # Get application pool name for the site
    $site = Get-Website -Name $SiteName
    $appPoolName = $site.applicationPool
    
    Write-Host "Application Pool: $appPoolName" -ForegroundColor Gray
    
    # Restart app pool
    Restart-WebAppPool -Name $appPoolName -ErrorAction Stop
    Write-Host "Application pool restarted" -ForegroundColor Green
} catch {
    Write-Host "Could not restart application pool" -ForegroundColor Yellow
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Gray
    Write-Host "You may need to restart manually in IIS Manager" -ForegroundColor Yellow
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Configuration Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor White
Write-Host "1. Deploy your application with web.config included" -ForegroundColor Gray
Write-Host "2. Run DeviceInstaller on client machines" -ForegroundColor Gray
Write-Host "3. Browser will automatically send client certificate" -ForegroundColor Gray
Write-Host "4. ASP.NET Core will validate certificate with custom logic" -ForegroundColor Gray
Write-Host ""
Write-Host "To test:" -ForegroundColor White
Write-Host "  - Open browser and navigate to: https://$SiteName" -ForegroundColor Gray
Write-Host "  - Browser should send certificate automatically" -ForegroundColor Gray
Write-Host "  - Check application logs for certificate validation" -ForegroundColor Gray
Write-Host ""

# Show example usage
Write-Host "Example usage:" -ForegroundColor Yellow
Write-Host "  .\Configure-IIS-ClientCert.ps1 -SiteName 'pvgascng.vn'" -ForegroundColor Gray
Write-Host "  .\Configure-IIS-ClientCert.ps1 -SiteName 'pvgascng.vn' -ApplicationPath 'stationcheck'" -ForegroundColor Gray
Write-Host ""
