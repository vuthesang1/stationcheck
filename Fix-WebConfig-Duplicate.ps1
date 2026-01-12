# Remove duplicate security section from web.config

$webConfigPath = "D:\host-station\Debug\net8.0\web.config"

Write-Host "Reading web.config..." -ForegroundColor Yellow
$content = Get-Content $webConfigPath -Raw

Write-Host "Removing all security sections..." -ForegroundColor Yellow
# Remove all security/access sections
$content = $content -replace '[\r\n\s]*<!--[^>]*IIS SSL Settings[^>]*-->[\r\n\s]*<security>[\s\S]*?</security>', ''

Write-Host "Saving cleaned config..." -ForegroundColor Yellow
Set-Content $webConfigPath -Value $content

Write-Host "Adding single security section..." -ForegroundColor Yellow
$content = Get-Content $webConfigPath -Raw

$securitySection = @"

      
      <!-- IIS SSL Settings: Request client certificate but don't require validation -->
      <security>
        <access sslFlags="Ssl,SslNegotiateCert" />
      </security>
"@

# Insert after rewrite closing tag
$content = $content -replace '(</rewrite>)', "`$1$securitySection"

Set-Content $webConfigPath -Value $content

Write-Host "`nUpdated web.config:" -ForegroundColor Green
Get-Content $webConfigPath | Select-Object -First 45

Write-Host "`nRestarting app pool..." -ForegroundColor Yellow
Restart-WebAppPool -Name "DefaultAppPool"

Write-Host "`nDone!" -ForegroundColor Green
