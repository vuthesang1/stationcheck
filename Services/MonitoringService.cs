using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Interfaces;
using StationCheck.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace StationCheck.Services
{
    public class MonitoringService : IMonitoringService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MonitoringService> _logger;
        private readonly TimeFrameHistoryService _historyService;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public MonitoringService(
            ApplicationDbContext context,
            ILogger<MonitoringService> logger,
            TimeFrameHistoryService historyService,
            AuthenticationStateProvider authStateProvider,
            IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _historyService = historyService;
            _authStateProvider = authStateProvider;
            _contextFactory = contextFactory;
        }

        // MonitoringConfiguration operations - COMMENTED OUT: Tables removed
        // TODO: Restore when MonitoringConfigurations/MonitoringProfiles tables added back
        /*
        public IQueryable<MonitoringConfiguration> GetMonitoringConfigurationsQueryable()
        {
            return _context.MonitoringConfigurations
                .Include(mc => mc.Station)
                .Include(mc => mc.Profile)
                .AsQueryable();
        }

        public async Task<MonitoringConfiguration?> GetMonitoringConfigurationByIdAsync(int id)
        {
            return await _context.MonitoringConfigurations
                .Include(mc => mc.Station)
                .Include(mc => mc.Profile)
                .ThenInclude(p => p.TimeFrames)
                .FirstOrDefaultAsync(mc => mc.Id == id);
        }

        public async Task<MonitoringConfiguration?> GetMonitoringConfigurationByStationIdAsync(int stationId)
        {
            return await _context.MonitoringConfigurations
                .Include(mc => mc.Station)
                .Include(mc => mc.Profile)
                .ThenInclude(p => p.TimeFrames)
                .FirstOrDefaultAsync(mc => mc.StationId == stationId);
        }

        public async Task<MonitoringConfiguration> CreateMonitoringConfigurationAsync(MonitoringConfiguration configuration)
        {
            configuration.CreatedAt = DateTime.UtcNow;
            _context.MonitoringConfigurations.Add(configuration);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created monitoring configuration: {configuration.Name} (ID: {configuration.Id})");
            return configuration;
        }

        public async Task UpdateMonitoringConfigurationAsync(int id, MonitoringConfiguration configuration)
        {
            var existing = await _context.MonitoringConfigurations.FindAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Monitoring configuration with ID {id} not found");
            }

            existing.Name = configuration.Name;
            existing.Description = configuration.Description;
            existing.IsEnabled = configuration.IsEnabled;
            existing.StationId = configuration.StationId;
            existing.ProfileId = configuration.ProfileId;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.ModifiedBy = configuration.ModifiedBy;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated monitoring configuration: {existing.Name} (ID: {id})");
        }

        public async Task DeleteMonitoringConfigurationAsync(int id)
        {
            var configuration = await _context.MonitoringConfigurations.FindAsync(id);
            if (configuration == null)
            {
                throw new KeyNotFoundException($"Monitoring configuration with ID {id} not found");
            }

            _context.MonitoringConfigurations.Remove(configuration);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Deleted monitoring configuration: {configuration.Name} (ID: {id})");
        }

        public async Task ToggleMonitoringAsync(int configurationId, bool isEnabled)
        {
            var configuration = await _context.MonitoringConfigurations.FindAsync(configurationId);
            if (configuration == null)
            {
                throw new KeyNotFoundException($"Monitoring configuration with ID {configurationId} not found");
            }

            configuration.IsEnabled = isEnabled;
            configuration.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Toggled monitoring configuration {configuration.Name}: {isEnabled}");
        }

        // MonitoringProfile operations
        public IQueryable<MonitoringProfile> GetMonitoringProfilesQueryable()
        {
            return _context.MonitoringProfiles
                .Include(mp => mp.TimeFrames)
                .AsQueryable();
        }

        public async Task<MonitoringProfile?> GetMonitoringProfileByIdAsync(int id)
        {
            return await _context.MonitoringProfiles
                .Include(mp => mp.TimeFrames)
                .FirstOrDefaultAsync(mp => mp.Id == id);
        }

        public async Task<MonitoringProfile> CreateMonitoringProfileAsync(MonitoringProfile profile)
        {
            profile.CreatedAt = DateTime.UtcNow;
            _context.MonitoringProfiles.Add(profile);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created monitoring profile: {profile.Name} (ID: {profile.Id})");
            return profile;
        }

        public async Task UpdateMonitoringProfileAsync(int id, MonitoringProfile profile)
        {
            var existing = await _context.MonitoringProfiles.FindAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Monitoring profile with ID {id} not found");
            }

            existing.Name = profile.Name;
            existing.Description = profile.Description;
            existing.IsActive = profile.IsActive;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.ModifiedBy = profile.ModifiedBy;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated monitoring profile: {existing.Name} (ID: {id})");
        }

        public async Task DeleteMonitoringProfileAsync(int id)
        {
            var profile = await _context.MonitoringProfiles.FindAsync(id);
            if (configuration == null)
            {
                throw new KeyNotFoundException($"Monitoring profile with ID {id} not found");
            }

            _context.MonitoringProfiles.Remove(profile);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Deleted monitoring profile: {profile.Name} (ID: {id})");
        }
        */

        // TimeFrame operations
        public IQueryable<TimeFrame> GetTimeFramesQueryable()
        {
            return _context.TimeFrames
                .Include(tf => tf.Profile)
                .AsQueryable();
        }

        public async Task<List<TimeFrame>> GetTimeFramesByProfileIdAsync(int profileId)
        {
            return await _context.TimeFrames
                .Where(tf => tf.ProfileId == profileId)
                .OrderBy(tf => tf.StartTime)
                .ToListAsync();
        }

        public async Task<TimeFrame?> GetTimeFrameByIdAsync(int id)
        {
            return await _context.TimeFrames
                .Include(tf => tf.Profile)
                .FirstOrDefaultAsync(tf => tf.Id == id);
        }

        public async Task<TimeFrame> CreateTimeFrameAsync(TimeFrame timeFrame)
        {
            timeFrame.CreatedAt = DateTime.UtcNow;
            _context.TimeFrames.Add(timeFrame);
            await _context.SaveChangesAsync();
            
            // ✅ Log history
            await _historyService.LogCreateAsync(timeFrame, "System", $"Created TimeFrame '{timeFrame.Name}'");
            
            // ✅ Log to ConfigurationAuditLog AFTER SaveChanges
            await CreateAuditLogAsync("TimeFrame", timeFrame.Id.ToString(), timeFrame.Name ?? "Unknown", "Create", null, timeFrame);
            
            _logger.LogInformation($"Created time frame: {timeFrame.Name} (ID: {timeFrame.Id})");
            return timeFrame;
        }

        public async Task UpdateTimeFrameAsync(int id, TimeFrame timeFrame)
        {
            var existing = await _context.TimeFrames.FindAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Time frame with ID {id} not found");
            }

            // ✅ Clone old value for history
            var oldTimeFrame = new TimeFrame
            {
                Id = existing.Id,
                Name = existing.Name,
                StationId = existing.StationId,
                StartTime = existing.StartTime,
                EndTime = existing.EndTime,
                FrequencyMinutes = existing.FrequencyMinutes,
                BufferMinutes = existing.BufferMinutes,
                DaysOfWeek = existing.DaysOfWeek,
                IsEnabled = existing.IsEnabled
            };

            existing.Name = timeFrame.Name;
            existing.StartTime = timeFrame.StartTime;
            existing.EndTime = timeFrame.EndTime;
            existing.FrequencyMinutes = timeFrame.FrequencyMinutes;
            existing.BufferMinutes = timeFrame.BufferMinutes;
            existing.DaysOfWeek = timeFrame.DaysOfWeek;
            existing.IsEnabled = timeFrame.IsEnabled;
            existing.ModifiedAt = DateTime.UtcNow;

            // ✅ Log history BEFORE SaveChanges to ensure it's captured in same transaction
            await _historyService.LogUpdateAsync(oldTimeFrame, existing, "System");
            
            await _context.SaveChangesAsync();
            
            // ✅ Log to ConfigurationAuditLog AFTER SaveChanges to avoid transaction conflicts
            await CreateAuditLogAsync("TimeFrame", existing.Id.ToString(), existing.Name ?? "Unknown", "Update", oldTimeFrame, existing);
            
            _logger.LogInformation($"Updated time frame: {existing.Name} (ID: {id})");
        }

        public async Task DeleteTimeFrameAsync(int id)
        {
            var timeFrame = await _context.TimeFrames.FindAsync(id);
            if (timeFrame == null)
            {
                throw new KeyNotFoundException($"Time frame with ID {id} not found");
            }

            // ✅ Log history before delete
            await _historyService.LogDeleteAsync(timeFrame, "System", $"Deleted TimeFrame '{timeFrame.Name}'");
            
            _context.TimeFrames.Remove(timeFrame);
            await _context.SaveChangesAsync();
            
            // ✅ Log to ConfigurationAuditLog AFTER SaveChanges
            await CreateAuditLogAsync("TimeFrame", timeFrame.Id.ToString(), timeFrame.Name ?? "Unknown", "Delete", timeFrame, null);
            
            _logger.LogInformation($"Deleted time frame: {timeFrame.Name} (ID: {id})");
        }

        public async Task ToggleTimeFrameAsync(int timeFrameId, bool isEnabled)
        {
            var timeFrame = await _context.TimeFrames.FindAsync(timeFrameId);
            if (timeFrame == null)
            {
                throw new KeyNotFoundException($"Time frame with ID {timeFrameId} not found");
            }

            timeFrame.IsEnabled = isEnabled;
            timeFrame.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Toggled time frame {timeFrame.Name}: {isEnabled}");
        }

        // ==================== STATION-BASED TIMEFRAME OPERATIONS ====================
        
        public async Task<List<TimeFrame>> GetTimeFramesByStationIdAsync(int stationId)
        {
            return await _context.TimeFrames
                .Where(tf => tf.StationId == stationId)
                .OrderBy(tf => tf.StartTime)
                .ToListAsync();
        }

        public async Task<TimeFrame> CreateTimeFrameForStationAsync(int stationId, TimeFrame timeFrame)
        {
            // Validate station exists
            var station = await _context.Stations.FindAsync(stationId);
            if (station == null)
            {
                throw new KeyNotFoundException($"Station with ID {stationId} not found");
            }

            // Validate EndTime > StartTime (no overnight support for now)
            if (timeFrame.EndTime <= timeFrame.StartTime)
            {
                throw new ArgumentException("End time must be greater than start time");
            }

            // Validate frequency range
            var timeSpan = timeFrame.EndTime - timeFrame.StartTime;
            var maxFrequencyMinutes = (int)timeSpan.TotalMinutes;
            
            if (timeFrame.FrequencyMinutes < 1)
            {
                throw new ArgumentException("Frequency must be at least 1 minute");
            }
            
            if (timeFrame.FrequencyMinutes > maxFrequencyMinutes)
            {
                throw new ArgumentException($"Frequency ({timeFrame.FrequencyMinutes} min) cannot exceed timeframe range ({maxFrequencyMinutes} min)");
            }

            // Set StationId
            timeFrame.StationId = stationId;
            timeFrame.CreatedAt = DateTime.UtcNow;
            
            _context.TimeFrames.Add(timeFrame);
            await _context.SaveChangesAsync();
            
            // ✅ Log to ConfigurationAuditLog AFTER SaveChanges
            await CreateAuditLogAsync("TimeFrame", timeFrame.Id.ToString(), timeFrame.Name ?? "Unknown", "Create", null, timeFrame);
            
            _logger.LogInformation($"Created TimeFrame '{timeFrame.Name}' for Station {station.Name} (ID: {stationId})");
            return timeFrame;
        }

        public async Task<TimeFrame> UpdateTimeFrameAsync(TimeFrame timeFrame)
        {
            var existing = await _context.TimeFrames.FindAsync(timeFrame.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"TimeFrame with ID {timeFrame.Id} not found");
            }

            // Validate EndTime > StartTime
            if (timeFrame.EndTime <= timeFrame.StartTime)
            {
                throw new ArgumentException("End time must be greater than start time");
            }

            // Validate frequency range
            var timeSpan = timeFrame.EndTime - timeFrame.StartTime;
            var maxFrequencyMinutes = (int)timeSpan.TotalMinutes;
            
            if (timeFrame.FrequencyMinutes < 1 || timeFrame.FrequencyMinutes > maxFrequencyMinutes)
            {
                throw new ArgumentException($"Frequency must be between 1 and {maxFrequencyMinutes} minutes");
            }

            // ✅ Clone old value for history
            var oldTimeFrame = new TimeFrame
            {
                Id = existing.Id,
                Name = existing.Name,
                StationId = existing.StationId,
                StartTime = existing.StartTime,
                EndTime = existing.EndTime,
                FrequencyMinutes = existing.FrequencyMinutes,
                BufferMinutes = existing.BufferMinutes,
                DaysOfWeek = existing.DaysOfWeek,
                IsEnabled = existing.IsEnabled
            };

            existing.Name = timeFrame.Name;
            existing.StartTime = timeFrame.StartTime;
            existing.EndTime = timeFrame.EndTime;
            existing.FrequencyMinutes = timeFrame.FrequencyMinutes;
            existing.BufferMinutes = timeFrame.BufferMinutes;  // ✅ Update buffer time
            existing.DaysOfWeek = timeFrame.DaysOfWeek;
            existing.IsEnabled = timeFrame.IsEnabled;
            existing.ModifiedAt = DateTime.UtcNow;

            // ✅ Log history BEFORE SaveChanges
            await _historyService.LogUpdateAsync(oldTimeFrame, existing, "System");

            await _context.SaveChangesAsync();
            
            // ✅ Log to ConfigurationAuditLog AFTER SaveChanges to avoid transaction conflicts
            await CreateAuditLogAsync("TimeFrame", existing.Id.ToString(), existing.Name ?? "Unknown", "Update", oldTimeFrame, existing);
            
            _logger.LogInformation($"Updated TimeFrame '{existing.Name}' (ID: {existing.Id})");
            return existing;
        }

        public async Task BulkToggleTimeFramesAsync(int stationId, bool isEnabled)
        {
            var timeFrames = await _context.TimeFrames
                .Where(tf => tf.StationId == stationId)
                .ToListAsync();

            if (!timeFrames.Any())
            {
                _logger.LogWarning($"No timeframes found for station {stationId}");
                return;
            }

            foreach (var tf in timeFrames)
            {
                tf.IsEnabled = isEnabled;
                tf.ModifiedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Bulk toggled {timeFrames.Count} timeframes for station {stationId}: {isEnabled}");
        }

        public async Task CopyTimeFramesAsync(int sourceStationId, int targetStationId)
        {
            // Validate both stations exist
            var sourceStation = await _context.Stations.FindAsync(sourceStationId);
            var targetStation = await _context.Stations.FindAsync(targetStationId);
            
            if (sourceStation == null)
            {
                throw new KeyNotFoundException($"Source station with ID {sourceStationId} not found");
            }
            
            if (targetStation == null)
            {
                throw new KeyNotFoundException($"Target station with ID {targetStationId} not found");
            }

            // Get source timeframes
            var sourceTimeFrames = await _context.TimeFrames
                .Where(tf => tf.StationId == sourceStationId)
                .ToListAsync();

            if (!sourceTimeFrames.Any())
            {
                throw new InvalidOperationException($"No timeframes found in source station '{sourceStation.Name}'");
            }

            // Copy each timeframe
            foreach (var sourceTf in sourceTimeFrames)
            {
                var newTf = new TimeFrame
                {
                    StationId = targetStationId,
                    Name = sourceTf.Name,
                    StartTime = sourceTf.StartTime,
                    EndTime = sourceTf.EndTime,
                    FrequencyMinutes = sourceTf.FrequencyMinutes,
                    DaysOfWeek = sourceTf.DaysOfWeek,
                    IsEnabled = sourceTf.IsEnabled,
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.TimeFrames.Add(newTf);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Copied {sourceTimeFrames.Count} timeframes from '{sourceStation.Name}' to '{targetStation.Name}'");
        }

        // Alert monitoring operations
        public TimeFrame? GetCurrentTimeFrame(MonitoringConfiguration config, DateTime currentTime, DayOfWeek currentDay)
        {
            if (config.Profile == null || config.Profile.TimeFrames == null || !config.Profile.TimeFrames.Any())
            {
                return null;
            }

            var currentTimeOfDay = currentTime.TimeOfDay;
            var currentDayInt = (int)currentDay; // 0=Sunday, 1=Monday, ..., 6=Saturday

            foreach (var timeFrame in config.Profile.TimeFrames.Where(tf => tf.IsEnabled))
            {
                // Check if current time is within time range
                bool isInTimeRange = false;
                if (timeFrame.StartTime <= timeFrame.EndTime)
                {
                    // Same day range (e.g., 08:00 - 17:00)
                    isInTimeRange = currentTimeOfDay >= timeFrame.StartTime && currentTimeOfDay <= timeFrame.EndTime;
                }
                else
                {
                    // Overnight range (e.g., 22:00 - 06:00)
                    isInTimeRange = currentTimeOfDay >= timeFrame.StartTime || currentTimeOfDay <= timeFrame.EndTime;
                }

                if (!isInTimeRange) continue;

                // Check if current day is enabled
                if (!string.IsNullOrWhiteSpace(timeFrame.DaysOfWeek))
                {
                    var enabledDays = timeFrame.DaysOfWeek.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(d => int.TryParse(d.Trim(), out int day) ? day : -1)
                        .Where(d => d >= 0 && d <= 6)
                        .ToList();

                    if (enabledDays.Any() && !enabledDays.Contains(currentDayInt))
                    {
                        continue; // Current day not enabled
                    }
                }

                return timeFrame;
            }

            return null;
        }

        public async Task CheckAndCreateAlertsAsync()
        {
            try
            {
                // COMMENTED OUT: MonitoringConfigurations table removed
                return;
                /* 
                var now = DateTime.Now;
                var currentDay = now.DayOfWeek;

                // Get all active stations with monitoring configurations
                var activeConfigs = await _context.MonitoringConfigurations
                    .Include(mc => mc.Station)
                    .Include(mc => mc.Profile)
                        .ThenInclude(p => p!.TimeFrames)
                    .Where(mc => mc.IsEnabled && mc.Station != null && mc.Station.IsActive && mc.Profile != null)
                    .ToListAsync();

                _logger.LogInformation($"[AlertCheck] Checking {activeConfigs.Count} active monitoring configurations at {now:HH:mm:ss}");

                foreach (var config in activeConfigs)
                {
                    var timeFrame = GetCurrentTimeFrame(config, now, currentDay);
                    if (timeFrame == null)
                    {
                        _logger.LogDebug($"[AlertCheck] Station {config.Station!.Name}: No active time frame at {now:HH:mm:ss}");
                        continue;
                    }

                    await CheckStationForAlertAsync(config.Station!, config, timeFrame);
                }
                */
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AlertCheck] Error checking alerts");
            }
        }

        public async Task<MotionAlert?> CheckStationForAlertAsync(Station station, MonitoringConfiguration config, TimeFrame timeFrame)
        {
            try
            {
                var now = DateTime.Now;

                // Find latest motion from this station (no need to check camera IDs)
                var latestMotion = await _context.MotionEvents
                    .Where(me => me.StationId == station.Id)
                    .OrderByDescending(me => me.DetectedAt)
                    .FirstOrDefaultAsync();

                if (latestMotion == null)
                {
                    _logger.LogDebug($"[AlertCheck] Station {station.Name}: No motion events yet");
                    return null;
                }

                var minutesSinceLastMotion = (int)(now - latestMotion.DetectedAt).TotalMinutes;

                // Check if alert should be created
                if (minutesSinceLastMotion < timeFrame.FrequencyMinutes)
                {
                    _logger.LogDebug($"[AlertCheck] Station {station.Name}: Last motion {minutesSinceLastMotion}min ago (threshold: {timeFrame.FrequencyMinutes}min) - OK");
                    return null;
                }

                // Check if alert already exists
                var existingAlert = await _context.MotionAlerts
                    .Where(a => a.StationId == station.Id && !a.IsResolved)
                    .FirstOrDefaultAsync();

                if (existingAlert != null)
                {
                    _logger.LogDebug($"[AlertCheck] Station {station.Name}: Alert already exists (ID: {existingAlert.Id})");
                    return null;
                }

                // Create new alert with configuration snapshot
                var configSnapshot = System.Text.Json.JsonSerializer.Serialize(new
                {
                    StationId = station.Id,
                    StationName = station.Name,
                    ProfileId = config.ProfileId,
                    ProfileName = config.Profile?.Name,
                    TimeFrameId = timeFrame.Id,
                    TimeFrameName = timeFrame.Name,
                    FrequencyMinutes = timeFrame.FrequencyMinutes,
                    StartTime = timeFrame.StartTime.ToString(@"hh\:mm"),
                    EndTime = timeFrame.EndTime.ToString(@"hh\:mm"),
                    DaysOfWeek = timeFrame.DaysOfWeek,
                    CheckedAt = now
                });

                var alert = new MotionAlert
                {
                    StationId = station.Id,
                    StationName = station.Name,
                    MonitoringConfigurationId = config.Id,
                    TimeFrameId = timeFrame.Id,
                    ConfigurationSnapshot = configSnapshot,
                    AlertTime = now,
                    Severity = minutesSinceLastMotion > timeFrame.FrequencyMinutes * 2
                        ? AlertSeverity.Critical
                        : AlertSeverity.Warning,
                    Message = $"Trạm '{station.Name}' không phát hiện chuyển động trong {minutesSinceLastMotion} phút (ngưỡng: {timeFrame.FrequencyMinutes} phút)",
                    LastMotionAt = latestMotion.DetectedAt,
                    LastMotionCameraId = latestMotion.CameraId,
                    LastMotionCameraName = latestMotion.CameraName,
                    MinutesSinceLastMotion = minutesSinceLastMotion,
                    IsResolved = false,
                    // ✅ Set legacy CameraId field (required by database NOT NULL constraint)
                    CameraId = latestMotion.CameraId ?? $"STATION_{station.Id}",
                    CameraName = station.Name
                };

                _context.MotionAlerts.Add(alert);
                await _context.SaveChangesAsync();

                _logger.LogWarning($"[AlertCheck] ⚠️ Created alert for station {station.Name} - {minutesSinceLastMotion}min since last motion (camera: {latestMotion.CameraName})");
                return alert;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[AlertCheck] Error checking station {station.Name}");
                return null;
            }
        }

        public async Task ResolveStationAlertsAsync(int stationId, string? resolvedBy = null)
        {
            try
            {
                var openAlerts = await _context.MotionAlerts
                    .Where(a => a.StationId == stationId && !a.IsResolved)
                    .ToListAsync();

                if (!openAlerts.Any())
                {
                    return;
                }

                var now = DateTime.Now;
                foreach (var alert in openAlerts)
                {
                    alert.IsResolved = true;
                    alert.ResolvedAt = now;
                    alert.ResolvedBy = resolvedBy ?? "System (Motion Detected)";
                }

                await _context.SaveChangesAsync();

                var station = await _context.Stations.FindAsync(stationId);
                _logger.LogInformation($"[AlertResolve] ✅ Resolved {openAlerts.Count} alert(s) for station {station?.Name ?? stationId.ToString()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[AlertResolve] Error resolving alerts for station {stationId}");
            }
        }

        private async Task CreateAuditLogAsync(string entityType, string entityId, string entityName, string actionType, object? oldValue, object? newValue)
        {
            _logger.LogInformation("[AuditLog] START CreateAuditLogAsync for {EntityType} {EntityId}", entityType, entityId);
            try
            {
                _logger.LogInformation("[AuditLog] Getting auth state...");
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                var userName = user?.Identity?.IsAuthenticated == true ? user.Identity.Name : "System";
                _logger.LogInformation("[AuditLog] User: {UserName}", userName);
                
                // Note: IP and UserAgent not available in Blazor Server circuit context
                string? ipAddress = null;
                string? userAgent = null;

                _logger.LogInformation("[AuditLog] Generating changes summary...");
                var changes = GenerateChangesSummary(oldValue, newValue);
                _logger.LogInformation("[AuditLog] Changes: {Changes}", changes);

                _logger.LogInformation("[AuditLog] Creating DB context...");
                // Use separate context for audit log to avoid conflicts
                using var auditContext = await _contextFactory.CreateDbContextAsync();
                _logger.LogInformation("[AuditLog] DB context created successfully");
                
                // Configure JSON serializer to ignore reference cycles
                var jsonOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                var auditLog = new ConfigurationAuditLog
                {
                    EntityType = entityType,
                    EntityId = int.Parse(entityId),
                    EntityName = entityName,
                    ActionType = actionType,
                    OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue, jsonOptions) : null,
                    NewValue = newValue != null ? JsonSerializer.Serialize(newValue, jsonOptions) : null,
                    Changes = changes,
                    ChangedAt = DateTime.Now,
                    ChangedBy = userName ?? "System",
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                _logger.LogInformation("[AuditLog] Adding audit log to context...");
                auditContext.ConfigurationAuditLogs.Add(auditLog);
                
                _logger.LogInformation("[AuditLog] Saving changes...");
                await auditContext.SaveChangesAsync();
                _logger.LogInformation("[AuditLog] Changes saved successfully");
                
                _logger.LogInformation("Created audit log for {EntityType} {EntityId} by {User}", entityType, entityId, userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuditLog] ERROR creating audit log for {EntityType} {EntityId}", entityType, entityId);
            }
        }

        private string GenerateChangesSummary(object? oldValue, object? newValue)
        {
            if (oldValue == null && newValue == null)
                return "No changes";

            if (oldValue == null)
                return "Created new record";

            if (newValue == null)
                return "Deleted record";

            var changes = new List<string>();
            var oldProps = oldValue.GetType().GetProperties();
            var newProps = newValue.GetType().GetProperties();

            foreach (var prop in oldProps)
            {
                // Skip audit-related properties and navigation properties
                if (prop.Name == "CreatedAt" || prop.Name == "ModifiedAt" || 
                    prop.Name == "CreatedBy" || prop.Name == "ModifiedBy" ||
                    prop.Name == "Id" ||
                    prop.Name == "Station" || prop.Name == "TimeFrames" || 
                    prop.Name == "StationEmployees" || prop.Name == "Profile" ||
                    prop.Name == "TimeFrame" || prop.Name == "MotionAlerts")
                    continue;

                var newProp = newProps.FirstOrDefault(p => p.Name == prop.Name);
                if (newProp == null) continue;

                var oldVal = prop.GetValue(oldValue);
                var newVal = newProp.GetValue(newValue);

                if (!Equals(oldVal, newVal))
                {
                    changes.Add($"{prop.Name}: '{oldVal}' → '{newVal}'");
                }
            }

            return changes.Any() ? string.Join("; ", changes) : "No changes";
        }
    }
}
