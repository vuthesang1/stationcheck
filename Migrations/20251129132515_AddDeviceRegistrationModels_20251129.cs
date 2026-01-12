using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceRegistrationModels_20251129 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CertificateThumbprint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CertificateSubject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CertificateIssuer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CertificateValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificateValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EKUOids = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_UserDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceCertificateAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_DeviceCertificateAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceCertificateAuditLogs_UserDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "UserDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceCertificateAuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeviceUserAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_DeviceUserAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceUserAssignments_UserDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "UserDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceUserAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 13, 25, 14, 677, DateTimeKind.Utc).AddTicks(8899));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 13, 25, 14, 677, DateTimeKind.Utc).AddTicks(8897));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 13, 25, 14, 678, DateTimeKind.Utc).AddTicks(6373));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 13, 25, 14, 678, DateTimeKind.Utc).AddTicks(6377));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 13, 25, 14, 163, DateTimeKind.Utc).AddTicks(6301), "$2a$12$08WcS9IQoeNFG89JflAvme1pJk077SPRSxfIo7nfP3IsFGC17e/R6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 13, 25, 14, 417, DateTimeKind.Utc).AddTicks(4685), "$2a$12$FUtYYlRPJIA9yx4RGR3JBe/t1kXn8B13J5FR46mbzszNDM3jKJSp2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 13, 25, 14, 673, DateTimeKind.Utc).AddTicks(5031), "$2a$12$5MeL/vt9QCcujV5gDSFLteSKcrGL1WlsVVfO0onglkRtlxIBRv1fm" });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceCertificateAuditLogs_Action",
                table: "DeviceCertificateAuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceCertificateAuditLogs_CreatedAt",
                table: "DeviceCertificateAuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceCertificateAuditLogs_DeviceId",
                table: "DeviceCertificateAuditLogs",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceCertificateAuditLogs_UserId",
                table: "DeviceCertificateAuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceUserAssignments_DeviceId_UserId",
                table: "DeviceUserAssignments",
                columns: new[] { "DeviceId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceUserAssignments_IsActive",
                table: "DeviceUserAssignments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceUserAssignments_UserId",
                table: "DeviceUserAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_CertificateThumbprint",
                table: "UserDevices",
                column: "CertificateThumbprint",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_IsApproved",
                table: "UserDevices",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_IsRevoked",
                table: "UserDevices",
                column: "IsRevoked");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceCertificateAuditLogs");

            migrationBuilder.DropTable(
                name: "DeviceUserAssignments");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 3, 39, 40, 740, DateTimeKind.Utc).AddTicks(628));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 3, 39, 40, 740, DateTimeKind.Utc).AddTicks(626));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 3, 39, 40, 741, DateTimeKind.Utc).AddTicks(334));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 3, 39, 40, 741, DateTimeKind.Utc).AddTicks(339));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 3, 39, 40, 199, DateTimeKind.Utc).AddTicks(4523), "$2a$12$Zw6BgCnmlNqDymudbrIBQO3L3r.pZECGDf.pCfE6x8mT0tiiVOjfO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 3, 39, 40, 456, DateTimeKind.Utc).AddTicks(5222), "$2a$12$9bW08/ejr0XvY4a7MNwpDOYZf06mAeI0XjL8rwC3zxQp1OhU8xbLO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 3, 39, 40, 714, DateTimeKind.Utc).AddTicks(4165), "$2a$12$jt/8Osf4OBFAek0RzOut6um6YW35Yj29w/wbgxgMQUnrn3cn0dwD6" });
        }
    }
}
