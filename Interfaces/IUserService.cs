using StationCheck.Models;

namespace StationCheck.Interfaces;

public interface IUserService
{
    Task<List<UserInfo>> GetAllUsersAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<ApplicationUser> CreateUserAsync(RegisterRequest request, string createdById);
    Task<ApplicationUser> UpdateUserAsync(string id, UpdateUserRequest request, string modifiedById);
    Task DeleteUserAsync(string id, string deletedById);
    Task<bool> CanModifyUserAsync(UserRole currentUserRole, UserRole targetUserRole);
}
