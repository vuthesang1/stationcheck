using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Models;
using System.Security.Claims;

namespace StationCheck.Middleware
{
    /// <summary>
    /// Middleware for device access control validation with MAC address authentication
    /// 
    /// Validates that authenticated users have active device assignments.
    /// For StationEmployee: Validates device is approved, not revoked, and user is assigned.
    /// For Admin/Manager: Bypasses device validation.
    /// Returns 401 Unauthorized if validation fails, forcing logout.
    /// Works with Blazor Server cookie-based authentication.
    /// </summary>
    public class AccessControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AccessControlMiddleware> _logger;

        public AccessControlMiddleware(RequestDelegate next, ILogger<AccessControlMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var path = context.Request.Path.Value ?? "";

            // Skip validation for non-protected endpoints
            if (ShouldSkipValidation(path))
            {
                await _next(context);
                return;
            }

            // Check if user is authenticated
            var isAuthenticated = context.User?.Identity?.IsAuthenticated == true;

            if (!isAuthenticated)
            {
                // User NOT authenticated (cookie expired or missing)
                // Check if this is a browser request
                var acceptHeader = context.Request.Headers["Accept"].ToString();
                var isBrowserRequest = acceptHeader.Contains("text/html");

                if (isBrowserRequest)
                {
                    // Browser: Redirect to login
                    _logger.LogInformation("[AccessControl] Unauthenticated browser request to {Path}, redirecting to /login", path);
                    context.Response.Redirect("/login?returnUrl=" + Uri.EscapeDataString(path), permanent: false);
                    return;
                }
                else
                {
                    // API: Return 401 JSON
                    _logger.LogWarning("[AccessControl] Unauthenticated API request to {Path}, returning 401", path);
                    await ReturnUnauthorizedJsonAsync(context, "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
                    return;
                }
            }

            // Only validate if user is authenticated (context.User populated from auth cookie or JWT)
            if (isAuthenticated)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
                var macAddress = context.User.FindFirst("MacAddress")?.Value;

                // ✅ FIX: Validate user still exists in database
                // This catches cases where:
                // 1. Cookie from old session (user A) conflicts with new login (user B)
                // 2. User was deleted but cookie still exists
                if (!string.IsNullOrEmpty(userId))
                {
                    var userExists = await dbContext.Users
                        .AnyAsync(u => u.Id == userId && !u.IsDeleted);

                    if (!userExists)
                    {
                        _logger.LogWarning("[AccessControl] User {UserId} in cookie not found in database. Clearing cookies and returning 401.", userId);
                        await ClearAuthenticationAndReturnUnauthorizedAsync(context, "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.");
                        return;
                    }
                }

                // Admin and Manager bypass device validation
                if (userRole == UserRole.Admin.ToString() || userRole == UserRole.Manager.ToString() || userRole == UserRole.StationEmployee.ToString())
                {
                    _logger.LogDebug("[AccessControl] User {UserId} is Admin/Manager - skipping device validation", userId);
                    await _next(context);
                    return;
                }

                // StationEmployee must have valid device
                if (!string.IsNullOrEmpty(userId))
                {
                    // Get MAC address from claims (new authentication method)
                    if (string.IsNullOrEmpty(macAddress))
                    {
                        _logger.LogWarning("[AccessControl] User {UserId} has no MAC address in claims on {Path}. Returning 401.", userId, path);
                        await ReturnUnauthorizedJsonAsync(context, "Không có thông tin thiết bị. Vui lòng đăng nhập qua ứng dụng máy tính.");
                        return;
                    }

                    // Validate device status and user assignment
                    var deviceQuery = await dbContext.UserDevices
                        .Where(d => d.MacAddress == macAddress && !d.IsDeleted)
                        .Select(d => new
                        {
                            d.Id,
                            d.IsApproved,
                            d.IsRevoked,
                            HasAssignment = d.Assignments!.Any(a => a.UserId == userId && a.IsActive && !a.IsDeleted)
                        })
                        .FirstOrDefaultAsync();

                    if (deviceQuery == null)
                    {
                        _logger.LogWarning("[AccessControl] Device with MAC {MacAddress} not found. Returning 401.", macAddress);
                        await ReturnUnauthorizedJsonAsync(context, "Thiết bị chưa được đăng ký.");
                        return;
                    }

                    if (!deviceQuery.IsApproved)
                    {
                        _logger.LogWarning("[AccessControl] Device {DeviceId} not approved. Clearing cookies and returning 401.", deviceQuery.Id);
                        await ClearAuthenticationAndReturnUnauthorizedAsync(context, "Thiết bị chưa được duyệt.");
                        return;
                    }

                    if (deviceQuery.IsRevoked)
                    {
                        _logger.LogWarning("[AccessControl] Device {DeviceId} is revoked. Clearing cookies and returning 401.", deviceQuery.Id);
                        await ClearAuthenticationAndReturnUnauthorizedAsync(context, "Thiết bị đã bị thu hồi. Vui lòng liên hệ quản trị viên.");
                        return;
                    }

                    if (!deviceQuery.HasAssignment)
                    {
                        _logger.LogWarning("[AccessControl] User {UserId} not assigned to device {DeviceId}. Clearing cookies and returning 401.", 
                            userId, deviceQuery.Id);
                        await ClearAuthenticationAndReturnUnauthorizedAsync(context, "Bạn đã bị gỡ quyền truy cập khỏi thiết bị này. Vui lòng liên hệ quản trị viên.");
                        return;
                    }

                    _logger.LogDebug("[AccessControl] User {UserId} validated for device {DeviceId} (MAC: {MacAddress})", 
                        userId, deviceQuery.Id, macAddress);
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Return 401 Unauthorized with JSON response for proper font display
        /// </summary>
        private async Task ReturnUnauthorizedJsonAsync(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Headers.Append("X-Force-Logout", "true");
            
            var response = System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = message,
                requiresLogout = true
            });
            
            await context.Response.WriteAsync(response);
        }

        /// <summary>
        /// Clear authentication cookies and return 401 Unauthorized
        /// Used when device is revoked, not approved, or user assignment removed
        /// </summary>
        private async Task ClearAuthenticationAndReturnUnauthorizedAsync(HttpContext context, string message)
        {
            _logger.LogInformation("[AccessControl] Clearing authentication cookies before returning 401");
            
            // IMPORTANT: Clear cookies BEFORE setting response status
            // Once response starts, headers/cookies cannot be modified
            
            // Force delete cookies with proper options
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(-1), // Expire in the past
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };
            
            context.Response.Cookies.Delete("StationCheck.Auth", cookieOptions);
            context.Response.Cookies.Delete(".AspNetCore.Cookies", cookieOptions);
            context.Response.Cookies.Delete(".AspNetCore.Identity.Application", cookieOptions);
            
            // Also append Set-Cookie headers to force clear
            context.Response.Headers.Append("Set-Cookie", "StationCheck.Auth=; Path=/; Expires=Thu, 01 Jan 1970 00:00:00 GMT; HttpOnly; Secure; SameSite=Lax");
            context.Response.Headers.Append("Set-Cookie", ".AspNetCore.Cookies=; Path=/; Expires=Thu, 01 Jan 1970 00:00:00 GMT; HttpOnly; Secure; SameSite=Lax");
            context.Response.Headers.Append("Set-Cookie", ".AspNetCore.Identity.Application=; Path=/; Expires=Thu, 01 Jan 1970 00:00:00 GMT; HttpOnly; Secure; SameSite=Lax");
            
            // Sign out from cookie authentication (this also clears cookies)
            try
            {
                await context.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[AccessControl] Error signing out from cookie authentication");
            }
            
            // Clear session if exists
            if (context.Session != null)
            {
                try
                {
                    context.Session.Clear();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[AccessControl] Error clearing session");
                }
            }
            
            // Check if this is a browser request (HTML) or API request (JSON)
            var acceptHeader = context.Request.Headers["Accept"].ToString();
            var isBrowserRequest = acceptHeader.Contains("text/html");
            
            if (isBrowserRequest)
            {
                // Browser request: Redirect to login page
                _logger.LogInformation("[AccessControl] Browser request detected, redirecting to /login");
                context.Response.Redirect("/login?reason=session_expired", permanent: false);
            }
            else
            {
                // API request: Return JSON 401
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.Headers.Append("X-Force-Logout", "true");
                context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate");
                
                var response = System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = message,
                    requiresLogout = true
                });
                
