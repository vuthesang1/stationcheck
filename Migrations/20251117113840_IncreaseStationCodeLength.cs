using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseStationCodeLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StationCode",
                table: "Stations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "CameraId",
                table: "MotionAlerts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StationCode",
                table: "EmailEvents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(640));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(637));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 39, 58, DateTimeKind.Utc).AddTicks(2512));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 39, 58, DateTimeKind.Utc).AddTicks(2514));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(8861));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(8863));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(8866));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(707));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(709));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(710));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(712));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(714));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(715));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(717));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(719));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(720));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(725));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(727));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(729));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(730));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(732));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(734));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(736));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(737));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(739));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(741));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(742));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(744));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(746));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(747));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(749));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(751));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(753));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(754));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(756));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(758));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(759));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(761));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(763));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(764));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(766));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(768));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(769));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(771));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(773));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(774));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(776));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(778));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(780));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(781));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(783));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(785));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(786));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(788));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(790));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(791));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(793));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(795));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(796));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(798));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(799));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(801));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(803));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(804));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(806));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(808));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(809));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(811));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(813));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(814));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 131,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(816));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 132,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(818));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 133,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 11, 38, 40, 74, DateTimeKind.Utc).AddTicks(820));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 17, 11, 38, 40, 70, DateTimeKind.Utc).AddTicks(1970), "$2a$12$J5Ugo55sKN.tcOF.NB.dweMp4cxoeH/P9CPHqcRr.KdH5GoV2uwN." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 17, 11, 38, 40, 70, DateTimeKind.Utc).AddTicks(1975), "$2a$12$AMQ5XhGtxu1bU9Sm1dfw.eLFAvUCsRtpStdJ59CvR8rx5GrhWKk5a" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 17, 11, 38, 40, 70, DateTimeKind.Utc).AddTicks(1983), "$2a$12$d3.x2K0gx3twhZpdUySkSuo7QweW7flZ4SBLiRLxty1utUjb1U0Aq" });

            migrationBuilder.CreateIndex(
                name: "IX_MotionAlerts_CameraId",
                table: "MotionAlerts",
                column: "CameraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MotionAlerts_CameraId",
                table: "MotionAlerts");

            migrationBuilder.AlterColumn<string>(
                name: "StationCode",
                table: "Stations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CameraId",
                table: "MotionAlerts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "StationCode",
                table: "EmailEvents",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(816));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(810));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 19, 659, DateTimeKind.Utc).AddTicks(2451));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 19, 659, DateTimeKind.Utc).AddTicks(2454));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 740, DateTimeKind.Utc).AddTicks(222));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 740, DateTimeKind.Utc).AddTicks(225));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 740, DateTimeKind.Utc).AddTicks(228));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(877));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(879));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(881));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(883));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(885));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(887));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(889));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(891));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(893));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(912));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(914));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(916));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(918));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(921));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(923));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(925));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(927));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(929));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(931));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(934));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(936));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(938));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(940));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(942));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(944));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(946));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(948));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(950));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(952));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(954));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(956));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(958));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(961));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(963));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(965));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(967));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(969));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(971));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(973));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(975));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(978));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(980));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(983));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(985));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(987));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(989));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(991));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(994));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(996));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1190));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1193));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1195));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1197));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1199));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1201));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1203));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1205));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1208));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1210));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1212));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1214));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1215));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1217));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 131,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1219));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 132,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1221));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 133,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 16, 14, 28, 20, 739, DateTimeKind.Utc).AddTicks(1223));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 16, 14, 28, 20, 735, DateTimeKind.Utc).AddTicks(3351), "$2a$12$9bB3aM2wtwbbtVl0EWqZF.7iZTmPLLLgMVm5PcvY2HzR2S7ob5fMS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 16, 14, 28, 20, 735, DateTimeKind.Utc).AddTicks(3356), "$2a$12$oohWYkn5PXh34VET5QdR7uCmbv4P5mvrwr7dk4DnpFn6lL/3ZIHQq" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 16, 14, 28, 20, 735, DateTimeKind.Utc).AddTicks(3361), "$2a$12$cvmhS4YQ1TRjrhKTxW4ocuc3dzRlzmVaYKmPDMaekG0/Lt.nuZZWu" });
        }
    }
}
