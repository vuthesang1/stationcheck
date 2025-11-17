-- Update existing stations to have StationCode
-- This script will generate StationCode for stations that don't have one yet

-- First, let's see stations without StationCode
SELECT Id, Name, StationCode 
FROM Stations 
WHERE StationCode IS NULL OR StationCode = ''
ORDER BY Id;

-- Update stations without StationCode
-- Generate format: ST000001, ST000002, ST000003, etc.

UPDATE Stations
SET StationCode = CONCAT('ST', RIGHT('000000' + CAST(Id AS VARCHAR(6)), 6))
WHERE StationCode IS NULL OR StationCode = '';

-- Verify the update
SELECT Id, StationCode, Name 
FROM Stations 
ORDER BY Id;
