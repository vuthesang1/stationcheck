using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Models;

namespace StationCheck.Services
{
    public class SystemConfigurationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SystemConfigurationService> _logger;

        public SystemConfigurationService(
            ApplicationDbContext context,
            ILogger<SystemConfigurationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get configuration value by key
        /// </summary>
        public async Task<string?> GetValueAsync(string key)
        {
            var config = await _context.SystemConfigurations
                .Where(c => c.Key == key)
                .FirstOrDefaultAsync();

            return config?.Value;
        }

        /// <summary>
        /// Get configuration value as integer
        /// </summary>
        public async Task<int?> GetIntValueAsync(string key)
        {
            var value = await GetValueAsync(key);
            return int.TryParse(value, out var result) ? result : null;
        }

        /// <summary>
        /// Get configuration value as TimeSpan (from seconds)
        /// </summary>
        public async Task<TimeSpan?> GetTimeSpanValueAsync(string key)
        {
            var seconds = await GetIntValueAsync(key);
            return seconds.HasValue ? TimeSpan.FromSeconds(seconds.Value) : null;
        }

        /// <summary>
        /// Get all configurations
        /// </summary>
        public async Task<List<SystemConfiguration>> GetAllAsync()
        {
            return await _context.SystemConfigurations
                .OrderBy(c => c.Category)
                .ThenBy(c => c.DisplayName)
                .ToListAsync();
        }

        /// <summary>
        /// Get configurations by category
        /// </summary>
        public async Task<List<SystemConfiguration>> GetByCategoryAsync(string category)
        {
            return await _context.SystemConfigurations
                .Where(c => c.Category == category)
                .OrderBy(c => c.DisplayName)
                .ToListAsync();
        }

        /// <summary>
        /// Update configuration value
        /// </summary>
        public async Task<bool> UpdateValueAsync(string key, string value, string modifiedBy)
        {
            var config = await _context.SystemConfigurations
                .Where(c => c.Key == key)
                .FirstOrDefaultAsync();

            if (config == null)
            {
                _logger.LogWarning("Configuration key '{Key}' not found", key);
                return false;
            }

            if (!config.IsEditable)
            {
                _logger.LogWarning("Configuration key '{Key}' is not editable", key);
                return false;
            }

            config.Value = value;
            config.ModifiedAt = DateTime.UtcNow;
            config.ModifiedBy = modifiedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Updated configuration '{Key}' = '{Value}' by {User}",
                key, value, modifiedBy
            );

            return true;
        }

        /// <summary>
        /// Update configuration by ID
        /// </summary>
        public async Task<bool> UpdateAsync(int id, string value, string modifiedBy)
        {
            var config = await _context.SystemConfigurations.FindAsync(id);

            if (config == null)
            {
                _logger.LogWarning("Configuration ID {Id} not found", id);
                return false;
            }

            if (!config.IsEditable)
            {
                _logger.LogWarning("Configuration '{Key}' is not editable", config.Key);
                return false;
            }

            config.Value = value;
            config.ModifiedAt = DateTime.UtcNow;
            config.ModifiedBy = modifiedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Updated configuration '{Key}' = '{Value}' by {User}",
                config.Key, value, modifiedBy
            );

            return true;
        }
    }
}
