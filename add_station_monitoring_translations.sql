-- Add translations for station monitoring status column

-- Vietnamese translations
INSERT INTO [dbo].[Translations] ([LanguageId], [TranslationKey], [TranslationValue])
SELECT 
    (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'vi'),
    'station.monitoring_status_column',
    N'Trạng thái giám sát'
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[Translations] 
    WHERE LanguageId = (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'vi')
    AND TranslationKey = 'station.monitoring_status_column'
);

INSERT INTO [dbo].[Translations] ([LanguageId], [TranslationKey], [TranslationValue])
SELECT 
    (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'vi'),
    'station.monitoring_active',
    N'Đang giám sát'
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[Translations] 
    WHERE LanguageId = (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'vi')
    AND TranslationKey = 'station.monitoring_active'
);

INSERT INTO [dbo].[Translations] ([LanguageId], [TranslationKey], [TranslationValue])
SELECT 
    (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'vi'),
    'station.monitoring_inactive',
    N'Tạm dừng'
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[Translations] 
    WHERE LanguageId = (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'vi')
    AND TranslationKey = 'station.monitoring_inactive'
);

-- English translations
INSERT INTO [dbo].[Translations] ([LanguageId], [TranslationKey], [TranslationValue])
SELECT 
    (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'en'),
    'station.monitoring_status_column',
    'Monitoring Status'
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[Translations] 
    WHERE LanguageId = (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'en')
    AND TranslationKey = 'station.monitoring_status_column'
);

INSERT INTO [dbo].[Translations] ([LanguageId], [TranslationKey], [TranslationValue])
SELECT 
    (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'en'),
    'station.monitoring_active',
    'Active'
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[Translations] 
    WHERE LanguageId = (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'en')
    AND TranslationKey = 'station.monitoring_active'
);

INSERT INTO [dbo].[Translations] ([LanguageId], [TranslationKey], [TranslationValue])
SELECT 
    (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'en'),
    'station.monitoring_inactive',
    'Paused'
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[Translations] 
    WHERE LanguageId = (SELECT TOP 1 Id FROM [dbo].[Languages] WHERE Code = 'en')
    AND TranslationKey = 'station.monitoring_inactive'
);
