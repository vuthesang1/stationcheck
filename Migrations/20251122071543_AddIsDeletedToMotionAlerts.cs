using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToMotionAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MotionAlerts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 22, 7, 15, 41, 164, DateTimeKind.Utc).AddTicks(254));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 22, 7, 15, 41, 164, DateTimeKind.Utc).AddTicks(253));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 22, 7, 15, 40, 396, DateTimeKind.Utc).AddTicks(3291));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 22, 7, 15, 40, 396, DateTimeKind.Utc).AddTicks(3297));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 22, 7, 15, 41, 164, DateTimeKind.Utc).AddTicks(5813));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 22, 7, 15, 41, 164, DateTimeKind.Utc).AddTicks(5817));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 22, 7, 15, 41, 159, DateTimeKind.Utc).AddTicks(3389), "$2a$12$VVPZ8HHRmC.pHOTqWNsj4.Tu4/HTCZcR1Mb/8cYfPDQeozdpdNF8q" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 22, 7, 15, 41, 159, DateTimeKind.Utc).AddTicks(3394), "$2a$12$3zNpax0Md8xaC.E1M8D3COr6W.ZUF09lMl0TR80M6MOVBSUZhXcC2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 22, 7, 15, 41, 159, DateTimeKind.Utc).AddTicks(3400), "$2a$12$dt3sJaCkyMq9dWqz/c8YYu5.WXWLdxIpE29P1R7zVxFI5NZbVW7DO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MotionAlerts");

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
    }
}
