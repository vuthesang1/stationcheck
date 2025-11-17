SELECT TOP 1 
    Id,
    AlertTime,
    StationId,
    StationName,
    CameraId,
    CameraName,
    LastMotionCameraId,
    LastMotionCameraName,
    MinutesSinceLastMotion,
    ExpectedFrequencyMinutes,
    Severity,
    IsResolved,
    Message,
    LEFT(ConfigurationSnapshot, 200) AS ConfigSnapshotPreview
FROM MotionAlerts 
WHERE IsResolved = 0 
ORDER BY AlertTime DESC;
