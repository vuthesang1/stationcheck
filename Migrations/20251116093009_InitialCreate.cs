using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    FlagIcon = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastMotionDetectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsEditable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalTable: "Languages",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AlarmEvent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AlarmInputChannelNo = table.Column<int>(type: "int", nullable: true),
                    AlarmInputChannelName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AlarmStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AlarmDeviceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AlarmName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AlarmDetails = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EmailMessageId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmailSubject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmailFrom = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmailReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RawEmailBody = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailEvents_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringProfile_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MotionEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CameraId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CameraName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmailMessageId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmailSubject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SnapshotPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotionEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotionEvents_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "StationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StationCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ConfigurationSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationHistories_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TimeFrames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProfileId = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    FrequencyMinutes = table.Column<int>(type: "int", nullable: false),
                    BufferMinutes = table.Column<int>(type: "int", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeFrames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeFrames_MonitoringProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MonitoringProfile",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TimeFrames_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MotionAlerts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TimeFrameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConfigurationSnapshot = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AlertTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExpectedFrequencyMinutes = table.Column<int>(type: "int", nullable: false),
                    LastMotionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MinutesSinceLastMotion = table.Column<int>(type: "int", nullable: false),
                    LastMotionCameraId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastMotionCameraName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CameraId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CameraName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotionAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotionAlerts_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MotionAlerts_TimeFrames_TimeFrameId",
                        column: x => x.TimeFrameId,
                        principalTable: "TimeFrames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimeFrameHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeFrameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConfigurationSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeFrameHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeFrameHistories_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeFrameHistories_TimeFrames_TimeFrameId",
                        column: x => x.TimeFrameId,
                        principalTable: "TimeFrames",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Code", "CreatedAt", "CreatedBy", "FlagIcon", "IsActive", "IsDefault", "ModifiedAt", "ModifiedBy", "Name", "NativeName" },
                values: new object[,]
                {
                    { "en", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9388), "System", "us", true, false, null, null, "English", "English" },
                    { "vi", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9385), "System", "vn", true, true, null, null, "Vietnamese", "Tiếng Việt" }
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "ContactPerson", "ContactPhone", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastMotionDetectedAt", "ModifiedAt", "ModifiedBy", "Name", "StationCode" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Quận Hoàn Kiếm, Hà Nội", "Nguyễn Văn A", "0123456789", new DateTime(2025, 11, 16, 9, 30, 6, 507, DateTimeKind.Utc).AddTicks(1003), "System", "Trạm quan trắc chất lượng nước sông Hồng", true, null, null, null, "Trạm Quan Trắc Sông Hồng", "SH01" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Quận Đống Đa, Hà Nội", "Trần Thị B", "0987654321", new DateTime(2025, 11, 16, 9, 30, 6, 507, DateTimeKind.Utc).AddTicks(1006), "System", "Trạm quan trắc chất lượng nước sông Tô Lịch", true, null, null, null, "Trạm Quan Trắc Sông Tô Lịch", "TL01" }
                });

            migrationBuilder.InsertData(
                table: "SystemConfigurations",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "DisplayName", "IsEditable", "Key", "ModifiedAt", "ModifiedBy", "Value", "ValueType" },
                values: new object[,]
                {
                    { 1, "BackgroundServices", new DateTime(2025, 11, 16, 9, 30, 7, 382, DateTimeKind.Utc).AddTicks(204), "System", "Khoảng thời gian quét email mới (giây)", "Email Monitor Interval", true, "EmailMonitorInterval", null, null, "180", "int" },
                    { 2, "BackgroundServices", new DateTime(2025, 11, 16, 9, 30, 7, 382, DateTimeKind.Utc).AddTicks(208), "System", "Khoảng thời gian kiểm tra và tạo cảnh báo (giây)", "Alert Generation Interval", true, "AlertGenerationInterval", null, null, "3600", "int" },
                    { 3, "BackgroundServices", new DateTime(2025, 11, 16, 9, 30, 7, 382, DateTimeKind.Utc).AddTicks(211), "System", "Khoảng thời gian kiểm tra chuyển động (giây)", "Motion Monitor Interval", true, "MotionMonitorInterval", null, null, "60", "int" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "LastLoginAt", "ModifiedAt", "ModifiedBy", "PasswordHash", "Role", "StationId", "Username" },
                values: new object[,]
                {
                    { "USR001", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9230), null, "admin@stationcheck.com", "System Administrator", true, null, null, null, "$2a$12$PdJYccmRTUBakyXhyWwxWOfkMLQiLvSJnGmIP7H/U3kiY66uh5tAi", 2, null, "admin" },
                    { "USR002", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9237), null, "manager@stationcheck.com", "Department Manager", true, null, null, null, "$2a$12$sQAmK/m23FnnNKYhP9KGcOOkd6TsEKoB.ZADChyJdK8XBQhgP683S", 1, null, "manager" }
                });

            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Key", "LanguageCode", "ModifiedAt", "ModifiedBy", "Value" },
                values: new object[,]
                {
                    { 1, "menu", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9406), "System", "menu.dashboard", "vi", null, null, "Trang chủ" },
                    { 2, "menu", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9408), "System", "menu.stations", "vi", null, null, "Quản lý Trạm" },
                    { 3, "button", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9410), "System", "button.add", "vi", null, null, "Thêm mới" },
                    { 101, "menu", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9412), "System", "menu.dashboard", "en", null, null, "Dashboard" },
                    { 102, "menu", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9415), "System", "menu.stations", "en", null, null, "Station Management" },
                    { 103, "button", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9417), "System", "button.add", "en", null, null, "Add New" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "LastLoginAt", "ModifiedAt", "ModifiedBy", "PasswordHash", "Role", "StationId", "Username" },
                values: new object[] { "USR003", new DateTime(2025, 11, 16, 9, 30, 7, 381, DateTimeKind.Utc).AddTicks(9251), "System", "employee1@stationcheck.com", "Nhân viên Trạm 1", true, null, null, null, "$2a$12$zXBPTAySnQ/y2W8alLuzeO0Nqgp8ZGN8TS3uhGwBAp8mEPvFsRKNi", 0, new Guid("11111111-1111-1111-1111-111111111111"), "employee1" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailEvents_EmailReceivedAt",
                table: "EmailEvents",
                column: "EmailReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmailEvents_IsProcessed",
                table: "EmailEvents",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_EmailEvents_StationCode",
                table: "EmailEvents",
                column: "StationCode");

            migrationBuilder.CreateIndex(
                name: "IX_EmailEvents_StationId",
                table: "EmailEvents",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDefault",
                table: "Languages",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringProfile_StationId",
                table: "MonitoringProfile",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_AlertTime",
                table: "MotionAlerts",
                column: "AlertTime");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_IsResolved",
                table: "MotionAlerts",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_Severity",
                table: "MotionAlerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_StationId",
                table: "MotionAlerts",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_TimeFrameId",
                table: "MotionAlerts",
                column: "TimeFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionEvents_CameraId",
                table: "MotionEvents",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionEvents_DetectedAt",
                table: "MotionEvents",
                column: "DetectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MotionEvents_IsProcessed",
                table: "MotionEvents",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_MotionEvents_StationId",
                table: "MotionEvents",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_StationHistories_ChangedAt",
                table: "StationHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StationHistories_StationId",
                table: "StationHistories",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_IsActive",
                table: "Stations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_Name",
                table: "Stations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_StationCode",
                table: "Stations",
                column: "StationCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfigurations_Category",
                table: "SystemConfigurations",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_SystemConfigurations_Key",
                table: "SystemConfigurations",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeFrameHistories_ChangedAt",
                table: "TimeFrameHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TimeFrameHistories_StationId",
                table: "TimeFrameHistories",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeFrameHistories_TimeFrameId",
                table: "TimeFrameHistories",
                column: "TimeFrameId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeFrameHistories_TimeFrameId_Version",
                table: "TimeFrameHistories",
                columns: new[] { "TimeFrameId", "Version" },
                unique: true,
                filter: "[TimeFrameId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TimeFrames_IsEnabled",
                table: "TimeFrames",
                column: "IsEnabled");

            migrationBuilder.CreateIndex(
                name: "IX_TimeFrames_ProfileId",
                table: "TimeFrames",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeFrames_StationId",
                table: "TimeFrames",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Category",
                table: "Translations",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_LanguageCode_Key",
                table: "Translations",
                columns: new[] { "LanguageCode", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StationId",
                table: "Users",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailEvents");

            migrationBuilder.DropTable(
                name: "MotionAlerts");

            migrationBuilder.DropTable(
                name: "MotionEvents");

            migrationBuilder.DropTable(
                name: "StationHistories");

            migrationBuilder.DropTable(
                name: "SystemConfigurations");

            migrationBuilder.DropTable(
                name: "TimeFrameHistories");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "TimeFrames");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "MonitoringProfile");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
