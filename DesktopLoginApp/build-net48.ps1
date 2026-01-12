# Build Script for .NET Framework 4.8 Version (Windows 7 Compatible)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building Desktop Login App for Windows 7" -ForegroundColor Cyan
Write-Host ".NET Framework 4.8 Version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Set location to DesktopLoginApp directory
Set-Location -Path $PSScriptRoot

# Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "bin\Release\Net48") {
    Remove-Item -Path "bin\Release\Net48" -Recurse -Force
}
if (Test-Path "bin\Debug\Net48") {
    Remove-Item -Path "bin\Debug\Net48" -Recurse -Force
}

# Restore NuGet packages
Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Yellow
nuget restore DesktopLoginApp-Net48.csproj -PackagesDirectory packages

# Build the project
Write-Host "🔨 Building project..." -ForegroundColor Yellow
msbuild DesktopLoginApp-Net48.csproj /p:Configuration=Release /p:Platform=AnyCPU /t:Rebuild /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ Build completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Output location: bin\Release\Net48\DesktopLoginApp.exe" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Next steps:" -ForegroundColor Yellow
Write-Host "1. Copy bin\Release\Net48 folder to Windows 7 machine"
Write-Host "2. Ensure .NET Framework 4.8 is installed on target machine"
Write-Host "3. Run DesktopLoginApp.exe"
Write-Host ""
Write-Host "📥 Download .NET Framework 4.8:" -ForegroundColor Yellow
Write-Host "https://dotnet.microsoft.com/download/dotnet-framework/net48"
Write-Host ""
