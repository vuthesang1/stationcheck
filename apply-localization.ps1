# Apply Localization Keys to Database
# Run this script to add all localization translations

param(
    [string]$ServerInstance = "localhost",
    [string]$Database = "StationCheckDb",
    [string]$SqlFile = "Migrations\add-localization-keys.sql"
)

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  StationCheck Localization Installer" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Check if SQL file exists
if (-not (Test-Path $SqlFile)) {
    Write-Host "ERROR: SQL file not found: $SqlFile" -ForegroundColor Red
    exit 1
}

Write-Host "Configuration:" -ForegroundColor Yellow
Write-Host "  Server: $ServerInstance"
Write-Host "  Database: $Database"
Write-Host "  SQL File: $SqlFile"
Write-Host ""

# Confirm before proceeding
$confirm = Read-Host "Do you want to proceed? (Y/N)"
if ($confirm -ne 'Y' -and $confirm -ne 'y') {
    Write-Host "Operation cancelled." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Applying localization keys..." -ForegroundColor Green

try {
    # Using Invoke-Sqlcmd if available
    if (Get-Command Invoke-Sqlcmd -ErrorAction SilentlyContinue) {
        Invoke-Sqlcmd -ServerInstance $ServerInstance -Database $Database -InputFile $SqlFile
        Write-Host "SUCCESS: Localization keys added successfully!" -ForegroundColor Green
    }
    # Fallback to sqlcmd.exe
    elseif (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
        sqlcmd -S $ServerInstance -d $Database -i $SqlFile
        if ($LASTEXITCODE -eq 0) {
            Write-Host "SUCCESS: Localization keys added successfully!" -ForegroundColor Green
        } else {
            throw "sqlcmd returned error code: $LASTEXITCODE"
        }
    }
    else {
        throw "Neither Invoke-Sqlcmd nor sqlcmd.exe found. Please install SQL Server tools."
    }
}
catch {
    Write-Host "ERROR: Failed to apply localization keys" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Update Razor files to use @GetText() for labels"
Write-Host "2. Test language switching in the application"
Write-Host "3. Refer to LOCALIZATION_GUIDE.md for details"
Write-Host ""
Write-Host "Done!" -ForegroundColor Green
