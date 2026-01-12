# Add Security Section to IIS web.config
# This script adds SSL client certificate settings directly to web.config

$webConfigPath = "D:\host-station\Debug\net8.0\web.config"

Write-Host "Step 1: Backing up web.config..." -ForegroundColor Yellow
Copy-Item $webConfigPath "$webConfigPath.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
Write-Host "Backup created!" -ForegroundColor Green
Write-Host ""

Write-Host "Step 2: Reading current content..." -ForegroundColor Yellow
$content = Get-Content $webConfigPath -Raw

Write-Host "Step 3: Adding security section..." -ForegroundColor Yellow
$securitySection = @"

      
      <!-- IIS SSL Settings: Request client certificate but don't require validation -->
      <security>
        <access sslFlags="Ssl,SslNegotiateCert" />
      </security>
"@

# Insert after rewrite closing tag
$content = $content -replace '(</rewrite>)', "`$1$securitySection"

Write-Host "Step 4: Saving changes..." -ForegroundColor Yellow
Set-Content $webConfigPath -Value $content
Write-Host "Changes saved!" -ForegroundColor Green
Write-Host ""

Write-Host "Step 5: Showing updated config..." -ForegroundColor Yellow
Get-Content $webConfigPath | Select-Object -First 40
Write-Host ""

Write-Host "Step 6: Restarting application pool..." -ForegroundColor Yellow
Restart-WebAppPool -Name "DefaultAppPool"
Write-Host "Application pool restarted!" -ForegroundColor Green
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Configuration Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Now test by accessing: https://localhost" -ForegroundColor White
Write-Host "Browser should request certificate but allow connection without it" -ForegroundColor Gray
Write-Host ""
