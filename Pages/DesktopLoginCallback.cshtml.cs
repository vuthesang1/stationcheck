using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StationCheck.Data;
using StationCheck.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StationCheck.Pages
{
    [AllowAnonymous]
    public class DesktopLoginCallbackModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DesktopLoginCallbackModel> _logger;
        private readonly ApplicationDbContext _context;

        public string StatusTitle { get; set; } = "Đang xử lý đăng nhập...";
        public string StatusMessage { get; set; } = "Vui lòng chờ trong giây lát";
        public string? ErrorMessage { get; set; }
        public string? RedirectUrl { get; set; }
        public int RedirectDelayMs { get; set; } = 1000;

        public DesktopLoginCallbackModel(
            IConfiguration configuration, 
            ILogger<DesktopLoginCallbackModel> logger,
            ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string? token)
        {
            _logger.LogInformation("=== DESKTOP LOGIN CALLBACK STARTED ===");
            _logger.LogInformation("Token present: {TokenPresent}", !string.IsNullOrEmpty(token));
            _logger.LogInformation("Request URL: {RequestUrl}", HttpContext.Request.GetDisplayUrl());

            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No token provided in callback");
                    StatusTitle = "Lỗi đăng nhập";
                    StatusMessage = "Không tìm thấy token xác thực";
                    ErrorMessage = "Vui lòng thử đăng nhập lại từ Desktop App";
                    RedirectUrl = "/login";
                    RedirectDelayMs = 3000;
                    return Page();
                }

                _logger.LogInformation("Validating JWT token...");

                // Validate JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret key not configured"));

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

                ClaimsPrincipal principal;
                try
                {
                    principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                    _logger.LogInformation("JWT token validated successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "JWT token validation failed");
                    StatusTitle = "Token không hợp lệ";
                    StatusMessage = "Token xác thực đã hết hạn hoặc không đúng";
                    ErrorMessage = "Vui lòng đăng nhập lại từ Desktop App";
                    RedirectUrl = "/login";
                    RedirectDelayMs = 3000;
                    return Page();
                }

                // Extract claims
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = principal.FindFirst("name")?.Value;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;
                var macAddress = principal.FindFirst("MacAddress")?.Value;
                var firstName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
                var lastName = principal.FindFirst(ClaimTypes.Surname)?.Value;

                _logger.LogInformation("Extracted claims - UserId: {UserId}, Username: {Username}, Role: {Role}, MAC: {MAC}",
                    userId, username, role, macAddress);

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                {
                    _logger.LogWarning("Missing required claims in token");
                    StatusTitle = "Lỗi token";
                    StatusMessage = "Token thiếu thông tin bắt buộc";
                    ErrorMessage = "Vui lòng thử đăng nhập lại";
                    RedirectUrl = "/login";
                    RedirectDelayMs = 3000;
                    return Page();
                }

                // Create claims for Cookie authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                };

                if (!string.IsNullOrEmpty(macAddress))
                {
                    claims.Add(new Claim("MacAddress", macAddress));
                }

                if (!string.IsNullOrEmpty(firstName))
                {
                    claims.Add(new Claim(ClaimTypes.GivenName, firstName));
                }

                if (!string.IsNullOrEmpty(lastName))
                {
                    claims.Add(new Claim(ClaimTypes.Surname, lastName));
                }

                _logger.LogInformation("Extracted {ClaimCount} claims, now checking device status...", claims.Count);

                // IMPORTANT: Check device status BEFORE creating cookie
                // Only create cookie if device is approved and user is assigned
                if (!string.IsNullOrEmpty(macAddress))
                {
                    var device = await _context.UserDevices
                        .Where(d => d.MacAddress == macAddress && !d.IsDeleted)
                        .Include(d => d.Assignments)
                        .OrderByDescending(d => d.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (device == null)
                    {
                        _logger.LogWarning("Device {MAC} not found in database", macAddress);
                        StatusTitle = "Thiết bị chưa đăng ký";
                        StatusMessage = "Thiết bị của bạn chưa được đăng ký trong hệ thống.";
                        ErrorMessage = "Vui lòng liên hệ Admin.";
                        RedirectUrl = null;
                        RedirectDelayMs = 0;
                        return Page();
                    }

                    _logger.LogInformation("Device found: IsApproved={IsApproved}, IsRevoked={IsRevoked}", 
                        device.IsApproved, device.IsRevoked);

                    if (!device.IsApproved)
                    {
                        _logger.LogWarning("Device {MAC} is pending approval. Not creating cookie.", macAddress);
                        StatusTitle = "Thiết bị đang chờ phê duyệt";
                        StatusMessage = "Thiết bị của bạn đã được đăng ký thành công. Vui lòng chờ Admin phê duyệt để có thể truy cập hệ thống.";
                        ErrorMessage = "Bạn sẽ nhận được thông báo khi thiết bị được duyệt.";
                        RedirectUrl = null;
                        RedirectDelayMs = 0;
                        return Page();
                    }

                    if (device.IsRevoked)
                    {
                        _logger.LogWarning("Device {MAC} has been revoked. Not creating cookie.", macAddress);
                        StatusTitle = "Thiết bị bị thu hồi";
                        StatusMessage = "Thiết bị của bạn đã bị thu hồi quyền truy cập.";
                        ErrorMessage = "Vui lòng liên hệ Admin để biết thêm chi tiết.";
                        RedirectUrl = null;
                        RedirectDelayMs = 0;
                        return Page();
                    }

                    // Check if user is assigned to this device
                    var hasAssignment = device.Assignments!.Any(a => 
                        a.UserId == userId && 
                        a.IsActive && 
                        !a.IsDeleted);

                    if (!hasAssignment)
                    {
                        _logger.LogWarning("User {UserId} not assigned to device {DeviceId}. Not creating cookie.", userId, device.Id);
                        StatusTitle = "Chưa được phân quyền";
                        StatusMessage = "Thiết bị đã được phê duyệt nhưng bạn chưa được phân quyền sử dụng.";
                        ErrorMessage = "Vui lòng liên hệ Admin để được phân quyền truy cập.";
                        RedirectUrl = null;
                        RedirectDelayMs = 0;
                        return Page();
                    }
                }

                _logger.LogInformation("Device validation passed. Creating Cookie authentication...");

                // Create Cookie authentication ONLY after device validation passed
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true
                    // Do NOT set ExpiresUtc - let cookie options handle it with SlidingExpiration
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("=== DESKTOP LOGIN CALLBACK SUCCESS ===");
                _logger.LogInformation("User {Username} authenticated with cookie successfully", username);

                // Success - device is approved and user is assigned, redirect to home
                StatusTitle = "Đăng nhập thành công!";
                StatusMessage = $"Chào mừng {firstName ?? username}";
                RedirectUrl = "/";
                RedirectDelayMs = 1000;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== DESKTOP LOGIN CALLBACK FAILED ===");
                StatusTitle = "Lỗi hệ thống";
                StatusMessage = "Đã xảy ra lỗi trong quá trình xử lý";
                ErrorMessage = ex.Message;
                RedirectUrl = "/login";
                RedirectDelayMs = 3000;
                return Page();
            }
        }
    }
}
