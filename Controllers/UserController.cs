using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StationCheck.Interfaces;
using StationCheck.Models;

namespace StationCheck.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<UserInfo>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationUser>> GetUser(string id)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUserRoleStr = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (!Enum.TryParse<UserRole>(currentUserRoleStr, out var currentUserRole))
        {
            return Unauthorized();
        }

        // Users can only view their own profile, Admin/Manager can view anyone
        if (currentUserRole != UserRole.Admin && currentUserRole != UserRole.Manager && currentUserId != id)
        {
            return Forbid();
        }

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.PasswordHash = string.Empty;
        return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApplicationUser>> CreateUser([FromBody] RegisterRequest request)
    {
        var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var currentUserRoleStr = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (!Enum.TryParse<UserRole>(currentUserRoleStr, out var currentUserRole))
        {
            return Unauthorized();
        }

        if (!await _userService.CanModifyUserAsync(currentUserRole, request.Role))
        {
            return Forbid();
        }

        var user = await _userService.CreateUserAsync(request, currentUsername);
        user.PasswordHash = string.Empty;

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApplicationUser>> UpdateUser(string id, [FromBody] UpdateUserRequest request)
    {
        var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var currentUserRoleStr = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (!Enum.TryParse<UserRole>(currentUserRoleStr, out var currentUserRole))
        {
            return Unauthorized();
        }

        var targetUser = await _userService.GetUserByIdAsync(id);
        if (targetUser == null)
        {
            return NotFound();
        }

        if (!await _userService.CanModifyUserAsync(currentUserRole, targetUser.Role))
        {
            return Forbid();
        }

        if (!await _userService.CanModifyUserAsync(currentUserRole, request.Role))
        {
            return Forbid();
        }

        var updatedUser = await _userService.UpdateUserAsync(id, request, currentUsername);
        updatedUser.PasswordHash = string.Empty;
        
        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var currentUserRoleStr = User.FindFirst(ClaimTypes.Role)?.Value;
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Enum.TryParse<UserRole>(currentUserRoleStr, out var currentUserRole))
        {
            return Unauthorized();
        }

        if (currentUserId == id)
        {
            return BadRequest(new { message = "Cannot delete your own account" });
        }

        var targetUser = await _userService.GetUserByIdAsync(id);
        if (targetUser == null)
        {
            return NotFound();
        }

        if (!await _userService.CanModifyUserAsync(currentUserRole, targetUser.Role))
        {
            return Forbid();
        }

        await _userService.DeleteUserAsync(id, currentUsername);
        return Ok(new { message = "User deleted successfully" });
    }
}
