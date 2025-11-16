using StationCheck.Models;

namespace StationCheck.Interfaces
{
    public interface IStationService
    {
        Task<List<Station>> GetAllStationsAsync();
        Task<Station?> GetStationByIdAsync(Guid id);
        Task<Station> CreateStationAsync(Station station);
        Task<Station> UpdateStationAsync(Station station);
        Task DeleteStationAsync(Guid id);
        IQueryable<Station> GetStationsQueryable();
        
        // Camera assignments
        Task<List<CameraInfo>> GetStationCamerasAsync(int stationId);
        Task AssignCameraToStationAsync(string cameraId, int stationId, string assignedBy);
        Task RemoveCameraFromStationAsync(string cameraId, int stationId);
        
        // Get stations accessible to user
        Task<List<Station>> GetUserStationsAsync(string userId, UserRole userRole);
    }
}
