using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using StationCheck.Data;
using System.Security.Claims;

namespace StationCheck.Services
{
    public interface IForceLogoutService
    {
        Task CheckAndLogoutIfNeededAsync(
            ClaimsPrincipal user,
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            HttpClient httpClient,
            NavigationManager navigation,
            IJSRuntime js,
            ILogger logger,
            Timer? accessCheckTimer,
            IHttpContextAccessor? httpContextAccessor = null);
    }

    public class ForceLogoutService : IForceLogoutService
    {
        public async Task CheckAndLogoutIfNeededAsync(
            ClaimsPrincipal user,
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            HttpClient httpClient,
            NavigationManager navigation,
            IJSRuntime js,
            ILogger logger,
            Timer? accessCheckTimer,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            try
            {
                // Admin và Manager không cần check - họ có quyền truy cập tất cả
                if (user?.IsInRole("Admin") == true || user?.IsInRole("Manager") == true)
                {
                    logger.LogInformation($"[ForceLogoutService] User has Admin/Manager role - skipping access check");
                    return;
                }

                // Extract user ID from claims (try multiple sources)
                var currentUserId = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? user?.FindFirst("sub")?.Value
                    ?? user?.Identity?.Name;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    logger.LogInformation($"[ForceLogoutService] No user ID found in claims");
                    return;
                }

                // Get certificate thumbprint from Claims (added during login)
                string? certificateThumbprint = user?.FindFirst("CertificateThumbprint")?.Value;
                
                // Fallback: try HttpContext if available
                if (string.IsNullOrEmpty(certificateThumbprint) && 
                    httpContextAccessor?.HttpContext?.Items.ContainsKey("CertificateThumbprint") == true)
                {
                    certificateThumbprint = httpContextAccessor.HttpContext.Items["CertificateThumbprint"]?.ToString();
                }
                
                logger.LogInformation($"[ForceLogoutService] Certificate thumbprint: {certificateThumbprint ?? "NULL"}");

                if (string.IsNullOrEmpty(certificateThumbprint))
                {
                    logger.LogWarning($"[ForceLogoutService] No certificate thumbprint found - cannot validate device");
                    return;
                }

                logger.LogInformation($"[ForceLogoutService] Checking access for user: {currentUserId}, device: {certificateThumbprint}");

                await using var context = dbContextFactory.CreateDbContext();

                // Find the current device being used
                var currentDevice = await context.UserDevices
                    .FirstOrDefaultAsync(d => d.CertificateThumbprint == certificateThumbprint && !d.IsDeleted);

                if (currentDevice == null)
                {
                    logger.LogWarning($"[ForceLogoutService] Device {certificateThumbprint} not found in database - FORCING LOGOUT");
                    await PerformLogoutAsync(httpClient, navigation, js, logger, accessCheckTimer, "Thiết bị không tồn tại trong hệ thống.");
                    return;
                }

                // Check if device is revoked
                if (currentDevice.IsRevoked)
                {
                    logger.LogWarning($"[ForceLogoutService] Device {certificateThumbprint} is REVOKED - FORCING LOGOUT");
                    await PerformLogoutAsync(httpClient, navigation, js, logger, accessCheckTimer, "Thiết bị đã bị vô hiệu hóa. Bạn sẽ bị logout.");
                    return;
                }

                // Check if device is approved (for StationEmployee)
                if (!currentDevice.IsApproved)
                {
                    logger.LogWarning($"[ForceLogoutService] Device {certificateThumbprint} is NOT APPROVED - FORCING LOGOUT");
                    await PerformLogoutAsync(httpClient, navigation, js, logger, accessCheckTimer, "Thiết bị chưa được phê duyệt.");
                    return;
                }

                // Check if user has active assignment for this device
                var hasAssignment = await context.DeviceUserAssignments
                    .AnyAsync(a => a.UserId == currentUserId 
                        && a.DeviceId == currentDevice.Id
                        && a.IsActive 
                        && !a.IsDeleted);

                if (!hasAssignment)
                {
                    logger.LogWarning($"[ForceLogoutService] User {currentUserId} has NO ASSIGNMENT for device {certificateThumbprint} - FORCING LOGOUT");
                    await PerformLogoutAsync(httpClient, navigation, js, logger, accessCheckTimer, "Bạn không có quyền sử dụng thiết bị này.");
                    return;
                }

                logger.LogInformation($"[ForceLogoutService] All checks passed - user {currentUserId} has valid access");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[ForceLogoutService] Access check error: {ex.Message}");
            }
        }

        private async Task PerformLogoutAsync(
            HttpClient httpClient,
            NavigationManager navigation,
            IJSRuntime js,
            ILogger logger,
            Timer? accessCheckTimer,
            string reason)
        {
            // Dispose timer immediately to prevent further checks
            if (accessCheckTimer != null)
            {
                logger.LogInformation($"[ForceLogoutService] Disposing access check timer");
                accessCheckTimer.Dispose();
            }

            // Show alert with reason
            try
            {
                await js.InvokeVoidAsync("alert", reason);
            }
            catch
            {
                // Ignore alert errors during circuit disconnect
            }

            // Call logout endpoint
            try
            {
                logger.LogInformation($"[ForceLogoutService] Calling logout endpoint");
                
                if (httpClient.BaseAddress == null)
                {
                    httpClient.BaseAddress = new Uri(navigation.BaseUri);
                    logger.LogInformation($"[ForceLogoutService] Set HttpClient BaseAddress");
                }

                var content = new StringContent("", System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("/api/auth/logout", content);
                logger.LogInformation($"[ForceLogoutService] Logout response: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[ForceLogoutService] Error calling logout endpoint: {ex.Message}");
            }

            // Clear localStorage
            try
            {
                logger.LogInformation($"[ForceLogoutService] Clearing localStorage tokens");
                await js.InvokeVoidAsync("localStorage.removeItem", "jwt_token");
                await js.InvokeVoidAsync("localStorage.removeItem", "user_info");
                await js.InvokeVoidAsync("localStorage.removeItem", "refresh_token");
                logger.LogInformation($"[ForceLogoutService] localStorage cleared");
            }
            catch (OperationCanceledException ex)
            {
                logger.LogWarning(ex, $"[ForceLogoutService] localStorage clear was canceled (circuit disconnect)");
            }
            catch (JSDisconnectedException ex)
            {
                logger.LogWarning(ex, $"[ForceLogoutService] Circuit disconnected, cannot clear localStorage");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[ForceLogoutService] Error clearing localStorage: {ex.Message}");
            }

            // Redirect to login
            try
            {
                logger.LogInformation($"[ForceLogoutService] Redirecting to login page");
                navigation.NavigateTo("/login", forceLoad: true);
            }
            catch (NavigationException ex)
            {
                logger.LogWarning(ex, $"[ForceLogoutService] Navigation error (expected during circuit disconnect)");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[ForceLogoutService] Error during navigation: {ex.Message}");
            }
        }
    }
}
