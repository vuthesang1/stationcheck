using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StationCheck.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCameraIdAndStationIdTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-0002-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-0003-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0001-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0002-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0003-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0004-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0005-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0006-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0007-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0008-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0009-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0010-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0011-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0012-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0013-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0014-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0015-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0016-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0017-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0018-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0019-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0020-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0021-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0022-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0023-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0024-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0025-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0026-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0027-0000-0000-000000000027"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0028-0000-0000-000000000028"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0029-0000-0000-000000000029"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0030-0000-0000-000000000030"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0031-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0032-0000-0000-000000000032"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0033-0000-0000-000000000033"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0101-0000-0000-000000000101"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0102-0000-0000-000000000102"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0103-0000-0000-000000000103"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0104-0000-0000-000000000104"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0105-0000-0000-000000000105"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0106-0000-0000-000000000106"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0107-0000-0000-000000000107"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0108-0000-0000-000000000108"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0109-0000-0000-000000000109"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0110-0000-0000-000000000110"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0111-0000-0000-000000000111"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0112-0000-0000-000000000112"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0113-0000-0000-000000000113"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0114-0000-0000-000000000114"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0115-0000-0000-000000000115"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0116-0000-0000-000000000116"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0117-0000-0000-000000000117"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0118-0000-0000-000000000118"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0119-0000-0000-000000000119"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0120-0000-0000-000000000120"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0121-0000-0000-000000000121"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0122-0000-0000-000000000122"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0123-0000-0000-000000000123"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0124-0000-0000-000000000124"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0125-0000-0000-000000000125"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0126-0000-0000-000000000126"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0127-0000-0000-000000000127"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0128-0000-0000-000000000128"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0129-0000-0000-000000000129"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0130-0000-0000-000000000130"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0131-0000-0000-000000000131"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0132-0000-0000-000000000132"));

            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-0133-0000-0000-000000000133"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "11111111-aaaa-aaaa-aaaa-111111111111");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "22222222-bbbb-bbbb-bbbb-222222222222");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "33333333-cccc-cccc-cccc-333333333333");

            migrationBuilder.AlterColumn<string>(
                name: "CameraId",
                table: "MotionEvents",
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

            migrationBuilder.InsertData(
                table: "SystemConfigurations",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "DisplayName", "IsEditable", "Key", "ModifiedAt", "ModifiedBy", "Value", "ValueType" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "BackgroundServices", new DateTime(2025, 11, 18, 12, 51, 12, 750, DateTimeKind.Utc).AddTicks(6228), "System", "Khoảng thời gian quét email mới (giây)", "Email Monitor Interval", true, "EmailMonitorInterval", null, null, "180", "int" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "BackgroundServices", new DateTime(2025, 11, 18, 12, 51, 12, 750, DateTimeKind.Utc).AddTicks(6233), "System", "Khoảng thời gian kiểm tra và tạo cảnh báo (giây)", "Alert Generation Interval", true, "AlertGenerationInterval", null, null, "3600", "int" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "BackgroundServices", new DateTime(2025, 11, 18, 12, 51, 12, 750, DateTimeKind.Utc).AddTicks(6237), "System", "Khoảng thời gian kiểm tra nhấn nút (giây)", "Motion Monitor Interval", true, "MotionMonitorInterval", null, null, "60", "int" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "FullName", "IsActive", "IsDeleted", "LastLoginAt", "ModifiedAt", "ModifiedBy", "PasswordHash", "Role", "StationId", "StationId1", "Username" },
                values: new object[,]
                {
                    { "USR001", new DateTime(2025, 11, 18, 12, 51, 12, 744, DateTimeKind.Utc).AddTicks(5320), null, null, null, "admin@stationcheck.com", "System Administrator", true, false, null, null, null, "$2a$12$lORUhnBCsqXr6FyToTmhSeRxXD9207sioiqy12GoRn2yfy4NFUnoG", 2, null, null, "admin" },
                    { "USR002", new DateTime(2025, 11, 18, 12, 51, 12, 744, DateTimeKind.Utc).AddTicks(5333), null, null, null, "manager@stationcheck.com", "Department Manager", true, false, null, null, null, "$2a$12$xa8O7HP0H1itsZ9ufy96A.V74gdPMtb2EgRFrZGzt5eAAK3NDuNOy", 1, null, null, "manager" },
                    { "USR003", new DateTime(2025, 11, 18, 12, 51, 12, 744, DateTimeKind.Utc).AddTicks(5341), null, null, null, "employee1@stationcheck.com", "Nhân viên Trạm 1", true, false, null, null, null, "$2a$12$aKxFVPOyjz38HdBlLuBNSesphK1KKz3eN25w0Z1CBbmzYnfWQjOr6", 0, null, null, "employee1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "SystemConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR001");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR002");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "USR003");

            migrationBuilder.AlterColumn<string>(
                name: "CameraId",
                table: "MotionEvents",
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
                value: new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1482));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Code",
                keyValue: "vi",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1480));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 6, 33, 52, 21, DateTimeKind.Utc).AddTicks(2664));

            migrationBuilder.UpdateData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 6, 33, 52, 21, DateTimeKind.Utc).AddTicks(2668));

            migrationBuilder.InsertData(
                table: "SystemConfigurations",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "DisplayName", "IsEditable", "Key", "ModifiedAt", "ModifiedBy", "Value", "ValueType" },
                values: new object[,]
                {
                    { new Guid("cccccccc-0001-0000-0000-000000000001"), "BackgroundServices", new DateTime(2025, 11, 18, 6, 33, 52, 861, DateTimeKind.Utc).AddTicks(3310), "System", "Khoảng thời gian quét email mới (giây)", "Email Monitor Interval", true, "EmailMonitorInterval", null, null, "180", "int" },
                    { new Guid("cccccccc-0002-0000-0000-000000000002"), "BackgroundServices", new DateTime(2025, 11, 18, 6, 33, 52, 861, DateTimeKind.Utc).AddTicks(3316), "System", "Khoảng thời gian kiểm tra và tạo cảnh báo (giây)", "Alert Generation Interval", true, "AlertGenerationInterval", null, null, "3600", "int" },
                    { new Guid("cccccccc-0003-0000-0000-000000000003"), "BackgroundServices", new DateTime(2025, 11, 18, 6, 33, 52, 861, DateTimeKind.Utc).AddTicks(3322), "System", "Khoảng thời gian kiểm tra nhấn nút (giây)", "Motion Monitor Interval", true, "MotionMonitorInterval", null, null, "60", "int" }
                });

            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Category", "CreatedAt", "Key", "LanguageCode", "ModifiedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0001-0000-0000-000000000001"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1527), "menu.dashboard", "vi", null, "Trang chủ" },
                    { new Guid("aaaaaaaa-0002-0000-0000-000000000002"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1529), "menu.stations", "vi", null, "Quản lý Trạm" },
                    { new Guid("aaaaaaaa-0003-0000-0000-000000000003"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1531), "menu.users", "vi", null, "Quản lý User" },
                    { new Guid("aaaaaaaa-0004-0000-0000-000000000004"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1533), "menu.settings", "vi", null, "Cấu hình" },
                    { new Guid("aaaaaaaa-0005-0000-0000-000000000005"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1535), "button.add", "vi", null, "Thêm mới" },
                    { new Guid("aaaaaaaa-0006-0000-0000-000000000006"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1537), "button.edit", "vi", null, "Chỉnh sửa" },
                    { new Guid("aaaaaaaa-0007-0000-0000-000000000007"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1539), "button.delete", "vi", null, "Xóa" },
                    { new Guid("aaaaaaaa-0008-0000-0000-000000000008"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1541), "button.save", "vi", null, "Lưu" },
                    { new Guid("aaaaaaaa-0009-0000-0000-000000000009"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1543), "button.cancel", "vi", null, "Hủy" },
                    { new Guid("aaaaaaaa-0010-0000-0000-000000000010"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1551), "station.name", "vi", null, "Tên trạm" },
                    { new Guid("aaaaaaaa-0011-0000-0000-000000000011"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1553), "station.address", "vi", null, "Địa chỉ" },
                    { new Guid("aaaaaaaa-0012-0000-0000-000000000012"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1555), "station.contact", "vi", null, "Người liên hệ" },
                    { new Guid("aaaaaaaa-0013-0000-0000-000000000013"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1557), "station.phone", "vi", null, "Số điện thoại" },
                    { new Guid("aaaaaaaa-0014-0000-0000-000000000014"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1560), "station.page_title", "vi", null, "Quản lý Trạm" },
                    { new Guid("aaaaaaaa-0015-0000-0000-000000000015"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1562), "station.list_title", "vi", null, "Danh sách Trạm Quan Trắc" },
                    { new Guid("aaaaaaaa-0016-0000-0000-000000000016"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1563), "station.add_button", "vi", null, "Thêm Trạm Mới" },
                    { new Guid("aaaaaaaa-0017-0000-0000-000000000017"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1565), "station.edit_title_add", "vi", null, "Thêm Trạm Mới" },
                    { new Guid("aaaaaaaa-0018-0000-0000-000000000018"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1567), "station.edit_title_edit", "vi", null, "Chỉnh sửa Trạm" },
                    { new Guid("aaaaaaaa-0019-0000-0000-000000000019"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1569), "station.search_placeholder", "vi", null, "Tìm kiếm..." },
                    { new Guid("aaaaaaaa-0020-0000-0000-000000000020"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1571), "station.name_column", "vi", null, "Tên Trạm" },
                    { new Guid("aaaaaaaa-0021-0000-0000-000000000021"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1573), "station.address_column", "vi", null, "Địa chỉ" },
                    { new Guid("aaaaaaaa-0022-0000-0000-000000000022"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1575), "station.contact_column", "vi", null, "Người liên hệ" },
                    { new Guid("aaaaaaaa-0023-0000-0000-000000000023"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1577), "station.phone_column", "vi", null, "Số điện thoại" },
                    { new Guid("aaaaaaaa-0024-0000-0000-000000000024"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1579), "station.actions_column", "vi", null, "Thao tác" },
                    { new Guid("aaaaaaaa-0025-0000-0000-000000000025"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1581), "station.name_label", "vi", null, "Tên Trạm:" },
                    { new Guid("aaaaaaaa-0026-0000-0000-000000000026"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1582), "station.address_label", "vi", null, "Địa chỉ:" },
                    { new Guid("aaaaaaaa-0027-0000-0000-000000000027"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1584), "station.description_label", "vi", null, "Mô tả:" },
                    { new Guid("aaaaaaaa-0028-0000-0000-000000000028"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1586), "station.contact_label", "vi", null, "Người liên hệ:" },
                    { new Guid("aaaaaaaa-0029-0000-0000-000000000029"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1601), "station.phone_label", "vi", null, "Số điện thoại:" },
                    { new Guid("aaaaaaaa-0030-0000-0000-000000000030"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1603), "station.active_label", "vi", null, "Kích hoạt giám sát:" },
                    { new Guid("aaaaaaaa-0031-0000-0000-000000000031"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1605), "message.confirm_delete_station", "vi", null, "Bạn có chắc muốn xóa trạm này?" },
                    { new Guid("aaaaaaaa-0032-0000-0000-000000000032"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1607), "message.delete_error", "vi", null, "Không thể xóa" },
                    { new Guid("aaaaaaaa-0033-0000-0000-000000000033"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1609), "message.error", "vi", null, "Lỗi" },
                    { new Guid("bbbbbbbb-0101-0000-0000-000000000101"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1611), "menu.dashboard", "en", null, "Dashboard" },
                    { new Guid("bbbbbbbb-0102-0000-0000-000000000102"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1613), "menu.stations", "en", null, "Station Management" },
                    { new Guid("bbbbbbbb-0103-0000-0000-000000000103"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1615), "menu.users", "en", null, "User Management" },
                    { new Guid("bbbbbbbb-0104-0000-0000-000000000104"), "menu", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1617), "menu.settings", "en", null, "Settings" },
                    { new Guid("bbbbbbbb-0105-0000-0000-000000000105"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1619), "button.add", "en", null, "Add New" },
                    { new Guid("bbbbbbbb-0106-0000-0000-000000000106"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1621), "button.edit", "en", null, "Edit" },
                    { new Guid("bbbbbbbb-0107-0000-0000-000000000107"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1622), "button.delete", "en", null, "Delete" },
                    { new Guid("bbbbbbbb-0108-0000-0000-000000000108"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1624), "button.save", "en", null, "Save" },
                    { new Guid("bbbbbbbb-0109-0000-0000-000000000109"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1626), "button.cancel", "en", null, "Cancel" },
                    { new Guid("bbbbbbbb-0110-0000-0000-000000000110"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1628), "station.name", "en", null, "Station Name" },
                    { new Guid("bbbbbbbb-0111-0000-0000-000000000111"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1630), "station.address", "en", null, "Address" },
                    { new Guid("bbbbbbbb-0112-0000-0000-000000000112"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1632), "station.contact", "en", null, "Contact Person" },
                    { new Guid("bbbbbbbb-0113-0000-0000-000000000113"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1634), "station.phone", "en", null, "Phone Number" },
                    { new Guid("bbbbbbbb-0114-0000-0000-000000000114"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1636), "station.page_title", "en", null, "Station Management" },
                    { new Guid("bbbbbbbb-0115-0000-0000-000000000115"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1637), "station.list_title", "en", null, "Monitoring Station List" },
                    { new Guid("bbbbbbbb-0116-0000-0000-000000000116"), "button", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1639), "station.add_button", "en", null, "Add New Station" },
                    { new Guid("bbbbbbbb-0117-0000-0000-000000000117"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1641), "station.edit_title_add", "en", null, "Add New Station" },
                    { new Guid("bbbbbbbb-0118-0000-0000-000000000118"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1643), "station.edit_title_edit", "en", null, "Edit Station" },
                    { new Guid("bbbbbbbb-0119-0000-0000-000000000119"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1645), "station.search_placeholder", "en", null, "Search..." },
                    { new Guid("bbbbbbbb-0120-0000-0000-000000000120"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1647), "station.name_column", "en", null, "Station Name" },
                    { new Guid("bbbbbbbb-0121-0000-0000-000000000121"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1649), "station.address_column", "en", null, "Address" },
                    { new Guid("bbbbbbbb-0122-0000-0000-000000000122"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1651), "station.contact_column", "en", null, "Contact Person" },
                    { new Guid("bbbbbbbb-0123-0000-0000-000000000123"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1653), "station.phone_column", "en", null, "Phone Number" },
                    { new Guid("bbbbbbbb-0124-0000-0000-000000000124"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1655), "station.actions_column", "en", null, "Actions" },
                    { new Guid("bbbbbbbb-0125-0000-0000-000000000125"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1656), "station.name_label", "en", null, "Station Name:" },
                    { new Guid("bbbbbbbb-0126-0000-0000-000000000126"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1658), "station.address_label", "en", null, "Address:" },
                    { new Guid("bbbbbbbb-0127-0000-0000-000000000127"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1683), "station.description_label", "en", null, "Description:" },
                    { new Guid("bbbbbbbb-0128-0000-0000-000000000128"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1685), "station.contact_label", "en", null, "Contact Person:" },
                    { new Guid("bbbbbbbb-0129-0000-0000-000000000129"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1687), "station.phone_label", "en", null, "Phone Number:" },
                    { new Guid("bbbbbbbb-0130-0000-0000-000000000130"), "label", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1688), "station.active_label", "en", null, "Enable Monitoring:" },
                    { new Guid("bbbbbbbb-0131-0000-0000-000000000131"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1690), "message.confirm_delete_station", "en", null, "Are you sure you want to delete this station?" },
                    { new Guid("bbbbbbbb-0132-0000-0000-000000000132"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1692), "message.delete_error", "en", null, "Cannot delete" },
                    { new Guid("bbbbbbbb-0133-0000-0000-000000000133"), "message", new DateTime(2025, 11, 18, 6, 33, 52, 856, DateTimeKind.Utc).AddTicks(1694), "message.error", "en", null, "Error" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Email", "FullName", "IsActive", "IsDeleted", "LastLoginAt", "ModifiedAt", "ModifiedBy", "PasswordHash", "Role", "StationId", "StationId1", "Username" },
                values: new object[,]
                {
                    { "11111111-aaaa-aaaa-aaaa-111111111111", new DateTime(2025, 11, 18, 6, 33, 52, 852, DateTimeKind.Utc).AddTicks(5388), null, null, null, "admin@stationcheck.com", "System Administrator", true, false, null, null, null, "$2a$12$FSPVbMyLtpvA./fz4KQ3ie5KW/FB5vqu1zc6jkBY93oYQ3XEuhsMS", 2, null, null, "admin" },
                    { "22222222-bbbb-bbbb-bbbb-222222222222", new DateTime(2025, 11, 18, 6, 33, 52, 852, DateTimeKind.Utc).AddTicks(5397), null, null, null, "manager@stationcheck.com", "Department Manager", true, false, null, null, null, "$2a$12$59vWYm2Mou4O3c29/g1ANuMoK7vcl9ETz5gqpVeiKf5PdxXKCW3FC", 1, null, null, "manager" },
                    { "33333333-cccc-cccc-cccc-333333333333", new DateTime(2025, 11, 18, 6, 33, 52, 852, DateTimeKind.Utc).AddTicks(5413), null, null, null, "employee1@stationcheck.com", "Nhân viên Trạm 1", true, false, null, null, null, "$2a$12$z3CpZAUQUCyhhWl7HbpbPuTTr4NyFQ6ZLIXx0wjfaqjYoPx5H7lZW", 0, new Guid("11111111-1111-1111-1111-111111111111"), null, "employee1" }
                });
        }
    }
}
