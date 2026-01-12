-- Fix DeviceStatus based on IsApproved and IsRevoked
-- This should have been done by migration but let's ensure it's correct

-- Update DeviceStatus for all existing devices
UPDATE UserDevices
SET DeviceStatus = CASE 
    WHEN IsApproved = 1 AND IsRevoked = 0 THEN 1  -- Approved
    WHEN IsRevoked = 1 THEN 2                     -- Rejected  
    ELSE 0                                         -- PendingApproval
END
WHERE DeviceStatus != CASE 
    WHEN IsApproved = 1 AND IsRevoked = 0 THEN 1
    WHEN IsRevoked = 1 THEN 2
    ELSE 0
END;

-- Show affected rows
SELECT 
    Id,
    DeviceName,
    MacAddress,
    IsApproved,
    IsRevoked,
    DeviceStatus,
    CASE DeviceStatus
        WHEN 0 THEN 'PendingApproval'
        WHEN 1 THEN 'Approved'
        WHEN 2 THEN 'Rejected'
        ELSE 'Unknown'
    END AS StatusText
FROM UserDevices
WHERE IsDeleted = 0
ORDER BY CreatedAt DESC;
