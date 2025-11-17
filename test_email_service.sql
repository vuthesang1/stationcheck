-- Test Email Parsing Service
-- This script tests the EmailService by inserting sample email data

USE StationCheckDb;
GO

-- 1. Update existing stations with StationCode
UPDATE Stations 
SET StationCode = 'ST' + RIGHT('000000' + CAST(Id AS VARCHAR(6)), 6)
WHERE StationCode = '' OR StationCode IS NULL;

-- Verify stations
SELECT Id, StationCode, Name FROM Stations ORDER BY Id;
GO

-- 2. Create a test email event manually (simulate what email parser would create)
DECLARE @TestEmailBody NVARCHAR(MAX) = 
'Alarm Event: Motion Detection
Alarm Input Channel No.: 2
Alarm Input Channel Name: IPC
Alarm Start Time (D/M/Y H:M:S): 12/11/2025 16:03:57
Alarm Device Name: NVR-6C39
Alarm Name:
IP Address: 192.168.1.200
Alarm Details:
';

-- Insert test email event
INSERT INTO EmailEvents (
    StationCode,
    StationId,
    AlarmEvent,
    AlarmInputChannelNo,
    AlarmInputChannelName,
    AlarmStartTime,
    AlarmDeviceName,
    AlarmName,
    IpAddress,
    AlarmDetails,
    EmailSubject,
    EmailFrom,
    EmailReceivedAt,
    RawEmailBody,
    CreatedAt,
    IsProcessed
)
VALUES (
    'ST000001',  -- StationCode from subject
    1,           -- StationId (assuming station ID 1 exists)
    'Motion Detection',
    2,
    'IPC',
    '2025-11-12 16:03:57',
    'NVR-6C39',
    NULL,
    '192.168.1.200',
    NULL,
    'ST000001 - Alarm Notification',  -- Email subject containing station code
    'nvr@monitoring.local',
    GETUTCDATE(),
    @TestEmailBody,
    GETUTCDATE(),
    0  -- Not yet processed
);

-- Verify email events
SELECT 
    Id,
    StationCode,
    StationId,
    AlarmEvent,
    AlarmInputChannelNo,
    AlarmDeviceName,
    IpAddress,
    AlarmStartTime,
    EmailReceivedAt,
    IsProcessed
FROM EmailEvents
ORDER BY EmailReceivedAt DESC;
GO
