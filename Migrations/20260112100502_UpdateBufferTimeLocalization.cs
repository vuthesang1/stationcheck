using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <summary>
    /// Migration to update buffer time localization strings
    /// Changes buffer time logic from ±BufferMinutes to +BufferMinutes only
    /// Example: Checkpoint 10:00, Buffer 30min → Valid range 10:00-10:30 (not 09:30-10:30)
    /// </summary>
    public partial class UpdateBufferTimeLocalization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing buffer hint to reflect new logic (checkpoint to checkpoint + buffer)
            migrationBuilder.Sql(@"
                UPDATE [Translations]
                SET [Value] = N'Dung sai cho check-in trễ (sau checkpoint). VD: Buffer 15 phút với tần suất 60 phút → Chấp nhận từ 10:00 đến 10:15 cho khung 10:00',
                    [UpdatedAt] = GETUTCDATE()
                WHERE [Key] = 'timeframe.buffer_hint' AND [LanguageCode] = 'vi';

                UPDATE [Translations]
                SET [Value] = 'Tolerance for late check-in (after checkpoint). Example: 15-minute buffer with 60-minute frequency → Accept from 10:00 to 10:15 for 10:00 frame',
                    [UpdatedAt] = GETUTCDATE()
                WHERE [Key] = 'timeframe.buffer_hint' AND [LanguageCode] = 'en';
            ");

            // Insert new localization keys for buffer time
            migrationBuilder.Sql(@"
                -- Alert buffer label
                INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                VALUES 
                    (NEWID(), 'vi', 'alert.buffer_label', N'Buffer', 'label', GETUTCDATE()),
                    (NEWID(), 'en', 'alert.buffer_label', 'Buffer', 'label', GETUTCDATE());

                -- Minutes unit
                INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                VALUES 
                    (NEWID(), 'vi', 'timeframe.minutes_unit', N'phút', 'label', GETUTCDATE()),
                    (NEWID(), 'en', 'timeframe.minutes_unit', 'min', 'label', GETUTCDATE());

                -- Buffer explanation
                INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                VALUES 
                    (NEWID(), 'vi', 'timeframe.buffer_explanation', N'Buffer là khoảng dung sai cho phép đến trễ (sau checkpoint).', 'message', GETUTCDATE()),
                    (NEWID(), 'en', 'timeframe.buffer_explanation', 'Buffer is the tolerance for late check-ins (after checkpoint).', 'message', GETUTCDATE());

                -- Example 1 (updated to show checkpoint to checkpoint + buffer)
                INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                VALUES 
                    (NEWID(), 'vi', 'timeframe.buffer_example1', N'VD 1: Tần suất 1 giờ, Buffer 15 phút → Checkpoint 10:00 sẽ chấp nhận đến từ 10:00 đến 10:15', 'message', GETUTCDATE()),
                    (NEWID(), 'en', 'timeframe.buffer_example1', 'Example 1: Frequency 1 hour, Buffer 15 min → Checkpoint 10:00 accepts from 10:00 to 10:15', 'message', GETUTCDATE());

                -- Example 2 (updated to show checkpoint to checkpoint + buffer)
                INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                VALUES 
                    (NEWID(), 'vi', 'timeframe.buffer_example2', N'VD 2: Tần suất 2 giờ, Buffer 30 phút → Checkpoint 12:00 sẽ chấp nhận đến từ 12:00 đến 12:30', 'message', GETUTCDATE()),
                    (NEWID(), 'en', 'timeframe.buffer_example2', 'Example 2: Frequency 2 hours, Buffer 30 min → Checkpoint 12:00 accepts from 12:00 to 12:30', 'message', GETUTCDATE());

                -- Optional label
                INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                VALUES 
                    (NEWID(), 'vi', 'timeframe.optional_label', N'(Tùy chọn)', 'label', GETUTCDATE()),
                    (NEWID(), 'en', 'timeframe.optional_label', '(Optional)', 'label', GETUTCDATE());

                -- Buffer display format (for UI components)
                INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                VALUES 
                    (NEWID(), 'vi', 'alert.buffer_display', N'Buffer: +{0} phút', 'format', GETUTCDATE()),
                    (NEWID(), 'en', 'alert.buffer_display', 'Buffer: +{0} min', 'format', GETUTCDATE());

                -- Tolerance window label
                INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                VALUES 
                    (NEWID(), 'vi', 'alert.tolerance_window', N'Cửa sổ chấp nhận (Tolerance Window)', 'label', GETUTCDATE()),
                    (NEWID(), 'en', 'alert.tolerance_window', 'Tolerance Window', 'label', GETUTCDATE());

                -- Tolerance window format (checkpoint to checkpoint + buffer)
                INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                VALUES 
                    (NEWID(), 'vi', 'alert.tolerance_window_format', N'+{0} phút sau checkpoint', 'format', GETUTCDATE()),
                    (NEWID(), 'en', 'alert.tolerance_window_format', '+{0} min after checkpoint', 'format', GETUTCDATE());
            ");

            // Add configuration key to track buffer time change date
            migrationBuilder.Sql(@"
                -- Store migration date for backward compatibility checks
                IF NOT EXISTS (SELECT 1 FROM [Translations] WHERE [Key] = 'system.buffer_time_change_date')
                BEGIN
                    INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt])
                    VALUES 
                        (NEWID(), 'vi', 'system.buffer_time_change_date', '2026-01-12T10:05:00Z', 'config', GETUTCDATE()),
                        (NEWID(), 'en', 'system.buffer_time_change_date', '2026-01-12T10:05:00Z', 'config', GETUTCDATE());
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert buffer hint to old logic (±BufferMinutes)
            migrationBuilder.Sql(@"
                UPDATE [Translations]
                SET [Value] = N'Dung sai cho check-in sớm/trễ. VD: Buffer 15 phút với tần suất 60 phút → Chấp nhận từ 09:45 đến 10:15 cho khung 10:00',
                    [UpdatedAt] = GETUTCDATE()
                WHERE [Key] = 'timeframe.buffer_hint' AND [LanguageCode] = 'vi';

                UPDATE [Translations]
                SET [Value] = 'Tolerance for early/late check-in. Example: 15-minute buffer with 60-minute frequency → Accept from 09:45 to 10:15 for 10:00 frame',
                    [UpdatedAt] = GETUTCDATE()
                WHERE [Key] = 'timeframe.buffer_hint' AND [LanguageCode] = 'en';
            ");

            // Remove new localization keys
            migrationBuilder.Sql(@"
                DELETE FROM [Translations] WHERE [Key] IN (
                    'alert.buffer_label',
                    'timeframe.minutes_unit',
                    'timeframe.buffer_explanation',
                    'timeframe.buffer_example1',
                    'timeframe.buffer_example2',
                    'timeframe.optional_label',
                    'alert.buffer_display',
                    'alert.tolerance_window',
                    'alert.tolerance_window_format',
                    'system.buffer_time_change_date'
                );
            ");
        }
    }
}
