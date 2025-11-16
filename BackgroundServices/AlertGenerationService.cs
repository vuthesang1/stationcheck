using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Models;
using StationCheck.Services;

namespace StationCheck.BackgroundServices
{
    /// <summary>
    /// Background service to generate motion alerts based on Station TimeFrame configuration
    /// Runs every 1 hour to check for stations without motion detection
    /// </summary>
    public class AlertGenerationService : BackgroundService
    {
        private readonly ILogger<AlertGenerationService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private TimeSpan _checkInterval = TimeSpan.FromHours(1); // Default: 1 hour

        public AlertGenerationService(
            ILogger<AlertGenerationService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[AlertGeneration] Service started");

            // Load interval from database
            await LoadIntervalAsync();

            // Wait a bit before starting to ensure app is fully initialized
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await GenerateAlertsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[AlertGeneration] Error during alert generation cycle");
                }

                // Reload interval from DB every cycle (in case admin changed it)
                await LoadIntervalAsync();

                // Wait for next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("[AlertGeneration] Service stopped");
        }

        private async Task LoadIntervalAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                using var db = await context.CreateDbContextAsync();
                
                var config = await db.SystemConfigurations
                    .FirstOrDefaultAsync(c => c.Key == "AlertGenerationInterval");
                
                if (config != null && int.TryParse(config.Value, out int seconds))
                {
                    _checkInterval = TimeSpan.FromSeconds(seconds);
                    _logger.LogInformation(
                        "[AlertGeneration] Loaded interval from config: {Interval} seconds",
                        _checkInterval.TotalSeconds
                    );
                }
                else
                {
                    _logger.LogWarning(
                        "[AlertGeneration] Using default interval: {Interval} seconds",
                        _checkInterval.TotalSeconds
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AlertGeneration] Error loading interval config, using default");
            }
        }

        private async Task GenerateAlertsAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            _logger.LogInformation("[AlertGeneration] Running check at {Time}", now);

            using var scope = _serviceProvider.CreateScope();
            var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            try
            {
                // 1. Lấy danh sách trạm có IsActive = 1
                var activeStations = await context.Stations
                    .Where(s => s.IsActive)
                    .Include(s => s.TimeFrames)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("[AlertGeneration] Found {Count} active stations", activeStations.Count);

                int alertsGenerated = 0;
                int alertsResolved = 0;

                foreach (var station in activeStations)
                {
                    try
                    {
                        // 2. Tìm khung cấu hình khớp với giờ hiện tại
                        var matchingTimeFrames = await GetMatchingTimeFramesAsync(
                            context, 
                            station.Id, 
                            now, 
                            cancellationToken
                        );

                        if (!matchingTimeFrames.Any())
                        {
                            _logger.LogDebug(
                                "[AlertGeneration] Station {StationId} has no matching timeframes for current time",
                                station.Id
                            );
                            continue;
                        }

                        // Use the first matching timeframe
                        var timeFrame = matchingTimeFrames.First();

                        // 3. Check if monitoring is enabled and alert condition is met
                        if (await ShouldGenerateAlertAsync(context, station, timeFrame, now, cancellationToken))
                        {
                            await CreateAlertAsync(context, station, timeFrame, now, cancellationToken);
                            alertsGenerated++;
                        }

                        // 4. Check if we should auto-resolve existing alerts
                        if (station.LastMotionDetectedAt.HasValue)
                        {
                            var resolved = await AutoResolveAlertsAsync(
                                context, 
                                station, 
                                now, 
                                cancellationToken
                            );
                            alertsResolved += resolved;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex, 
                            "[AlertGeneration] Error processing station {StationId}",
                            station.Id
                        );
                    }
                }

                _logger.LogInformation(
                    "[AlertGeneration] Check completed. Alerts generated: {Generated}, Alerts resolved: {Resolved}",
                    alertsGenerated,
                    alertsResolved
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AlertGeneration] Error in alert generation");
            }
        }

        /// <summary>
        /// Find TimeFrames that match current time
        /// </summary>
        private async Task<List<TimeFrame>> GetMatchingTimeFramesAsync(
            ApplicationDbContext context,
            Guid stationId,
            DateTime now,
            CancellationToken cancellationToken)
        {
            var currentTime = now.TimeOfDay;
            var currentDayOfWeek = ((int)now.DayOfWeek == 0 ? 7 : (int)now.DayOfWeek).ToString(); // 1=Monday, 7=Sunday

            var timeFrames = await context.TimeFrames
                .Where(tf => tf.StationId == stationId && tf.IsEnabled)
                .ToListAsync(cancellationToken);

            return timeFrames
                .Where(tf => 
                    // Check if current time is within timeframe
                    currentTime >= tf.StartTime && currentTime <= tf.EndTime &&
                    // Check if current day is included
                    (string.IsNullOrEmpty(tf.DaysOfWeek) || tf.DaysOfWeek.Contains(currentDayOfWeek))
                )
                .ToList();
        }

        /// <summary>
        /// Check if alert should be generated for this station
        /// Checks if there's any motion within the tolerance window (frequency + buffer)
        /// </summary>
        private async Task<bool> ShouldGenerateAlertAsync(
            ApplicationDbContext context,
            Station station,
            TimeFrame timeFrame,
            DateTime now,
            CancellationToken cancellationToken)
        {
            // ✅ Calculate the tolerance window
            // If frequency is 60 min and buffer is 15 min:
            // - Check if there's any motion in the last 75 minutes
            // - If yes, no alert needed
            var toleranceMinutes = timeFrame.FrequencyMinutes + timeFrame.BufferMinutes;
            var windowStart = now.AddMinutes(-toleranceMinutes);

            // Check if there's any motion event in the tolerance window
            var hasRecentMotion = await context.MotionEvents
                .Where(me => me.StationId == station.Id && me.DetectedAt >= windowStart)
                .AnyAsync(cancellationToken);

            if (hasRecentMotion)
            {
                _logger.LogDebug(
                    "[AlertGeneration] Station {StationId} has motion within tolerance window. Window: {WindowStart} to {Now}, Frequency: {Frequency}, Buffer: {Buffer}",
                    station.Id,
                    windowStart.ToString("yyyy-MM-dd HH:mm:ss"),
                    now.ToString("yyyy-MM-dd HH:mm:ss"),
                    timeFrame.FrequencyMinutes,
                    timeFrame.BufferMinutes
                );
                return false;
            }

            // No motion in tolerance window - generate alert
            var lastMotion = station.LastMotionDetectedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never";
            var minutesSinceLastMotion = station.LastMotionDetectedAt.HasValue
                ? (now - station.LastMotionDetectedAt.Value).TotalMinutes
                : double.MaxValue;

            _logger.LogInformation(
                "[AlertGeneration] Station {StationId} exceeds tolerance window. Last motion: {LastMotion}, Minutes: {Minutes:F0}, Tolerance: {Tolerance} min (Frequency: {Frequency} + Buffer: {Buffer})",
                station.Id,
                lastMotion,
                minutesSinceLastMotion,
                toleranceMinutes,
                timeFrame.FrequencyMinutes,
                timeFrame.BufferMinutes
            );

            return true;
        }

        /// <summary>
        /// Create a new motion alert with configuration snapshot
        /// Each check cycle creates a new alert if conditions are met (no duplicate prevention)
        /// </summary>
        private async Task CreateAlertAsync(
            ApplicationDbContext context,
            Station station,
            TimeFrame timeFrame,
            DateTime now,
            CancellationToken cancellationToken)
        {
            // ✅ REMOVED: No longer check for existing alerts
            // Each check cycle should create a new alert if conditions are met
            // This allows multiple alerts for the same timeframe (e.g., 9h, 10h, 11h)

            // ✅ Get latest TimeFrameHistory for audit trail
            var timeFrameHistory = await context.TimeFrameHistories
                .Where(h => h.TimeFrameId == timeFrame.Id)
                .OrderByDescending(h => h.Version)
                .FirstOrDefaultAsync(cancellationToken);

            // If no TimeFrameHistory exists, create one now
            if (timeFrameHistory == null && timeFrame.StationId.HasValue)
            {
                var tfSnapshot = new
                {
                    TimeFrameId = timeFrame.Id,
                    Name = timeFrame.Name,
                    StartTime = timeFrame.StartTime.ToString(),
                    EndTime = timeFrame.EndTime.ToString(),
                    FrequencyMinutes = timeFrame.FrequencyMinutes,
                    BufferMinutes = timeFrame.BufferMinutes,
                    DaysOfWeek = timeFrame.DaysOfWeek,
                    IsEnabled = timeFrame.IsEnabled
                };

                timeFrameHistory = new TimeFrameHistory
                {
                    TimeFrameId = timeFrame.Id,
                    StationId = timeFrame.StationId.Value,
                    Version = 1,
                    Action = "Create",
                    ConfigurationSnapshot = JsonSerializer.Serialize(tfSnapshot),
                    ChangeDescription = $"Auto-created history for TimeFrame '{timeFrame.Name}'",
                    ChangedBy = "System (AlertGeneration)",
                    ChangedAt = DateTime.Now
                };

                context.TimeFrameHistories.Add(timeFrameHistory);
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "[AlertGeneration] Created TimeFrameHistory v1 for TimeFrame {TimeFrameId}",
                    timeFrame.Id
                );
            }

            // Create configuration snapshot for audit
            var configSnapshot = new
            {
                TimeFrameId = timeFrame.Id,
                TimeFrameName = timeFrame.Name,
                StartTime = timeFrame.StartTime.ToString(),
                EndTime = timeFrame.EndTime.ToString(),
                FrequencyMinutes = timeFrame.FrequencyMinutes,
                DaysOfWeek = timeFrame.DaysOfWeek,
                CheckedAt = now
            };

            var minutesSinceLastMotion = station.LastMotionDetectedAt.HasValue
                ? (int)(now - station.LastMotionDetectedAt.Value).TotalMinutes
                : int.MaxValue;

            // Get last motion details from most recent MotionEvent
            var lastMotionEvent = await context.MotionEvents
                .Where(me => me.StationId == station.Id)
                .OrderByDescending(me => me.DetectedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var alert = new MotionAlert
            {
                StationId = station.Id,
                StationName = station.Name,
                TimeFrameId = timeFrame.Id,
                ConfigurationSnapshot = JsonSerializer.Serialize(configSnapshot),
                AlertTime = now,
                Severity = minutesSinceLastMotion > timeFrame.FrequencyMinutes * 2 
                    ? AlertSeverity.Critical 
                    : AlertSeverity.Warning,
                Message = $"Không phát hiện chuyển động tại {station.Name} trong {minutesSinceLastMotion} phút (mong đợi: {timeFrame.FrequencyMinutes} phút)",
                ExpectedFrequencyMinutes = timeFrame.FrequencyMinutes,
                LastMotionAt = station.LastMotionDetectedAt,
                LastMotionCameraId = lastMotionEvent?.CameraId,
                LastMotionCameraName = lastMotionEvent?.CameraName,
                MinutesSinceLastMotion = minutesSinceLastMotion,
                IsResolved = false
            };

            context.MotionAlerts.Add(alert);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogWarning(
                "[AlertGeneration] Created alert {AlertId} for Station {StationId} ({StationName}). TimeFrameVersion: {TimeFrameVersion}, Last motion: {LastMotion}, Minutes since: {Minutes}",
                alert.Id,
                station.Id,
                station.Name,
                timeFrameHistory?.Version.ToString() ?? "N/A",
                station.LastMotionDetectedAt?.ToString() ?? "Never",
                minutesSinceLastMotion
            );
        }

        /// <summary>
        /// Auto-resolve active alerts when new motion is detected
        /// Only resolves alerts if motion falls within tolerance window (±BufferMinutes around AlertTime)
        /// </summary>
        private async Task<int> AutoResolveAlertsAsync(
            ApplicationDbContext context,
            Station station,
            DateTime now,
            CancellationToken cancellationToken)
        {
            if (!station.LastMotionDetectedAt.HasValue)
            {
                return 0;
            }

            // Find all active (unresolved) alerts for this station
            var activeAlerts = await context.MotionAlerts
                .Include(a => a.TimeFrame) // Need TimeFrame for BufferMinutes
                .Where(a => a.StationId == station.Id 
                         && !a.IsResolved
                         && a.TimeFrameId.HasValue)
                .ToListAsync(cancellationToken);

            if (!activeAlerts.Any())
            {
                return 0;
            }

            int resolvedCount = 0;
            var motionTime = station.LastMotionDetectedAt.Value;

            foreach (var alert in activeAlerts)
            {
                if (alert.TimeFrame == null) continue;

                // Calculate tolerance window: ± BufferMinutes around AlertTime
                var windowStart = alert.AlertTime.AddMinutes(-alert.TimeFrame.BufferMinutes);
                var windowEnd = alert.AlertTime.AddMinutes(alert.TimeFrame.BufferMinutes);

                // Check if motion falls within this alert's tolerance window
                if (motionTime >= windowStart && motionTime <= windowEnd)
                {
                    alert.IsResolved = true;
                    alert.ResolvedAt = now;
                    alert.ResolvedBy = "System (Auto-resolved by motion detection)";
                    alert.Notes = $"Motion detected at {motionTime:yyyy-MM-dd HH:mm:ss} within tolerance [{windowStart:HH:mm}-{windowEnd:HH:mm}]";

                    resolvedCount++;

                    _logger.LogInformation(
                        "[AlertGeneration] Resolved alert {AlertId} for Station {StationId} (AlertTime={AlertTime:HH:mm}, Tolerance=[{WindowStart:HH:mm}-{WindowEnd:HH:mm}], Motion={MotionTime:HH:mm})",
                        alert.Id,
                        station.Id,
                        alert.AlertTime,
                        windowStart,
                        windowEnd,
                        motionTime
                    );
                }
                else
                {
                    _logger.LogDebug(
                        "[AlertGeneration] Alert {AlertId} NOT resolved - Motion {MotionTime:HH:mm} outside tolerance [{WindowStart:HH:mm}-{WindowEnd:HH:mm}]",
                        alert.Id,
                        motionTime,
                        windowStart,
                        windowEnd
                    );
                }
            }

            if (resolvedCount > 0)
            {
                await context.SaveChangesAsync(cancellationToken);
            }

            return resolvedCount;
        }
    }
}
