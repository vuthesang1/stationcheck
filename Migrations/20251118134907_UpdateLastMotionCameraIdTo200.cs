using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLastMotionCameraIdTo200 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastMotionCameraId",
                table: "MotionAlerts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 49, 6, 921, DateTimeKind.Utc).AddTicks(7790));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 49, 6, 921, DateTimeKind.Utc).AddTicks(7787));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 49, 5, 998, DateTimeKind.Utc).AddTicks(2235));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 49, 5, 998, DateTimeKind.Utc).AddTicks(2246));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 49, 6, 922, DateTimeKind.Utc).AddTicks(4317));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 49, 6, 922, DateTimeKind.Utc).AddTicks(4321));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 49, 6, 922, DateTimeKind.Utc).AddTicks(4324));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 13, 49, 6, 918, DateTimeKind.Utc).AddTicks(5821), "$2a$12$Lye4Rp3wiZac3e2zMQkwIeT6FMH2iOgGZ7B526s8I7aYK8BPouWPK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 13, 49, 6, 918, DateTimeKind.Utc).AddTicks(5827), "$2a$12$8P2eOBhstCiFFRlVmm7Td.LTRqVGAR55FsTRSmhiNkYB9oauY15Ie" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 13, 49, 6, 918, DateTimeKind.Utc).AddTicks(5831), "$2a$12$qaW7nc4Q.1AdUawwfHdL0OqW3mDk4yJC1dLiMqQYwV0XFLVssgaHK" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastMotionCameraId",
                table: "MotionAlerts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 38, 1, 984, DateTimeKind.Utc).AddTicks(3008));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 38, 1, 984, DateTimeKind.Utc).AddTicks(3004));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 38, 1, 32, DateTimeKind.Utc).AddTicks(7073));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 38, 1, 32, DateTimeKind.Utc).AddTicks(7085));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 38, 1, 985, DateTimeKind.Utc).AddTicks(3040));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 38, 1, 985, DateTimeKind.Utc).AddTicks(3046));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 13, 38, 1, 985, DateTimeKind.Utc).AddTicks(3050));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 13, 38, 1, 978, DateTimeKind.Utc).AddTicks(851), "$2a$12$YBoccqB.nly6LYcySuvnteVMo7rjZqlAIDyTf/vMVV33tQ4Xhir.O" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 13, 38, 1, 978, DateTimeKind.Utc).AddTicks(855), "$2a$12$GRgkpRs/GBtGLtJC40IuK.QLiW/1a14mi1NTwNLFcJmtydCXL1zSu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 13, 38, 1, 978, DateTimeKind.Utc).AddTicks(860), "$2a$12$AszImnJQTocjMpcO9i0OjON8DfeYIp0F2OvvjcGMs2N3hC59z9Gmm" });
        }
    }
}
