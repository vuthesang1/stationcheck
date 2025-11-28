using Microsoft.EntityFrameworkCore;
using StationCheck.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace StationCheck.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<MotionEvent> MotionEvents => Set<MotionEvent>();
    public DbSet<MotionAlert> MotionAlerts => Set<MotionAlert>();
    
    // Authentication
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    // Station Management
    public DbSet<Station> Stations => Set<Station>();
    
    // Localization
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Translation> Translations => Set<Translation>();
    
    // Monitoring Configuration
    public DbSet<TimeFrame> TimeFrames => Set<TimeFrame>();
    public DbSet<TimeFrameHistory> TimeFrameHistories => Set<TimeFrameHistory>();
    public DbSet<ConfigurationAuditLog> ConfigurationAuditLogs => Set<ConfigurationAuditLog>();
    
    // System Configuration
    public DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    
    // Email Events
    public DbSet<EmailEvent> EmailEvents => Set<EmailEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global query filter for soft delete
        modelBuilder.Entity<Station>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TimeFrame>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MonitoringConfiguration>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MonitoringProfile>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ApplicationUser>().HasQueryFilter(e => !e.IsDeleted);

        // MotionEvent configuration
        modelBuilder.Entity<MotionEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CameraId).HasMaxLength(200).IsRequired();
            // Ensure StationId is Guid (no change needed if already correct)
            entity.Property(e => e.CameraName).HasMaxLength(200);
            entity.Property(e => e.EventType).HasMaxLength(100);
            entity.Property(e => e.DetectedAt).HasDefaultValueSql("GETDATE()");
            
            entity.HasIndex(e => e.CameraId);
            entity.HasIndex(e => e.DetectedAt);
            entity.HasIndex(e => e.IsProcessed);
        });

        // MotionAlert configuration
        modelBuilder.Entity<MotionAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
#pragma warning disable CS0618 // Type or member is obsolete
            entity.Property(e => e.CameraId).HasMaxLength(200);  // ✅ Made nullable - legacy field
            entity.Property(e => e.CameraName).HasMaxLength(200);
#pragma warning restore CS0618
            entity.Property(e => e.StationName).HasMaxLength(200);
            entity.Property(e => e.LastMotionCameraId).HasMaxLength(200);  // ✅ Ensure LastMotionCameraId is 200
            entity.Property(e => e.LastMotionCameraName).HasMaxLength(200);
            entity.Property(e => e.ConfigurationSnapshot).HasMaxLength(4000);
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.ResolvedBy).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.AlertTime).HasDefaultValueSql("GETDATE()");
            
            // Relationships
            entity.HasOne(e => e.Station)
                .WithMany()
                .HasForeignKey(e => e.StationId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.TimeFrame)
                .WithMany()
                .HasForeignKey(e => e.TimeFrameId)
                .OnDelete(DeleteBehavior.NoAction);
            
            // TimeFrameHistory snapshot relationship
            entity.HasOne(e => e.TimeFrameHistorySnapshot)
                .WithMany()
                .HasForeignKey(e => e.TimeFrameHistoryId)
                .OnDelete(DeleteBehavior.NoAction);
            
#pragma warning disable CS0618 // Type or member is obsolete
            entity.HasIndex(e => e.CameraId);
