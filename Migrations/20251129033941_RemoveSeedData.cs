using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 3, 4, 45, 464, DateTimeKind.Utc).AddTicks(3125));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 3, 4, 45, 464, DateTimeKind.Utc).AddTicks(3123));

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "ContactPerson", "ContactPhone", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastMotionDetectedAt", "ModifiedAt", "ModifiedBy", "Name", "StationCode" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Quận Hoàn Kiếm, Hà Nội", "Nguyễn Văn A", "0123456789", new DateTime(2025, 11, 29, 3, 4, 43, 749, DateTimeKind.Utc).AddTicks(1355), null, null, null, "Trạm quan trắc chất lượng nước sông Hồng", true, false, null, null, null, "Trạm Quan Trắc Sông Hồng", "123123123" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Quận Đống Đa, Hà Nội", "Trần Thị B", "0987654321", new DateTime(2025, 11, 29, 3, 4, 43, 749, DateTimeKind.Utc).AddTicks(1361), null, null, null, "Trạm quan trắc chất lượng nước sông Tô Lịch", true, false, null, null, null, "Trạm Quan Trắc Sông Tô Lịch", "121123123" }
                });

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 3, 4, 45, 465, DateTimeKind.Utc).AddTicks(710));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 29, 3, 4, 45, 465, DateTimeKind.Utc).AddTicks(716));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 3, 4, 44, 900, DateTimeKind.Utc).AddTicks(9885), "$2a$12$YHU80hUdfBm9Nq9qoMiLA.qNAOH7UzBpHSkwm33Qahypqv02fM7d." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 3, 4, 45, 189, DateTimeKind.Utc).AddTicks(5561), "$2a$12$ar5nkYvphcfS/vt0XIikKe278jeV.ZoSsM20SqsMuXeL01AQEYRp." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 3, 4, 45, 457, DateTimeKind.Utc).AddTicks(9638), "$2a$12$VrwOLJ3AHe04KoNrR6ycE.FP7/Aq0LEciqqsNHqATkBSoVyQUfmGq" });
        }
    }
}
