using StationCheck.Models;

namespace StationCheck.Interfaces
{
    public interface IStationService
    {
        Task<List<Station>> GetAllStationsAsync();
        Task<Station?> GetStationByIdAsync(Guid id);
        Task<Station> CreateStationAsync(Station station);
        Task<Station> UpdateStationAsync(Guid id, Station station);
        Task DeleteStationAsync(Guid id);
        IQueryable<Station> GetStationsQueryable();
        // Get stations accessible to user
        Task<List<Station>> GetUserStationsAsync(string userId, UserRole userRole);
    }
}
