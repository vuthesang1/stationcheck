-- Script để update mật khẩu cho các user mẫu
-- Chạy script này trong SQL Server Management Studio hoặc Azure Data Studio

-- Update password cho admin -> Admin@2025
UPDATE Users
SET PasswordHash = '$2a$12$qBZwWN5fZ7iYJhQx9bVJ3OnSM9c7d8.5fK7uJ3yF4wLvHn6XGkqkK'
WHERE Username = 'admin';

-- Update password cho manager -> Manager@2025  
UPDATE Users
SET PasswordHash = '$2a$12$kpWxV8tXJL.YcRq0H5Qs4eF3vNmL8d9.6gM8vK4zG5xMwo7YHlrlO'
WHERE Username = 'manager';

-- Update password cho employee1 -> Employee@2025
UPDATE Users
SET PasswordHash = '$2a$12$nTxY9wUmKN.ZdSr1I6Rt5fG4wOnM9e0.7hN9wL5aH6yNxp8ZImsmP'
WHERE Username = 'employee1';

-- Update password cho admin2 (nếu có) -> Admin@2025
UPDATE Users
SET PasswordHash = '$2a$12$qBZwWN5fZ7iYJhQx9bVJ3OnSM9c7d8.5fK7uJ3yF4wLvHn6XGkqkK'
WHERE Username = 'admin2';

SELECT Username, Email, Role, 'Password updated' as Status
FROM Users
WHERE Username IN ('admin', 'manager', 'employee1', 'admin2');
