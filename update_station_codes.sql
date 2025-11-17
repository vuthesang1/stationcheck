-- Update existing stations with StationCode
UPDATE Stations 
SET StationCode = 'ST' + RIGHT('000000' + CAST(Id AS VARCHAR(6)), 6)
WHERE StationCode = '' OR StationCode IS NULL;

-- Verify
SELECT Id, StationCode, Name FROM Stations ORDER BY Id;
