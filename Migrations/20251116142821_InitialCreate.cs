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
                name: "ConfigurationAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Changes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationAuditLogs", x => x.Id);
                });

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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
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
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "MonitoringProfileHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonitoringProfileId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ProfileSnapshot = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringProfileHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringProfileHistory_MonitoringProfile_MonitoringProfileId",
                        column: x => x.MonitoringProfileId,
                        principalTable: "MonitoringProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: true),
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false)
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
                name: "MonitoringConfiguration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: true),
                    ProfileId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringConfiguration_MonitoringProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "MonitoringProfile",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonitoringConfiguration_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MotionEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CameraId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CameraName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StationId = table.Column<int>(type: "int", nullable: true),
                    EmailMessageId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmailSubject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SnapshotPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotionEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotionEvents_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimeFrames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: true),
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
                    StationId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StationId1 = table.Column<int>(type: "int", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Users_Stations_StationId1",
                        column: x => x.StationId1,
                        principalTable: "Stations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimeFrameHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeFrameId = table.Column<int>(type: "int", nullable: true),
                    StationId = table.Column<int>(type: "int", nullable: false),
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TimeFrameHistories_TimeFrames_TimeFrameId",
                        column: x => x.TimeFrameId,
                        principalTable: "TimeFrames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MotionAlerts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: true),
                    StationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MonitoringConfigurationId = table.Column<int>(type: "int", nullable: true),
                    TimeFrameId = table.Column<int>(type: "int", nullable: true),
                    ProfileHistoryId = table.Column<int>(type: "int", nullable: true),
                    TimeFrameHistoryId = table.Column<int>(type: "int", nullable: true),
                    ConfigurationSnapshot = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AlertTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpectedFrequencyMinutes = table.Column<int>(type: "int", nullable: false),
                    LastMotionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MinutesSinceLastMotion = table.Column<int>(type: "int", nullable: false),
                    LastMotionCameraId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastMotionCameraName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CameraId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CameraName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotionAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotionAlerts_MonitoringConfiguration_MonitoringConfigurationId",
                        column: x => x.MonitoringConfigurationId,
                        principalTable: "MonitoringConfiguration",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MotionAlerts_MonitoringProfileHistory_ProfileHistoryId",
                        column: x => x.ProfileHistoryId,
                        principalTable: "MonitoringProfileHistory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MotionAlerts_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MotionAlerts_TimeFrameHistories_TimeFrameHistoryId",
                        column: x => x.TimeFrameHistoryId,
                        principalTable: "TimeFrameHistories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MotionAlerts_TimeFrames_TimeFrameId",
                        column: x => x.TimeFrameId,
                        principalTable: "TimeFrames",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Code", "CreatedAt", "FlagIcon", "IsActive", "IsDefault", "Name", "NativeName" },
                values: new object[,]
                {
                    { "en", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(816), "us", true, false, "English", "English" },
                    { "vi", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(810), "vn", true, true, "Vietnamese", "Tiếng Việt" }
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "ContactPerson", "ContactPhone", "CreatedAt", "CreatedBy", "Description", "IsActive", "LastMotionDetectedAt", "ModifiedAt", "ModifiedBy", "Name", "StationCode" },
                values: new object[,]
                {
                    { 1, "Quận Hoàn Kiếm, Hà Nội", "Nguyễn Văn A", "0123456789", new DateTime(2025, 11, 16, 14, 28, 19, 659, DateTimeKind.Utc).AddTicks(2451), null, "Trạm quan trắc chất lượng nước sông Hồng", true, null, null, null, "Trạm Quan Trắc Sông Hồng", "123123123" },
                    { 2, "Quận Đống Đa, Hà Nội", "Trần Thị B", "0987654321", new DateTime(2025, 11, 16, 14, 28, 19, 659, DateTimeKind.Utc).AddTicks(2454), null, "Trạm quan trắc chất lượng nước sông Tô Lịch", true, null, null, null, "Trạm Quan Trắc Sông Tô Lịch", "121123123" }
                });

            migrationBuilder.InsertData(
                table: "SystemConfigurations",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "DisplayName", "IsEditable", "Key", "ModifiedAt", "ModifiedBy", "Value", "ValueType" },
                values: new object[,]
                {
                    { 1, "BackgroundServices", new DateTime(2025, 11, 16, 14, 28, 20, 740, DateTimeKind.Utc).AddTicks(222), "System", "Khoảng thời gian quét email mới (giây)", "Email Monitor Interval", true, "EmailMonitorInterval", null, null, "180", "int" },
                    { 2, "BackgroundServices", new DateTime(2025, 11, 16, 14, 28, 20, 740, DateTimeKind.Utc).AddTicks(225), "System", "Khoảng thời gian kiểm tra và tạo cảnh báo (giây)", "Alert Generation Interval", true, "AlertGenerationInterval", null, null, "3600", "int" },
                    { 3, "BackgroundServices", new DateTime(2025, 11, 16, 14, 28, 20, 740, DateTimeKind.Utc).AddTicks(228), "System", "Khoảng thời gian kiểm tra chuyển động (giây)", "Motion Monitor Interval", true, "MotionMonitorInterval", null, null, "60", "int" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "LastLoginAt", "ModifiedAt", "ModifiedBy", "PasswordHash", "Role", "StationId", "StationId1", "Username" },
                values: new object[,]
                {
                    { "USR001", new DateTime(2025, 11, 16, 14, 28, 20, 735, DateTimeKind.Utc).AddTicks(3351), null, "admin@stationcheck.com", "System Administrator", true, null, null, null, "$2a$12$9bB3aM2wtwbbtVl0EWqZF.7iZTmPLLLgMVm5PcvY2HzR2S7ob5fMS", 2, null, null, "admin" },
                    { "USR002", new DateTime(2025, 11, 16, 14, 28, 20, 735, DateTimeKind.Utc).AddTicks(3356), null, "manager@stationcheck.com", "Department Manager", true, null, null, null, "$2a$12$oohWYkn5PXh34VET5QdR7uCmbv4P5mvrwr7dk4DnpFn6lL/3ZIHQq", 1, null, null, "manager" }
                });

            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Category", "CreatedAt", "Key", "LanguageCode", "ModifiedAt", "Value" },
                values: new object[,]
                {
                    { 1, "menu", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(877), "menu.dashboard", "vi", null, "Trang chủ" },
                    { 2, "menu", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(879), "menu.stations", "vi", null, "Quản lý Trạm" },
                    { 3, "menu", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(881), "menu.users", "vi", null, "Quản lý User" },
                    { 4, "menu", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(883), "menu.settings", "vi", null, "Cấu hình" },
                    { 5, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(885), "button.add", "vi", null, "Thêm mới" },
                    { 6, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(887), "button.edit", "vi", null, "Chỉnh sửa" },
                    { 7, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(889), "button.delete", "vi", null, "Xóa" },
                    { 8, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(891), "button.save", "vi", null, "Lưu" },
                    { 9, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(893), "button.cancel", "vi", null, "Hủy" },
                    { 10, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(912), "station.name", "vi", null, "Tên trạm" },
                    { 11, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(914), "station.address", "vi", null, "Địa chỉ" },
                    { 12, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(916), "station.contact", "vi", null, "Người liên hệ" },
                    { 13, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(918), "station.phone", "vi", null, "Số điện thoại" },
                    { 14, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(921), "station.page_title", "vi", null, "Quản lý Trạm" },
                    { 15, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(923), "station.list_title", "vi", null, "Danh sách Trạm Quan Trắc" },
                    { 16, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(925), "station.add_button", "vi", null, "Thêm Trạm Mới" },
                    { 17, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(927), "station.edit_title_add", "vi", null, "Thêm Trạm Mới" },
                    { 18, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(929), "station.edit_title_edit", "vi", null, "Chỉnh sửa Trạm" },
                    { 19, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(931), "station.search_placeholder", "vi", null, "Tìm kiếm..." },
                    { 20, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(934), "station.name_column", "vi", null, "Tên Trạm" },
                    { 21, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(936), "station.address_column", "vi", null, "Địa chỉ" },
                    { 22, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(938), "station.contact_column", "vi", null, "Người liên hệ" },
                    { 23, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(940), "station.phone_column", "vi", null, "Số điện thoại" },
                    { 24, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(942), "station.actions_column", "vi", null, "Thao tác" },
                    { 25, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(944), "station.name_label", "vi", null, "Tên Trạm:" },
                    { 26, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(946), "station.address_label", "vi", null, "Địa chỉ:" },
                    { 27, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(948), "station.description_label", "vi", null, "Mô tả:" },
                    { 28, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(950), "station.contact_label", "vi", null, "Người liên hệ:" },
                    { 29, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(952), "station.phone_label", "vi", null, "Số điện thoại:" },
                    { 30, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(954), "station.active_label", "vi", null, "Kích hoạt giám sát:" },
                    { 31, "message", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(956), "message.confirm_delete_station", "vi", null, "Bạn có chắc muốn xóa trạm này?" },
                    { 32, "message", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(958), "message.delete_error", "vi", null, "Không thể xóa" },
                    { 33, "message", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(961), "message.error", "vi", null, "Lỗi" },
                    { 101, "menu", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(963), "menu.dashboard", "en", null, "Dashboard" },
                    { 102, "menu", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(965), "menu.stations", "en", null, "Station Management" },
                    { 103, "menu", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(967), "menu.users", "en", null, "User Management" },
                    { 104, "menu", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(969), "menu.settings", "en", null, "Settings" },
                    { 105, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(971), "button.add", "en", null, "Add New" },
                    { 106, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(973), "button.edit", "en", null, "Edit" },
                    { 107, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(975), "button.delete", "en", null, "Delete" },
                    { 108, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(978), "button.save", "en", null, "Save" },
                    { 109, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(980), "button.cancel", "en", null, "Cancel" },
                    { 110, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(983), "station.name", "en", null, "Station Name" },
                    { 111, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(985), "station.address", "en", null, "Address" },
                    { 112, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(987), "station.contact", "en", null, "Contact Person" },
                    { 113, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(989), "station.phone", "en", null, "Phone Number" },
                    { 114, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(991), "station.page_title", "en", null, "Station Management" },
                    { 115, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(994), "station.list_title", "en", null, "Monitoring Station List" },
                    { 116, "button", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(996), "station.add_button", "en", null, "Add New Station" },
                    { 117, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1190), "station.edit_title_add", "en", null, "Add New Station" },
                    { 118, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1193), "station.edit_title_edit", "en", null, "Edit Station" },
                    { 119, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1195), "station.search_placeholder", "en", null, "Search..." },
                    { 120, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1197), "station.name_column", "en", null, "Station Name" },
                    { 121, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1199), "station.address_column", "en", null, "Address" },
                    { 122, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1201), "station.contact_column", "en", null, "Contact Person" },
                    { 123, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1203), "station.phone_column", "en", null, "Phone Number" },
                    { 124, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1205), "station.actions_column", "en", null, "Actions" },
                    { 125, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1208), "station.name_label", "en", null, "Station Name:" },
                    { 126, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1210), "station.address_label", "en", null, "Address:" },
                    { 127, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1212), "station.description_label", "en", null, "Description:" },
                    { 128, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1214), "station.contact_label", "en", null, "Contact Person:" },
                    { 129, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1215), "station.phone_label", "en", null, "Phone Number:" },
                    { 130, "label", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1217), "station.active_label", "en", null, "Enable Monitoring:" },
                    { 131, "message", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1219), "message.confirm_delete_station", "en", null, "Are you sure you want to delete this station?" },
                    { 132, "message", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1221), "message.delete_error", "en", null, "Cannot delete" },
                    { 133, "message", new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1223), "message.error", "en", null, "Error" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FullName", "IsActive", "LastLoginAt", "ModifiedAt", "ModifiedBy", "PasswordHash", "Role", "StationId", "StationId1", "Username" },
                values: new object[] { "USR003", new DateTime(2025, 11, 16, 14, 28, 20, 735, DateTimeKind.Utc).AddTicks(3361), null, "employee1@stationcheck.com", "Nhân viên Trạm 1", true, null, null, null, "$2a$12$cvmhS4YQ1TRjrhKTxW4ocuc3dzRlzmVaYKmPDMaekG0/Lt.nuZZWu", 0, 1, null, "employee1" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationAuditLogs_ChangedAt",
                table: "ConfigurationAuditLogs",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationAuditLogs_EntityId",
                table: "ConfigurationAuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationAuditLogs_EntityType",
                table: "ConfigurationAuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationAuditLogs_EntityType_EntityId",
                table: "ConfigurationAuditLogs",
                columns: new[] { "EntityType", "EntityId" });

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
                name: "IX_MonitoringConfiguration_ProfileId",
                table: "MonitoringConfiguration",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringConfiguration_StationId",
                table: "MonitoringConfiguration",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringProfileHistory_MonitoringProfileId",
                table: "MonitoringProfileHistory",
                column: "MonitoringProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_AlertTime",
                table: "MotionAlerts",
                column: "AlertTime");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_IsResolved",
                table: "MotionAlerts",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_MonitoringConfigurationId",
                table: "MotionAlerts",
                column: "MonitoringConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_ProfileHistoryId",
                table: "MotionAlerts",
                column: "ProfileHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_Severity",
                table: "MotionAlerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_StationId",
                table: "MotionAlerts",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_TimeFrameHistoryId",
                table: "MotionAlerts",
                column: "TimeFrameHistoryId");

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
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

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
                name: "IX_Users_StationId1",
                table: "Users",
                column: "StationId1");

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
                name: "ConfigurationAuditLogs");

            migrationBuilder.DropTable(
                name: "EmailEvents");

            migrationBuilder.DropTable(
                name: "MotionAlerts");

            migrationBuilder.DropTable(
                name: "MotionEvents");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "SystemConfigurations");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "MonitoringConfiguration");

            migrationBuilder.DropTable(
                name: "MonitoringProfileHistory");

            migrationBuilder.DropTable(
                name: "TimeFrameHistories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "TimeFrames");

            migrationBuilder.DropTable(
                name: "MonitoringProfile");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
