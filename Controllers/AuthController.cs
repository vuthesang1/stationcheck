using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StationCheck.Interfaces;
using StationCheck.Models;
using StationCheck.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace StationCheck.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ICertificateService _certificateService;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(
        IAuthService authService,
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        ICertificateService certificateService,
        ILogger<AuthController> logger,
        IConfiguration configuration)
    {
        _authService = authService;
        _dbContextFactory = dbContextFactory;
        _certificateService = certificateService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        // For Desktop Login App, add MAC address if provided in header
        if (Request.Headers.TryGetValue("X-Device-Mac", out var macHeader))
        {
            request.MacAddress = macHeader.ToString();
            _logger.LogInformation($"[Login] MAC address from header: {request.MacAddress}");
        }

        var response = await _authService.LoginAsync(request);
        
        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// Web browser login endpoint for Admin/Manager - sets authentication cookie
    /// </summary>
    [HttpPost("web-login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> WebLogin([FromBody] LoginRequest request)
    {
        _logger.LogInformation($"[WebLogin] Login attempt from browser for user: {request.Username}");

        // Clear any existing authentication state first
        if (User.Identity?.IsAuthenticated == true)
        {
            var oldUsername = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
            _logger.LogInformation($"[WebLogin] Clearing existing session for user: {oldUsername}");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("StationCheck.Auth");
            Response.Cookies.Delete(".AspNetCore.Cookies");
        }

        var response = await _authService.LoginAsync(request);
        
        if (!response.Success)
        {
            _logger.LogWarning($"[WebLogin] Login failed for user: {request.Username}, Message: {response.Message}");
            return Unauthorized(response);
        }

        var user = response.User;
        if (user == null)
        {
            _logger.LogError($"[WebLogin] Login succeeded but User is null for: {request.Username}");
            return StatusCode(500, new { message = "Internal server error" });
        }

        // // Only Admin and Manager can login via browser
        // if (user.Role == UserRole.StationEmployee)
        // {
        //     _logger.LogWarning($"[WebLogin] StationEmployee {user.Username} attempted browser login - must use Desktop App");
        //     return Unauthorized(new LoginResponse
        //     {
        //         Success = false,
        //         Message = "Bạn cần đăng nhập qua Desktop Login App",
        //         RequiresDeviceRegistration = true
        //     });
        // }

        // Create claims for cookie authentication
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("FullName", user.FullName ?? string.Empty)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true, // Remember me
            AllowRefresh = true, // Allow sliding expiration
            // Do NOT set ExpiresUtc - let cookie options handle expiration with SlidingExpiration
        };

        // Sign in with cookie authentication
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties
        );

        _logger.LogInformation($"[WebLogin] Login successful for {user.Role} user: {user.Username}, Cookie created");

        return Ok(response);
    }

    /// <summary>
    /// Device login endpoint for desktop app with MAC address authentication
    /// </summary>
    [HttpPost("device-login")]
    [AllowAnonymous]
    public async Task<ActionResult<DeviceLoginResponse>> DeviceLogin([FromBody] DeviceLoginRequest request)
    {
        _logger.LogInformation("[DeviceLogin] Request from MAC: {MacAddress}, User: {Username}", 
            request.MacAddress, request.Username);

        var response = await _authService.DeviceLoginAsync(request);

        if (response.Status == DeviceStatus.Approved && response.Success)
        {
            _logger.LogInformation("[DeviceLogin] Success - Device {DeviceId} approved", response.DeviceId);
            return Ok(response);
        }
        else if (response.Status == DeviceStatus.PendingApproval)
        {
            _logger.LogInformation("[DeviceLogin] Device {DeviceId} pending approval", response.DeviceId);
            return StatusCode(202, response); // 202 Accepted - waiting for approval
        }
        else
        {
            _logger.LogWarning("[DeviceLogin] Rejected - Status: {Status}, Message: {Message}", 
                response.Status, response.Message);
            return Unauthorized(response);
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);
        
        if (!response.Success)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<ActionResult> RevokeToken([FromBody] string refreshToken)
    {
        var result = await _authService.RevokeTokenAsync(refreshToken);
        
        if (!result)
        {
            return BadRequest(new { message = "Invalid refresh token" });
        }

        return Ok(new { message = "Token revoked successfully" });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _authService.ChangePasswordAsync(request);
        
        if (!result)
        {
            return BadRequest(new { message = "Failed to change password. Invalid current password." });
        }

        return Ok(new { message = "Password changed successfully" });
    }

    [HttpPost("desktop-login-token")]
    [AllowAnonymous]
    public async Task<ActionResult<DesktopLoginTokenResponse>> CreateDesktopLoginToken()
    {
        _logger.LogInformation("[CreateDesktopLoginToken] Method called");
        
        // Extract JWT token from Authorization header and parse manually
        var authHeader = Request.Headers["Authorization"].ToString();
        _logger.LogInformation($"[CreateDesktopLoginToken] Authorization header: {(string.IsNullOrEmpty(authHeader) ? "EMPTY" : "Present")}");
        
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            _logger.LogWarning("[CreateDesktopLoginToken] No token provided");
            return Unauthorized(new { message = "No token provided" });
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        _logger.LogInformation($"[CreateDesktopLoginToken] Token extracted, length: {token.Length}");
        
        string? userId = null;
        string? username = null;
        string? macAddress = null;
        System.Security.Claims.ClaimsPrincipal? principal = null;
        
        try
        {
            _logger.LogInformation("[CreateDesktopLoginToken] Starting token validation...");
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new Exception("JWT Key not configured"));
            
            var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            // Try different claim types - .NET may map "sub" to NameIdentifier
            userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                  ?? principal.FindFirst("sub")?.Value
                  ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            
            username = principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                    ?? principal.FindFirst("unique_name")?.Value
                    ?? principal.FindFirst("name")?.Value;
            
            macAddress = principal.Claims.FirstOrDefault(c => c.Type == "MacAddress")?.Value;
            
            _logger.LogInformation($"[CreateDesktopLoginToken] Token validated. UserId: {userId}, Username: {username}");
            _logger.LogInformation($"[CreateDesktopLoginToken] All claims: {string.Join(", ", principal.Claims.Select(c => $"{c.Type}={c.Value}"))}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"[CreateDesktopLoginToken] Invalid token: {ex.Message}");
            return Unauthorized(new { message = "Invalid or expired token" });
        }

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning($"[CreateDesktopLoginToken] UserId is null or empty");
            return Unauthorized(new { message = "Invalid user ID in token" });
        }

        _logger.LogInformation($"[CreateDesktopLoginToken] Creating token for user {username} (ID: {userId})");

        // Get user info
        using var context = await _dbContextFactory.CreateDbContextAsync();
        var user = await context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        _logger.LogInformation($"[CreateDesktopLoginToken] MAC address from token: {macAddress ?? "NOT FOUND"}");

        // Generate new JWT token for browser login
        var loginRequest = new LoginRequest
        {
            Username = user.Username,
            Password = string.Empty, // Not needed for token-based auth
            MacAddress = macAddress // Pass MAC address from original token
        };

        var loginResponse = await _authService.LoginAsync(loginRequest, skipPasswordCheck: true);
        
        if (!loginResponse.Success || string.IsNullOrEmpty(loginResponse.Token))
        {
            return BadRequest(new { message = "Failed to generate token" });
        }

        _logger.LogInformation($"[CreateDesktopLoginToken] Token created successfully for user {userId}");

        return Ok(new DesktopLoginTokenResponse
        {
            Success = true,
            Token = loginResponse.Token,
            RefreshToken = loginResponse.RefreshToken ?? string.Empty,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Message = "Token created successfully"
        });
    }

    [HttpPost("validate-token")]
    [AllowAnonymous]
    public async Task<ActionResult> ValidateToken()
    {
        // Extract token from Authorization header
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized(new { message = "No token provided" });
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        
        try
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new Exception("JWT Key not configured"));
            
            var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            // Use ClaimTypes.NameIdentifier for "sub", but "name" claim is not mapped automatically
            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = principal.FindFirst("name")?.Value; // Use string literal, not ClaimTypes.Name
            var role = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            _logger.LogInformation($"[ValidateToken] All claims: {string.Join(", ", principal.Claims.Select(c => $"{c.Type}={c.Value}"))}");
            _logger.LogInformation($"[ValidateToken] Token valid for user {username} (ID: {userId})");

            // Get user details
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var user = await context.Users.FindAsync(userId);

            return Ok(new
            {
                success = true,
                username = username,
                role = role,
                userId = userId,
                user = user != null ? new
                {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.Role,
                    user.StationId
                } : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"[ValidateToken] Invalid token: {ex.Message}");
            return Unauthorized(new { message = "Invalid or expired token" });
        }
    }

    /// <summary>
    /// Set authentication cookie for Blazor Server from JWT token (Desktop App SSO flow)
    /// </summary>
    [HttpPost("set-cookie-from-token")]
    [AllowAnonymous]
    public async Task<ActionResult> SetCookieFromToken()
    {
        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { message = "No Bearer token provided" });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT secret key not configured"));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = principal.FindFirst("name")?.Value;
            var macAddress = principal.FindFirst("MacAddress")?.Value; // Extract MAC from JWT

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Invalid token claims" });
            }

            _logger.LogInformation($"[SetCookieFromToken] Setting cookie for user {username} (ID: {userId}), MAC: {macAddress}");

            // Get user from database
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var user = await context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Create claims for cookie authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("FullName", user.FullName ?? string.Empty)
            };

            // CRITICAL: Add MAC address from JWT to cookie
            if (!string.IsNullOrEmpty(macAddress))
            {
                claims.Add(new Claim("MacAddress", macAddress));
                _logger.LogInformation($"[SetCookieFromToken] Added MacAddress claim to cookie: {macAddress}");
            }
            else
            {
                _logger.LogWarning($"[SetCookieFromToken] No MacAddress in JWT token for user {username}");
            }

            if (user.StationId.HasValue)
            {
                claims.Add(new Claim("StationId", user.StationId.Value.ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in with cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true // Allow sliding expiration
                    // Do NOT set ExpiresUtc - let cookie options handle expiration
                });

            _logger.LogInformation($"[SetCookieFromToken] Cookie authentication successful for user {username}");

            return Ok(new { success = true, message = "Authentication cookie set successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"[SetCookieFromToken] Error: {ex.Message}");
            return Unauthorized(new { message = "Invalid or expired token" });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value ?? "unknown";
        var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "unknown";
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "unknown";
        
        _logger.LogInformation($"[Logout] User {username} (ID: {userId}, Role: {role}) is logging out");
        
        // Revoke the refresh token if needed
        var refreshTokenClaim = User.FindFirst("refresh_token");
        if (refreshTokenClaim != null)
        {
            _logger.LogInformation($"[Logout] Revoking refresh token");
            await _authService.RevokeTokenAsync(refreshTokenClaim.Value);
        }

        // Sign out from Cookie authentication
        await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation($"[Logout] Signed out from cookie authentication");

        // ✅ FIX: Force delete ALL auth-related cookies with proper settings
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(-1), // Set to past date
            Path = "/",
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        };
        
        Response.Cookies.Delete("StationCheck.Auth", cookieOptions);
        Response.Cookies.Delete(".AspNetCore.Cookies", cookieOptions);
        Response.Cookies.Delete(".AspNetCore.Session", cookieOptions);
        Response.Cookies.Delete(".AspNetCore.Antiforgery", cookieOptions);
        
        // Also set expired cookies to force browser to clear
        Response.Cookies.Append("StationCheck.Auth", "", cookieOptions);
        Response.Cookies.Append(".AspNetCore.Cookies", "", cookieOptions);
        
        _logger.LogInformation($"[Logout] Deleted all cookies");

        _logger.LogInformation($"[Logout] Logout successful for user {username}");
        
        // ✅ Return special flag to client to force full page reload
        return Ok(new 
        { 
            message = "Logged out successfully",
            forceReload = true // Signal to client to do full page reload
        });
    }

    /// <summary>
    /// Check if current user has active device assignments (for debugging)
    /// </summary>
    [HttpGet("check-access")]
    [Authorize]
    public async Task<ActionResult> CheckAccess()
    {
        var userId = User.FindFirst("sub")?.Value;
        _logger.LogInformation($"[AuthController.CheckAccess] Checking access for user {userId}");

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning($"[AuthController.CheckAccess] No user ID found");
            return Unauthorized(new { message = "No user ID found" });
        }

        await using var context = _dbContextFactory.CreateDbContext();
        var hasActiveAccess = await context.DeviceUserAssignments
            .AnyAsync(a => a.UserId == userId && a.IsActive && !a.IsDeleted);

        _logger.LogInformation($"[AuthController.CheckAccess] User {userId} hasActiveAccess: {hasActiveAccess}");

        return Ok(new {
            userId = userId,
            hasActiveAccess = hasActiveAccess,
            message = hasActiveAccess ? "User has active device assignments" : "User has NO active device assignments"
        });
    }

    #region Device Registration

    /// <summary>
    /// Download device installer executable
    /// </summary>
    [HttpGet("download-installer")]
    [Authorize]
    public IActionResult DownloadInstaller()
    {
        try
        {
            // TODO: Return pre-built DeviceInstaller.exe from wwwroot or bin folder
            // For now, return a placeholder
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "DeviceInstaller.exe");
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "Installer not found" });
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", "DeviceInstaller.exe");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading installer");
            return StatusCode(500, new { message = "Error downloading installer" });
        }
    }

    /// <summary>
    /// Get all pending devices (Admin only)
    /// </summary>
    [HttpGet("pending-devices")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<PendingDeviceDto>>> GetPendingDevices()
    {
        try
        {
            _logger.LogInformation("[GetPendingDevices] API called");
            
            await using var context = _dbContextFactory.CreateDbContext();
            
            var totalDevices = await context.UserDevices.CountAsync();
            _logger.LogInformation("[GetPendingDevices] Total devices in DB: {TotalCount}", totalDevices);
            
            var pendingDevices = await context.UserDevices
                .Where(d => !d.IsApproved && !d.IsRevoked && !d.IsDeleted)
                .Select(d => new PendingDeviceDto
                {
                    Id = d.Id,
                    DeviceName = d.DeviceName,
                    CertificateThumbprint = d.CertificateThumbprint,
                    CertificateSubject = d.CertificateSubject,
                    CertificateValidFrom = d.CertificateValidFrom,
                    CertificateValidTo = d.CertificateValidTo,
                    CreatedAt = d.CreatedAt
                })
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            _logger.LogInformation("[GetPendingDevices] Found {PendingCount} pending devices", pendingDevices.Count);
            foreach (var device in pendingDevices)
            {
                _logger.LogInformation("[GetPendingDevices]   - Device: {DeviceId}, Name: {DeviceName}, Approved: false, Revoked: false", device.Id, device.DeviceName);
            }

            return Ok(pendingDevices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GetPendingDevices] Error getting pending devices");
            return StatusCode(500, new { message = "Error retrieving pending devices" });
        }
    }

    /// <summary>
    /// Approve a pending device (Admin only)
    /// </summary>
    [HttpPost("approve-device/{deviceId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ApproveDevice(Guid deviceId)
    {
        try
        {
            var adminId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized();

            await using var context = _dbContextFactory.CreateDbContext();
            var device = await context.UserDevices.FindAsync(deviceId);
            
            if (device == null || device.IsDeleted)
                return NotFound(new { message = "Device not found" });

            if (device.IsApproved)
                return BadRequest(new { message = "Device already approved" });

            // Approve device - update BOTH legacy and new fields
            device.IsApproved = true;
            device.DeviceStatus = DeviceStatus.Approved;
            device.ApprovedBy = adminId;
            device.ApprovedAt = DateTime.UtcNow;

            // Log approval
            var auditLog = new DeviceCertificateAuditLog
            {
                Id = Guid.NewGuid(),
                DeviceId = device.Id,
                UserId = adminId,
                Action = DeviceAuditAction.Approved,
                Notes = $"Device approved by admin",
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            context.DeviceCertificateAuditLogs.Add(auditLog);
            await context.SaveChangesAsync();

            _logger.LogInformation("Device {DeviceId} approved by {AdminId}", deviceId, adminId);

            return Ok(new { message = "Device approved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving device");
            return StatusCode(500, new { message = "Error approving device" });
        }
    }

    /// <summary>
    /// Revoke a device (Admin or device owner)
    /// </summary>
    [HttpPost("revoke-device/{deviceId}")]
    [Authorize]
    public async Task<ActionResult> RevokeDevice(Guid deviceId)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await using var context = _dbContextFactory.CreateDbContext();
            var device = await context.UserDevices.FindAsync(deviceId);
            
            if (device == null || device.IsDeleted)
                return NotFound(new { message = "Device not found" });

            // Only Admin can revoke devices
            if (userRole != "Admin")
                return Forbid();

            // Revoke device - update BOTH legacy and new fields
            device.IsRevoked = true;
            device.DeviceStatus = DeviceStatus.Rejected;

            // Log revocation
            var auditLog = new DeviceCertificateAuditLog
            {
                Id = Guid.NewGuid(),
                DeviceId = device.Id,
                UserId = userId,
                Action = DeviceAuditAction.Revoked,
                Notes = $"Device revoked by admin",
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            context.DeviceCertificateAuditLogs.Add(auditLog);
            await context.SaveChangesAsync();

            _logger.LogInformation("Device {DeviceId} revoked by {UserId}", deviceId, userId);

            return Ok(new { message = "Device revoked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking device");
            return StatusCode(500, new { message = "Error revoking device" });
        }
    }

    /// <summary>
    /// Get current user's registered devices
    /// </summary>
    [HttpGet("user-devices")]
    [Authorize]
    public async Task<ActionResult<List<UserDeviceDto>>> GetUserDevices()
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await using var context = _dbContextFactory.CreateDbContext();
            var devices = await context.UserDevices
                .Where(d => !d.IsDeleted)
                .Select(d => new UserDeviceDto
                {
                    Id = d.Id,
                    DeviceName = d.DeviceName,
                    CertificateThumbprint = d.CertificateThumbprint,
                    CertificateSubject = d.CertificateSubject,
                    CertificateValidFrom = d.CertificateValidFrom,
                    CertificateValidTo = d.CertificateValidTo,
                    IsApproved = d.IsApproved,
                    IsRevoked = d.IsRevoked,
                    ApprovedAt = d.ApprovedAt,
                    ApprovedBy = d.ApprovedBy,
                    LastUsedAt = d.LastUsedAt,
                    CreatedAt = d.CreatedAt
                })
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return Ok(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user devices");
            return StatusCode(500, new { message = "Error retrieving user devices" });
        }
    }

    /// <summary>
    /// Get all approved devices with assignment information (Admin only)
    /// </summary>
    [HttpGet("device-assignments")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<DeviceAssignmentDetailsDto>>> GetDeviceAssignments()
    {
        try
        {
            await using var context = _dbContextFactory.CreateDbContext();
            var devices = await context.UserDevices
                .Where(d => d.IsApproved && !d.IsRevoked && !d.IsDeleted)
                .Include(d => d.Assignments!.Where(a => !a.IsDeleted))
                .ToListAsync();

            var result = devices.Select(d => new DeviceAssignmentDetailsDto
            {
                DeviceId = d.Id,
                DeviceName = d.DeviceName,
                OwnerUserId = d.Assignments?.FirstOrDefault(a => a.IsActive)?.UserId ?? "(Unassigned)",
                OwnerUsername = d.Assignments?.FirstOrDefault(a => a.IsActive)?.User?.Username ?? "(Unassigned)",
                AssignedUsersCount = d.Assignments!.Where(a => a.IsActive && !a.IsDeleted).Count(),
                LastUsedAt = d.LastUsedAt
            })
            .OrderByDescending(d => d.LastUsedAt ?? DateTime.MinValue)
            .ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device assignments");
            return StatusCode(500, new { message = "Error retrieving device assignments" });
        }
    }

    /// <summary>
    /// Get users assigned to a specific device
    /// </summary>
    [HttpGet("device-assignments/{deviceId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<AssignedUserDto>>> GetDeviceAssignedUsers(Guid deviceId)
    {
        try
        {
            await using var context = _dbContextFactory.CreateDbContext();
            
            // Verify device exists and is approved
            var device = await context.UserDevices.FindAsync(deviceId);
            if (device == null || device.IsDeleted || !device.IsApproved)
                return NotFound(new { message = "Device not found" });

            var assignedUsers = await context.DeviceUserAssignments
                .Where(a => a.DeviceId == deviceId && a.IsActive && !a.IsDeleted)
                .Include(a => a.User)
                .Select(a => new AssignedUserDto
                {
                    UserId = a.UserId,
                    Username = a.User!.Username,
                    FullName = a.User!.FullName
                })
                .OrderBy(u => u.Username)
                .ToListAsync();

            return Ok(assignedUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assigned users for device");
            return StatusCode(500, new { message = "Error retrieving assigned users" });
        }
    }

    /// <summary>
    /// Get all users except those already assigned to a device (Admin only)
    /// </summary>
    [HttpGet("device-available-users/{deviceId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<AvailableUserDto>>> GetAvailableUsersForDevice(Guid deviceId)
    {
        try
        {
            await using var context = _dbContextFactory.CreateDbContext();
            
            // Verify device exists and is approved
            var device = await context.UserDevices.FindAsync(deviceId);
            if (device == null || device.IsDeleted || !device.IsApproved)
                return NotFound(new { message = "Device not found" });

            // Get already assigned users
            var assignedUserIds = await context.DeviceUserAssignments
                .Where(a => a.DeviceId == deviceId && a.IsActive && !a.IsDeleted)
                .Select(a => a.UserId)
                .ToListAsync();

            // Get all users except assigned ones and device owner
            var availableUsers = await context.Users
                .Where(u => !assignedUserIds.Contains(u.Id))
                .Select(u => new AvailableUserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName
                })
                .OrderBy(u => u.Username)
                .ToListAsync();

            return Ok(availableUsers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available users");
            return StatusCode(500, new { message = "Error retrieving available users" });
        }
    }

    /// <summary>
    /// Add a user assignment to a device (Admin only)
    /// </summary>
    [HttpPost("device-assignments/{deviceId}/assign-user")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AssignUserToDevice(Guid deviceId, [FromBody] AssignUserRequest request)
    {
        try
        {
            var adminId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized();

            if (string.IsNullOrEmpty(request.UserId))
                return BadRequest(new { message = "UserId is required" });

            await using var context = _dbContextFactory.CreateDbContext();
            
            // Verify device exists and is approved
            var device = await context.UserDevices.FindAsync(deviceId);
            if (device == null || device.IsDeleted || !device.IsApproved)
                return NotFound(new { message = "Device not found" });

            // Verify user exists
            var user = await context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Check if assignment already exists
            var existingAssignment = await context.DeviceUserAssignments
                .FirstOrDefaultAsync(a => a.DeviceId == deviceId && a.UserId == request.UserId && !a.IsDeleted);

            if (existingAssignment != null)
            {
                if (existingAssignment.IsActive)
                    return BadRequest(new { message = "User is already assigned to this device" });
                else
                {
                    // Reactivate existing assignment
                    existingAssignment.IsActive = true;
                    existingAssignment.AssignedAt = DateTime.UtcNow;
                    existingAssignment.AssignedBy = adminId;
                }
            }
            else
            {
                // Create new assignment
                var assignment = new DeviceUserAssignment
                {
                    Id = Guid.NewGuid(),
                    DeviceId = deviceId,
                    UserId = request.UserId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = adminId,
                    IsActive = true
                };
                context.DeviceUserAssignments.Add(assignment);
            }

            // Log the assignment
            var auditLog = new DeviceCertificateAuditLog
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                UserId = adminId,
                Action = DeviceAuditAction.UserAssigned,
                Notes = $"User {request.UserId} assigned to device by admin",
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            context.DeviceCertificateAuditLogs.Add(auditLog);

            await context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} assigned to device {DeviceId} by {AdminId}", request.UserId, deviceId, adminId);

            return Ok(new { message = "User assigned to device successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning user to device");
            return StatusCode(500, new { message = "Error assigning user to device" });
        }
    }

    /// <summary>
    /// Remove a user assignment from a device (Admin only)
    /// </summary>
    [HttpDelete("device-assignments/{deviceId}/users/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RemoveUserFromDevice(Guid deviceId, string userId)
    {
        try
        {
            var adminId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized();

            await using var context = _dbContextFactory.CreateDbContext();
            
            // Verify device exists and is approved
            var device = await context.UserDevices.FindAsync(deviceId);
            if (device == null || device.IsDeleted || !device.IsApproved)
                return NotFound(new { message = "Device not found" });

            // Check if assignment exists
            var assignment = await context.DeviceUserAssignments
                .FirstOrDefaultAsync(a => a.DeviceId == deviceId && a.UserId == userId && !a.IsDeleted);

            if (assignment == null)
                return NotFound(new { message = "User assignment not found" });

            // Cannot remove device owner
            var userAssignment = device.Assignments?.FirstOrDefault(a => a.UserId == userId && a.IsActive);
            if (userAssignment == null)
                return BadRequest(new { message = "User is not assigned to this device" });

            // Soft delete the assignment
            assignment.IsActive = false;
            assignment.IsDeleted = true;
            assignment.DeletedAt = DateTime.UtcNow;
            assignment.DeletedBy = adminId;

            // Log the removal
            var auditLog = new DeviceCertificateAuditLog
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                UserId = adminId,
                Action = DeviceAuditAction.UserRemoved,
                Notes = $"User {userId} removed from device by admin",
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            context.DeviceCertificateAuditLogs.Add(auditLog);

            await context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} removed from device {DeviceId} by {AdminId}", userId, deviceId, adminId);

            return Ok(new { message = "User removed from device successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from device");
            return StatusCode(500, new { message = "Error removing user from device" });
        }
    }

    /// <summary>
    /// Get approved device count for each user (Admin only)
    /// </summary>
    [HttpGet("user-device-counts")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Dictionary<string, int>>> GetUserDeviceCounts()
    {
        try
        {
            await using var context = _dbContextFactory.CreateDbContext();
            
            // Get count of approved devices for each user
            var userDevices = await context.UserDevices
                .Where(d => d.IsApproved && !d.IsRevoked && !d.IsDeleted)
                .Include(d => d.Assignments)
                .ToListAsync();

            var result = userDevices
                .GroupBy(d => d.Assignments?.FirstOrDefault(a => a.IsActive)?.UserId ?? "(Unassigned)")
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionary(x => x.UserId, x => x.Count);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user device counts");
            return StatusCode(500, new { message = "Error retrieving user device counts" });
        }
    }

    /// <summary>
    /// Download device installer package as ZIP
    /// This endpoint is public (AllowAnonymous) to allow users to download the installer before logging in
    /// </summary>
    [HttpGet("download-certificate")]
    [AllowAnonymous]
    public IActionResult DownloadCertificate()
    {
        try
        {
            var packagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DeviceInstallerPackage");
            
            if (!Directory.Exists(packagePath))
            {
                return NotFound(new { message = "Device installer package not found" });
            }

            // Create a zip file in memory
            using (var memoryStream = new System.IO.MemoryStream())
            {
                using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    var files = Directory.GetFiles(packagePath, "*", System.IO.SearchOption.AllDirectories);
                    
                    foreach (var file in files)
                    {
                        var fileName = Path.GetRelativePath(packagePath, file);
                        var entry = archive.CreateEntry(fileName);
                        
                        using (var stream = System.IO.File.OpenRead(file))
                        using (var entryStream = entry.Open())
                        {
                            stream.CopyTo(entryStream);
                        }
                    }
                }

                memoryStream.Position = 0;
                var zipBytes = memoryStream.ToArray();
                return File(zipBytes, "application/zip", "DeviceInstaller.zip");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading device installer");
            return StatusCode(500, new { message = "Error downloading installer package" });
        }
    }

    /// <summary>
    /// Register a new device from Desktop Login App
    /// Requires Username, Password, MacAddress
    /// </summary>
    [HttpPost("register-device")]
    [AllowAnonymous]
    public async Task<ActionResult<DeviceRegistrationResponse>> RegisterDevice([FromBody] DeviceRegistrationRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new DeviceRegistrationResponse
                {
                    Success = false,
                    Message = "Username và password là bắt buộc"
                });
            }

            if (string.IsNullOrWhiteSpace(request.MacAddress))
            {
                return BadRequest(new DeviceRegistrationResponse
                {
                    Success = false,
                    Message = "Không thể lấy thông tin MAC address"
                });
            }

            await using var context = await _dbContextFactory.CreateDbContextAsync();

            // Verify user credentials
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && !u.IsDeleted);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new DeviceRegistrationResponse
                {
                    Success = false,
                    Message = "Username hoặc password không đúng"
                });
            }

            // Check if user is StationEmployee
            if (user.Role != UserRole.StationEmployee)
            {
                return BadRequest(new DeviceRegistrationResponse
                {
                    Success = false,
                    Message = "Chỉ StationEmployee mới cần đăng ký thiết bị"
                });
            }

            // Check if device already registered by MAC address OR CertificateThumbprint (including soft-deleted ones)
            // This handles both MAC-based devices and prevents duplicate CertificateThumbprint
            // IMPORTANT: Use IgnoreQueryFilters() to bypass any global soft-delete filters
            var expectedThumbprint = $"MAC-{request.MacAddress}";
            
            // First, try to find and PERMANENTLY DELETE any soft-deleted devices with this thumbprint
            var deletedDevices = await context.UserDevices
                .IgnoreQueryFilters()
                .Where(d => d.IsDeleted && d.CertificateThumbprint == expectedThumbprint)
                .ToListAsync();
            
            if (deletedDevices.Any())
            {
                _logger.LogWarning($"[RegisterDevice] Found {deletedDevices.Count} soft-deleted device(s) with CertificateThumbprint={expectedThumbprint}. Permanently deleting...");
                context.UserDevices.RemoveRange(deletedDevices);
                await context.SaveChangesAsync();
                _logger.LogInformation($"[RegisterDevice] Permanently deleted {deletedDevices.Count} device(s)");
            }
            
            // Now check for existing active devices
            var existingDevice = await context.UserDevices
                .Where(d => d.MacAddress == request.MacAddress && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync();
            
            _logger.LogInformation($"[RegisterDevice] Checking for existing device: MAC={request.MacAddress}, ExpectedThumbprint={expectedThumbprint}, Found={existingDevice != null}, IsDeleted={existingDevice?.IsDeleted}");

            if (existingDevice != null)
            {
                // Device exists and is active - return current status
                // Use DeviceStatus enum directly (it's the source of truth)
                var deviceStatus = existingDevice.DeviceStatus;

                var assignment = await context.DeviceUserAssignments
                    .FirstOrDefaultAsync(a => a.UserId == user.Id && a.DeviceId == existingDevice.Id && a.IsActive && !a.IsDeleted);

                string message = deviceStatus switch
                {
                    DeviceStatus.Approved when assignment != null => "Thiết bị đã được phê duyệt và bạn đã được phân quyền. Vui lòng đăng nhập lại.",
                    DeviceStatus.Approved => "Thiết bị đã được phê duyệt nhưng bạn chưa được phân quyền. Vui lòng liên hệ Admin.",
                    DeviceStatus.Rejected => "Thiết bị đã bị thu hồi. Vui lòng liên hệ Admin.",
                    DeviceStatus.PendingApproval => "Thiết bị đang chờ phê duyệt. Vui lòng liên hệ Admin để được phê duyệt.",
                    _ => "Thiết bị đã đăng ký"
                };

                return Ok(new DeviceRegistrationResponse
                {
                    Success = deviceStatus != DeviceStatus.Rejected,
                    Status = deviceStatus,
                    Message = message,
                    DeviceId = existingDevice.Id
                });
            }

            // Before creating new device, check if there's any existing device with CertificateThumbprint = "N/A"
            // This can happen if multiple devices were deleted and recreated
            var deviceWithNACert = await context.UserDevices
                .IgnoreQueryFilters() // Bypass global soft-delete filter
                .Where(d => d.CertificateThumbprint == "N/A")
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefaultAsync();

            if (deviceWithNACert != null)
            {
                _logger.LogWarning($"Found existing device with CertificateThumbprint='N/A': DeviceId={deviceWithNACert.Id}, IsDeleted={deviceWithNACert.IsDeleted}, MAC={deviceWithNACert.MacAddress}");
                
                // Update this device instead of creating new one
                deviceWithNACert.MacAddress = request.MacAddress;
                deviceWithNACert.DeviceName = request.DeviceName ?? "Unknown Device";
                deviceWithNACert.IsDeleted = false;
                deviceWithNACert.DeletedAt = null;
                deviceWithNACert.DeletedBy = null;
                deviceWithNACert.DeviceStatus = DeviceStatus.PendingApproval;
                deviceWithNACert.IsApproved = false;
                deviceWithNACert.IsRevoked = false;
                deviceWithNACert.ModifiedAt = DateTime.UtcNow;
                deviceWithNACert.ModifiedBy = user.Username;
                deviceWithNACert.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                deviceWithNACert.UserAgent = Request.Headers["User-Agent"].ToString();
                
                // Check if user already has assignment
                var existingAssignment = await context.DeviceUserAssignments
                    .FirstOrDefaultAsync(a => a.UserId == user.Id && a.DeviceId == deviceWithNACert.Id);
                
                if (existingAssignment != null)
                {
                    if (existingAssignment.IsDeleted)
                    {
                        existingAssignment.IsDeleted = false;
                        existingAssignment.DeletedAt = null;
                        existingAssignment.DeletedBy = null;
                    }
                    existingAssignment.IsActive = false;
                    existingAssignment.ModifiedAt = DateTime.UtcNow;
                    existingAssignment.ModifiedBy = "System (Reuse)";
                }
                else
                {
                    var reuseAssignment = new DeviceUserAssignment
                    {
                        Id = Guid.NewGuid(),
                        DeviceId = deviceWithNACert.Id,
                        UserId = user.Id,
                        IsActive = false,
                        AssignedAt = DateTime.UtcNow,
                        AssignedBy = "System (Auto)",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };
                    context.DeviceUserAssignments.Add(reuseAssignment);
                }
                
                await context.SaveChangesAsync();
                
                _logger.LogInformation($"Reused device with N/A cert: MAC={request.MacAddress}, User={user.Username}, DeviceId={deviceWithNACert.Id}");
                
                return Ok(new DeviceRegistrationResponse
                {
                    Success = true,
                    Status = DeviceStatus.PendingApproval,
                    Message = "Đăng ký thiết bị thành công! Vui lòng chờ Admin phê duyệt.",
                    DeviceId = deviceWithNACert.Id
                });
            }

            // Create new device registration
            var newDevice = new UserDevice
            {
                Id = Guid.NewGuid(),
                DeviceName = request.DeviceName ?? "Unknown Device",
                MacAddress = request.MacAddress,
                DeviceStatus = DeviceStatus.PendingApproval,
                CertificateThumbprint = $"MAC-{request.MacAddress}", // Use MAC as unique identifier instead of "N/A"
                CertificateSubject = $"CN={request.MacAddress}",
                IsApproved = false,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.Username,
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            context.UserDevices.Add(newDevice);

            // Auto-create assignment for the user who registered
            var newAssignment = new DeviceUserAssignment
            {
                Id = Guid.NewGuid(),
                DeviceId = newDevice.Id,
                UserId = user.Id,
                IsActive = false, // Will be activated when approved
                AssignedAt = DateTime.UtcNow,
                AssignedBy = "System (Auto)",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            context.DeviceUserAssignments.Add(newAssignment);

            await context.SaveChangesAsync();

            _logger.LogInformation($"Device registered successfully: MAC={request.MacAddress}, User={user.Username}, DeviceId={newDevice.Id}");

            return Ok(new DeviceRegistrationResponse
            {
                Success = true,
                Status = DeviceStatus.PendingApproval,
                Message = "Đăng ký thiết bị thành công! Vui lòng chờ Admin phê duyệt.",
                DeviceId = newDevice.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering device");
            return StatusCode(500, new DeviceRegistrationResponse
            {
                Success = false,
                Message = $"Lỗi đăng ký thiết bị: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Check device registration status
    /// </summary>
    [HttpGet("device-status/{deviceId}")]
    [AllowAnonymous]
    public async Task<ActionResult<DeviceStatusResponse>> GetDeviceStatus(Guid deviceId)
    {
        try
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync();

            var device = await context.UserDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && !d.IsDeleted);

            if (device == null)
            {
                return NotFound(new DeviceStatusResponse
                {
                    Success = false,
                    Message = "Không tìm thấy thiết bị"
                });
            }

            var assignment = await context.DeviceUserAssignments
                .FirstOrDefaultAsync(a => a.DeviceId == deviceId && a.IsActive && !a.IsDeleted);

            var status = device.IsRevoked ? DeviceStatus.Rejected :
                        device.IsApproved ? DeviceStatus.Approved :
                        DeviceStatus.PendingApproval;

            string message = status switch
            {
                DeviceStatus.PendingApproval => "Thiết bị đang chờ phê duyệt",
                DeviceStatus.Approved when assignment != null => "Thiết bị đã được phê duyệt và bạn đã được phân quyền",
                DeviceStatus.Approved => "Thiết bị đã được phê duyệt nhưng bạn chưa được phân quyền",
                DeviceStatus.Rejected => "Thiết bị đã bị từ chối",
                _ => "Trạng thái không xác định"
            };

            return Ok(new DeviceStatusResponse
            {
                Success = true,
                Status = status,
                Message = message,
                IsApproved = device.IsApproved,
                IsRevoked = device.IsRevoked,
                IsUserAssigned = assignment != null,
                DeviceName = device.DeviceName,
                MacAddress = device.MacAddress,
                ApprovedAt = device.ApprovedAt,
                ApprovedBy = device.ApprovedBy
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device status");
            return StatusCode(500, new DeviceStatusResponse
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Download Desktop Login App executable
    /// This endpoint is public (AllowAnonymous) to allow employee users to download the login app
    /// </summary>
    [HttpGet("download-desktop-login-app")]
    [AllowAnonymous]
    public IActionResult DownloadDesktopLoginApp()
    {
        try
        {
            var exePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DesktopLoginApp", "DesktopLoginApp.exe");
            
            if (!System.IO.File.Exists(exePath))
            {
                _logger.LogError($"Desktop Login App not found at: {exePath}");
                return NotFound(new { message = "Desktop Login App không tìm thấy. Vui lòng liên hệ Admin." });
            }

            var fileBytes = System.IO.File.ReadAllBytes(exePath);
            return File(fileBytes, "application/octet-stream", "DesktopLoginApp.exe");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading Desktop Login App");
            return StatusCode(500, new { message = "Lỗi khi tải Desktop Login App" });
        }
    }

    #endregion
}
