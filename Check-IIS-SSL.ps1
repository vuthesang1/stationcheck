# Check IIS SSL Settings
# Run this script to diagnose why IIS is blocking without certificate

param(
    [Parameter(Mandatory=$true)]
    [string]$SiteName
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "IIS SSL Settings Diagnostic" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Import WebAdministration module
Import-Module WebAdministration -ErrorAction SilentlyContinue
if (-not (Get-Module WebAdministration)) {
    Write-Host "ERROR: WebAdministration module not found!" -ForegroundColor Red
    exit 1
}

$iisPath = "IIS:\Sites\$SiteName"

Write-Host "Checking site: $SiteName" -ForegroundColor White
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

Write-Host "Current SSL Settings:" -ForegroundColor Yellow
Write-Host "---------------------" -ForegroundColor Gray

try {
    $sslFlags = Get-WebConfigurationProperty -PSPath $iisPath -Filter "system.webServer/security/access" -Name "sslFlags" -ErrorAction Stop
    
    Write-Host "SSL Flags: $($sslFlags.Value)" -ForegroundColor White
    Write-Host ""
    
    # Parse flags
    $flagsString = $sslFlags.Value.ToString()
    
    Write-Host "Interpretation:" -ForegroundColor Yellow
    
    if ($flagsString -like "*Ssl*") {
        Write-Host "  ✓ HTTPS Required" -ForegroundColor Green
    } else {
        Write-Host "  ✗ HTTPS NOT Required" -ForegroundColor Red
    }
    
    if ($flagsString -like "*SslRequireCert*") {
        Write-Host "  ✗ Client Certificate REQUIRED (blocks users without cert)" -ForegroundColor Red
        Write-Host "    Problem: Users cannot download DeviceInstaller!" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "FIX: Change to SslNegotiateCert instead:" -ForegroundColor Yellow
        Write-Host "  Set-WebConfigurationProperty -PSPath '$iisPath' -Filter 'system.webServer/security/access' -Name 'sslFlags' -Value 'Ssl,SslNegotiateCert'" -ForegroundColor Gray
    }
    elseif ($flagsString -like "*SslNegotiateCert*") {
        Write-Host "  ✓ Client Certificate REQUESTED (optional - allows download)" -ForegroundColor Green
        Write-Host "    Good: Users can access without certificate" -ForegroundColor Gray
    }
    else {
        Write-Host "  ℹ Client Certificate NOT configured" -ForegroundColor Cyan
    }
    
    if ($flagsString -like "*SslMapCert*") {
        Write-Host "  ✓ Certificate Mapping Enabled" -ForegroundColor Green
    }
    
} catch {
    Write-Host "✗ Could not read SSL settings" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

# Show bindings
Write-Host ""
Write-Host "Site Bindings:" -ForegroundColor Yellow
Write-Host "-------------" -ForegroundColor Gray

$site = Get-Website -Name $SiteName
foreach ($binding in $site.bindings.Collection) {
    $protocol = $binding.protocol
    $bindingInfo = $binding.bindingInformation
    
    Write-Host "  $protocol : $bindingInfo" -ForegroundColor White
    
    if ($protocol -eq "https") {
        # Check SSL certificate
        $parts = $bindingInfo -split ':'
        $port = $parts[1]
        $hostname = $parts[2]
        
        try {
            $cert = Get-ChildItem Cert:\LocalMachine\My | Where-Object {
                $_.Subject -like "*$hostname*" -or $hostname -eq ""
            } | Select-Object -First 1
            
            if ($cert) {
                Write-Host "    Certificate: $($cert.Subject)" -ForegroundColor Gray
                Write-Host "    Thumbprint: $($cert.Thumbprint)" -ForegroundColor Gray
                Write-Host "    Valid: $($cert.NotBefore) to $($cert.NotAfter)" -ForegroundColor Gray
            } else {
                Write-Host "    ⚠ No SSL certificate found" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "    ⚠ Could not check certificate" -ForegroundColor Yellow
        }
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Recommendations:" -ForegroundColor Yellow
Write-Host "---------------" -ForegroundColor Gray

if ($sslFlags.Value -like "*SslRequireCert*") {
    Write-Host ""
    Write-Host "CRITICAL: Change SslRequireCert to SslNegotiateCert" -ForegroundColor Red
    Write-Host ""
    Write-Host "Run this command:" -ForegroundColor Yellow
    Write-Host "  Set-WebConfigurationProperty -PSPath ""$iisPath"" -Filter ""system.webServer/security/access"" -Name ""sslFlags"" -Value ""Ssl,SslNegotiateCert""" -ForegroundColor White
    Write-Host "  Restart-WebAppPool -Name ""$($site.applicationPool)""" -ForegroundColor White
    Write-Host ""
}
elseif ($sslFlags.Value -like "*SslNegotiateCert*") {
    Write-Host ""
    Write-Host "Configuration looks correct!" -ForegroundColor Green
    Write-Host ""
    Write-Host "If users still get 403.16 error:" -ForegroundColor Yellow
    Write-Host "1. Check if users have old/invalid certificates installed" -ForegroundColor Gray
    Write-Host "2. Tell users to close ALL browser windows and reopen" -ForegroundColor Gray
    Write-Host "3. Or access in Incognito/Private mode" -ForegroundColor Gray
    Write-Host ""
}
else {
    Write-Host ""
    Write-Host "Client certificate not configured" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Run this to enable:" -ForegroundColor Yellow
    Write-Host "  .\Configure-IIS-ClientCert.ps1 -SiteName ""$SiteName""" -ForegroundColor White
    Write-Host ""
}
