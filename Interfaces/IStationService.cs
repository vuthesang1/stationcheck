using StationCheck.Models;

namespace StationCheck.Interfaces
{
    public interface IStationService
    {
        Task<List<Station>> GetAllStationsAsync();
        Task<Station?> GetStationByIdAsync(int id);
        Task<Station> CreateStationAsync(Station station);
        Task<Station> UpdateStationAsync(int id, Station station);
        Task DeleteStationAsync(int id);
        IQueryable<Station> GetStationsQueryable();
        // Get stations accessible to user
        Task<List<Station>> GetUserStationsAsync(string userId, UserRole userRole);
    }
}
