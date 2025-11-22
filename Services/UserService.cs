using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Interfaces;
using StationCheck.Models;

namespace StationCheck.Services;

public class UserService : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly ILogger<UserService> _logger;

    public UserService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<UserService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<List<UserInfo>> GetAllUsersAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var users = await context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
            
        return users.Select(u => new UserInfo
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        }).ToList();
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.FindAsync(id);
    }

    public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ApplicationUser> CreateUserAsync(RegisterRequest request, string createdBy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        // Check if email already exists (including soft-deleted users)
        var existingUser = await context.Users
            .IgnoreQueryFilters() // Include soft-deleted users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            if (existingUser.IsDeleted)
            {
                throw new InvalidOperationException($"Email '{request.Email}' đã tồn tại trong hệ thống (user đã bị xóa trước đó). Vui lòng sử dụng email khác hoặc liên hệ quản trị viên để khôi phục tài khoản.");
            }
            else
            {
                throw new InvalidOperationException($"Email '{request.Email}' đã được sử dụng bởi user khác.");
            }
        }

        // Check if username already exists
        var existingUsername = await context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (existingUsername != null)
        {
            if (existingUsername.IsDeleted)
            {
                throw new InvalidOperationException($"Username '{request.Username}' đã tồn tại trong hệ thống (user đã bị xóa trước đó). Vui lòng sử dụng username khác hoặc liên hệ quản trị viên để khôi phục tài khoản.");
            }
            else
            {
                throw new InvalidOperationException($"Username '{request.Username}' đã được sử dụng.");
            }
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

        _logger.LogInformation($"User {user.Username} created by {createdBy}");

        return user;
    }

    public async Task<ApplicationUser> UpdateUserAsync(string id, UpdateUserRequest request, string modifiedBy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users.FindAsync(id);
        if (user == null)
            throw new InvalidOperationException($"User {id} not found");

        // Check if email is being changed and if new email already exists
        if (user.Email != request.Email)
        {
            var existingUser = await context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != id);

            if (existingUser != null)
            {
                if (existingUser.IsDeleted)
                {
                    throw new InvalidOperationException($"Email '{request.Email}' đã tồn tại trong hệ thống (user đã bị xóa trước đó). Vui lòng sử dụng email khác.");
                }
                else
                {
                    throw new InvalidOperationException($"Email '{request.Email}' đã được sử dụng bởi user khác.");
                }
            }
        }

        user.Email = request.Email;
        user.FullName = request.FullName;
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.ModifiedBy = modifiedBy;
        user.ModifiedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        _logger.LogInformation($"User {user.Username} updated by {modifiedBy}");

        return user;
    }

    public async Task DeleteUserAsync(string id, string deletedBy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var user = await context.Users.FindAsync(id);
        if (user == null)
            throw new InvalidOperationException($"User {id} not found");

        // Soft delete
        user.IsActive = false;
        user.ModifiedBy = deletedBy;
        user.ModifiedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        _logger.LogInformation($"User {user.Username} deleted (soft) by {deletedBy}");
    }

    public async Task<bool> CanModifyUserAsync(UserRole currentUserRole, UserRole targetUserRole)
    {
        await Task.CompletedTask;

        // Admin can modify anyone
        if (currentUserRole == UserRole.Admin)
            return true;

        // Manager can modify StationEmployee only (not Admin or other Managers)
        if (currentUserRole == UserRole.Manager && targetUserRole == UserRole.StationEmployee)
            return true;

        // StationEmployee cannot modify anyone
        return false;
    }
}
