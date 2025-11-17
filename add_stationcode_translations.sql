-- Add StationCode column translations
-- Run this in SQL Server Management Studio or Azure Data Studio

-- Vietnamese translations
INSERT INTO Translations ([Key], [Value], LanguageCode, Category, CreatedAt)
VALUES 
('station.code_column', N'Mã Trạm', 'vi', 'station', GETDATE()),
('station.code_label', N'Mã Trạm', 'vi', 'station', GETDATE()),
('station.code_readonly_note', N'Mã trạm tự động sinh ra và không thể chỉnh sửa', 'vi', 'station', GETDATE());

-- English translations
INSERT INTO Translations ([Key], [Value], LanguageCode, Category, CreatedAt)
VALUES 
('station.code_column', N'Station Code', 'en', 'station', GETDATE()),
('station.code_label', N'Station Code', 'en', 'station', GETDATE()),
('station.code_readonly_note', N'Station code is auto-generated and cannot be edited', 'en', 'station', GETDATE());

-- Verify
SELECT * FROM Translations WHERE [Key] LIKE 'station.code%' ORDER BY LanguageCode, [Key];
