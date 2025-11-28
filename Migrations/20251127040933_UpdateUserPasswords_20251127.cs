using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPasswords_20251127 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 4, 9, 31, 855, DateTimeKind.Utc).AddTicks(8354));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 4, 9, 31, 855, DateTimeKind.Utc).AddTicks(8352));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 4, 9, 30, 332, DateTimeKind.Utc).AddTicks(6473));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 4, 9, 30, 332, DateTimeKind.Utc).AddTicks(6476));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 4, 9, 31, 856, DateTimeKind.Utc).AddTicks(2915));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 4, 9, 31, 856, DateTimeKind.Utc).AddTicks(2919));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 27, 4, 9, 31, 348, DateTimeKind.Utc).AddTicks(729), "$2a$12$TmPM8b1fhw4PB4w2qKPSoeZ7DSlRzB4yGfAa0iLxC/qhC6jhVwjH2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 27, 4, 9, 31, 600, DateTimeKind.Utc).AddTicks(4314), "$2a$12$zzHK8Ytppz0.htb.WkavMO60WazOICLxR2HMWvyGVHIRQgbrjuF9." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 27, 4, 9, 31, 852, DateTimeKind.Utc).AddTicks(6344), "$2a$12$Rx3GZxR6ECPq.qf57k5xXu.IsV8a/fg4xLelWoLb5f4CgrQjpM12W" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 3, 35, 55, 335, DateTimeKind.Utc).AddTicks(6456));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 3, 35, 55, 335, DateTimeKind.Utc).AddTicks(6455));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 3, 35, 54, 544, DateTimeKind.Utc).AddTicks(4694));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 3, 35, 54, 544, DateTimeKind.Utc).AddTicks(4697));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 3, 35, 55, 336, DateTimeKind.Utc).AddTicks(721));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 3, 35, 55, 336, DateTimeKind.Utc).AddTicks(725));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 27, 3, 35, 55, 333, DateTimeKind.Utc).AddTicks(2067), "$2a$12$LdcvidOKZ08CEzGOIxBBDur34KI7NojTD3YG6uEGwTDIM4q.wZVcG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 27, 3, 35, 55, 333, DateTimeKind.Utc).AddTicks(2076), "$2a$12$wlNFXmf96rBzTS7TduoJk./0AAtf0c/yMAHNTSWE9ogvyKnfLWsQu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 27, 3, 35, 55, 333, DateTimeKind.Utc).AddTicks(2079), "$2a$12$vDFXjIQdDmcjJN0ZjPHueuM4LlMnA.PMFerl.T9twoCMIe/EjwqIG" });
        }
    }
}
