using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMotionAlertsCameraIdLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CameraId",
                table: "MotionAlerts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CameraId",
                table: "MotionAlerts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 12, 51, 12, 749, DateTimeKind.Utc).AddTicks(7641));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 12, 51, 12, 749, DateTimeKind.Utc).AddTicks(7638));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 12, 51, 11, 888, DateTimeKind.Utc).AddTicks(5438));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 12, 51, 11, 888, DateTimeKind.Utc).AddTicks(5446));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 12, 51, 12, 750, DateTimeKind.Utc).AddTicks(6228));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 12, 51, 12, 750, DateTimeKind.Utc).AddTicks(6233));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 12, 51, 12, 750, DateTimeKind.Utc).AddTicks(6237));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 12, 51, 12, 744, DateTimeKind.Utc).AddTicks(5320), "$2a$12$lORUhnBCsqXr6FyToTmhSeRxXD9207sioiqy12GoRn2yfy4NFUnoG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 12, 51, 12, 744, DateTimeKind.Utc).AddTicks(5333), "$2a$12$xa8O7HP0H1itsZ9ufy96A.V74gdPMtb2EgRFrZGzt5eAAK3NDuNOy" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 12, 51, 12, 744, DateTimeKind.Utc).AddTicks(5341), "$2a$12$aKxFVPOyjz38HdBlLuBNSesphK1KKz3eN25w0Z1CBbmzYnfWQjOr6" });
        }
    }
}
