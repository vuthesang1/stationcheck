-- ============================================================================
-- Test Script for New Email-Based Alert System
-- ============================================================================
-- Flow:
-- 1. EmailMonitoringService checks email every 5 minutes
-- 2. Parse email → Create MotionEvent → Update Station.LastMotionDetectedAt
-- 3. AlertGenerationService runs every 1 hour
-- 4. Check stations with TimeFrames → Generate alerts if no motion detected
-- 5. Auto-resolve alerts when new motion detected
-- ============================================================================

-- ============================================================================
-- SETUP: Create test Station with TimeFrame
-- ============================================================================

-- 1. Create test station
DECLARE @StationId INT = 3;
DECLARE @StationCode NVARCHAR(10) = 'ST000003';

IF NOT EXISTS (SELECT 1 FROM Stations WHERE Id = @StationId)
BEGIN
    SET IDENTITY_INSERT Stations ON;
    INSERT INTO Stations (Id, StationCode, Name, Address, IsActive, CreatedAt, LastMotionDetectedAt)
    VALUES (@StationId, @StationCode, N'Trạm Test Email Alert', N'Test Address', 1, GETDATE(), NULL);
    SET IDENTITY_INSERT Stations OFF;
    PRINT 'Created test station: ' + @StationCode;
END
ELSE
BEGIN
    UPDATE Stations 
    SET StationCode = @StationCode,
        IsActive = 1,
        LastMotionDetectedAt = NULL
    WHERE Id = @StationId;
    PRINT 'Updated test station: ' + @StationCode;
END

-- 2. Create TimeFrame for this station (8:00 AM - 6:00 PM, every 10 minutes)
IF NOT EXISTS (SELECT 1 FROM TimeFrames WHERE StationId = @StationId)
BEGIN
    INSERT INTO TimeFrames (Name, StationId, StartTime, EndTime, FrequencyMinutes, DaysOfWeek, IsEnabled, CreatedAt)
    VALUES (
        N'Test TimeFrame - Working Hours',
        @StationId,
        '08:00:00',  -- 8:00 AM
        '18:00:00',  -- 6:00 PM
        10,          -- Check every 10 minutes
        '1,2,3,4,5', -- Monday to Friday
        1,           -- Enabled
        GETDATE()
    );
    PRINT 'Created TimeFrame for Station ' + CAST(@StationId AS NVARCHAR);
END

GO

-- ============================================================================
-- TEST 1: Simulate Email → MotionEvent Creation
-- ============================================================================

PRINT '============================================================================';
PRINT 'TEST 1: Create MotionEvent from Email (simulating EmailService)';
PRINT '============================================================================';

DECLARE @TestStationId INT = 3;
DECLARE @TestStationCode NVARCHAR(10) = 'ST000003';

-- Simulate parsed email data
DECLARE @EmailPayload NVARCHAR(2000) = N'{
    "AlarmEvent": "Motion Detection",
    "AlarmInputChannelNo": 2,
    "AlarmInputChannelName": "IPC",
    "AlarmStartTime": "' + FORMAT(GETDATE(), 'yyyy-MM-dd HH:mm:ss') + '",
    "AlarmDeviceName": "NVR-6C39",
    "IpAddress": "192.168.1.200",
    "EmailSubject": "' + @TestStationCode + ' - Alarm Notification",
    "EmailFrom": "nvr@monitoring.local",
    "EmailReceivedAt": "' + FORMAT(GETDATE(), 'yyyy-MM-dd HH:mm:ss') + '"
}';

-- Insert MotionEvent
INSERT INTO MotionEvents (
    Id,
    StationId,
    EventType,
    EmailSubject,
    Payload,
    DetectedAt,
    CreatedAt,
    IsProcessed
)
VALUES (
    NEWID(),
    @TestStationId,
    'Motion',
    @TestStationCode + ' - Alarm Notification',
    @EmailPayload,
    GETDATE(),
    GETDATE(),
    0
);

