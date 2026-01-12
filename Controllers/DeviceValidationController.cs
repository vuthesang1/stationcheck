using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StationCheck.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace StationCheck.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeviceValidationController : ControllerBase
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ILogger<DeviceValidationController> _logger;

    public DeviceValidationController(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        ILogger<DeviceValidationController> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Validate if current user still has access to the device
    /// Called periodically by Desktop App and Web App
    /// </summary>
    [HttpGet("validate-device-access")]
    public async Task<ActionResult<DeviceValidationResponse>> ValidateDeviceAccess()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var macAddress = User.FindFirst("MacAddress")?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(macAddress))
            {
                _logger.LogWarning($"[ValidateDeviceAccess] Missing userId or macAddress in token. User: {username}");
                return Unauthorized(new DeviceValidationResponse
                {
                    IsValid = false,
                    Reason = "InvalidToken",
                    Message = "Token không hợp lệ hoặc thiếu thông tin"
                });
            }

            using var context = await _dbContextFactory.CreateDbContextAsync();

            // Check if device exists and is approved
            var device = await context.UserDevices
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MacAddress == macAddress);

            if (device == null)
            {
                _logger.LogWarning($"[ValidateDeviceAccess] Device not found. MAC: {macAddress}, User: {username}");
                return Ok(new DeviceValidationResponse
                {
                    IsValid = false,
                    Reason = "DeviceNotFound",
                    Message = "Thiết bị không tồn tại trong hệ thống"
                });
            }

            if (!device.IsApproved)
            {
                _logger.LogWarning($"[ValidateDeviceAccess] Device not approved. MAC: {macAddress}, User: {username}");
                return Ok(new DeviceValidationResponse
                {
                    IsValid = false,
                    Reason = "DeviceNotApproved",
                    Message = "Thiết bị chưa được phê duyệt"
                });
            }

            if (device.IsRevoked)
            {
                _logger.LogWarning($"[ValidateDeviceAccess] Device revoked. MAC: {macAddress}, User: {username}");
                return Ok(new DeviceValidationResponse
                {
                    IsValid = false,
                    Reason = "DeviceRevoked",
                    Message = "Thiết bị đã bị thu hồi quyền truy cập"
                });
            }

            // Check if user is still assigned to this device
            var assignment = await context.DeviceUserAssignments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => 
                    a.DeviceId == device.Id && 
                    a.UserId == userId &&
                    !a.IsDeleted);

            if (assignment == null)
            {
                _logger.LogWarning($"[ValidateDeviceAccess] User not assigned to device. MAC: {macAddress}, User: {username}");
                return Ok(new DeviceValidationResponse
                {
                    IsValid = false,
                    Reason = "UserNotAssigned",
                    Message = "Bạn không còn quyền truy cập thiết bị này"
                });
            }

            // Check if user account is still active
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning($"[ValidateDeviceAccess] User inactive or not found. User: {username}");
                return Ok(new DeviceValidationResponse
                {
                    IsValid = false,
                    Reason = "UserInactive",
                    Message = "Tài khoản của bạn đã bị vô hiệu hóa"
                });
            }

            // All validations passed
            _logger.LogInformation($"[ValidateDeviceAccess] ✅ Valid. User: {username}, MAC: {macAddress}");
            return Ok(new DeviceValidationResponse
            {
                IsValid = true,
                Reason = "Valid",
                Message = "Quyền truy cập hợp lệ"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ValidateDeviceAccess] Error validating device access");
            return StatusCode(500, new DeviceValidationResponse
            {
                IsValid = false,
                Reason = "ServerError",
                Message = "Lỗi server khi kiểm tra quyền truy cập"
            });
        }
    }
}

public class DeviceValidationResponse
{
    public bool IsValid { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
