-- Add MacAddress and DeviceStatus columns to UserDevices table
-- Run this script manually using SQL Server Management Studio or sqlcmd

USE StationCheckDb;
GO

-- Check if columns already exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UserDevices') AND name = 'MacAddress')
BEGIN
    ALTER TABLE UserDevices
    ADD MacAddress NVARCHAR(17) NULL;
    
    PRINT 'MacAddress column added successfully';
END
ELSE
BEGIN
    PRINT 'MacAddress column already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('UserDevices') AND name = 'DeviceStatus')
BEGIN
    ALTER TABLE UserDevices
    ADD DeviceStatus INT NOT NULL DEFAULT 0;
    
    PRINT 'DeviceStatus column added successfully';
END
ELSE
BEGIN
    PRINT 'DeviceStatus column already exists';
END
GO

-- Verify columns were added
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH, 
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'UserDevices'
AND COLUMN_NAME IN ('MacAddress', 'DeviceStatus');
GO
