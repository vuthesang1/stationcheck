-- Check all timeframes for "Trạm Quan Trắc Sông Hồng"
SELECT 
    tf.Id,
    tf.Name,
    tf.StartTime,
    tf.EndTime,
    tf.FrequencyMinutes,
    tf.BufferMinutes,
    tf.DaysOfWeek,
    tf.IsEnabled,
    tf.CreatedAt,
    s.Name AS StationName
FROM TimeFrames tf
JOIN Stations s ON tf.StationId = s.Id
WHERE s.Name LIKE N'%Sông Hồng%' OR s.Name LIKE N'%Song Hong%'
ORDER BY tf.IsEnabled DESC, tf.Id;

-- Check if any enabled timeframes exist
SELECT 
    s.Name AS StationName,
    COUNT(*) AS TotalTimeFrames,
    SUM(CASE WHEN tf.IsEnabled = 1 THEN 1 ELSE 0 END) AS EnabledTimeFrames,
    SUM(CASE WHEN tf.IsEnabled = 0 THEN 1 ELSE 0 END) AS DisabledTimeFrames
FROM Stations s
LEFT JOIN TimeFrames tf ON s.Id = tf.StationId
WHERE s.Name LIKE N'%Sông Hồng%' OR s.Name LIKE N'%Song Hong%'
GROUP BY s.Name;
