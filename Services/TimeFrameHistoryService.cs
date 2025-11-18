using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Models;

namespace StationCheck.Services
{
    /// <summary>
    /// Service để quản lý TimeFrame history và audit log
    /// </summary>
    public class TimeFrameHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TimeFrameHistoryService> _logger;

        public TimeFrameHistoryService(
            ApplicationDbContext context,
            ILogger<TimeFrameHistoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tạo history record khi create TimeFrame
        /// </summary>
        public async Task<TimeFrameHistory> LogCreateAsync(
            TimeFrame timeFrame,
            string changedBy,
            string? changeDescription = null)
        {
            var version = await GetNextVersionAsync(timeFrame.Id);
            var snapshot = SerializeTimeFrame(timeFrame);

            var history = new TimeFrameHistory
            {
                TimeFrameId = timeFrame.Id,
                StationId = timeFrame.StationId ?? throw new InvalidOperationException("TimeFrame must have StationId"),
                Version = version,
                Action = "Create",
                ConfigurationSnapshot = snapshot,
                ChangeDescription = changeDescription ?? $"Created TimeFrame '{timeFrame.Name}'",
                ChangedBy = changedBy,
                ChangedAt = DateTime.UtcNow
            };

            _context.TimeFrameHistories.Add(history);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "[TimeFrameHistory] Created history {HistoryId} for TimeFrame {TimeFrameId} (Version {Version})",
                history.Id,
                timeFrame.Id,
                version
            );

            return history;
        }

        /// <summary>
        /// Tạo history record khi update TimeFrame
        /// </summary>
        public async Task<TimeFrameHistory> LogUpdateAsync(
            TimeFrame oldTimeFrame,
            TimeFrame newTimeFrame,
            string changedBy,
            string? changeDescription = null)
        {
            var version = await GetNextVersionAsync(newTimeFrame.Id);
            var snapshot = SerializeTimeFrame(newTimeFrame);

            // Generate change description if not provided
            if (string.IsNullOrEmpty(changeDescription))
            {
                changeDescription = GenerateChangeDescription(oldTimeFrame, newTimeFrame);
            }

            var history = new TimeFrameHistory
            {
                TimeFrameId = newTimeFrame.Id,
                StationId = newTimeFrame.StationId ?? throw new InvalidOperationException("TimeFrame must have StationId"),
                Version = version,
                Action = "Update",
                ConfigurationSnapshot = snapshot,
                ChangeDescription = changeDescription,
                ChangedBy = changedBy,
                ChangedAt = DateTime.UtcNow
            };

            _context.TimeFrameHistories.Add(history);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "[TimeFrameHistory] Updated history {HistoryId} for TimeFrame {TimeFrameId} (Version {Version}): {Changes}",
                history.Id,
                newTimeFrame.Id,
                version,
                changeDescription
            );

            return history;
        }

        /// <summary>
        /// Tạo history record khi delete TimeFrame
        /// </summary>
        public async Task<TimeFrameHistory> LogDeleteAsync(
            TimeFrame timeFrame,
            string changedBy,
            string? changeDescription = null)
        {
            var version = await GetNextVersionAsync(timeFrame.Id);
            var snapshot = SerializeTimeFrame(timeFrame);

            var history = new TimeFrameHistory
            {
                TimeFrameId = timeFrame.Id,
                StationId = timeFrame.StationId ?? throw new InvalidOperationException("TimeFrame must have StationId"),
                Version = version,
                Action = "Delete",
                ConfigurationSnapshot = snapshot,
                ChangeDescription = changeDescription ?? $"Deleted TimeFrame '{timeFrame.Name}'",
                ChangedBy = changedBy,
                ChangedAt = DateTime.UtcNow
            };

            _context.TimeFrameHistories.Add(history);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "[TimeFrameHistory] Deleted history {HistoryId} for TimeFrame {TimeFrameId} (Version {Version})",
                history.Id,
                timeFrame.Id,
                version
            );

            return history;
        }

        /// <summary>
        /// Get lịch sử thay đổi của TimeFrame
        /// </summary>
        public async Task<List<TimeFrameHistory>> GetHistoryAsync(Guid timeFrameId)
        {
            return await _context.TimeFrameHistories
                .Where(h => h.TimeFrameId == timeFrameId)
                .OrderByDescending(h => h.Version)
                .ToListAsync();
        }

        /// <summary>
        /// Get lịch sử thay đổi của Station
        /// </summary>
        public async Task<List<TimeFrameHistory>> GetStationHistoryAsync(Guid stationId)
        {
            return await _context.TimeFrameHistories
                .Where(h => h.StationId == stationId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get latest version của TimeFrame (để reference khi tạo alert)
        /// </summary>
        public async Task<TimeFrameHistory?> GetLatestVersionAsync(Guid timeFrameId)
        {
            return await _context.TimeFrameHistories
                .Where(h => h.TimeFrameId == timeFrameId)
                .OrderByDescending(h => h.Version)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Serialize TimeFrame thành JSON snapshot
        /// </summary>
        private string SerializeTimeFrame(TimeFrame timeFrame)
        {
            var snapshot = new
            {
                timeFrame.Id,
                timeFrame.Name,
                timeFrame.StationId,
                timeFrame.StartTime,
                timeFrame.EndTime,
                timeFrame.FrequencyMinutes,
                timeFrame.BufferMinutes,
                timeFrame.DaysOfWeek,
                timeFrame.IsEnabled,
                timeFrame.CreatedAt,
                timeFrame.ModifiedAt
            };

            return JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        /// <summary>
        /// Generate change description by comparing old and new values
        /// </summary>
        private string GenerateChangeDescription(TimeFrame oldTf, TimeFrame newTf)
        {
            var changes = new List<string>();

            if (oldTf.Name != newTf.Name)
                changes.Add($"Name: '{oldTf.Name}' → '{newTf.Name}'");

            if (oldTf.StartTime != newTf.StartTime)
                changes.Add($"StartTime: {oldTf.StartTime} → {newTf.StartTime}");

            if (oldTf.EndTime != newTf.EndTime)
                changes.Add($"EndTime: {oldTf.EndTime} → {newTf.EndTime}");

            if (oldTf.FrequencyMinutes != newTf.FrequencyMinutes)
                changes.Add($"Frequency: {oldTf.FrequencyMinutes} → {newTf.FrequencyMinutes} min");

            if (oldTf.BufferMinutes != newTf.BufferMinutes)
                changes.Add($"Buffer: {oldTf.BufferMinutes} → {newTf.BufferMinutes} min");

            if (oldTf.DaysOfWeek != newTf.DaysOfWeek)
                changes.Add($"DaysOfWeek: '{oldTf.DaysOfWeek}' → '{newTf.DaysOfWeek}'");

            if (oldTf.IsEnabled != newTf.IsEnabled)
                changes.Add($"IsEnabled: {oldTf.IsEnabled} → {newTf.IsEnabled}");

            return changes.Any()
                ? string.Join(", ", changes)
                : "No changes detected";
        }

        /// <summary>
        /// Get next version number for TimeFrame
        /// </summary>
        private async Task<int> GetNextVersionAsync(Guid timeFrameId)
        {
            var latestVersion = await _context.TimeFrameHistories
                .Where(h => h.TimeFrameId == timeFrameId)
                .MaxAsync(h => (int?)h.Version) ?? 0;

            return latestVersion + 1;
        }
    }
}
