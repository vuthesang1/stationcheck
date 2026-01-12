# Approve device for Desktop App login
# Usage: .\approve-device.ps1 -MacAddress "24:2F:D0:D9:78:95"

param(
    [Parameter(Mandatory=$false)]
    [string]$MacAddress = "24:2F:D0:D9:78:95"
)

Write-Host "Approving device with MAC: $MacAddress" -ForegroundColor Cyan

# SQL Server connection
$connectionString = "Server=VUTHESANG\SQLEXPRESS01;Database=StationCheckDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"

# SQL Query to approve device
$sqlQuery = @"
UPDATE UserDevices 
SET DeviceStatus = 1, -- 1 = Approved (0=PendingApproval, 2=Rejected)
    UpdatedAt = GETDATE()
WHERE MacAddress = '$MacAddress' 
  AND IsDeleted = 0;

-- Show updated device
SELECT Id, MacAddress, DeviceStatus, IsRevoked, CreatedAt, UpdatedAt
FROM UserDevices 
WHERE MacAddress = '$MacAddress';
"@

try {
    # Create SQL connection
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "Database connection successful" -ForegroundColor Green

    # Execute update
    $command = $connection.CreateCommand()
    $command.CommandText = $sqlQuery
    
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
    $dataset = New-Object System.Data.DataSet
    $adapter.Fill($dataset) | Out-Null

    # Show results
    if ($dataset.Tables[0].Rows.Count -gt 0) {
        Write-Host "`nDevice updated successfully:" -ForegroundColor Green
        $dataset.Tables[0] | Format-Table -AutoSize
        
        $status = $dataset.Tables[0].Rows[0]["DeviceStatus"]
        if ($status -eq 1) {
            Write-Host "✓ Device status is now: Approved" -ForegroundColor Green
        } else {
            Write-Host "⚠ Device status: $status (Expected: 1 for Approved)" -ForegroundColor Yellow
        }
    } else {
        Write-Host "⚠ Device not found or already deleted" -ForegroundColor Yellow
    }

    $connection.Close()
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host "`nYou can now test the Desktop App login again." -ForegroundColor Cyan
