using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailBodyToMotionEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("01a972e2-2e46-4a53-a809-9f9b90331cf7"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("0aebf9c4-6ec5-447e-9059-34a9269c9c71"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("0dc77265-5e58-41e4-86d7-ac3a210a0079"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("0eec31bc-5ef9-455a-81ad-64b8de7d23a8"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("1ff18562-4c7c-4675-ad39-2c1bb3f3ec7e"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("308f2eb9-4d46-43c0-93db-67cb7ff0994b"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("36bad358-762d-4dfd-a029-6a89647e086a"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("48c5195c-9cd7-451c-99ae-eb39fe88fcb7"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("6cb5470a-05ad-4616-8c78-b40331b0402e"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("6eccf979-6a69-4878-a30b-7cb395de864b"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("71d97313-e14e-4e06-993e-eadb748450f8"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("728e94e5-68c1-4f82-943e-95d7c34b192f"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("84dcf4bf-462f-43ea-82d0-3f854f476124"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("8ac3808d-fef1-46fc-9f8c-406c04b66a79"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("92b38ae9-33f0-4f03-ba78-f063ab8b517d"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("a59fb50e-3e6e-4063-af21-796062cb1f55"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("b9ed5095-8211-47f3-8780-7aa954125561"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("c5b560d1-e995-44c3-94fb-5c68520b9bed"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("dbc80867-ae3e-4ca9-9fe9-108e07ef4608"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("dbf4fea3-b9f1-45df-8129-92c93ed03b02"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("f690fdee-6c40-43bf-b0af-fce6a101ec0e"));

            migrationBuilder.AddColumn<string>(
                name: "EmailBody",
                table: "MotionEvents",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DropColumn(
                name: "EmailBody",
                table: "MotionEvents");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 15, 19, 21, 973, DateTimeKind.Utc).AddTicks(4770));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 15, 19, 21, 973, DateTimeKind.Utc).AddTicks(4768));

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "ContactPerson", "ContactPhone", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastMotionDetectedAt", "ModifiedAt", "ModifiedBy", "Name", "StationCode" },
                values: new object[,]
                {
                    { new Guid("01a972e2-2e46-4a53-a809-9f9b90331cf7"), "Mỹ Phước 3 Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1208), null, null, null, null, true, false, null, null, null, "MỸ PHƯỚC 3", "MYPHUOC3" },
                    { new Guid("0aebf9c4-6ec5-447e-9059-34a9269c9c71"), "Hà Thanh Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1179), null, null, null, null, true, false, null, null, null, "HÀ THANH", "HATHANH" },
                    { new Guid("0dc77265-5e58-41e4-86d7-ac3a210a0079"), "Acredo Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1165), null, null, null, null, true, false, null, null, null, "ACREDO", "ACREDO" },
                    { new Guid("0eec31bc-5ef9-455a-81ad-64b8de7d23a8"), "Nestle 1 Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1210), null, null, null, null, true, false, null, null, null, "NESTLE 1", "NESTLE1" },
                    { new Guid("1ff18562-4c7c-4675-ad39-2c1bb3f3ec7e"), "Dielac 1 Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1174), null, null, null, null, true, false, null, null, null, "DIELAC 1", "DIELAC1" },
                    { new Guid("308f2eb9-4d46-43c0-93db-67cb7ff0994b"), "Đại Thiên Lộc Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1168), null, null, null, null, true, false, null, null, null, "ĐẠI THIÊN LỘC", "DAITHIENLOC" },
                    { new Guid("36bad358-762d-4dfd-a029-6a89647e086a"), "Zhiyi Zinc Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1264), null, null, null, null, true, false, null, null, null, "ZHIYI ZINC", "ZHIYIZINC" },
                    { new Guid("48c5195c-9cd7-451c-99ae-eb39fe88fcb7"), "URC2 Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1259), null, null, null, null, true, false, null, null, null, "URC2", "URC2" },
                    { new Guid("6cb5470a-05ad-4616-8c78-b40331b0402e"), "Saigon Cafe Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1215), null, null, null, null, true, false, null, null, null, "SAIGON CAFE", "SAIGONCAFE" },
                    { new Guid("6eccf979-6a69-4878-a30b-7cb395de864b"), "Thép Thủ Đức Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1220), null, null, null, null, true, false, null, null, null, "THÉP THỦ ĐỨC", "THEPTHUDUC" },
                    { new Guid("71d97313-e14e-4e06-993e-eadb748450f8"), "Vinasoy Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1262), null, null, null, null, true, false, null, null, null, "VINASOY", "VINASOY" },
                    { new Guid("728e94e5-68c1-4f82-943e-95d7c34b192f"), "Hoàng Sơn Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1182), null, null, null, null, true, false, null, null, null, "HOÀNG SƠN", "HOANGSON" },
                    { new Guid("84dcf4bf-462f-43ea-82d0-3f854f476124"), "URC1 Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1257), null, null, null, null, true, false, null, null, null, "URC1", "URC1" },
                    { new Guid("8ac3808d-fef1-46fc-9f8c-406c04b66a79"), "Thuận Đạo Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1222), null, null, null, null, true, false, null, null, null, "THUẬN ĐẠO", "THUANDAO" },
                    { new Guid("92b38ae9-33f0-4f03-ba78-f063ab8b517d"), "Kolon Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1198), null, null, null, null, true, false, null, null, null, "KOLON", "KOLON" },
                    { new Guid("a59fb50e-3e6e-4063-af21-796062cb1f55"), "URC 3 Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1255), null, null, null, null, true, false, null, null, null, "URC 3", "URC3" },
                    { new Guid("b9ed5095-8211-47f3-8780-7aa954125561"), "Lê Phú Gia Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1200), null, null, null, null, true, false, null, null, null, "LÊ PHÚ GIA", "LEPHUGIA" },
                    { new Guid("c5b560d1-e995-44c3-94fb-5c68520b9bed"), "Dielac 2 Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1176), null, null, null, null, true, false, null, null, null, "DIELAC 2", "DIELAC2" },
                    { new Guid("dbc80867-ae3e-4ca9-9fe9-108e07ef4608"), "Lixil Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1203), null, null, null, null, true, false, null, null, null, "LIXIL", "LIXIL" },
                    { new Guid("dbf4fea3-b9f1-45df-8129-92c93ed03b02"), "Nestle 2 Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1212), null, null, null, null, true, false, null, null, null, "NESTLE 2", "NESTLE2" },
                    { new Guid("f690fdee-6c40-43bf-b0af-fce6a101ec0e"), "Lmat Station", null, null, new DateTime(2025, 11, 27, 15, 19, 20, 444, DateTimeKind.Utc).AddTicks(1206), null, null, null, null, true, false, null, null, null, "LMAT", "LMAT" }
                });

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 15, 19, 21, 974, DateTimeKind.Utc).AddTicks(1159));

            migrationBuilder.UpdateData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 27, 15, 19, 21, 974, DateTimeKind.Utc).AddTicks(1161));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 27, 15, 19, 21, 466, DateTimeKind.Utc).AddTicks(4648), "$2a$12$FKY.URnpScprgBbtLEYjjebPqcK5kELXAUxjixkRbomXgK5ieG52i" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 27, 15, 19, 21, 717, DateTimeKind.Utc).AddTicks(2894), "$2a$12$HLH9JGUlM6o33Q3c45nmCu0C8qb4ynyqlCCjfknEeMz3jTv9bMfsK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003",
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 27, 15, 19, 21, 970, DateTimeKind.Utc).AddTicks(2888), "$2a$12$7JGRqTv2PKanAnlK5zPrW.aW3XMh4cde.a9ZDfC5YXjCpcQZJCeO." });
        }
    }
}
