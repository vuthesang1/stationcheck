# Test checkpoint logic for 24/7 timeframe
# StartTime: 00:00:00, EndTime: 23:59:59, Frequency: 1h (60 minutes)

$startTime = [TimeSpan]::Zero
$endTime = [TimeSpan]::new(23, 59, 59)
$frequencyMinutes = 60

Write-Host "=== Testing Checkpoint Logic ===" -ForegroundColor Cyan
Write-Host "StartTime: $($startTime.ToString())"
Write-Host "EndTime: $($endTime.ToString())"
Write-Host "Frequency: $frequencyMinutes minutes"
Write-Host ""

# Test all hours in a day
for ($hour = 0; $hour -lt 24; $hour++) {
    $currentTime = [TimeSpan]::new($hour, 0, 0)
    
    # Check if within timeframe
    $withinTimeFrame = $currentTime -ge $startTime -and $currentTime -le $endTime
    
    # Calculate elapsed time
    $elapsed = $currentTime - $startTime
    
    # Check if at checkpoint (within ±1 minute tolerance)
    $isCheckpoint = $elapsed.TotalMinutes -ge 0 -and ($elapsed.TotalMinutes % $frequencyMinutes) -le 1
    
    $status = if ($isCheckpoint -and $withinTimeFrame) {
        "✅ CHECK"
    } elseif ($withinTimeFrame) {
        "⏭️ SKIP (not checkpoint)"
    } else {
        "❌ OUT OF RANGE"
    }
    
    Write-Host ("{0:D2}:00 - Elapsed: {1:F0}min, Modulo: {2:F1}, {3}" -f `
        $hour, `
        $elapsed.TotalMinutes, `
        ($elapsed.TotalMinutes % $frequencyMinutes), `
        $status)
}

Write-Host ""
Write-Host "=== Testing 00:00 next day (24:00) ===" -ForegroundColor Yellow
$nextDayMidnight = [TimeSpan]::Zero
$withinTimeFrame = $nextDayMidnight -ge $startTime -and $nextDayMidnight -le $endTime
$elapsed = $nextDayMidnight - $startTime
$isCheckpoint = $elapsed.TotalMinutes -ge 0 -and ($elapsed.TotalMinutes % $frequencyMinutes) -le 1

Write-Host "00:00 (next day) - Elapsed: $($elapsed.TotalMinutes)min, Within TimeFrame: $withinTimeFrame, Is Checkpoint: $isCheckpoint"
Write-Host "Result: This is the FIRST checkpoint of the new day ✅" -ForegroundColor Green
