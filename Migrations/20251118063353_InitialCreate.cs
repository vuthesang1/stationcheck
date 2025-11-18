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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringProfile", x => x.Id);
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
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonitoringProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    FrequencyMinutes = table.Column<int>(type: "int", nullable: false),
                    BufferMinutes = table.Column<int>(type: "int", nullable: false),
                    DaysOfWeek = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StationId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MonitoringConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimeFrameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProfileHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimeFrameHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    CameraId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    { "en", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1482), "us", true, false, "English", "English" },
                    { "vi", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1480), "vn", true, true, "Vietnamese", "Tiếng Việt" }
                });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "ContactPerson", "ContactPhone", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastMotionDetectedAt", "ModifiedAt", "ModifiedBy", "Name", "StationCode" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Quận Hoàn Kiếm, Hà Nội", "Nguyễn Văn A", "0123456789", new DateTime(2025, 11, 18, 6, 33, 52, 21, DateTimeKind.Utc).AddTicks(2664), null, null, null, "Trạm quan trắc chất lượng nước sông Hồng", true, false, null, null, null, "Trạm Quan Trắc Sông Hồng", "123123123" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Quận Đống Đa, Hà Nội", "Trần Thị B", "0987654321", new DateTime(2025, 11, 18, 6, 33, 52, 21, DateTimeKind.Utc).AddTicks(2668), null, null, null, "Trạm quan trắc chất lượng nước sông Tô Lịch", true, false, null, null, null, "Trạm Quan Trắc Sông Tô Lịch", "121123123" }
                });

            migrationBuilder.InsertData(
                table: "SystemConfigurations",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "DisplayName", "IsEditable", "Key", "ModifiedAt", "ModifiedBy", "Value", "ValueType" },
                values: new object[,]
                {
                    { new Guid("cccccccc-0001-0000-0000-000000000001"), "BackgroundServices", new DateTime(2025, 11, 18, 6, 33, 52, 861, DateTimeKind.Utc).AddTicks(3310), "System", "Khoảng thời gian quét email mới (giây)", "Email Monitor Interval", true, "EmailMonitorInterval", null, null, "180", "int" },
                    { new Guid("cccccccc-0002-0000-0000-000000000002"), "BackgroundServices", new DateTime(2025, 11, 18, 6, 33, 52, 861, DateTimeKind.Utc).AddTicks(3316), "System", "Khoảng thời gian kiểm tra và tạo cảnh báo (giây)", "Alert Generation Interval", true, "AlertGenerationInterval", null, null, "3600", "int" },
                    { new Guid("cccccccc-0003-0000-0000-000000000003"), "BackgroundServices", new DateTime(2025, 11, 18, 6, 33, 52, 861, DateTimeKind.Utc).AddTicks(3322), "System", "Khoảng thời gian kiểm tra chuyển động (giây)", "Motion Monitor Interval", true, "MotionMonitorInterval", null, null, "60", "int" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "FullName", "IsActive", "IsDeleted", "LastLoginAt", "ModifiedAt", "ModifiedBy", "PasswordHash", "Role", "StationId", "StationId1", "Username" },
                values: new object[,]
                {
                    { "11111111-aaaa-aaaa-aaaa-111111111111", new DateTime(2025, 11, 18, 6, 33, 52, 852, DateTimeKind.Utc).AddTicks(5388), null, null, null, "admin@stationcheck.com", "System Administrator", true, false, null, null, null, "$2a$12$FSPVbMyLtpvA./fz4KQ3ie5KW/FB5vqu1zc6jkBY93oYQ3XEuhsMS", 2, null, null, "admin" },
                    { "22222222-bbbb-bbbb-bbbb-222222222222", new DateTime(2025, 11, 18, 6, 33, 52, 852, DateTimeKind.Utc).AddTicks(5397), null, null, null, "manager@stationcheck.com", "Department Manager", true, false, null, null, null, "$2a$12$59vWYm2Mou4O3c29/g1ANuMoK7vcl9ETz5gqpVeiKf5PdxXKCW3FC", 1, null, null, "manager" }
                });

            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Category", "CreatedAt", "Key", "LanguageCode", "ModifiedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0001-0000-0000-000000000001"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1527), "menu.dashboard", "vi", null, "Trang chủ" },
                    { new Guid("aaaaaaaa-0002-0000-0000-000000000002"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1529), "menu.stations", "vi", null, "Quản lý Trạm" },
                    { new Guid("aaaaaaaa-0003-0000-0000-000000000003"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1531), "menu.users", "vi", null, "Quản lý User" },
                    { new Guid("aaaaaaaa-0004-0000-0000-000000000004"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1533), "menu.settings", "vi", null, "Cấu hình" },
                    { new Guid("aaaaaaaa-0005-0000-0000-000000000005"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1535), "button.add", "vi", null, "Thêm mới" },
                    { new Guid("aaaaaaaa-0006-0000-0000-000000000006"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1537), "button.edit", "vi", null, "Chỉnh sửa" },
                    { new Guid("aaaaaaaa-0007-0000-0000-000000000007"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1539), "button.delete", "vi", null, "Xóa" },
                    { new Guid("aaaaaaaa-0008-0000-0000-000000000008"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1541), "button.save", "vi", null, "Lưu" },
                    { new Guid("aaaaaaaa-0009-0000-0000-000000000009"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1543), "button.cancel", "vi", null, "Hủy" },
                    { new Guid("aaaaaaaa-0010-0000-0000-000000000010"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1551), "station.name", "vi", null, "Tên trạm" },
                    { new Guid("aaaaaaaa-0011-0000-0000-000000000011"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1553), "station.address", "vi", null, "Địa chỉ" },
                    { new Guid("aaaaaaaa-0012-0000-0000-000000000012"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1555), "station.contact", "vi", null, "Người liên hệ" },
                    { new Guid("aaaaaaaa-0013-0000-0000-000000000013"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1557), "station.phone", "vi", null, "Số điện thoại" },
                    { new Guid("aaaaaaaa-0014-0000-0000-000000000014"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1560), "station.page_title", "vi", null, "Quản lý Trạm" },
                    { new Guid("aaaaaaaa-0015-0000-0000-000000000015"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1562), "station.list_title", "vi", null, "Danh sách Trạm Quan Trắc" },
                    { new Guid("aaaaaaaa-0016-0000-0000-000000000016"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1563), "station.add_button", "vi", null, "Thêm Trạm Mới" },
                    { new Guid("aaaaaaaa-0017-0000-0000-000000000017"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1565), "station.edit_title_add", "vi", null, "Thêm Trạm Mới" },
                    { new Guid("aaaaaaaa-0018-0000-0000-000000000018"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1567), "station.edit_title_edit", "vi", null, "Chỉnh sửa Trạm" },
                    { new Guid("aaaaaaaa-0019-0000-0000-000000000019"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1569), "station.search_placeholder", "vi", null, "Tìm kiếm..." },
                    { new Guid("aaaaaaaa-0020-0000-0000-000000000020"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1571), "station.name_column", "vi", null, "Tên Trạm" },
                    { new Guid("aaaaaaaa-0021-0000-0000-000000000021"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1573), "station.address_column", "vi", null, "Địa chỉ" },
                    { new Guid("aaaaaaaa-0022-0000-0000-000000000022"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1575), "station.contact_column", "vi", null, "Người liên hệ" },
                    { new Guid("aaaaaaaa-0023-0000-0000-000000000023"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1577), "station.phone_column", "vi", null, "Số điện thoại" },
                    { new Guid("aaaaaaaa-0024-0000-0000-000000000024"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1579), "station.actions_column", "vi", null, "Thao tác" },
                    { new Guid("aaaaaaaa-0025-0000-0000-000000000025"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1581), "station.name_label", "vi", null, "Tên Trạm:" },
                    { new Guid("aaaaaaaa-0026-0000-0000-000000000026"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1582), "station.address_label", "vi", null, "Địa chỉ:" },
                    { new Guid("aaaaaaaa-0027-0000-0000-000000000027"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1584), "station.description_label", "vi", null, "Mô tả:" },
                    { new Guid("aaaaaaaa-0028-0000-0000-000000000028"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1586), "station.contact_label", "vi", null, "Người liên hệ:" },
                    { new Guid("aaaaaaaa-0029-0000-0000-000000000029"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1601), "station.phone_label", "vi", null, "Số điện thoại:" },
                    { new Guid("aaaaaaaa-0030-0000-0000-000000000030"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1603), "station.active_label", "vi", null, "Kích hoạt giám sát:" },
                    { new Guid("aaaaaaaa-0031-0000-0000-000000000031"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1605), "message.confirm_delete_station", "vi", null, "Bạn có chắc muốn xóa trạm này?" },
                    { new Guid("aaaaaaaa-0032-0000-0000-000000000032"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1607), "message.delete_error", "vi", null, "Không thể xóa" },
                    { new Guid("aaaaaaaa-0033-0000-0000-000000000033"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1609), "message.error", "vi", null, "Lỗi" },
                    { new Guid("bbbbbbbb-0101-0000-0000-000000000101"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1611), "menu.dashboard", "en", null, "Dashboard" },
                    { new Guid("bbbbbbbb-0102-0000-0000-000000000102"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1613), "menu.stations", "en", null, "Station Management" },
                    { new Guid("bbbbbbbb-0103-0000-0000-000000000103"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1615), "menu.users", "en", null, "User Management" },
                    { new Guid("bbbbbbbb-0104-0000-0000-000000000104"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1617), "menu.settings", "en", null, "Settings" },
                    { new Guid("bbbbbbbb-0105-0000-0000-000000000105"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1619), "button.add", "en", null, "Add New" },
                    { new Guid("bbbbbbbb-0106-0000-0000-000000000106"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1621), "button.edit", "en", null, "Edit" },
                    { new Guid("bbbbbbbb-0107-0000-0000-000000000107"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1622), "button.delete", "en", null, "Delete" },
                    { new Guid("bbbbbbbb-0108-0000-0000-000000000108"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1624), "button.save", "en", null, "Save" },
                    { new Guid("bbbbbbbb-0109-0000-0000-000000000109"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1626), "button.cancel", "en", null, "Cancel" },
                    { new Guid("bbbbbbbb-0110-0000-0000-000000000110"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1628), "station.name", "en", null, "Station Name" },
                    { new Guid("bbbbbbbb-0111-0000-0000-000000000111"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1630), "station.address", "en", null, "Address" },
                    { new Guid("bbbbbbbb-0112-0000-0000-000000000112"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1632), "station.contact", "en", null, "Contact Person" },
                    { new Guid("bbbbbbbb-0113-0000-0000-000000000113"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1634), "station.phone", "en", null, "Phone Number" },
                    { new Guid("bbbbbbbb-0114-0000-0000-000000000114"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1636), "station.page_title", "en", null, "Station Management" },
                    { new Guid("bbbbbbbb-0115-0000-0000-000000000115"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1637), "station.list_title", "en", null, "Monitoring Station List" },
                    { new Guid("bbbbbbbb-0116-0000-0000-000000000116"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1639), "station.add_button", "en", null, "Add New Station" },
                    { new Guid("bbbbbbbb-0117-0000-0000-000000000117"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1641), "station.edit_title_add", "en", null, "Add New Station" },
                    { new Guid("bbbbbbbb-0118-0000-0000-000000000118"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1643), "station.edit_title_edit", "en", null, "Edit Station" },
                    { new Guid("bbbbbbbb-0119-0000-0000-000000000119"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1645), "station.search_placeholder", "en", null, "Search..." },
                    { new Guid("bbbbbbbb-0120-0000-0000-000000000120"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1647), "station.name_column", "en", null, "Station Name" },
                    { new Guid("bbbbbbbb-0121-0000-0000-000000000121"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1649), "station.address_column", "en", null, "Address" },
                    { new Guid("bbbbbbbb-0122-0000-0000-000000000122"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1651), "station.contact_column", "en", null, "Contact Person" },
                    { new Guid("bbbbbbbb-0123-0000-0000-000000000123"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1653), "station.phone_column", "en", null, "Phone Number" },
                    { new Guid("bbbbbbbb-0124-0000-0000-000000000124"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1655), "station.actions_column", "en", null, "Actions" },
                    { new Guid("bbbbbbbb-0125-0000-0000-000000000125"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1656), "station.name_label", "en", null, "Station Name:" },
                    { new Guid("bbbbbbbb-0126-0000-0000-000000000126"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1658), "station.address_label", "en", null, "Address:" },
                    { new Guid("bbbbbbbb-0127-0000-0000-000000000127"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1683), "station.description_label", "en", null, "Description:" },
                    { new Guid("bbbbbbbb-0128-0000-0000-000000000128"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1685), "station.contact_label", "en", null, "Contact Person:" },
                    { new Guid("bbbbbbbb-0129-0000-0000-000000000129"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1687), "station.phone_label", "en", null, "Phone Number:" },
                    { new Guid("bbbbbbbb-0130-0000-0000-000000000130"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1688), "station.active_label", "en", null, "Enable Monitoring:" },
                    { new Guid("bbbbbbbb-0131-0000-0000-000000000131"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1690), "message.confirm_delete_station", "en", null, "Are you sure you want to delete this station?" },
                    { new Guid("bbbbbbbb-0132-0000-0000-000000000132"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1692), "message.delete_error", "en", null, "Cannot delete" },
                    { new Guid("bbbbbbbb-0133-0000-0000-000000000133"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1694), "message.error", "en", null, "Error" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "FullName", "IsActive", "IsDeleted", "LastLoginAt", "ModifiedAt", "ModifiedBy", "PasswordHash", "Role", "StationId", "StationId1", "Username" },
                values: new object[] { "33333333-cccc-cccc-cccc-333333333333", new DateTime(2025, 11, 18, 6, 33, 52, 852, DateTimeKind.Utc).AddTicks(5413), null, null, null, "employee1@stationcheck.com", "Nhân viên Trạm 1", true, false, null, null, null, "$2a$12$z3CpZAUQUCyhhWl7HbpbPuTTr4NyFQ6ZLIXx0wjfaqjYoPx5H7lZW", 0, new Guid("11111111-1111-1111-1111-111111111111"), null, "employee1" });

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
                name: "IX_MotionAlerts_CameraId",
                table: "MotionAlerts",
                column: "CameraId");

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