-- Update Station.LastMotionDetectedAt (simulating EmailService.SaveMotionEventAsync)
UPDATE Stations
SET LastMotionDetectedAt = GETDATE()
WHERE Id = @TestStationId;

PRINT 'Created MotionEvent and updated LastMotionDetectedAt for Station ' + CAST(@TestStationId AS NVARCHAR);

-- Verify
SELECT 
    s.Id AS StationId,
    s.StationCode,
    s.Name AS StationName,
    s.LastMotionDetectedAt,
    DATEDIFF(MINUTE, s.LastMotionDetectedAt, GETDATE()) AS MinutesSinceLastMotion,
    COUNT(me.Id) AS TotalMotionEvents
FROM Stations s
LEFT JOIN MotionEvents me ON me.StationId = s.Id
WHERE s.Id = @TestStationId
GROUP BY s.Id, s.StationCode, s.Name, s.LastMotionDetectedAt;

GO

-- ============================================================================
-- TEST 2: Check TimeFrame Matching (simulating AlertGenerationService logic)
-- ============================================================================

PRINT '============================================================================';
PRINT 'TEST 2: Check TimeFrame Matching';
PRINT '============================================================================';

DECLARE @TestStationId2 INT = 3;
DECLARE @CurrentTime TIME = CAST(GETDATE() AS TIME);
DECLARE @CurrentDayOfWeek INT = CASE WHEN DATEPART(WEEKDAY, GETDATE()) = 1 THEN 7 ELSE DATEPART(WEEKDAY, GETDATE()) - 1 END;

SELECT 
    tf.Id AS TimeFrameId,
    tf.Name AS TimeFrameName,
    tf.StationId,
    s.StationCode,
    tf.StartTime,
    tf.EndTime,
    tf.FrequencyMinutes,
    @CurrentTime AS CurrentTime,
    @CurrentDayOfWeek AS CurrentDayOfWeek,
    tf.DaysOfWeek,
    CASE 
        WHEN @CurrentTime >= tf.StartTime AND @CurrentTime <= tf.EndTime 
             AND (tf.DaysOfWeek IS NULL OR tf.DaysOfWeek LIKE '%' + CAST(@CurrentDayOfWeek AS NVARCHAR) + '%')
        THEN 'MATCHED'
        ELSE 'NOT MATCHED'
    END AS MatchStatus,
    s.LastMotionDetectedAt,
    DATEDIFF(MINUTE, s.LastMotionDetectedAt, GETDATE()) AS MinutesSinceLastMotion,
    CASE 
        WHEN s.LastMotionDetectedAt IS NULL THEN 'SHOULD ALERT (No motion yet)'
        WHEN DATEDIFF(MINUTE, s.LastMotionDetectedAt, GETDATE()) > tf.FrequencyMinutes THEN 'SHOULD ALERT (Exceeded interval)'
        ELSE 'NO ALERT (Within interval)'
    END AS AlertDecision
FROM TimeFrames tf
INNER JOIN Stations s ON s.Id = tf.StationId
WHERE tf.StationId = @TestStationId2 AND tf.IsEnabled = 1;

GO

-- ============================================================================
-- TEST 3: Simulate Alert Generation (when no motion for too long)
-- ============================================================================

PRINT '============================================================================';
PRINT 'TEST 3: Generate Alert (simulating AlertGenerationService)';
PRINT '============================================================================';

DECLARE @TestStationId3 INT = 3;

-- Set LastMotionDetectedAt to 20 minutes ago (should trigger alert if frequency is 10 min)
UPDATE Stations
SET LastMotionDetectedAt = DATEADD(MINUTE, -20, GETDATE())
WHERE Id = @TestStationId3;

PRINT 'Set LastMotionDetectedAt to 20 minutes ago';

-- Get TimeFrame info
DECLARE @TimeFrameId INT;
DECLARE @FrequencyMinutes INT;
DECLARE @StationName NVARCHAR(200);

SELECT TOP 1
    @TimeFrameId = tf.Id,
    @FrequencyMinutes = tf.FrequencyMinutes,
    @StationName = s.Name
