using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteAndAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TimeFrames",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "TimeFrames",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TimeFrames",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Stations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Stations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "MonitoringProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "MonitoringProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MonitoringProfile",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "MonitoringProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MonitoringProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "MonitoringConfiguration",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "MonitoringConfiguration",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MonitoringConfiguration",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "MonitoringConfiguration",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MonitoringConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2061));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2059));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { new DateTime(2025, 11, 18, 4, 12, 6, 617, DateTimeKind.Utc).AddTicks(7016), null, null, false });

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DeletedAt", "DeletedBy", "IsDeleted" },
                values: new object[] { new DateTime(2025, 11, 18, 4, 12, 6, 617, DateTimeKind.Utc).AddTicks(7020), null, null, false });

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(8068));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(8070));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(8072));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2088));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2090));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2092));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2093));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2095));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2096));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2098));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2099));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2101));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2107));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2109));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2110));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2112));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2113));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2115));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2116));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2118));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2119));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2121));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2122));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2124));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2125));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2127));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2128));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2130));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2131));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2133));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2134));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2136));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2137));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2139));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2140));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2142));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 101,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2143));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 102,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2145));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 103,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2146));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 104,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2148));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 105,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2149));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 106,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2151));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 107,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2152));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 108,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2154));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 109,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2155));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 110,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2157));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 111,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2158));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 112,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2160));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 113,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2161));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 114,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2163));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 115,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2166));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 116,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2167));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 117,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2169));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 118,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2170));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 119,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2172));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 120,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2173));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 121,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2175));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 122,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2176));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 123,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2178));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 124,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2179));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 125,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2181));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 126,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2182));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 127,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2184));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 128,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2185));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 129,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2187));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 130,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2188));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 131,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2190));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 132,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2191));

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: 133,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 4, 12, 7, 625, DateTimeKind.Utc).AddTicks(2193));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "DeletedAt", "DeletedBy", "IsDeleted", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 4, 12, 7, 622, DateTimeKind.Utc).AddTicks(599), null, null, false, "$2a$12$ji4FH.1gJgNCQcZXKH0YGOY1ILZiO0U7ypmjN2By8eH1ve3wbLbu." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "DeletedAt", "DeletedBy", "IsDeleted", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 4, 12, 7, 622, DateTimeKind.Utc).AddTicks(603), null, null, false, "$2a$12$HAIqn7gev23sgk0k3AM2JesgtSM7jgq7eVjd5kPGD4N4CMBI7Vd6y" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "DeletedAt", "DeletedBy", "IsDeleted", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 18, 4, 12, 7, 622, DateTimeKind.Utc).AddTicks(611), null, null, false, "$2a$12$9AYUVelqsF1udUEGX/OATuvhdgUoE1wPenTl0CAF88wu0c9BSm6r." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TimeFrames");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TimeFrames");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TimeFrames");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MonitoringProfile");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "MonitoringProfile");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MonitoringProfile");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MonitoringConfiguration");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "MonitoringConfiguration");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MonitoringConfiguration");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "MonitoringProfile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "MonitoringProfile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "MonitoringConfiguration",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "MonitoringConfiguration",
                type: "nvarchar(max)",
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
        }
    }
}
