# Script to fix int to Guid conversion errors

Write-Host "Fixing Guid conversion errors..." -ForegroundColor Green

# Fix common patterns:
# 1. == 0 comparisons -> == Guid.Empty
# 2. int literals -> Guid.Parse() or Guid.NewGuid()
# 3. Type declarations Dictionary<int, ...> -> Dictionary<Guid, ...>

$files = @(
    "Services\StationService.cs",
    "Services\MonitoringService.cs",
    "Services\TimeFrameHistoryService.cs",
    "Services\LocalizationService.cs",
    "Services\EmailService.cs",
    "Services\MotionDetectionService.cs",
    "Components\StationStatusPanel.razor",
    "Components\TimeFrameForm.razor",
    "Components\TimeFrameConfigModal.razor",
    "Components\MotionAlertsPanel.razor",
    "Pages\Stations.razor",
    "Pages\Reports.razor",
    "Pages\SystemConfig.razor",
    "Pages\Translations.razor",
    "Data\ApplicationDbContext.cs",
    "Data\DbSeeder.cs",
    "BackgroundServices\AlertGenerationService.cs"
)

foreach ($file in $files) {
    $path = Join-Path "d:\station-c" $file
    if (Test-Path $path) {
        Write-Host "Processing $file..." -ForegroundColor Yellow
        $content = Get-Content $path -Raw
        
        # Common replacements
        $content = $content -replace '== 0([^x])', '== Guid.Empty$1'  # avoid 0x hex
        $content = $content -replace '!= 0([^x])', '!= Guid.Empty$1'
        $content = $content -replace '> 0([^x])', '!= Guid.Empty$1'  # Guid doesn't have > operator
        $content = $content -replace '\?? 0([^x])', '?? Guid.Empty$1'
        $content = $content -replace 'StationId = stationId', 'StationId = stationId'  # keep as is
        $content = $content -replace 'Dictionary<int, int>', 'Dictionary<Guid, int>'
        $content = $content -replace 'List<int>', 'List<Guid>'
        $content = $content -replace 'IEnumerable<int>', 'IEnumerable<Guid>'
        $content = $content -replace 'int\?', 'Guid?'  # Careful with this one
        
        Set-Content $path $content -NoNewline
    }
}

Write-Host "Done! Now manually review and fix remaining errors." -ForegroundColor Green
