using Microsoft.EntityFrameworkCore;
using StationCheck.Models;

namespace StationCheck.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MotionEvent> MotionEvents { get; set; }
        public DbSet<MotionAlert> MotionAlerts { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<TimeFrame> TimeFrames { get; set; }
        public DbSet<TimeFrameHistory> TimeFrameHistories { get; set; }
        public DbSet<StationHistory> StationHistories { get; set; }
        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public DbSet<EmailEvent> EmailEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // MotionEvent configuration
            modelBuilder.Entity<MotionEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.CameraId).HasMaxLength(50);
                entity.Property(e => e.CameraName).HasMaxLength(200);
                entity.Property(e => e.EventType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.EmailMessageId).HasMaxLength(500);
                entity.Property(e => e.EmailSubject).HasMaxLength(500);
                entity.Property(e => e.SnapshotPath).HasMaxLength(500);
                entity.Property(e => e.Payload).HasMaxLength(2000);
                entity.Property(e => e.DetectedAt).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.Station)
                    .WithMany()
                    .HasForeignKey(e => e.StationId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasIndex(e => e.CameraId);
                entity.HasIndex(e => e.StationId);
                entity.HasIndex(e => e.DetectedAt);
                entity.HasIndex(e => e.IsProcessed);
            });

            // MotionAlert configuration
            modelBuilder.Entity<MotionAlert>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.StationName).HasMaxLength(200);
                entity.Property(e => e.ConfigurationSnapshot).HasMaxLength(4000);
                entity.Property(e => e.Message).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.ResolvedBy).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.LastMotionCameraId).HasMaxLength(50);
                entity.Property(e => e.LastMotionCameraName).HasMaxLength(200);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.AlertTime).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(e => e.Station)
                    .WithMany()
                    .HasForeignKey(e => e.StationId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasOne(e => e.TimeFrame)
                    .WithMany()
                    .HasForeignKey(e => e.TimeFrameId)
                    .OnDelete(DeleteBehavior.NoAction);
                
                entity.HasIndex(e => e.StationId);
                entity.HasIndex(e => e.AlertTime);
                entity.HasIndex(e => e.IsResolved);
                entity.HasIndex(e => e.Severity);
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

            // TimeFrame configuration
            modelBuilder.Entity<TimeFrame>(entity =>
            {
                entity.HasKey(tf => tf.Id);
                entity.Property(tf => tf.Name).HasMaxLength(200).IsRequired();
                entity.Property(tf => tf.DaysOfWeek).HasMaxLength(50);
                
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
                
                entity.HasOne(tfh => tfh.TimeFrame)
                    .WithMany()
                    .HasForeignKey(tfh => tfh.TimeFrameId)
                    .OnDelete(DeleteBehavior.NoAction);
                
                entity.HasOne(tfh => tfh.Station)
                    .WithMany()
                    .HasForeignKey(tfh => tfh.StationId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasIndex(tfh => tfh.TimeFrameId);
                entity.HasIndex(tfh => tfh.StationId);
                entity.HasIndex(tfh => new { tfh.TimeFrameId, tfh.Version }).IsUnique();
                entity.HasIndex(tfh => tfh.ChangedAt);
            });

            // StationHistory configuration
            modelBuilder.Entity<StationHistory>(entity =>
            {
                entity.HasKey(sh => sh.Id);
                entity.Property(sh => sh.Action).HasMaxLength(50).IsRequired();
                entity.Property(sh => sh.StationCode).HasMaxLength(200).IsRequired();
                entity.Property(sh => sh.StationName).HasMaxLength(200).IsRequired();
                entity.Property(sh => sh.ConfigurationSnapshot).IsRequired();
                entity.Property(sh => sh.ChangeDescription).HasMaxLength(1000);
                entity.Property(sh => sh.ChangedBy).HasMaxLength(200).IsRequired();
                entity.Property(sh => sh.OldValues).HasMaxLength(4000);
                entity.Property(sh => sh.NewValues).HasMaxLength(4000);
                
                entity.HasOne(sh => sh.Station)
                    .WithMany()
                    .HasForeignKey(sh => sh.StationId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasIndex(sh => sh.StationId);
                entity.HasIndex(sh => sh.ChangedAt);
            });

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
                
                entity.HasOne(e => e.Station)
                      .WithMany()
                      .HasForeignKey(e => e.StationId)
                      .OnDelete(DeleteBehavior.SetNull);
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
            
            // EmailEvent configuration
            modelBuilder.Entity<EmailEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StationCode).HasMaxLength(10).IsRequired();
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

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var station1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var station2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
            
            modelBuilder.Entity<Station>().HasData(
                new Station
                {
                    Id = station1Id,
                    StationCode = "SH01",
                    Name = "Trạm Quan Trắc Sông Hồng",
                    Address = "Quận Hoàn Kiếm, Hà Nội",
                    Description = "Trạm quan trắc chất lượng nước sông Hồng",
                    ContactPerson = "Nguyễn Văn A",
                    ContactPhone = "0123456789",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Station
                {
                    Id = station2Id,
                    StationCode = "TL01",
                    Name = "Trạm Quan Trắc Sông Tô Lịch",
                    Address = "Quận Đống Đa, Hà Nội",
                    Description = "Trạm quan trắc chất lượng nước sông Tô Lịch",
                    ContactPerson = "Trần Thị B",
                    ContactPhone = "0987654321",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
            
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
                    StationId = station1Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
            
            modelBuilder.Entity<Language>().HasData(
                new Language { Code = "vi", Name = "Vietnamese", NativeName = "Tiếng Việt", IsActive = true, IsDefault = true, FlagIcon = "vn", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Language { Code = "en", Name = "English", NativeName = "English", IsActive = true, IsDefault = false, FlagIcon = "us", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
            );
            
            var viTranslations = new[]
            {
                new Translation { Id = 1, LanguageCode = "vi", Key = "menu.dashboard", Value = "Trang chủ", Category = "menu", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Translation { Id = 2, LanguageCode = "vi", Key = "menu.stations", Value = "Quản lý Trạm", Category = "menu", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Translation { Id = 3, LanguageCode = "vi", Key = "button.add", Value = "Thêm mới", Category = "button", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
            };
            
            var enTranslations = new[]
            {
                new Translation { Id = 101, LanguageCode = "en", Key = "menu.dashboard", Value = "Dashboard", Category = "menu", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Translation { Id = 102, LanguageCode = "en", Key = "menu.stations", Value = "Station Management", Category = "menu", CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Translation { Id = 103, LanguageCode = "en", Key = "button.add", Value = "Add New", Category = "button", CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
            };
            
            modelBuilder.Entity<Translation>().HasData(viTranslations);
            modelBuilder.Entity<Translation>().HasData(enTranslations);

            modelBuilder.Entity<SystemConfiguration>().HasData(
                new SystemConfiguration { Id = 1, Key = "EmailMonitorInterval", Value = "180", DisplayName = "Email Monitor Interval", Description = "Khoảng thời gian quét email mới (giây)", ValueType = "int", Category = "BackgroundServices", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new SystemConfiguration { Id = 2, Key = "AlertGenerationInterval", Value = "3600", DisplayName = "Alert Generation Interval", Description = "Khoảng thời gian kiểm tra và tạo cảnh báo (giây)", ValueType = "int", Category = "BackgroundServices", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new SystemConfiguration { Id = 3, Key = "MotionMonitorInterval", Value = "60", DisplayName = "Motion Monitor Interval", Description = "Khoảng thời gian kiểm tra chuyển động (giây)", ValueType = "int", Category = "BackgroundServices", IsEditable = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" }
            );
        }
    }
}
