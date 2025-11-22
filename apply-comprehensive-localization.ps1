<#
.SYNOPSIS
    Apply comprehensive localization keys to StationCheck database

.DESCRIPTION
    This script executes the comprehensive-localization-keys.sql file
    to add 400+ translation keys for Vietnamese and English languages

.PARAMETER ServerInstance
    SQL Server instance name (default: localhost)

.PARAMETER Database
    Database name (default: StationCheckDb)

.PARAMETER SqlFile
    Path to SQL file (default: Migrations/comprehensive-localization-keys.sql)

.EXAMPLE
    .\apply-comprehensive-localization.ps1

.EXAMPLE
    .\apply-comprehensive-localization.ps1 -ServerInstance "localhost\SQLEXPRESS" -Database "StationCheckDb"

.NOTES
    Author: StationCheck Team
    Date: 2025-11-21
    Version: 2.0
#>

param(
    [string]$ServerInstance = "localhost",
    [string]$Database = "StationCheckDb",
    [string]$SqlFile = "Migrations/comprehensive-localization-keys.sql"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "🌍 StationCheck Comprehensive Localization Installer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if SQL file exists
if (-not (Test-Path $SqlFile)) {
    Write-Host "❌ ERROR: SQL file not found at: $SqlFile" -ForegroundColor Red
    Write-Host "Please ensure the file exists and try again." -ForegroundColor Red
    exit 1
}

Write-Host "📋 Configuration:" -ForegroundColor Yellow
Write-Host "  Server: $ServerInstance" -ForegroundColor Gray
Write-Host "  Database: $Database" -ForegroundColor Gray
Write-Host "  SQL File: $SqlFile" -ForegroundColor Gray
Write-Host ""

# Prompt for confirmation
$confirm = Read-Host "⚠️  This will add 400+ translation keys to the database. Continue? (Y/N)"
if ($confirm -ne 'Y' -and $confirm -ne 'y') {
    Write-Host "❌ Operation cancelled by user." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "🔄 Applying localization keys..." -ForegroundColor Cyan

# Try using Invoke-Sqlcmd first (SQL Server PowerShell module)
try {
    Write-Host "  Attempting to use Invoke-Sqlcmd..." -ForegroundColor Gray
    
    # Check if SqlServer module is available
    if (Get-Module -ListAvailable -Name SqlServer) {
        Import-Module SqlServer -ErrorAction Stop
        
        # Execute SQL file
        Invoke-Sqlcmd -ServerInstance $ServerInstance `
                      -Database $Database `
                      -InputFile $SqlFile `
                      -QueryTimeout 300 `
                      -ErrorAction Stop
        
        Write-Host ""
        Write-Host "✅ SUCCESS! Localization keys applied successfully!" -ForegroundColor Green
        Write-Host ""
        
        # Verify installation
        Write-Host "🔍 Verifying installation..." -ForegroundColor Cyan
        $countQuery = "SELECT COUNT(*) as Total FROM Translations"
        $result = Invoke-Sqlcmd -ServerInstance $ServerInstance `
                                -Database $Database `
                                -Query $countQuery `
                                -ErrorAction Stop
        
        $totalKeys = $result.Total
        Write-Host "  Total translation keys in database: $totalKeys" -ForegroundColor Green
        
        if ($totalKeys -ge 400) {
            Write-Host "  ✓ Verification passed!" -ForegroundColor Green
        } else {
            Write-Host "  ⚠️  Expected 400+ keys, but found $totalKeys" -ForegroundColor Yellow
        }
        
    } else {
        throw "SqlServer module not found"
    }
    
} catch {
    Write-Host "  SqlServer module not available or error occurred" -ForegroundColor Yellow
    Write-Host "  Falling back to sqlcmd.exe..." -ForegroundColor Gray
    
    # Fallback to sqlcmd.exe
    try {
        $sqlcmdPath = Get-Command sqlcmd.exe -ErrorAction Stop
        
        # Execute with sqlcmd
        & sqlcmd.exe -S $ServerInstance -d $Database -i $SqlFile -b
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "✅ SUCCESS! Localization keys applied successfully!" -ForegroundColor Green
            Write-Host ""
            
            # Verify with sqlcmd
            Write-Host "🔍 Verifying installation..." -ForegroundColor Cyan
            $countQuery = "SELECT COUNT(*) as Total FROM Translations"
            $result = & sqlcmd.exe -S $ServerInstance -d $Database -Q $countQuery -h -1 -W
            
            if ($result) {
                Write-Host "  Total translation keys in database: $($result.Trim())" -ForegroundColor Green
            }
            
        } else {
            throw "sqlcmd.exe failed with exit code $LASTEXITCODE"
        }
        
    } catch {
        Write-Host ""
        Write-Host "❌ ERROR: Failed to execute SQL file" -ForegroundColor Red
        Write-Host "  $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        Write-Host "💡 Manual Installation Options:" -ForegroundColor Yellow
        Write-Host "  1. Open SQL Server Management Studio (SSMS)" -ForegroundColor Gray
        Write-Host "  2. Connect to server: $ServerInstance" -ForegroundColor Gray
        Write-Host "  3. Open file: $SqlFile" -ForegroundColor Gray
        Write-Host "  4. Select database: $Database" -ForegroundColor Gray
        Write-Host "  5. Execute the script (F5)" -ForegroundColor Gray
        Write-Host ""
        Write-Host "  OR use command line:" -ForegroundColor Gray
        Write-Host "  sqlcmd -S $ServerInstance -d $Database -i $SqlFile" -ForegroundColor Gray
        Write-Host ""
        exit 1
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "📚 Next Steps:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "1. Review COMPLETE_LOCALIZATION_GUIDE.md for implementation details" -ForegroundColor White
Write-Host "2. Update Razor files to use @GetText() instead of hardcoded text" -ForegroundColor White
Write-Host "3. Update C# services with localized exception messages" -ForegroundColor White
Write-Host "4. Test language switching in the application" -ForegroundColor White
Write-Host ""
Write-Host "✨ Happy localizing! ✨" -ForegroundColor Green
Write-Host ""
