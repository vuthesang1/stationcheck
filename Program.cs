using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StationCheck.BackgroundServices;
using StationCheck.Components;
using StationCheck.Data;
using StationCheck.Interfaces;
using StationCheck.Services;
using StationCheck.Models;
using StationCheck.Middleware;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Blazored.LocalStorage;

// Configure Serilog with separate log files for different services
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    // Web Application logs (exclude background services)
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(logEvent => 
            logEvent.Properties.ContainsKey("SourceContext") &&
            logEvent.Properties["SourceContext"].ToString().Contains("BackgroundServices"))
        .WriteTo.File(
            path: "Logs/webapp-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
            retainedFileCountLimit: 30))
    // AlertGenerationService logs
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(logEvent => 
            logEvent.Properties.ContainsKey("SourceContext") &&
            logEvent.Properties["SourceContext"].ToString().Contains("AlertGenerationService"))
        .WriteTo.File(
            path: "Logs/alert-generation-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            retainedFileCountLimit: 30))
    // MotionMonitoringService logs
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(logEvent => 
            logEvent.Properties.ContainsKey("SourceContext") &&
            logEvent.Properties["SourceContext"].ToString().Contains("MotionMonitoringService"))
        .WriteTo.File(
            path: "Logs/motion-monitoring-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
            retainedFileCountLimit: 30))
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
builder.Host.UseSerilog();

// Configure Kestrel to require client certificates for browser auto-detection
// Browser will automatically send matching certificate (CN=localhost) without prompting
// If no matching cert or multiple certs exist, browser will show selection dialog
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // HTTPS configuration - require client certificates to trigger browser auto-send
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        // AllowCertificate: Browser sends cert if available, but connection allowed without cert
        // This allows testing without certificate, and with self-signed DeviceInstaller certs
        // Change to RequireCertificate for production if you want to block non-certificate users
        httpsOptions.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
        
        // Custom validation - only accept certificates from DeviceInstaller
        // DeviceInstaller creates self-signed certs with CN=localhost (or CN=<hostname>)
        httpsOptions.ClientCertificateValidation = (certificate, chain, sslPolicyErrors) =>
        {
            if (certificate == null)
                return false;

            // Accept self-signed certificates from DeviceInstaller
            // Characteristics of DeviceInstaller certs:
            // 1. Self-signed (Issuer = Subject)
            // 2. CN contains hostname (localhost or computer name)
            // 3. Has Client Authentication EKU (1.3.6.1.5.5.7.3.2)
            
            var subject = certificate.Subject;
            var issuer = certificate.Issuer;
            
            // Check if self-signed (Issuer = Subject)
            bool isSelfSigned = subject.Equals(issuer, StringComparison.OrdinalIgnoreCase);
            
            // Check for Client Authentication EKU
            bool hasClientAuthEku = false;
            // Check for CUSTOM OID Extension (DeviceInstaller marker)
            bool hasDeviceInstallerOid = false;
            
            foreach (var extension in certificate.Extensions)
            {
                if (extension is System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension ekuExt)
                {
                    foreach (var eku in ekuExt.EnhancedKeyUsages)
                    {
                        if (eku.Value == "1.3.6.1.5.5.7.3.2") // Client Authentication
                        {
                            hasClientAuthEku = true;
                            break;
                        }
                    }
                }
                
                // Check for custom OID: 1.3.6.1.4.1.99999.1 (DeviceInstaller marker)
                if (extension.Oid?.Value == "1.3.6.1.4.1.99999.1")
                {
                    try
                    {
                        var value = System.Text.Encoding.UTF8.GetString(extension.RawData);
                        if (value.Contains("StationCheck-DeviceInstaller"))
                        {
                            hasDeviceInstallerOid = true;
                        }
                    }
                    catch
                    {
                        // Invalid extension data, skip
                    }
                }
            }
            
            // STRICT: Only accept certificates that have DeviceInstaller custom OID
            // DeviceInstaller format: CN=<deviceName>, OU=<hostname>
            // This is REVERSED from old format for better browser display
            bool hasValidCN = false;
            bool hasOUField = false;
            
            // Check if has OU field (DeviceInstaller pattern) - any value is OK
            if (subject.Contains(", OU=", StringComparison.OrdinalIgnoreCase))
            {
                hasOUField = true;
                hasValidCN = true; // If it has OU, consider CN valid (DeviceInstaller always adds OU)
            }
            
            // PRIMARY CHECK: MUST have DeviceInstaller custom OID (1.3.6.1.4.1.99999.1)
            // SECONDARY CHECKS: self-signed + client auth EKU + valid CN + OU field
            // This definitively identifies ONLY DeviceInstaller certs
            bool isValid = hasDeviceInstallerOid && isSelfSigned && hasClientAuthEku && hasValidCN && hasOUField;
            
            if (!isValid)
            {
                Log.Logger.Warning("Certificate rejected - Subject: {Subject}, Issuer: {Issuer}, SelfSigned: {SelfSigned}, ClientAuthEKU: {ClientAuthEKU}, ValidCN: {ValidCN}, HasDeviceInstallerOID: {HasOID}",
                    subject, issuer, isSelfSigned, hasClientAuthEku, hasValidCN, hasDeviceInstallerOid);
            }
            
            return isValid;
        };
    });
});

