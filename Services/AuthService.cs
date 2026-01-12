using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using StationCheck.Data;
using StationCheck.Interfaces;
using StationCheck.Models;

namespace StationCheck.Services;

public class AuthService : IAuthService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _contextFactory = contextFactory;
        _configuration = configuration;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, bool skipPasswordCheck = false)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            _logger.LogWarning($"Login failed: User '{request.Username}' not found");
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        // Skip password check for desktop token generation (already authenticated)
        if (!skipPasswordCheck)
        {
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            _logger.LogInformation($"Login attempt for user '{user.Username}': Password valid={isPasswordValid}");

            if (!isPasswordValid)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }
        }
        else
        {
            _logger.LogInformation($"Skipping password check for user '{user.Username}' (token generation)");
        }

        // Role-based device validation:
        // - Admin/Manager: No device validation required
        // - StationEmployee: Requires MAC address validation (device must be registered, approved, and assigned)
        
        if (user.Role == UserRole.Admin || user.Role == UserRole.Manager)
        {
            // Admin/Manager bypass ALL device validation
            _logger.LogInformation($"{user.Role.ToString()} user '{user.Username}' login successful (no device validation required)");
        }
        // else if (user.Role == UserRole.StationEmployee)
        // {
        //     // StationEmployee login from web (no MAC address) - must use Desktop Login App
        //     if (string.IsNullOrWhiteSpace(request.MacAddress))
        //     {
        //         _logger.LogInformation($"StationEmployee '{user.Username}' login from web (no MAC address) - must download Desktop Login App");
                
        //         return new LoginResponse
        //         {
        //             Success = false,
        //             Message = "Thiết bị chưa được đăng ký. Vui lòng tải Desktop Login App để đăng ký thiết bị.",
        //             RequiresDeviceRegistration = true
        //         };
        //     }

        //     // MAC address is present - validate device
        //     _logger.LogInformation($"StationEmployee login: User='{user.Username}', MAC Address='{request.MacAddress}'");

        //     var device = await context.UserDevices
        //         .FirstOrDefaultAsync(d => d.MacAddress == request.MacAddress && !d.IsDeleted);

        //     if (device == null)
        //     {
        //         _logger.LogWarning($"Login failed: Device MAC '{request.MacAddress}' not found in database");
        //         return new LoginResponse
        //         {
        //             Success = false,
        //             Message = "Thiết bị chưa được đăng ký. Vui lòng đăng ký thiết bị trong Desktop Login App.",
        //             RequiresDeviceRegistration = true
        //         };
        //     }

        //     // Check if device is revoked
        //     if (device.IsRevoked)
        //     {
        //         _logger.LogWarning($"Login failed: Device {request.MacAddress} is revoked");
        //         return new LoginResponse
        //         {
        //             Success = false,
        //             Message = "Thiết bị đã bị thu hồi. Vui lòng liên hệ Admin."
        //         };
        //     }

        //     // Check device approval
        //     if (!device.IsApproved)
        //     {
        //         _logger.LogWarning($"Login failed: Device {request.MacAddress} not approved yet");
        //         return new LoginResponse
        //         {
        //             Success = false,
        //             Message = "Thiết bị chưa được phê duyệt. Vui lòng liên hệ Admin để phê duyệt."
        //         };
        //     }

        //     // Check user assignment to device
        //     var deviceAssignment = await context.DeviceUserAssignments
        //         .FirstOrDefaultAsync(a => a.UserId == user.Id && a.DeviceId == device.Id && a.IsActive && !a.IsDeleted);

        //     if (deviceAssignment == null)
        //     {
        //         _logger.LogWarning($"Login failed: StationEmployee '{user.Username}' not assigned to device {request.MacAddress}");
        //         return new LoginResponse
        //         {
        //             Success = false,
        //             Message = "Bạn chưa được phân quyền để đăng nhập từ thiết bị này. Vui lòng liên hệ Admin."
        //         };
        //     }

        //     _logger.LogInformation($"StationEmployee user '{user.Username}' login successful - device {request.MacAddress} validated (approved + assigned)");
            
        //     // Update device LastUsedAt timestamp
        //     device.LastUsedAt = DateTime.UtcNow;
        //     context.UserDevices.Update(device);
        // }

        // Check if account is active
        if (!user.IsActive)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Account is disabled"
            };
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var token = GenerateJwtToken(user, request.MacAddress);
        var refreshToken = GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        context.RefreshTokens.Add(refreshTokenEntity);
        await context.SaveChangesAsync();

        _logger.LogInformation($"User {user.Username} logged in successfully");

        return new LoginResponse
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            AccessToken = token,
            RefreshToken = refreshToken,
            User = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            }
        };
    }

    public async Task<DeviceLoginResponse> DeviceLoginAsync(DeviceLoginRequest request)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        _logger.LogInformation("[DeviceLoginAsync] Login attempt for user '{Username}' from MAC: {MacAddress}", 
            request.Username, request.MacAddress);

        // 1. Validate username and password
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && !u.IsDeleted);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("[DeviceLoginAsync] Invalid credentials for user '{Username}'", request.Username);
            return new DeviceLoginResponse
            {
                Success = false,
                Message = "Tên đăng nhập hoặc mật khẩu không đúng",
                Status = DeviceStatus.PendingApproval
            };
        }

        // Only StationEmployee can login via desktop app
        if (user.Role != UserRole.StationEmployee)
        {
            _logger.LogWarning("[DeviceLoginAsync] User '{Username}' is not StationEmployee (Role: {Role})", 
                user.Username, user.Role);
            return new DeviceLoginResponse
            {
                Success = false,
                Message = "Chỉ nhân viên trạm mới có thể đăng nhập qua ứng dụng này",
                Status = DeviceStatus.Rejected
            };
        }

        // 2. Check if MAC address exists in database
        var device = await context.UserDevices
            .Include(d => d.Assignments!.Where(a => a.IsActive))
            .FirstOrDefaultAsync(d => d.MacAddress == request.MacAddress && !d.IsDeleted);

        if (device == null)
        {
            // 3. Create new device with PendingApproval status
            device = new UserDevice
            {
                Id = Guid.NewGuid(),
                DeviceName = request.DeviceName ?? $"Device-{request.MacAddress.Replace(":", "")}",
                MacAddress = request.MacAddress,
                DeviceStatus = DeviceStatus.PendingApproval,
                IsApproved = false,
                IsRevoked = false,
                CertificateThumbprint = "N/A", // Not using certificates
                IPAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString(),
                CreatedBy = user.Username,
                CreatedAt = DateTime.UtcNow
            };

            context.UserDevices.Add(device);
            await context.SaveChangesAsync();

            _logger.LogInformation("[DeviceLoginAsync] Created new device {DeviceId} for MAC {MacAddress}, status: PendingApproval",
                device.Id, device.MacAddress);

            return new DeviceLoginResponse
            {
                Success = false,
                Message = "Thiết bị mới đã được đăng ký. Vui lòng chờ quản trị viên phê duyệt.",
                Status = DeviceStatus.PendingApproval,
                DeviceId = device.Id
            };
        }

        // 4. Check device status
        if (device.DeviceStatus == DeviceStatus.PendingApproval)
        {
            _logger.LogInformation("[DeviceLoginAsync] Device {DeviceId} is pending approval", device.Id);
            return new DeviceLoginResponse
            {
                Success = false,
                Message = "Thiết bị đang chờ phê duyệt. Vui lòng liên hệ quản trị viên.",
                Status = DeviceStatus.PendingApproval,
                DeviceId = device.Id
            };
        }

        if (device.DeviceStatus == DeviceStatus.Rejected || device.IsRevoked)
        {
            _logger.LogWarning("[DeviceLoginAsync] Device {DeviceId} is rejected or revoked", device.Id);
            return new DeviceLoginResponse
            {
                Success = false,
                Message = "Thiết bị đã bị từ chối hoặc thu hồi. Vui lòng liên hệ quản trị viên.",
                Status = DeviceStatus.Rejected,
                DeviceId = device.Id
            };
        }

        // 5. Device is approved - check if user is assigned to this device
        var hasAssignment = device.Assignments?.Any(a => a.UserId == user.Id && a.IsActive) ?? false;

        if (!hasAssignment)
        {
            // Auto-assign user to their own device
            var assignment = new DeviceUserAssignment
            {
                Id = Guid.NewGuid(),
                DeviceId = device.Id,
                UserId = user.Id,
                AssignedBy = "System",
                AssignedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            context.DeviceUserAssignments.Add(assignment);
            await context.SaveChangesAsync();

            _logger.LogInformation("[DeviceLoginAsync] Auto-assigned user {UserId} to device {DeviceId}",
                user.Id, device.Id);
        }

        // 6. Update device last used
        device.LastUsedAt = DateTime.UtcNow;
        device.IPAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        await context.SaveChangesAsync();

        // 7. Update user last login
        user.LastLoginAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        // 8. Generate JWT token with MAC address claim
        var token = GenerateJwtToken(user, request.MacAddress);

        _logger.LogInformation("[DeviceLoginAsync] Login successful for user '{Username}' from device {DeviceId}",
            user.Username, device.Id);

        return new DeviceLoginResponse
        {
            Success = true,
            Message = "Đăng nhập thành công",
            Token = token,
            Status = DeviceStatus.Approved,
            DeviceId = device.Id
        };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, string? createdBy = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // Check if username exists
        if (await context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Username already exists"
            };
        }

        // Check if email exists
        if (await context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Email already exists"
            };
        }

        var user = new ApplicationUser
        {
            Username = request.Username,
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        _logger.LogInformation($"New user {user.Username} registered by {createdBy ?? "self"}");

        var token = GenerateJwtToken(user, null);
        var refreshToken = GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        context.RefreshTokens.Add(refreshTokenEntity);
        await context.SaveChangesAsync();

        return new LoginResponse
        {
            Success = true,
            Message = "Registration successful",
            Token = token,
            AccessToken = token,
            RefreshToken = refreshToken,
            User = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var refreshToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked);

        if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid or expired refresh token"
            };
        }

        if (refreshToken.User == null || !refreshToken.User.IsActive)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "User account is disabled"
            };
        }

        // Device re-validation for StationEmployee and Manager roles during token refresh
        if (refreshToken.User.Role == UserRole.StationEmployee || refreshToken.User.Role == UserRole.Manager)
        {
            var hasApprovedDevice = await context.DeviceUserAssignments
                .Where(a => a.UserId == refreshToken.User.Id && a.IsActive && !a.IsDeleted)
                .Include(a => a.Device)
                .AnyAsync(a => a.Device != null && a.Device.IsApproved && !a.Device.IsRevoked && !a.Device.IsDeleted);

            if (!hasApprovedDevice)
            {
                _logger.LogWarning($"Refresh token invalidated: Device status changed for {refreshToken.User.Role} user '{refreshToken.User.Username}'");
                
                // Revoke the refresh token immediately
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();

                return new LoginResponse
                {
                    Success = false,
                    Message = "Device access revoked or removed. Please login again"
                };
            }

            _logger.LogInformation($"Device re-validation passed for {refreshToken.User.Role} user '{refreshToken.User.Username}' during token refresh");
        }

        // Revoke old refresh token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var newToken = GenerateJwtToken(refreshToken.User, null);
        var newRefreshToken = GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = refreshToken.UserId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        context.RefreshTokens.Add(newRefreshTokenEntity);
        await context.SaveChangesAsync();

        return new LoginResponse
        {
            Success = true,
            Message = "Token refreshed successfully",
            Token = newToken,
            AccessToken = newToken,
            RefreshToken = newRefreshToken,
            User = new UserInfo
            {
                Id = refreshToken.User.Id,
                Username = refreshToken.User.Username,
                Email = refreshToken.User.Email,
                FullName = refreshToken.User.FullName,
                Role = refreshToken.User.Role,
                IsActive = refreshToken.User.IsActive,
                CreatedAt = refreshToken.User.CreatedAt,
                LastLoginAt = refreshToken.User.LastLoginAt
            }
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var token = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null)
            return false;

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users.FindAsync(request.UserId);
        if (user == null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.ModifiedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        _logger.LogInformation($"User {user.Username} changed password");

        return true;
    }

    public string GenerateJwtToken(ApplicationUser user, string? macAddress = null)
    {
        _logger.LogInformation($"[GenerateJwtToken] Called for user {user.Username}, macAddress: {macAddress ?? "NULL"}");
        
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourVerySecureSecretKeyForJWT_MinimumLength32Characters!";
        var issuer = jwtSettings["Issuer"] ?? "StationCheck";
        var audience = jwtSettings["Audience"] ?? "StationCheckApp";
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("FullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add MAC address if provided (new authentication method)
        if (!string.IsNullOrEmpty(macAddress))
        {
            claims.Add(new Claim("MacAddress", macAddress));
            _logger.LogInformation($"[GenerateJwtToken] Added MacAddress claim: {macAddress}");
        }
        else
        {
            _logger.LogWarning($"[GenerateJwtToken] No MAC address provided - JWT will not contain device info");
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
