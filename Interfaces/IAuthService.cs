using StationCheck.Models;

namespace StationCheck.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, bool skipPasswordCheck = false);
    Task<DeviceLoginResponse> DeviceLoginAsync(DeviceLoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request, string? createdBy = null);
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
    string GenerateJwtToken(ApplicationUser user, string? macAddress = null);
    string GenerateRefreshToken();
}
