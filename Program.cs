using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StationCheck.BackgroundServices;
using StationCheck.Components;
using StationCheck.Data;
using StationCheck.Interfaces;
using StationCheck.Services;
using StationCheck.Models;
using Microsoft.OpenApi.Models;
using Serilog;

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
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAbove", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("AllUsers", policy => policy.RequireRole("Admin", "Manager", "StationEmployee"));
});

// Add Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add DevExpress Blazor
builder.Services.AddDevExpressBlazor(options => {
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v4;
});

// Add Authentication State Provider for Blazor
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

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
builder.Services.AddScoped<TimeFrameHistoryService>();
builder.Services.AddScoped<SystemConfigurationService>();
builder.Services.AddScoped<IMotionDetectionService, MotionDetectionService>();
builder.Services.AddScoped<IMonitoringService, MonitoringService>();

// Add Background Services
// MotionMonitoringService removed - alerts now generated by AlertGenerationService
builder.Services.AddHostedService<EmailMonitoringService>(); // Check emails every 5 minutes
builder.Services.AddHostedService<AlertGenerationService>(); // Generate alerts every 1 hour

var app = builder.Build();

// Auto migrate database and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    
    // Seed initial data
    await DbSeeder.SeedAsync(db);
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
    app.UseHsts();
}

app.UseHttpsRedirection();
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

app.UseAuthentication();
app.UseAuthorization();

// Map API Controllers
app.MapControllers();

app.MapBlazorHub();
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
