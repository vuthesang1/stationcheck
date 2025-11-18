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

    public AuthService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _contextFactory = contextFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
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

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        _logger.LogInformation($"Login attempt for user '{user.Username}': Password={request.Password}, Hash={user.PasswordHash}, Valid={isPasswordValid}");

        if (!isPasswordValid)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

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

        var token = GenerateJwtToken(user);
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

        var token = GenerateJwtToken(user);
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

        // Revoke old refresh token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var newToken = GenerateJwtToken(refreshToken.User);
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

    public string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourVerySecureSecretKeyForJWT_MinimumLength32Characters!";
        var issuer = jwtSettings["Issuer"] ?? "StationCheck";
        var audience = jwtSettings["Audience"] ?? "StationCheckApp";
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("FullName", user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

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