#pragma warning restore CS0618
            entity.HasIndex(e => e.StationId);
            entity.HasIndex(e => e.AlertTime);
            entity.HasIndex(e => e.IsResolved);
            entity.HasIndex(e => e.Severity);
            entity.HasIndex(e => e.TimeFrameHistoryId);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUser = GetCurrentUsername();
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            // Set audit fields for entities with BaseAuditEntity properties
            if (entry.State == EntityState.Added)
            {
                // Set CreatedAt and CreatedBy for new entities
                if (entry.Entity.GetType().GetProperty("CreatedAt") != null)
                {
                    entry.Property("CreatedAt").CurrentValue = now;
                }
                if (entry.Entity.GetType().GetProperty("CreatedBy") != null && !string.IsNullOrEmpty(currentUser))
                {
                    entry.Property("CreatedBy").CurrentValue = currentUser;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                // Set ModifiedAt and ModifiedBy for updated entities
                if (entry.Entity.GetType().GetProperty("ModifiedAt") != null)
                {
                    entry.Property("ModifiedAt").CurrentValue = now;
                }
                if (entry.Entity.GetType().GetProperty("ModifiedBy") != null && !string.IsNullOrEmpty(currentUser))
                {
                    entry.Property("ModifiedBy").CurrentValue = currentUser;
                }

                // For soft delete: set DeletedAt and DeletedBy
                if (entry.Entity.GetType().GetProperty("IsDeleted") != null)
                {
                    var isDeleted = (bool)entry.Property("IsDeleted").CurrentValue!;
                    if (isDeleted)
                    {
                        if (entry.Entity.GetType().GetProperty("DeletedAt") != null)
                        {
                            entry.Property("DeletedAt").CurrentValue = now;
                        }
                        if (entry.Entity.GetType().GetProperty("DeletedBy") != null && !string.IsNullOrEmpty(currentUser))
                        {
                            entry.Property("DeletedBy").CurrentValue = currentUser;
                        }
                    }
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private string? GetCurrentUsername()
    {
        if (_httpContextAccessor?.HttpContext == null)
            return null;

        var user = _httpContextAccessor.HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        // Try to get username from claims
        return user.FindFirst(ClaimTypes.Name)?.Value 
            ?? user.FindFirst("username")?.Value 
            ?? user.Identity.Name;
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // ApplicationUser configuration
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.IsActive);
            
            // Foreign key to Station
            entity.HasOne(e => e.Station)
                  .WithMany()
                  .HasForeignKey(e => e.StationId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Station configuration
        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StationCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ContactPerson).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            
            entity.HasIndex(e => e.StationCode).IsUnique();
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Token).HasMaxLength(500).IsRequired();
            
            entity.HasIndex(e => e.Token);
            entity.HasIndex(e => e.UserId);
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed sample stations
        modelBuilder.Entity<Station>().HasData(
            new Station
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                StationCode = "123123123",
                Name = "Trạm Quan Trắc Sông Hồng",
                Address = "Quận Hoàn Kiếm, Hà Nội",
                Description = "Trạm quan trắc chất lượng nước sông Hồng",
                ContactPerson = "Nguyễn Văn A",
                ContactPhone = "0123456789",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                Id = new Guid("22222222-2222-2222-2222-222222222222"),
                StationCode = "121123123",
                Name = "Trạm Quan Trắc Sông Tô Lịch",
                Address = "Quận Đống Đa, Hà Nội",
                Description = "Trạm quan trắc chất lượng nước sông Tô Lịch",
                ContactPerson = "Trần Thị B",
                ContactPhone = "0987654321",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
        
        // Seed default users
        // Generate hashes at seed time to ensure they're correct
        var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12);
        var managerHash = BCrypt.Net.BCrypt.HashPassword("Manager@2025", 12);
        var employeeHash = BCrypt.Net.BCrypt.HashPassword("Employee@2025", 12);
        
        modelBuilder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = "USR001",
                Username = "admin",
                Email = "admin@stationcheck.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@2025", 12),
                FullName = "System Administrator",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = "USR002",
                Username = "manager",
                Email = "manager@stationcheck.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@2025", 12),
                FullName = "Department Manager",
                Role = UserRole.Manager,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                Id = "USR003",
                Username = "employee1",
                Email = "employee1@stationcheck.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee@2025", 12),
                FullName = "Nhân viên Trạm 1",
                Role = UserRole.StationEmployee,
                IsActive = true,
                StationId = null,  // Set to null instead of Guid.Empty
                CreatedAt = DateTime.UtcNow
            }
        );
        
        // TimeFrame configuration
        modelBuilder.Entity<TimeFrame>(entity =>
        {
            entity.HasKey(tf => tf.Id);
            entity.Property(tf => tf.Name).HasMaxLength(200).IsRequired();
            entity.Property(tf => tf.DaysOfWeek).HasMaxLength(50);
            entity.Property(tf => tf.CreatedBy).HasMaxLength(50);
            entity.Property(tf => tf.ModifiedBy).HasMaxLength(50);
            
            // Foreign key to Station
            entity.HasOne(tf => tf.Station)
                .WithMany(s => s.TimeFrames)
                .HasForeignKey(tf => tf.StationId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(tf => tf.StationId);
            entity.HasIndex(tf => tf.IsEnabled);
        });
        
        // TimeFrameHistory configuration
        modelBuilder.Entity<TimeFrameHistory>(entity =>
        {
            entity.HasKey(tfh => tfh.Id);
            entity.Property(tfh => tfh.Action).HasMaxLength(50).IsRequired();
            entity.Property(tfh => tfh.ConfigurationSnapshot).IsRequired();
            entity.Property(tfh => tfh.ChangeDescription).HasMaxLength(1000);
            entity.Property(tfh => tfh.ChangedBy).HasMaxLength(200).IsRequired();
            
            // Foreign key to TimeFrame (nullable - timeframe might be deleted)
            entity.HasOne(tfh => tfh.TimeFrame)
                .WithMany()
                .HasForeignKey(tfh => tfh.TimeFrameId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Foreign key to Station (required - always need to know which station)
            // NoAction to avoid multiple cascade paths
            entity.HasOne(tfh => tfh.Station)
                .WithMany()
                .HasForeignKey(tfh => tfh.StationId)
                .OnDelete(DeleteBehavior.NoAction);
            
            // Indexes for efficient querying
            entity.HasIndex(tfh => tfh.TimeFrameId);
            entity.HasIndex(tfh => tfh.StationId);
            entity.HasIndex(tfh => new { tfh.TimeFrameId, tfh.Version }).IsUnique();
            entity.HasIndex(tfh => tfh.ChangedAt);
        });
        
        // ConfigurationAuditLog configuration
        modelBuilder.Entity<ConfigurationAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityName).HasMaxLength(200);
            entity.Property(e => e.ActionType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Changes).HasMaxLength(1000);
            entity.Property(e => e.ChangedBy).HasMaxLength(200).IsRequired();
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.EntityId);
            entity.HasIndex(e => e.ChangedAt);
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
        });
        
        // Language configuration
        modelBuilder.Entity<Language>(entity =>
        {
            entity.HasKey(l => l.Code);
            entity.Property(l => l.Name).IsRequired();
            entity.HasIndex(l => l.IsDefault);
        });
        
        // Translation configuration
        modelBuilder.Entity<Translation>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => new { t.LanguageCode, t.Key }).IsUnique();
            entity.HasIndex(t => t.Category);
            
            entity.HasOne(t => t.Language)
                .WithMany(l => l.Translations)
                .HasForeignKey(t => t.LanguageCode)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Seed default languages
        modelBuilder.Entity<Language>().HasData(
            new Language
            {
                Code = "vi",
                Name = "Vietnamese",
                NativeName = "Tiếng Việt",
                IsActive = true,
                IsDefault = true,
                FlagIcon = "vn",
                CreatedAt = DateTime.UtcNow
            },
            new Language
            {
                Code = "en",
                Name = "English",
                NativeName = "English",
                IsActive = true,
                IsDefault = false,
                FlagIcon = "us",
                CreatedAt = DateTime.UtcNow
            }
        );
        
        // Seed default translations (Vietnamese) - REMOVED: Not compatible with Guid IDs
        // Translations will be managed through the UI instead
        var viTranslations = Array.Empty<Translation>();
        
        // Seed default translations (English) - REMOVED: Not compatible with Guid IDs
        // Translations will be managed through the UI instead
        var enTranslations = Array.Empty<Translation>();
        
        // modelBuilder.Entity<Translation>().HasData(viTranslations);
        // modelBuilder.Entity<Translation>().HasData(enTranslations);
        
        // EmailEvent configuration
        modelBuilder.Entity<EmailEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AlarmEvent).HasMaxLength(200);
            entity.Property(e => e.AlarmInputChannelName).HasMaxLength(200);
            entity.Property(e => e.AlarmDeviceName).HasMaxLength(200);
            entity.Property(e => e.AlarmName).HasMaxLength(200);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.AlarmDetails).HasMaxLength(2000);
            entity.Property(e => e.EmailSubject).HasMaxLength(200);
            entity.Property(e => e.EmailFrom).HasMaxLength(500);
            entity.Property(e => e.RawEmailBody).HasMaxLength(4000);
            
            entity.HasOne(e => e.Station)
                .WithMany()
                .HasForeignKey(e => e.StationId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasIndex(e => e.StationId);
            entity.HasIndex(e => e.EmailReceivedAt);
            entity.HasIndex(e => e.IsProcessed);
        });

        // SystemConfiguration configuration
        modelBuilder.Entity<SystemConfiguration>(entity =>
        {
            entity.HasKey(sc => sc.Id);
            entity.Property(sc => sc.Key).HasMaxLength(100).IsRequired();
            entity.Property(sc => sc.Value).IsRequired();
            entity.Property(sc => sc.DisplayName).HasMaxLength(200);
            entity.Property(sc => sc.Description).HasMaxLength(500);
            entity.Property(sc => sc.ValueType).HasMaxLength(50);
            entity.Property(sc => sc.Category).HasMaxLength(100);
            entity.Property(sc => sc.CreatedBy).HasMaxLength(200).IsRequired();
            entity.Property(sc => sc.ModifiedBy).HasMaxLength(200);

            entity.HasIndex(sc => sc.Key).IsUnique();
            entity.HasIndex(sc => sc.Category);
        });

        // Seed SystemConfigurations
        modelBuilder.Entity<SystemConfiguration>().HasData(
            new SystemConfiguration
            {
                Id = new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                Key = "EmailMonitorInterval",
                Value = "180", // 3 minutes in seconds
                DisplayName = "Email Monitor Interval",
                Description = "Khoảng thời gian quét email mới (giây)",
                ValueType = "int",
                Category = "BackgroundServices",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new SystemConfiguration
            {
                Id = new Guid("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Key = "AlertGenerationInterval",
                Value = "3600", // 1 hour in seconds
                DisplayName = "Alert Generation Interval",
                Description = "Khoảng thời gian kiểm tra và tạo cảnh báo (giây)",
                ValueType = "int",
                Category = "BackgroundServices",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );
    }
}
