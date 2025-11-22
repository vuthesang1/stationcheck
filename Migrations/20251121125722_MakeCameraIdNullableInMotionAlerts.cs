using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class MakeCameraIdNullableInMotionAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.AlterColumn<string>(
                name: "CameraId",
                table: "MotionAlerts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 12, 57, 21, 591, DateTimeKind.Utc).AddTicks(3533));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 12, 57, 21, 591, DateTimeKind.Utc).AddTicks(3531));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 12, 57, 20, 744, DateTimeKind.Utc).AddTicks(8398));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 12, 57, 20, 744, DateTimeKind.Utc).AddTicks(8404));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 12, 57, 21, 591, DateTimeKind.Utc).AddTicks(8944));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 12, 57, 21, 591, DateTimeKind.Utc).AddTicks(8949));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 21, 12, 57, 21, 587, DateTimeKind.Utc).AddTicks(5160), "$2a$12$HeqLWUpLvHHYg7dS4M3uPOZMabTVmkickoVczah45TXgSpLfUV3n2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 21, 12, 57, 21, 587, DateTimeKind.Utc).AddTicks(5173), "$2a$12$1ojy46mqTwGt0HEg.aPH3.npmODCH39kQRGyzGeFuM2fbKVZfb9di" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 21, 12, 57, 21, 587, DateTimeKind.Utc).AddTicks(5184), "$2a$12$M3ttokFdNhTJAt6RdkoFFu4Y60J7U.mliV5ukwcWUvX2oy.D/VYMa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CameraId",
                table: "MotionAlerts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
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

            migrationBuilder.InsertData(
                table: "SystemConfigurations",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "DisplayName", "IsEditable", "Key", "ModifiedAt", "ModifiedBy", "Value", "ValueType" },
                values: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "BackgroundServices", new DateTime(2025, 11, 18, 13, 49, 6, 922, DateTimeKind.Utc).AddTicks(4324), "System", "Khoảng thời gian kiểm tra chuyển động (giây)", "Motion Monitor Interval", true, "MotionMonitorInterval", null, null, "60", "int" });

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
    }
}
