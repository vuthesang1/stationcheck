using Microsoft.EntityFrameworkCore;
using StationCheck.Models;

namespace StationCheck.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
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

        // MotionEvent configuration
        modelBuilder.Entity<MotionEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CameraId).HasMaxLength(50).IsRequired();
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
            entity.Property(e => e.CameraId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CameraName).HasMaxLength(200);
            entity.Property(e => e.StationName).HasMaxLength(200);
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
            
            entity.HasIndex(e => e.CameraId);
            entity.HasIndex(e => e.StationId);
            entity.HasIndex(e => e.AlertTime);
            entity.HasIndex(e => e.IsResolved);
            entity.HasIndex(e => e.Severity);
            entity.HasIndex(e => e.TimeFrameHistoryId);
        });

        // Seed data
        SeedData(modelBuilder);
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
                Id = 1,
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
                Id = 2,
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
        var managerHash = BCrypt.Net.BCrypt.HashPassword("Manager@123", 12);
        var employeeHash = BCrypt.Net.BCrypt.HashPassword("Employee@123", 12);
        
        modelBuilder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = "USR001",
                Username = "admin",
                Email = "admin@stationcheck.com",
                PasswordHash = adminHash,
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
                PasswordHash = managerHash,
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
                PasswordHash = employeeHash,
                FullName = "Nhân viên Trạm 1",
                Role = UserRole.StationEmployee,
                IsActive = true,
                StationId = 1,
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
        
        // Seed default translations (Vietnamese)
        var viTranslations = new[]
        {
            new Translation { Id = 1, LanguageCode = "vi", Key = "menu.dashboard", Value = "Trang chủ", Category = "menu", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 2, LanguageCode = "vi", Key = "menu.stations", Value = "Quản lý Trạm", Category = "menu", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 3, LanguageCode = "vi", Key = "menu.users", Value = "Quản lý User", Category = "menu", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 4, LanguageCode = "vi", Key = "menu.settings", Value = "Cấu hình", Category = "menu", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 5, LanguageCode = "vi", Key = "button.add", Value = "Thêm mới", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 6, LanguageCode = "vi", Key = "button.edit", Value = "Chỉnh sửa", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 7, LanguageCode = "vi", Key = "button.delete", Value = "Xóa", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 8, LanguageCode = "vi", Key = "button.save", Value = "Lưu", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 9, LanguageCode = "vi", Key = "button.cancel", Value = "Hủy", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 10, LanguageCode = "vi", Key = "station.name", Value = "Tên trạm", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 11, LanguageCode = "vi", Key = "station.address", Value = "Địa chỉ", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 12, LanguageCode = "vi", Key = "station.contact", Value = "Người liên hệ", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 13, LanguageCode = "vi", Key = "station.phone", Value = "Số điện thoại", Category = "label", CreatedAt = DateTime.UtcNow },
            // Station page translations
            new Translation { Id = 14, LanguageCode = "vi", Key = "station.page_title", Value = "Quản lý Trạm", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 15, LanguageCode = "vi", Key = "station.list_title", Value = "Danh sách Trạm Quan Trắc", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 16, LanguageCode = "vi", Key = "station.add_button", Value = "Thêm Trạm Mới", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 17, LanguageCode = "vi", Key = "station.edit_title_add", Value = "Thêm Trạm Mới", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 18, LanguageCode = "vi", Key = "station.edit_title_edit", Value = "Chỉnh sửa Trạm", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 19, LanguageCode = "vi", Key = "station.search_placeholder", Value = "Tìm kiếm...", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 20, LanguageCode = "vi", Key = "station.name_column", Value = "Tên Trạm", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 21, LanguageCode = "vi", Key = "station.address_column", Value = "Địa chỉ", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 22, LanguageCode = "vi", Key = "station.contact_column", Value = "Người liên hệ", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 23, LanguageCode = "vi", Key = "station.phone_column", Value = "Số điện thoại", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 24, LanguageCode = "vi", Key = "station.actions_column", Value = "Thao tác", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 25, LanguageCode = "vi", Key = "station.name_label", Value = "Tên Trạm:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 26, LanguageCode = "vi", Key = "station.address_label", Value = "Địa chỉ:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 27, LanguageCode = "vi", Key = "station.description_label", Value = "Mô tả:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 28, LanguageCode = "vi", Key = "station.contact_label", Value = "Người liên hệ:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 29, LanguageCode = "vi", Key = "station.phone_label", Value = "Số điện thoại:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 30, LanguageCode = "vi", Key = "station.active_label", Value = "Kích hoạt giám sát:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 31, LanguageCode = "vi", Key = "message.confirm_delete_station", Value = "Bạn có chắc muốn xóa trạm này?", Category = "message", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 32, LanguageCode = "vi", Key = "message.delete_error", Value = "Không thể xóa", Category = "message", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 33, LanguageCode = "vi", Key = "message.error", Value = "Lỗi", Category = "message", CreatedAt = DateTime.UtcNow },
        };
        
        var enTranslations = new[]
        {
            new Translation { Id = 101, LanguageCode = "en", Key = "menu.dashboard", Value = "Dashboard", Category = "menu", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 102, LanguageCode = "en", Key = "menu.stations", Value = "Station Management", Category = "menu", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 103, LanguageCode = "en", Key = "menu.users", Value = "User Management", Category = "menu", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 104, LanguageCode = "en", Key = "menu.settings", Value = "Settings", Category = "menu", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 105, LanguageCode = "en", Key = "button.add", Value = "Add New", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 106, LanguageCode = "en", Key = "button.edit", Value = "Edit", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 107, LanguageCode = "en", Key = "button.delete", Value = "Delete", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 108, LanguageCode = "en", Key = "button.save", Value = "Save", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 109, LanguageCode = "en", Key = "button.cancel", Value = "Cancel", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 110, LanguageCode = "en", Key = "station.name", Value = "Station Name", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 111, LanguageCode = "en", Key = "station.address", Value = "Address", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 112, LanguageCode = "en", Key = "station.contact", Value = "Contact Person", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 113, LanguageCode = "en", Key = "station.phone", Value = "Phone Number", Category = "label", CreatedAt = DateTime.UtcNow },
            // Station page translations
            new Translation { Id = 114, LanguageCode = "en", Key = "station.page_title", Value = "Station Management", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 115, LanguageCode = "en", Key = "station.list_title", Value = "Monitoring Station List", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 116, LanguageCode = "en", Key = "station.add_button", Value = "Add New Station", Category = "button", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 117, LanguageCode = "en", Key = "station.edit_title_add", Value = "Add New Station", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 118, LanguageCode = "en", Key = "station.edit_title_edit", Value = "Edit Station", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 119, LanguageCode = "en", Key = "station.search_placeholder", Value = "Search...", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 120, LanguageCode = "en", Key = "station.name_column", Value = "Station Name", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 121, LanguageCode = "en", Key = "station.address_column", Value = "Address", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 122, LanguageCode = "en", Key = "station.contact_column", Value = "Contact Person", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 123, LanguageCode = "en", Key = "station.phone_column", Value = "Phone Number", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 124, LanguageCode = "en", Key = "station.actions_column", Value = "Actions", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 125, LanguageCode = "en", Key = "station.name_label", Value = "Station Name:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 126, LanguageCode = "en", Key = "station.address_label", Value = "Address:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 127, LanguageCode = "en", Key = "station.description_label", Value = "Description:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 128, LanguageCode = "en", Key = "station.contact_label", Value = "Contact Person:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 129, LanguageCode = "en", Key = "station.phone_label", Value = "Phone Number:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 130, LanguageCode = "en", Key = "station.active_label", Value = "Enable Monitoring:", Category = "label", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 131, LanguageCode = "en", Key = "message.confirm_delete_station", Value = "Are you sure you want to delete this station?", Category = "message", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 132, LanguageCode = "en", Key = "message.delete_error", Value = "Cannot delete", Category = "message", CreatedAt = DateTime.UtcNow },
            new Translation { Id = 133, LanguageCode = "en", Key = "message.error", Value = "Error", Category = "message", CreatedAt = DateTime.UtcNow },
        };
        
        modelBuilder.Entity<Translation>().HasData(viTranslations);
        modelBuilder.Entity<Translation>().HasData(enTranslations);
        
        // EmailEvent configuration
        modelBuilder.Entity<EmailEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StationCode).HasMaxLength(50).IsRequired();
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
                
            entity.HasIndex(e => e.StationCode);
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
                Id = 1,
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
                Id = 2,
                Key = "AlertGenerationInterval",
                Value = "3600", // 1 hour in seconds
                DisplayName = "Alert Generation Interval",
                Description = "Khoảng thời gian kiểm tra và tạo cảnh báo (giây)",
                ValueType = "int",
                Category = "BackgroundServices",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new SystemConfiguration
            {
                Id = 3,
                Key = "MotionMonitorInterval",
                Value = "60", // 1 minute in seconds
                DisplayName = "Motion Monitor Interval",
                Description = "Khoảng thời gian kiểm tra chuyển động (giây)",
                ValueType = "int",
                Category = "BackgroundServices",
                IsEditable = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );
    }
}
