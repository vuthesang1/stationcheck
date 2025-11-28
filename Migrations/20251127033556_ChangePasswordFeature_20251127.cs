using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class ChangePasswordFeature_20251127 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
