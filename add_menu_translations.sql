-- Add missing menu translations for Vietnamese
INSERT INTO [Translations] ([Id], [Key], [Value], [LanguageCode], [Category], [CreatedAt])
VALUES
    (NEWID(), 'menu.management', N'Quản lý', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.my_station', N'Trạm của tôi', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.motion_monitoring', N'Giám sát Chuyển động', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.schedules', N'Cấu hình Lịch', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.personnel', N'Nhân sự', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.employees', N'Nhân viên', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.system', N'Hệ thống', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.languages', N'Ngôn ngữ', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.translations', N'Bản dịch', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.profile', N'Hồ sơ', 'vi', 'menu', GETDATE()),
    (NEWID(), 'menu.logout', N'Đăng xuất', 'vi', 'menu', GETDATE());

-- Add missing menu translations for English
INSERT INTO [Translations] ([Id], [Key], [Value], [LanguageCode], [Category], [CreatedAt])
VALUES
    (NEWID(), 'menu.management', 'Management', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.my_station', 'My Station', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.motion_monitoring', 'Motion Monitoring', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.schedules', 'Schedules', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.personnel', 'Personnel', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.employees', 'Employees', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.system', 'System', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.languages', 'Languages', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.translations', 'Translations', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.profile', 'Profile', 'en', 'menu', GETDATE()),
    (NEWID(), 'menu.logout', 'Logout', 'en', 'menu', GETDATE());

-- Add missing station column translations
INSERT INTO [Translations] ([Id], [Key], [Value], [LanguageCode], [Category], [CreatedAt])
VALUES
    (NEWID(), 'station.description_column', N'Mô tả', 'vi', 'station', GETDATE()),
    (NEWID(), 'station.status_column', N'Trạng thái', 'vi', 'station', GETDATE()),
    (NEWID(), 'station.created_column', N'Ngày tạo', 'vi', 'station', GETDATE()),
    (NEWID(), 'station.modified_column', N'Ngày cập nhật', 'vi', 'station', GETDATE());

INSERT INTO [Translations] ([Id], [Key], [Value], [LanguageCode], [Category], [CreatedAt])
VALUES
    (NEWID(), 'station.description_column', 'Description', 'en', 'station', GETDATE()),
    (NEWID(), 'station.status_column', 'Status', 'en', 'station', GETDATE()),
    (NEWID(), 'station.created_column', 'Created At', 'en', 'station', GETDATE()),
    (NEWID(), 'station.modified_column', 'Modified At', 'en', 'station', GETDATE());
