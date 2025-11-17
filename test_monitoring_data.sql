-- Test Data for Monitoring System
-- Run this to create sample monitoring configurations and test alert flow

USE StationCheckDb;
GO

-- 1. Create Monitoring Profile: "Giám Sát 24/7"
INSERT INTO MonitoringProfiles (Name, Description, IsActive, CreatedAt)
VALUES 
('Giám Sát 24/7', 'Profile giám sát cả ngày, kiểm tra mỗi 5 phút', 1, GETUTCDATE());

DECLARE @Profile247Id INT = SCOPE_IDENTITY();

-- 2. Create TimeFrames for Profile "24/7"
INSERT INTO TimeFrames (ProfileId, Name, StartTime, EndTime, FrequencyMinutes, DaysOfWeek, IsEnabled, CreatedAt)
VALUES
-- Ca sáng: 06:00 - 12:00, check mỗi 5 phút
(@Profile247Id, 'Ca Sáng', '06:00:00', '12:00:00', 5, '1,2,3,4,5,6,7', 1, GETUTCDATE()),
-- Ca chiều: 12:00 - 18:00, check mỗi 10 phút
(@Profile247Id, 'Ca Chiều', '12:00:00', '18:00:00', 10, '1,2,3,4,5,6,7', 1, GETUTCDATE()),
-- Ca tối: 18:00 - 22:00, check mỗi 15 phút
(@Profile247Id, 'Ca Tối', '18:00:00', '22:00:00', 15, '1,2,3,4,5,6,7', 1, GETUTCDATE());

-- 3. Create Monitoring Profile: "Giờ Hành Chính"
INSERT INTO MonitoringProfiles (Name, Description, IsActive, CreatedAt)
VALUES 
('Giờ Hành Chính', 'Profile giám sát giờ hành chính, thứ 2-6', 1, GETUTCDATE());

DECLARE @ProfileOfficeId INT = SCOPE_IDENTITY();

-- 4. Create TimeFrames for Profile "Giờ Hành Chính"
INSERT INTO TimeFrames (ProfileId, Name, StartTime, EndTime, FrequencyMinutes, DaysOfWeek, IsEnabled, CreatedAt)
VALUES
-- Sáng: 08:00 - 12:00, check mỗi 30 phút
(@ProfileOfficeId, 'Sáng', '08:00:00', '12:00:00', 30, '1,2,3,4,5', 1, GETUTCDATE()),
-- Chiều: 13:00 - 17:00, check mỗi 30 phút
(@ProfileOfficeId, 'Chiều', '13:00:00', '17:00:00', 30, '1,2,3,4,5', 1, GETUTCDATE());

-- 5. Link Station 1 to Profile "24/7"
INSERT INTO MonitoringConfigurations (Name, Description, IsEnabled, StationId, ProfileId, CreatedAt)
VALUES
('Config Trạm Sông Hồng', 'Cấu hình giám sát 24/7 cho Trạm Quan Trắc Sông Hồng', 1, 1, @Profile247Id, GETUTCDATE());

-- 6. Insert old MotionEvents (to simulate "no motion" scenario)
-- Last motion was 30 minutes ago
INSERT INTO MotionEvents (Id, CameraId, CameraName, StationId, EventType, DetectedAt, CreatedAt, IsProcessed)
VALUES
(NEWID(), 'CAM001', 'Camera Trạm Sông Hồng', 1, 'VideoMotion', DATEADD(MINUTE, -30, GETDATE()), GETUTCDATE(), 1),
(NEWID(), 'CAM001', 'Camera Trạm Sông Hồng', 1, 'VideoMotion', DATEADD(MINUTE, -35, GETDATE()), GETUTCDATE(), 1),
(NEWID(), 'CAM001', 'Camera Trạm Sông Hồng', 1, 'VideoMotion', DATEADD(MINUTE, -40, GETDATE()), GETUTCDATE(), 1);

-- 7. Show results
PRINT '=== Monitoring Profiles ===';
SELECT Id, Name, Description, IsActive FROM MonitoringProfiles WHERE Id IN (@Profile247Id, @ProfileOfficeId);

PRINT '=== TimeFrames ===';
SELECT Id, ProfileId, Name, StartTime, EndTime, FrequencyMinutes, DaysOfWeek, IsEnabled 
FROM TimeFrames 
WHERE ProfileId IN (@Profile247Id, @ProfileOfficeId)
ORDER BY ProfileId, StartTime;

PRINT '=== Monitoring Configurations ===';
SELECT mc.Id, mc.Name, mc.IsEnabled, mc.StationId, s.Name as StationName, mc.ProfileId, p.Name as ProfileName
FROM MonitoringConfigurations mc
LEFT JOIN Stations s ON mc.StationId = s.Id
LEFT JOIN MonitoringProfiles p ON mc.ProfileId = p.Id;

PRINT '=== Recent Motion Events ===';
SELECT TOP 5 Id, CameraId, CameraName, StationId, DetectedAt, DATEDIFF(MINUTE, DetectedAt, GETDATE()) as MinutesAgo
FROM MotionEvents 
ORDER BY DetectedAt DESC;

PRINT '=== Current Alerts (should be empty initially) ===';
SELECT Id, StationName, AlertTime, Severity, Message, IsResolved, MinutesSinceLastMotion
FROM MotionAlerts 
WHERE IsResolved = 0;

GO
