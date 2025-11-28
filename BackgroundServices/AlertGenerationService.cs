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
        private readonly ConfigurationChangeNotifier _notifier;
        private TimeSpan _checkInterval = TimeSpan.FromHours(1); // Default: 1 hour
        private CancellationTokenSource? _delayCts;

        public AlertGenerationService(
            ILogger<AlertGenerationService> logger,
            IServiceProvider serviceProvider,
            ConfigurationChangeNotifier notifier)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _notifier = notifier;
            
            // ✅ Subscribe to configuration changes
            _notifier.Subscribe("AlertGenerationInterval", OnConfigurationChanged);
        }

        private void OnConfigurationChanged()
        {
            _logger.LogInformation("[AlertGeneration] Configuration changed, reloading interval immediately...");
            // Cancel the current delay to trigger immediate reload
            _delayCts?.Cancel();
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

                // Wait for next check interval (can be cancelled by config change)
                try
                {
                    _delayCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    await Task.Delay(_checkInterval, _delayCts.Token);
                }
                catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
                {
                    // Config changed, reload immediately
                    _logger.LogInformation("[AlertGeneration] Delay cancelled due to configuration change");
                }
            }

            _logger.LogInformation("[AlertGeneration] Service stopped");
        }

        private async Task LoadIntervalAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<SystemConfigurationService>();
                
                var interval = await configService.GetTimeSpanValueAsync("AlertGenerationInterval");
                
                if (interval.HasValue)
                {
                    _checkInterval = interval.Value;
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
            // Use local time (UTC+7 for Vietnam) since TimeFrame is stored in local time
            var now = DateTime.Now;
            _logger.LogInformation("[AlertGeneration] Running check at {Time}", now);

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
                            _logger.LogInformation(
                                "[AlertGeneration] Station {StationId} ({StationName}) has no matching timeframes for current time {Time} (UTC+7: {LocalTime})",
                                station.Id,
                                station.Name,
                                now.ToString("HH:mm:ss"),
                                now.AddHours(7).ToString("HH:mm:ss")
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
            // Convert UTC to local time (UTC+7) for TimeFrame comparison
            var localNow = now.AddHours(7);
            var currentTime = localNow.TimeOfDay;
            var currentDayOfWeek = ((int)localNow.DayOfWeek == 0 ? 7 : (int)localNow.DayOfWeek).ToString(); // 1=Monday, 7=Sunday

            var timeFrames = await context.TimeFrames
                .Where(tf => tf.StationId == stationId && tf.IsEnabled)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "[AlertGeneration] Station {StationId}: Found {Count} enabled TimeFrames. Current time: {Time} (Local UTC+7), Day: {Day}",
                stationId,
                timeFrames.Count,
                currentTime.ToString(@"hh\:mm\:ss"),
                currentDayOfWeek
            );

            foreach (var tf in timeFrames)
            {
                var timeMatch = currentTime >= tf.StartTime && currentTime <= tf.EndTime;
                var dayMatch = string.IsNullOrEmpty(tf.DaysOfWeek) || tf.DaysOfWeek.Contains(currentDayOfWeek);
                
                _logger.LogInformation(
                    "[AlertGeneration]   TimeFrame '{Name}': Start={Start}, End={End}, Days={Days}, TimeMatch={TimeMatch}, DayMatch={DayMatch}",
                    tf.Name,
                    tf.StartTime.ToString(@"hh\:mm\:ss"),
                    tf.EndTime.ToString(@"hh\:mm\:ss"),
                    tf.DaysOfWeek ?? "All",
                    timeMatch,
                    dayMatch
                );
            }

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
            // ✅ STEP 1: Check if current time is AFTER a checkpoint (within tolerance window)
            // Example: Checkpoint at 09:00, Job runs at 09:01 → Check ✓
            //          Checkpoint at 09:00, Job runs at 08:59 → Skip (too early)
            //          Checkpoint at 09:00, Job runs at 09:04 → Skip (too late, already checked)
            
            // Convert to local time for TimeFrame comparison (TimeSpan is stored as local time)
            var localNow = now.AddHours(7);
            var currentTime = localNow.TimeOfDay;
            var startTime = timeFrame.StartTime;
            
            // Calculate elapsed time since start of timeframe
            var elapsed = currentTime - startTime;
            
            // Tolerance: Check within 3 minutes AFTER checkpoint (not before)
            const double toleranceMinutes = 3.0;
            
            if (elapsed.TotalMinutes < 0)
            {
                _logger.LogDebug(
                    "[AlertGeneration] Station {StationId} - Before timeframe start (Current={Current}, Start={Start})",
                    station.Id,
                    currentTime.ToString(@"hh\:mm\:ss"),
                    startTime.ToString(@"hh\:mm\:ss")
                );
                return false;
            }
            
            // Calculate distance from last checkpoint
            // Example: elapsed=61min, freq=60min → remainder=1min (1 minute AFTER checkpoint)
            //          elapsed=59min, freq=60min → remainder=59min (1 minute BEFORE next checkpoint)
            var remainder = elapsed.TotalMinutes % timeFrame.FrequencyMinutes;
            
            // Check if we're within tolerance window AFTER a checkpoint
            // remainder 0-3 = within 0-3 minutes after checkpoint ✓
            // remainder > 3 = too late, already processed ✗
            if (remainder > toleranceMinutes)
            {
                _logger.LogDebug(
                    "[AlertGeneration] Station {StationId} - NOT in checkpoint window. Current={Current}, MinutesAfterCheckpoint={Minutes:F1}min, Tolerance={Tolerance}min (Elapsed={Elapsed:F1}min, Freq={Freq}min)",
                    station.Id,
                    currentTime.ToString(@"hh\:mm\:ss"),
                    remainder,
                    toleranceMinutes,
                    elapsed.TotalMinutes,
                    timeFrame.FrequencyMinutes
                );
                return false;
            }
            
            _logger.LogInformation(
                "[AlertGeneration] Station {StationId} - IN checkpoint window ✓ Current={Current}, MinutesAfterCheckpoint={Minutes:F1}min (Elapsed={Elapsed:F1}min, Freq={Freq}min, Start={Start})",
                station.Id,
                currentTime.ToString(@"hh\:mm\:ss"),
                remainder,
                elapsed.TotalMinutes,
                timeFrame.FrequencyMinutes,
                startTime.ToString(@"hh\:mm\:ss")
            );

            // ✅ STEP 2: Calculate the motion check window
            // If frequency is 60 min and buffer is 5 min:
            // - Check if there's any motion in the last 65 minutes (60+5)
            // - If yes, no alert needed
            var motionCheckWindowMinutes = timeFrame.FrequencyMinutes + timeFrame.BufferMinutes;
            var windowStart = now.AddMinutes(-motionCheckWindowMinutes);

            // Check if there's any motion event in the tolerance window
            var hasRecentMotion = await context.MotionEvents
                .Where(me => me.StationId == station.Id && me.DetectedAt >= windowStart)
                .AnyAsync(cancellationToken);

            if (hasRecentMotion)
            {
                _logger.LogInformation(
                    "[AlertGeneration] Station {StationId} has motion within check window ✓ Window: {WindowStart} to {Now} (UTC), Frequency: {Frequency}min, Buffer: {Buffer}min. Last motion: {LastMotion}",
                    station.Id,
                    windowStart.ToString("yyyy-MM-dd HH:mm:ss"),
                    now.ToString("yyyy-MM-dd HH:mm:ss"),
                    timeFrame.FrequencyMinutes,
                    timeFrame.BufferMinutes,
                    station.LastMotionDetectedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Unknown"
                );
                // Return true to create a resolved alert for station status calculation
                return true;
            }

            // No motion in tolerance window - generate alert
            var lastMotion = station.LastMotionDetectedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never";
            var minutesSinceLastMotion = station.LastMotionDetectedAt.HasValue
                ? (now - station.LastMotionDetectedAt.Value).TotalMinutes
                : double.MaxValue;

            _logger.LogWarning(
                "[AlertGeneration] ⚠️ Station {StationId} exceeds check window. Last motion: {LastMotion}, Minutes: {Minutes:F0}, Window: {Window} min (Frequency: {Frequency} + Buffer: {Buffer})",
                station.Id,
                lastMotion,
                minutesSinceLastMotion,
                motionCheckWindowMinutes,
                timeFrame.FrequencyMinutes,
                timeFrame.BufferMinutes
            );

            return true;
        }

        /// <summary>
        /// Create a new motion alert with configuration snapshot
        /// Creates alert for every check cycle to track station status:
        /// - If no motion in tolerance window → Alert unresolved (station needs attention)
        /// - If motion detected → Alert auto-resolved and deleted (station status = normal)
        /// </summary>
        private async Task CreateAlertAsync(
            ApplicationDbContext context,
            Station station,
            TimeFrame timeFrame,
            DateTime now,
            CancellationToken cancellationToken)
        {
            // ✅ STEP 1: Calculate the LAST PASSED checkpoint time (without seconds)
            // Example: if now is 19:26 and freq=3min, checkpoint is 19:24 (NOT 19:27)
            // Use Math.Floor to get the last checkpoint that has ALREADY occurred
            var localNow = now.AddHours(7);
            var currentTime = localNow.TimeOfDay;
            var elapsed = currentTime - timeFrame.StartTime;
            
            // Find the last checkpoint that has passed (use Floor, not Round)
            var checkpointsSinceStart = Math.Floor(elapsed.TotalMinutes / timeFrame.FrequencyMinutes);
            var checkpointTime = timeFrame.StartTime.Add(TimeSpan.FromMinutes(checkpointsSinceStart * timeFrame.FrequencyMinutes));
            
            // Convert checkpoint time to UTC DateTime (remove seconds)
            var checkpointDateTime = new DateTime(
                now.Year, now.Month, now.Day,
                checkpointTime.Hours, checkpointTime.Minutes, 0,
                DateTimeKind.Utc
            ).AddHours(-7); // Convert local back to UTC
            
            _logger.LogDebug(
                "[AlertGeneration] Station {StationId} - Checkpoint calculated: {Checkpoint}, Now: {Now}",
                station.Id,
                checkpointDateTime.AddHours(7).ToString("HH:mm:ss"),
                now.AddHours(7).ToString("HH:mm:ss")
            );
            
            // ✅ STEP 2: Check if alert already exists for this checkpoint
            // Look for alerts within ±1 minute of checkpoint time to handle race conditions
            var checkpointWindowStart = checkpointDateTime.AddMinutes(-1);
            var checkpointWindowEnd = checkpointDateTime.AddMinutes(1);
            
            var existingAlert = await context.MotionAlerts
                .Where(a => a.StationId == station.Id 
                         && a.TimeFrameId == timeFrame.Id
                         && a.AlertTime >= checkpointWindowStart
                         && a.AlertTime <= checkpointWindowEnd
                         && !a.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (existingAlert != null)
            {
                _logger.LogInformation(
                    "[AlertGeneration] Alert already exists for Station {StationId} at checkpoint {Checkpoint}. Skipping. AlertId={AlertId}, IsResolved={IsResolved}",
                    station.Id,
                    checkpointDateTime.AddHours(7).ToString("HH:mm"),
                    existingAlert.Id,
                    existingAlert.IsResolved
                );
                return; // Skip creating duplicate alert
            }
            
            // ✅ STEP 3: Check if there's motion in tolerance window for this checkpoint
            var checkToleranceMinutes = timeFrame.FrequencyMinutes + timeFrame.BufferMinutes;
            var checkWindowStart = now.AddMinutes(-checkToleranceMinutes);
            var hasRecentMotion = await context.MotionEvents
                .Where(me => me.StationId == station.Id && me.DetectedAt >= checkWindowStart)
                .AnyAsync(cancellationToken);

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
                    StationId = timeFrame.StationId,
                    Version = 1,
                    Action = "Create",
                    ConfigurationSnapshot = JsonSerializer.Serialize(tfSnapshot),
                    ChangeDescription = $"Auto-created history for TimeFrame '{timeFrame.Name}'",
                    ChangedBy = "System (AlertGeneration)",
                    ChangedAt = DateTime.UtcNow
                };

                context.TimeFrameHistories.Add(timeFrameHistory);
                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "[AlertGeneration] Created TimeFrameHistory v1 for TimeFrame {TimeFrameId}",
                    timeFrame.Id
                );
            }

            // ✅ Get ProfileHistory if TimeFrame is linked to a Profile
            MonitoringProfileHistory? profileHistory = null;
            #pragma warning disable CS0618 // Type or member is obsolete
            if (timeFrame.ProfileId.HasValue)
            {
                // COMMENTED OUT: MonitoringProfileHistories/MonitoringProfiles tables removed
                /* Get latest version of the profile
                profileHistory = await context.MonitoringProfileHistories
                    .Where(h => h.MonitoringProfileId == timeFrame.ProfileId.Value)
                    .OrderByDescending(h => h.Version)
                    .FirstOrDefaultAsync(cancellationToken);
                
                // If no history exists, create one for current profile state
                if (profileHistory == null)
                {
                    var profile = await context.MonitoringProfiles
                        .Include(p => p.TimeFrames)
                        .FirstOrDefaultAsync(p => p.Id == timeFrame.ProfileId.Value, cancellationToken);
                    
                    if (profile != null)
                    {
                        var profileSnapshot = new
                        {
                            ProfileId = profile.Id,
                            ProfileName = profile.Name,
                            Version = profile.Version,
                            TimeFrames = profile.TimeFrames.Select(tf => new
                            {
                                tf.Id,
                                tf.Name,
                                StartTime = tf.StartTime.ToString(),
                                EndTime = tf.EndTime.ToString(),
                                tf.FrequencyMinutes,
                                tf.DaysOfWeek
                            }).ToList()
                        };
                        
                        profileHistory = new MonitoringProfileHistory
                        {
                            MonitoringProfileId = profile.Id,
                            Version = profile.Version,
                            ProfileSnapshot = JsonSerializer.Serialize(profileSnapshot),
                            ModifiedBy = "System (Auto-created for alert)",
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        context.MonitoringProfileHistories.Add(profileHistory);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }
                */
            }
            #pragma warning restore CS0618

            // Create configuration snapshot for audit
            var configSnapshot = new
            {
                TimeFrameId = timeFrame.Id,
                TimeFrameName = timeFrame.Name,
                StartTime = timeFrame.StartTime.ToString(),
                EndTime = timeFrame.EndTime.ToString(),
                FrequencyMinutes = timeFrame.FrequencyMinutes,
                DaysOfWeek = timeFrame.DaysOfWeek,
                ProfileVersion = profileHistory?.Version,
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

            // ✅ LEGACY: Ensure CameraId is never NULL for database compatibility
            // Use shorter format: STN_{last8chars}_{cameraName} to avoid truncation
            string stationIdShort = station.Id.ToString().Substring(station.Id.ToString().Length - 8);
            string cameraId = $"STN_{stationIdShort}_MAIN";
            string cameraName = station.Name;
            
            if (lastMotionEvent != null)
            {
                cameraId = lastMotionEvent.CameraId ?? cameraId;
                cameraName = lastMotionEvent.CameraName ?? cameraName;
                _logger.LogDebug(
                    "[AlertGeneration] Found last motion event for Station {StationId}: CameraId={CameraId}, CameraName={CameraName}",
                    station.Id, cameraId, cameraName
                );
            }
            else
            {
                _logger.LogDebug(
                    "[AlertGeneration] No motion events found for Station {StationId}, using default CameraId={CameraId}",
                    station.Id, cameraId
                );
            }

            // Calculate tolerance window for display in message
            var toleranceMinutes = timeFrame.FrequencyMinutes + timeFrame.BufferMinutes;
            var windowStart = now.AddMinutes(-toleranceMinutes);
            var windowStartLocal = windowStart.AddHours(7).ToString("HH:mm");
            var checkpointLocal = checkpointDateTime.AddHours(7).ToString("HH:mm");
            var bufferFrom = now.AddHours(7).AddMinutes(-timeFrame.BufferMinutes).ToString("HH:mm");
            var bufferTo = now.AddHours(7).AddMinutes(timeFrame.BufferMinutes).ToString("HH:mm");

            // Calculate severity based on how long since last motion
            // - Warning: If just missed (within FrequencyMinutes to FrequencyMinutes * 1.5)
            // - Critical: If significantly overdue (> FrequencyMinutes * 1.5)
            var severityThreshold = timeFrame.FrequencyMinutes * 1.5;
            var severity = minutesSinceLastMotion > severityThreshold
                ? AlertSeverity.Critical
                : AlertSeverity.Warning;

            // ✅ STEP 4: Determine alert status and message
            // - If motion detected → Resolved (Online)
            // - If no motion → Unresolved (Offline at checkpoint time)
            var isResolved = hasRecentMotion;
            var message = hasRecentMotion
                ? $"Trạm {station.Name} Online lúc {checkpointLocal}"
                : $"Trạm {station.Name} Offline lúc {checkpointLocal}";

            var alert = new MotionAlert
            {
                StationId = station.Id,
                StationName = station.Name,
                TimeFrameId = timeFrame.Id,
                ProfileHistoryId = profileHistory?.Id,  // ✅ Link to profile version
                TimeFrameHistoryId = timeFrameHistory?.Id,  // ✅ Link to timeframe version
                ConfigurationSnapshot = JsonSerializer.Serialize(configSnapshot),
                AlertTime = checkpointDateTime,  // Use checkpoint time, not current time
                Severity = severity,
                Message = message,
                ExpectedFrequencyMinutes = timeFrame.FrequencyMinutes,
                LastMotionAt = station.LastMotionDetectedAt,
                LastMotionCameraId = lastMotionEvent?.CameraId,
                LastMotionCameraName = lastMotionEvent?.CameraName,
                MinutesSinceLastMotion = minutesSinceLastMotion,
                IsResolved = isResolved,
                ResolvedAt = isResolved ? now : null,
                ResolvedBy = isResolved ? "System" : null,
                IsDeleted = false, // Keep all alerts visible - use IsResolved for status tracking
                Notes = isResolved ? $"Motion detected within tolerance window at checkpoint {checkpointLocal}" : null
            };

            context.MotionAlerts.Add(alert);
            await context.SaveChangesAsync(cancellationToken);

            if (isResolved)
            {
                _logger.LogInformation(
                    "[AlertGeneration] Created RESOLVED alert {AlertId} for Station {StationId} ({StationName}). Motion detected at checkpoint. Last motion: {LastMotion}",
                    alert.Id,
                    station.Id,
                    station.Name,
                    station.LastMotionDetectedAt?.ToString() ?? "Unknown"
                );
            }
            else
            {
                _logger.LogWarning(
                    "[AlertGeneration] Created UNRESOLVED alert {AlertId} for Station {StationId} ({StationName}). ProfileVersion: {ProfileVersion}, TimeFrameVersion: {TimeFrameVersion}, Last motion: {LastMotion}, Minutes since: {Minutes}",
                    alert.Id,
                    station.Id,
                    station.Name,
                    profileHistory?.Version.ToString() ?? "N/A",
                    timeFrameHistory?.Version.ToString() ?? "N/A",
                    station.LastMotionDetectedAt?.ToString() ?? "Never",
                    minutesSinceLastMotion
                );
            }

            // ✅ Additional check: Auto-resolve if motion detected within buffer window of AlertTime
            // This handles cases where motion event arrived BEFORE alert was created
            // Example: Motion at 20:38:15, Alert created at 20:39:00 for window 20:38-20:40
            // Only check if alert is not already resolved
            if (!alert.IsResolved)
            {
                var alertWindowStart = checkpointDateTime.AddMinutes(-timeFrame.BufferMinutes);
                var alertWindowEnd = checkpointDateTime.AddMinutes(timeFrame.BufferMinutes);

                var recentMotion = await context.MotionEvents
                    .Where(me => me.StationId == station.Id 
                              && me.DetectedAt >= alertWindowStart 
                              && me.DetectedAt <= alertWindowEnd)
                    .OrderByDescending(me => me.DetectedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                if (recentMotion != null)
                {
                    alert.IsResolved = true;
                    alert.ResolvedAt = now;
                    alert.ResolvedBy = "System (Auto-resolved - motion detected in tolerance window)";
                    // Keep alert visible - IsDeleted stays false
                    
                    // Convert UTC times to Vietnam local time (UTC+7) for display
                    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    var motionLocalTime = TimeZoneInfo.ConvertTimeFromUtc(recentMotion.DetectedAt, vietnamTimeZone);
                    var alertWindowStartLocal = TimeZoneInfo.ConvertTimeFromUtc(alertWindowStart, vietnamTimeZone);
                    var alertWindowEndLocal = TimeZoneInfo.ConvertTimeFromUtc(alertWindowEnd, vietnamTimeZone);
                    
                    alert.Notes = $"Motion detected at {motionLocalTime:yyyy-MM-dd HH:mm:ss} within tolerance [{alertWindowStartLocal:HH:mm}-{alertWindowEndLocal:HH:mm}]";
                    alert.Message = $"Trạm {station.Name} Online lúc {checkpointLocal}"; // Update message to Online
                    
                    await context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation(
                        "[AlertGeneration] Auto-resolved alert {AlertId} immediately after creation. Motion at {MotionTime:HH:mm:ss} within tolerance [{WindowStart:HH:mm}-{WindowEnd:HH:mm}]",
                        alert.Id,
                        recentMotion.DetectedAt,
                        alertWindowStart,
                        alertWindowEnd
                    );
                }
            }
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
                    // Keep alert visible - IsDeleted stays false
                    
                    // Convert UTC times to Vietnam local time (UTC+7) for display
                    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    var motionLocalTime = TimeZoneInfo.ConvertTimeFromUtc(motionTime, vietnamTimeZone);
                    var windowStartLocal = TimeZoneInfo.ConvertTimeFromUtc(windowStart, vietnamTimeZone);
                    var windowEndLocal = TimeZoneInfo.ConvertTimeFromUtc(windowEnd, vietnamTimeZone);
                    var alertTimeLocal = alert.AlertTime.AddHours(7).ToString("HH:mm");
                    
                    alert.Notes = $"Motion detected at {motionLocalTime:yyyy-MM-dd HH:mm:ss} within tolerance [{windowStartLocal:HH:mm}-{windowEndLocal:HH:mm}]";
                    alert.Message = $"Trạm {alert.StationName} Online lúc {alertTimeLocal}"; // Update message to Online

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