FROM TimeFrames tf
INNER JOIN Stations s ON s.Id = tf.StationId
WHERE tf.StationId = @TestStationId3 AND tf.IsEnabled = 1;

-- Create alert (simulating AlertGenerationService.CreateAlertAsync)
DECLARE @MinutesSinceLastMotion INT;
SELECT @MinutesSinceLastMotion = DATEDIFF(MINUTE, LastMotionDetectedAt, GETDATE())
FROM Stations WHERE Id = @TestStationId3;

DECLARE @ConfigSnapshot NVARCHAR(4000) = N'{
    "TimeFrameId": ' + CAST(@TimeFrameId AS NVARCHAR) + ',
    "TimeFrameName": "Test TimeFrame - Working Hours",
    "StartTime": "08:00:00",
    "EndTime": "18:00:00",
    "FrequencyMinutes": ' + CAST(@FrequencyMinutes AS NVARCHAR) + ',
    "DaysOfWeek": "1,2,3,4,5",
    "CheckedAt": "' + FORMAT(GETDATE(), 'yyyy-MM-dd HH:mm:ss') + '"
}';

INSERT INTO MotionAlerts (
    Id,
    StationId,
    StationName,
    TimeFrameId,
    ConfigurationSnapshot,
    AlertTime,
    Severity,
    Message,
    ExpectedFrequencyMinutes,
    LastMotionAt,
    MinutesSinceLastMotion,
    IsResolved
)
VALUES (
    NEWID(),
    @TestStationId3,
    @StationName,
    @TimeFrameId,
    @ConfigSnapshot,
    GETDATE(),
    1, -- Warning
    N'Không phát hiện chuyển động tại ' + @StationName + ' trong ' + CAST(@MinutesSinceLastMotion AS NVARCHAR) + ' phút (mong đợi: ' + CAST(@FrequencyMinutes AS NVARCHAR) + ' phút)',
    @FrequencyMinutes,
    (SELECT LastMotionDetectedAt FROM Stations WHERE Id = @TestStationId3),
    @MinutesSinceLastMotion,
    0 -- Not resolved
);

PRINT 'Created alert for Station ' + CAST(@TestStationId3 AS NVARCHAR);

-- View active alerts
SELECT 
    ma.Id AS AlertId,
    ma.StationId,
    ma.StationName,
    ma.AlertTime,
    ma.Severity,
    ma.Message,
    ma.ExpectedFrequencyMinutes,
    ma.MinutesSinceLastMotion,
    ma.LastMotionAt,
    ma.IsResolved,
    ma.ResolvedAt
FROM MotionAlerts ma
WHERE ma.StationId = @TestStationId3 AND ma.IsResolved = 0
ORDER BY ma.AlertTime DESC;

GO

-- ============================================================================
-- TEST 4: Auto-Resolve Alert (when new motion detected)
-- ============================================================================

PRINT '============================================================================';
PRINT 'TEST 4: Auto-Resolve Alert (simulating new motion detection)';
PRINT '============================================================================';

DECLARE @TestStationId4 INT = 3;

-- Simulate new motion event
DECLARE @NewEmailPayload NVARCHAR(2000) = N'{
    "AlarmEvent": "Motion Detection",
    "AlarmStartTime": "' + FORMAT(GETDATE(), 'yyyy-MM-dd HH:mm:ss') + '",
    "AlarmDeviceName": "NVR-6C39"
}';

INSERT INTO MotionEvents (
    Id,
    StationId,
    EventType,
    Payload,
    DetectedAt,
    CreatedAt,
    IsProcessed
)
VALUES (
    NEWID(),
    @TestStationId4,
    'Motion',
    @NewEmailPayload,
    GETDATE(),
    GETDATE(),
    0
);

-- Update LastMotionDetectedAt
UPDATE Stations
SET LastMotionDetectedAt = GETDATE()
WHERE Id = @TestStationId4;