                await context.Response.WriteAsync(response);
            }
        }

        /// <summary>
        /// Determine if path should skip device access validation
        /// </summary>
        private bool ShouldSkipValidation(string path)
        {
            var lowerPath = path.ToLowerInvariant();
            
            // Public/Auth endpoints - no validation needed
            if (lowerPath.StartsWith("/login") ||
                lowerPath.StartsWith("/tokenlogin") ||
                lowerPath.StartsWith("/register") ||
                lowerPath.StartsWith("/forgot-password") ||
                lowerPath.StartsWith("/desktop-login-callback") ||
                lowerPath.StartsWith("/api/auth"))
            {
                return true;
            }

            // Static files - no validation
            if (lowerPath.StartsWith("/_framework") ||
                lowerPath.StartsWith("/_content") ||
                lowerPath.StartsWith("/css") ||
                lowerPath.StartsWith("/lib") ||
                lowerPath.StartsWith("/js") ||
                lowerPath.StartsWith("/img") ||
                lowerPath.StartsWith("/fonts"))
            {
                return true;
            }

            // Blazor internal - skip (authorization policy handles these)
            if (lowerPath.StartsWith("/_blazor"))
            {
                return true;
            }

            return false;
        }
    }

    public static class AccessControlMiddlewareExtensions
    {
        public static IApplicationBuilder UseAccessControl(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AccessControlMiddleware>();
        }
    }
}