// Configure Certificate Forwarding for IIS
// When behind IIS, client certificates come via X-ARR-ClientCert header
builder.Services.AddCertificateForwarding(options =>
{
    options.CertificateHeader = "X-ARR-ClientCert";
    options.HeaderConverter = (headerValue) =>
    {
        if (string.IsNullOrWhiteSpace(headerValue))
            return null!;

        try
        {
            // IIS sends certificate as base64-encoded string
            byte[] bytes = Convert.FromBase64String(headerValue);
            return new System.Security.Cryptography.X509Certificates.X509Certificate2(bytes);
        }
        catch (Exception ex)
        {
            Log.Logger.Warning(ex, "Failed to parse client certificate from IIS header");
            return null!;
        }
    };
});

// ===== DATA PROTECTION - FIX "Unprotect ticket failed" =====
// Configure DataProtection with stable application name and persistent key storage
// This ensures cookie encryption is consistent across app restarts
var keysPath = Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys");
Directory.CreateDirectory(keysPath);

builder.Services.AddDataProtection()
    .SetApplicationName("StationCheck")
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90)); // Keys valid for 90 days

// Add Database - Use DbContextFactory để tránh concurrency issues
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm DbContext dạng Scoped để dùng với Dependency Injection truyền thống
builder.Services.AddScoped(p => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourVerySecureSecretKeyForJWT_MinimumLength32Characters!";

builder.Services.AddAuthentication(options =>
{
    // Use Cookie as default for Blazor Server
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
    
    // Allow JWT from both Header and SignalR query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
})
.AddCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/login";
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // 7 days absolute expiration
    options.SlidingExpiration = true; // Renew cookie if > 50% of time elapsed
    
    // Cookie Configuration - CRITICAL for avoiding "Unprotect ticket failed"
    options.Cookie.Name = "StationCheck.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.Cookie.IsEssential = true; // Required for authentication
    
    // IMPORTANT: Use same DataProtection for cookie encryption
    // This ensures cookies can be decrypted after app restart
    options.DataProtectionProvider = null; // Use default DataProtection (configured above)
    
    // Add detailed logging for debugging
    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
    {
        OnValidatePrincipal = async context =>
        {
            if (context.Principal?.Identity?.IsAuthenticated == true)
            {
                // Check if cookie is expired
                var issuedUtc = context.Properties.IssuedUtc;
                var expiresUtc = context.Properties.ExpiresUtc;
                var currentUtc = DateTimeOffset.UtcNow;

                if (expiresUtc.HasValue && currentUtc > expiresUtc.Value)
                {
                    Log.Logger.Warning("[Cookie] Cookie expired for user {Username}. Issued: {Issued}, Expires: {Expires}, Now: {Now}",
                        context.Principal.Identity.Name, issuedUtc, expiresUtc, currentUtc);
                    
                    // Reject expired cookie - force re-authentication
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                    return;
                }

                Log.Logger.Debug("[Cookie] Principal validated: {Username} (Expires: {Expires})", 
                    context.Principal.Identity.Name, expiresUtc);
            }
            else
            {
                Log.Logger.Warning("[Cookie] Principal validation failed");
            }
        }
    };
});

// Add policy for API endpoints to support both Cookie and JWT
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.AuthenticationSchemes.Add(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAbove", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("AllUsers", policy => policy.RequireRole("Admin", "Manager", "StationEmployee"));
});

// Add Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Blazored LocalStorage for token management
builder.Services.AddBlazoredLocalStorage();

// HttpClient for Blazor components - handles relative URIs properly in Blazor Server
builder.Services.AddScoped<HttpClient>(sp =>
{
    return new HttpClient();
});

// Add HttpContextAccessor for accessing current user in DbContext
builder.Services.AddHttpContextAccessor();

// Add DevExpress Blazor
builder.Services.AddDevExpressBlazor(options => {
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});

// Add Authentication State Provider for Blazor - use built-in to read from Cookie
builder.Services.AddScoped<AuthenticationStateProvider, Microsoft.AspNetCore.Components.Server.ServerAuthenticationStateProvider>();