PRINT 'Created new MotionEvent and updated LastMotionDetectedAt to NOW';

-- Auto-resolve active alerts (simulating AlertGenerationService.AutoResolveAlertsAsync)
UPDATE MotionAlerts
SET 
    IsResolved = 1,
    ResolvedAt = GETDATE(),
    ResolvedBy = 'System (Auto-resolved by motion detection)',
    Notes = N'Phát hiện chuyển động lúc ' + FORMAT(GETDATE(), 'dd/MM/yyyy HH:mm:ss')
WHERE 
    StationId = @TestStationId4 
    AND IsResolved = 0
    AND AlertTime < (SELECT LastMotionDetectedAt FROM Stations WHERE Id = @TestStationId4);

PRINT 'Auto-resolved active alerts for Station ' + CAST(@TestStationId4 AS NVARCHAR);

-- View resolved alerts
SELECT 
    ma.Id AS AlertId,
    ma.StationId,
    ma.StationName,
    ma.AlertTime,
    ma.Message,
    ma.MinutesSinceLastMotion AS MinutesWhenAlerted,
    ma.IsResolved,
    ma.ResolvedAt,
    ma.ResolvedBy,
    ma.Notes,
    DATEDIFF(MINUTE, ma.AlertTime, ma.ResolvedAt) AS MinutesToResolve
FROM MotionAlerts ma
WHERE ma.StationId = @TestStationId4
ORDER BY ma.AlertTime DESC;

GO

-- ============================================================================
-- SUMMARY: View Complete Flow
-- ============================================================================

PRINT '============================================================================';
PRINT 'SUMMARY: Complete Test Results';
PRINT '============================================================================';

SELECT 
    s.Id AS StationId,
    s.StationCode,
    s.Name AS StationName,
    s.IsActive,
    s.LastMotionDetectedAt,
    DATEDIFF(MINUTE, s.LastMotionDetectedAt, GETDATE()) AS MinutesSinceLastMotion,
    
    -- TimeFrame info
    tf.Id AS TimeFrameId,
    tf.Name AS TimeFrameName,
    tf.FrequencyMinutes,
    tf.IsEnabled AS TimeFrameEnabled,
    
    -- Motion events count
    (SELECT COUNT(*) FROM MotionEvents me WHERE me.StationId = s.Id) AS TotalMotionEvents,
    
    -- Active alerts
    (SELECT COUNT(*) FROM MotionAlerts ma WHERE ma.StationId = s.Id AND ma.IsResolved = 0) AS ActiveAlerts,
    
    -- Resolved alerts
    (SELECT COUNT(*) FROM MotionAlerts ma WHERE ma.StationId = s.Id AND ma.IsResolved = 1) AS ResolvedAlerts
FROM Stations s
LEFT JOIN TimeFrames tf ON tf.StationId = s.Id AND tf.IsEnabled = 1
WHERE s.Id = 3;

-- Detail view of all alerts
SELECT 
    ma.Id AS AlertId,
    ma.AlertTime,
    ma.StationId,
    ma.StationName,
    ma.Message,
    ma.ExpectedFrequencyMinutes,
    ma.MinutesSinceLastMotion,
    ma.Severity,
    ma.IsResolved,
    ma.ResolvedAt,
    ma.ResolvedBy,
    CASE 
        WHEN ma.IsResolved = 1 THEN DATEDIFF(MINUTE, ma.AlertTime, ma.ResolvedAt)
        ELSE NULL
    END AS MinutesToResolve
FROM MotionAlerts ma
WHERE ma.StationId = 3
ORDER BY ma.AlertTime DESC;

GO

-- ============================================================================
-- CLEANUP (Optional)
-- ============================================================================

/*
-- Uncomment to cleanup test data:

DELETE FROM MotionAlerts WHERE StationId = 3;
DELETE FROM MotionEvents WHERE StationId = 3;
DELETE FROM TimeFrames WHERE StationId = 3;
DELETE FROM Stations WHERE Id = 3;

PRINT 'Test data cleaned up';
*/
