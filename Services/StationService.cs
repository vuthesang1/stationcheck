using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Interfaces;
using StationCheck.Models;

namespace StationCheck.Services
{
    public class StationService : IStationService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILogger<StationService> _logger;

        public StationService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<StationService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<Station>> GetAllStationsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Stations
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public IQueryable<Station> GetStationsQueryable()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Stations
                .OrderBy(s => s.Name);
        }

        public async Task<Station?> GetStationByIdAsync(Guid id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Stations
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Station> CreateStationAsync(Station station)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            // Auto-generate StationCode if not provided
            if (string.IsNullOrEmpty(station.StationCode))
            {
                station.StationCode = await GenerateStationCodeAsync(context);
            }
            
            station.CreatedAt = DateTime.UtcNow;
            station.IsActive = true;
            
            context.Stations.Add(station);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Created station {StationId} - {StationCode} - {StationName}", station.Id, station.StationCode, station.Name);
            return station;
        }

        private async Task<string> GenerateStationCodeAsync(ApplicationDbContext context)
        {
            // Get the last station code
            var lastStation = await context.Stations
                .OrderByDescending(s => s.Id)
                .Select(s => new { s.StationCode })
                .FirstOrDefaultAsync();

            if (lastStation == null || string.IsNullOrEmpty(lastStation.StationCode))
            {
                // First station
                return "ST000001";
            }

            // Extract number from last code (e.g., "ST000001" -> 1)
            var lastCodeNumber = lastStation.StationCode.Replace("ST", "");
            if (int.TryParse(lastCodeNumber, out int lastNumber))
            {
                // Increment and format with leading zeros
                var newNumber = lastNumber + 1;
                return $"ST{newNumber:D6}"; // D6 = 6 digits with leading zeros
            }

            // Fallback if parsing fails
            return $"ST{DateTime.UtcNow.Ticks % 1000000:D6}";
        }

        public async Task<Station> UpdateStationAsync(Station station)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var existingStation = await context.Stations.FindAsync(station.Id);
            if (existingStation == null)
            {
                throw new KeyNotFoundException($"Station with ID {station.Id} not found");
            }

            existingStation.Name = station.Name;
            existingStation.Address = station.Address;
            existingStation.Description = station.Description;
            existingStation.ContactPerson = station.ContactPerson;
            existingStation.ContactPhone = station.ContactPhone;
            existingStation.IsActive = station.IsActive;
            existingStation.ModifiedAt = DateTime.UtcNow;
            existingStation.ModifiedBy = station.ModifiedBy;

            await context.SaveChangesAsync();
            
            _logger.LogInformation("Updated station {StationId} - {StationName}", station.Id, station.Name);
            return existingStation;
        }

        public async Task DeleteStationAsync(Guid id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var station = await context.Stations.FindAsync(id);
            if (station == null)
            {
                throw new KeyNotFoundException($"Station with ID {id} not found");
            }

            // Check if there are users assigned to this station
            var hasUsers = await context.Users.AnyAsync(u => u.StationId == id);
            if (hasUsers)
            {
                throw new InvalidOperationException("Cannot delete station with assigned employees. Please reassign employees first.");
            }

            // Delete related entities first using ExecuteDelete (EF Core 7+)
            // This avoids loading entities into memory and deletes them directly in database
            
            // Delete TimeFrames
            var deletedTimeFrames = await context.TimeFrames
                .Where(tf => tf.StationId == id)
                .ExecuteDeleteAsync();
            if (deletedTimeFrames > 0)
            {
                _logger.LogInformation("Deleted {Count} TimeFrames for station {StationId}", deletedTimeFrames, id);
            }

            // Finally, delete the station
            context.Stations.Remove(station);
            await context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted station {StationId} - {StationName}", id, station.Name);
        }

        public async Task<List<CameraInfo>> GetStationCamerasAsync(int stationId)
        {
            await Task.CompletedTask;
            return new List<CameraInfo>();
        }

        public async Task AssignCameraToStationAsync(string cameraId, int stationId, string assignedBy)
        {
            await Task.CompletedTask;
        }

        public async Task RemoveCameraFromStationAsync(string cameraId, int stationId)
        {
            await Task.CompletedTask;
        }

        public async Task<List<Station>> GetUserStationsAsync(string userId, UserRole userRole)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            // Admin and Manager can see all stations
            if (userRole == UserRole.Admin || userRole == UserRole.Manager)
            {
                return await GetAllStationsAsync();
            }
            
            // StationEmployee can only see their assigned station
            var user = await context.Users
                .Include(u => u.Station)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user?.StationId == null)
            {
                return new List<Station>();
            }

            var station = await GetStationByIdAsync(user.StationId.Value);
            return station != null ? new List<Station> { station } : new List<Station>();
        }
    }
}