// Add API Controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "StationCheck API",
        Version = "v1",
        Description = "API for StationCheck - Water Station Monitoring System",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "StationCheck Team"
        }
    });

    // Thêm JWT Authentication vào Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments nếu có
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configure Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register services - All using DbContextFactory now
builder.Services.AddSingleton<INvrService, MockNvrService>();
builder.Services.AddScoped<IFaceRecognitionService, MockFaceRecognitionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<LocalizationStateService>();
builder.Services.AddScoped<Localizer>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<TimeFrameHistoryService>();
builder.Services.AddScoped<SystemConfigurationService>();
builder.Services.AddScoped<IMotionDetectionService, MotionDetectionService>();
builder.Services.AddScoped<IMonitoringService, MonitoringService>();
builder.Services.AddScoped<IForceLogoutService, ForceLogoutService>();
builder.Services.AddScoped<IDeviceAuthService, DeviceAuthService>();

// ✅ Add ConfigurationChangeNotifier as Singleton (shared across all services)
builder.Services.AddSingleton<ConfigurationChangeNotifier>();

// Add SignalR for real-time notifications
builder.Services.AddSignalR();

// Add Background Services
// MotionMonitoringService removed - alerts now generated by AlertGenerationService
builder.Services.AddHostedService<EmailMonitoringService>(); // Check emails every 5 minutes
builder.Services.AddHostedService<AlertGenerationService>(); // Generate alerts every 1 hour
builder.Services.AddHostedService<DeviceStatusMonitorService>(); // Monitor device status changes real-time

var app = builder.Build();

// Auto migrate database and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("🔍 Checking database migration status...");
        
        // ✅ CLEANUP FIRST: Remove orphaned migration records directly with raw SQL
        logger.LogInformation("🔍 Cleaning up orphaned migration records...");
        try
        {
            await db.Database.ExecuteSqlRawAsync(@"
                DELETE FROM __EFMigrationsHistory 
                WHERE MigrationId IN (
                    '20251207052000_AddMacAddressAndDeviceStatusColumns',
                    '20251207052100_FixDeviceStatusData',
                    '20251207073000_SyncDeviceStatusWithLegacyFields'
                )
            ");
            logger.LogInformation("✅ Orphaned migration records removed");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "⚠️ Could not remove orphaned migrations (table might not exist yet)");
        }
        
        // Get current applied migrations AFTER cleanup
        var appliedMigrations = db.Database.GetAppliedMigrations().ToList();
        logger.LogInformation("📊 Applied migrations: {Count}", appliedMigrations.Count);
        
        // Apply pending migrations only
        var pendingMigrations = db.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            logger.LogWarning("⚠️ Found {Count} pending migrations. Applying now...", pendingMigrations.Count);
            foreach (var migration in pendingMigrations)
            {
                logger.LogInformation("  📦 Pending: {Migration}", migration);
            }
            db.Database.Migrate();
            logger.LogInformation("✅ All migrations applied successfully");
        }
        else
        {
            logger.LogInformation("✅ Database is up to date. No pending migrations.");
        }
        
        // ✅ CHỈ seed data nếu database TRỐNG (lần đầu tiên chạy)
        if (!db.Users.Any())
        {
            logger.LogInformation("📝 Database is empty. Seeding initial data...");
            await DbSeeder.SeedAsync(db);
            await DbSeeder.SeedStationsAsync(db);
            logger.LogInformation("✅ Initial data seeded successfully");
        }
        else
        {
            logger.LogInformation("ℹ️ Database already has data. Skipping seed.");
        }
        
        logger.LogInformation("🎉 Database initialization completed");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ An error occurred while migrating or seeding the database");
        throw;
    }
}

// Configure Swagger (chạy ở cả Development và Production để test API dễ dàng)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "StationCheck API v1");
    options.RoutePrefix = "api/docs"; // Truy cập Swagger UI tại: /api/docs
    options.DocumentTitle = "StationCheck API Documentation";
    options.DefaultModelsExpandDepth(-1); // Ẩn Models section
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Thu gọn tất cả endpoints
    options.EnableTryItOutByDefault(); // Bật "Try it out" mặc định
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // app.UseHsts(); // Disabled - no HTTPS redirect
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable static files for DevExpress Blazor
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Add cache headers for static files
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=3600");
    }
});

app.UseRouting();

// CRITICAL: Middleware order matters!
// 1. Authentication MUST come before Authorization
// 2. Authentication reads Cookie and creates ClaimsPrincipal
// 3. Authorization checks if user has required role
app.UseAuthentication();
app.UseAuthorization();

// 4. Custom middleware runs AFTER authentication/authorization
// Validate device access - returns 401 if user has no active assignments (force logout)
app.UseAccessControl();

// Map API Controllers
app.MapControllers();

app.MapBlazorHub();
app.MapHub<StationCheck.Hubs.AuthHub>("/hubs/auth");
app.MapHub<StationCheck.Hubs.DeviceStatusHub>("/hubs/device-status"); // Real-time device status notifications
app.MapFallbackToPage("/_Host");

try
{
    Log.Information("Starting StationCheck application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
