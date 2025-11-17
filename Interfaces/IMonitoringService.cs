using StationCheck.Models;

namespace StationCheck.Interfaces
{
    public interface IMonitoringService
    {
        // COMMENTED OUT: MonitoringConfiguration/MonitoringProfile tables removed
        // TODO: Restore when tables are added back
        /*
        // MonitoringConfiguration operations
        IQueryable<MonitoringConfiguration> GetMonitoringConfigurationsQueryable();
        Task<MonitoringConfiguration?> GetMonitoringConfigurationByIdAsync(int id);
        Task<MonitoringConfiguration?> GetMonitoringConfigurationByStationIdAsync(int stationId);
        Task<MonitoringConfiguration> CreateMonitoringConfigurationAsync(MonitoringConfiguration configuration);
        Task UpdateMonitoringConfigurationAsync(int id, MonitoringConfiguration configuration);
        Task DeleteMonitoringConfigurationAsync(int id);
        Task ToggleMonitoringAsync(int configurationId, bool isEnabled);
        
        // MonitoringProfile operations
        IQueryable<MonitoringProfile> GetMonitoringProfilesQueryable();
        Task<MonitoringProfile?> GetMonitoringProfileByIdAsync(int id);
        Task<MonitoringProfile> CreateMonitoringProfileAsync(MonitoringProfile profile);
        Task UpdateMonitoringProfileAsync(int id, MonitoringProfile profile);
        Task DeleteMonitoringProfileAsync(int id);
        */
        
        // TimeFrame operations
        IQueryable<TimeFrame> GetTimeFramesQueryable();
        Task<List<TimeFrame>> GetTimeFramesByProfileIdAsync(int profileId);
        Task<TimeFrame?> GetTimeFrameByIdAsync(int id);
        Task<TimeFrame> CreateTimeFrameAsync(TimeFrame timeFrame);
        Task UpdateTimeFrameAsync(int id, TimeFrame timeFrame);
        Task DeleteTimeFrameAsync(int id);
        Task ToggleTimeFrameAsync(int timeFrameId, bool isEnabled);
        
        // Station-based TimeFrame operations (NEW)
        /// <summary>
        /// Get all TimeFrames for a specific station
        /// </summary>
        Task<List<TimeFrame>> GetTimeFramesByStationIdAsync(int stationId);
        
        /// <summary>
        /// Create TimeFrame directly for a station with validation
        /// </summary>
        Task<TimeFrame> CreateTimeFrameForStationAsync(int stationId, TimeFrame timeFrame);
        
        /// <summary>
        /// Bulk enable/disable all TimeFrames for a station
        /// </summary>
        Task BulkToggleTimeFramesAsync(int stationId, bool isEnabled);
        
        /// <summary>
        /// Copy all TimeFrames from one station to another
        /// </summary>
        Task CopyTimeFramesAsync(int sourceStationId, int targetStationId);
        
        /// <summary>
        /// Update an existing TimeFrame with validation
        /// </summary>
        Task<TimeFrame> UpdateTimeFrameAsync(TimeFrame timeFrame);
        
        // Alert monitoring operations
        /// <summary>
        /// Get current active TimeFrame for a monitoring configuration based on current time and day
        /// </summary>
        TimeFrame? GetCurrentTimeFrame(MonitoringConfiguration config, DateTime currentTime, DayOfWeek currentDay);
        
        /// <summary>
        /// Check and create alerts for all active stations
        /// </summary>
        Task CheckAndCreateAlertsAsync();
        
        /// <summary>
        /// Check specific station and create alert if needed
        /// </summary>
        Task<MotionAlert?> CheckStationForAlertAsync(Station station, MonitoringConfiguration config, TimeFrame timeFrame);
        
        /// <summary>
        /// Resolve all open alerts for a station when motion is detected
        /// </summary>
        Task ResolveStationAlertsAsync(int stationId, string? resolvedBy = null);
    }
}
