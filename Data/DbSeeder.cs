using Microsoft.EntityFrameworkCore;
using StationCheck.Models;

namespace StationCheck.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Languages
        if (!await context.Languages.AnyAsync())
        {
            var languages = new[]
            {
                new Language
                {
                    Code = "vi",
                    Name = "Vietnamese",
                    NativeName = "Tiếng Việt",
                    IsActive = true,
                    IsDefault = true,
                    FlagIcon = "vi",
                    CreatedAt = DateTime.UtcNow
                },
                new Language
                {
                    Code = "en",
                    Name = "English",
                    NativeName = "English",
                    IsActive = true,
                    IsDefault = false,
                    FlagIcon = "us",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Languages.AddRangeAsync(languages);
            await context.SaveChangesAsync();
        }

        // Seed Translations
        if (!await context.Translations.AnyAsync())
        {
            var translations = new List<Translation>
            {
                // Dashboard translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "dashboard.page_title", Value = "Bảng điều khiển", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "dashboard.title", Value = "Bảng điều khiển", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "dashboard.total_stations", Value = "Tổng số trạm", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "dashboard.active_cameras", Value = "Camera đang hoạt động", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "dashboard.pending_alerts", Value = "Cảnh báo chưa xử lý", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "dashboard.employees", Value = "Nhân viên", Category = "dashboard", CreatedAt = DateTime.UtcNow },

                // Dashboard translations - English
                new Translation { LanguageCode = "en", Key = "dashboard.page_title", Value = "Dashboard", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "dashboard.title", Value = "Dashboard", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "dashboard.total_stations", Value = "Total Stations", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "dashboard.active_cameras", Value = "Active Cameras", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "dashboard.pending_alerts", Value = "Pending Alerts", Category = "dashboard", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "dashboard.employees", Value = "Employees", Category = "dashboard", CreatedAt = DateTime.UtcNow },

                // Menu translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "menu.dashboard", Value = "Bảng điều khiển", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.management", Value = "Quản lý", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.stations", Value = "Quản lý Trạm", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.my_station", Value = "Trạm của tôi", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.motion_monitoring", Value = "Giám sát Chuyển động", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.schedules", Value = "Cấu hình Lịch", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.monitoring", Value = "Giám sát", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.monitoring_settings", Value = "Cấu hình Giám sát", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.monitoring_profiles", Value = "Profile Giám sát", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.personnel", Value = "Nhân sự", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.employees", Value = "Nhân viên", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.users", Value = "Quản lý User", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.system", Value = "Hệ thống", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.languages", Value = "Ngôn ngữ", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.translations", Value = "Bản dịch", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.profile", Value = "Hồ sơ", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "menu.logout", Value = "Đăng xuất", Category = "menu", CreatedAt = DateTime.UtcNow },

                // Menu translations - English
                new Translation { LanguageCode = "en", Key = "menu.dashboard", Value = "Dashboard", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.management", Value = "Management", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.stations", Value = "Station Management", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.my_station", Value = "My Station", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.motion_monitoring", Value = "Motion Monitoring", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.schedules", Value = "Schedules", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.monitoring", Value = "Monitoring", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.monitoring_settings", Value = "Monitoring Settings", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.monitoring_profiles", Value = "Monitoring Profiles", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.personnel", Value = "Personnel", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.employees", Value = "Employees", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.users", Value = "User Management", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.system", Value = "System", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.languages", Value = "Languages", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.translations", Value = "Translations", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.profile", Value = "Profile", Category = "menu", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "menu.logout", Value = "Logout", Category = "menu", CreatedAt = DateTime.UtcNow },

                // Station page translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "station.title", Value = "Quản lý Trạm", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.page_title", Value = "Quản lý Trạm", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.list_title", Value = "Danh sách Trạm Quan Trắc", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.add_button", Value = "Thêm Trạm Mới", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.add_new", Value = "Thêm Trạm mới", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.name_column", Value = "Tên Trạm", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.address_column", Value = "Địa chỉ", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.contact_column", Value = "Người liên hệ", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.phone_column", Value = "Điện thoại", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.description_column", Value = "Mô tả", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.status_column", Value = "Trạng thái", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.created_column", Value = "Ngày tạo", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.modified_column", Value = "Ngày cập nhật", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.actions_column", Value = "Thao tác", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.search_placeholder", Value = "Tìm kiếm...", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.manage_cameras_tooltip", Value = "Quản lý Camera", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.edit_tooltip", Value = "Chỉnh sửa", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.delete_tooltip", Value = "Xóa", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.name_placeholder", Value = "Nhập tên trạm", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.address_placeholder", Value = "Nhập địa chỉ", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.description_placeholder", Value = "Nhập mô tả", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.contact_placeholder", Value = "Tên người liên hệ", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.phone_placeholder", Value = "Nhập số điện thoại", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.edit_title_add", Value = "Thêm Trạm Mới", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.edit_title_edit", Value = "Chỉnh sửa Trạm", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.name_label", Value = "Tên Trạm", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.address_label", Value = "Địa chỉ", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.description_label", Value = "Mô tả", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.contact_label", Value = "Người liên hệ", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.phone_label", Value = "Điện thoại", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "station.active_label", Value = "Trạng thái hoạt động", Category = "station", CreatedAt = DateTime.UtcNow },

                // Station page translations - English
                new Translation { LanguageCode = "en", Key = "station.title", Value = "Station Management", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.page_title", Value = "Station Management", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.list_title", Value = "Monitoring Station List", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.add_button", Value = "Add New Station", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.add_new", Value = "Add New Station", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.name_column", Value = "Station Name", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.address_column", Value = "Address", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.contact_column", Value = "Contact Person", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.phone_column", Value = "Phone Number", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.description_column", Value = "Description", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.status_column", Value = "Status", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.created_column", Value = "Created At", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.modified_column", Value = "Modified At", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.actions_column", Value = "Actions", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.search_placeholder", Value = "Search...", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.manage_cameras_tooltip", Value = "Manage Cameras", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.edit_tooltip", Value = "Edit", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.delete_tooltip", Value = "Delete", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.name_placeholder", Value = "Enter station name", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.address_placeholder", Value = "Enter address", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.description_placeholder", Value = "Enter description", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.contact_placeholder", Value = "Contact person name", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.phone_placeholder", Value = "Enter phone number", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.edit_title_add", Value = "Add New Station", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.edit_title_edit", Value = "Edit Station", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.name_label", Value = "Station Name", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.address_label", Value = "Address", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.description_label", Value = "Description", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.contact_label", Value = "Contact Person", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.phone_label", Value = "Phone Number", Category = "station", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "station.active_label", Value = "Active Status", Category = "station", CreatedAt = DateTime.UtcNow },

                // Message translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "message.confirm_delete_station", Value = "Bạn có chắc chắn muốn xóa trạm này?", Category = "message", CreatedAt = DateTime.UtcNow },
                
                // Message translations - English
                new Translation { LanguageCode = "en", Key = "message.confirm_delete_station", Value = "Are you sure you want to delete this station?", Category = "message", CreatedAt = DateTime.UtcNow },

                // Common translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "common.edit", Value = "Sửa", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.delete", Value = "Xóa", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.save", Value = "Lưu", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.cancel", Value = "Hủy", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.close", Value = "Đóng", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.confirm", Value = "Xác nhận", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.yes", Value = "Có", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.no", Value = "Không", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.loading", Value = "Đang tải...", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.error", Value = "Lỗi", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "common.success", Value = "Thành công", Category = "common", CreatedAt = DateTime.UtcNow },

                // Button translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "button.save", Value = "Lưu", Category = "button", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "button.cancel", Value = "Hủy", Category = "button", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "button.edit", Value = "Chỉnh sửa", Category = "button", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "button.delete", Value = "Xóa", Category = "button", CreatedAt = DateTime.UtcNow },

                // Common translations - English
                new Translation { LanguageCode = "en", Key = "common.edit", Value = "Edit", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.delete", Value = "Delete", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.save", Value = "Save", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.cancel", Value = "Cancel", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.close", Value = "Close", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.confirm", Value = "Confirm", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.yes", Value = "Yes", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.no", Value = "No", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.loading", Value = "Loading...", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.error", Value = "Error", Category = "common", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "common.success", Value = "Success", Category = "common", CreatedAt = DateTime.UtcNow },

                // Button translations - English
                new Translation { LanguageCode = "en", Key = "button.save", Value = "Save", Category = "button", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "button.cancel", Value = "Cancel", Category = "button", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "button.edit", Value = "Edit", Category = "button", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "button.delete", Value = "Delete", Category = "button", CreatedAt = DateTime.UtcNow },

                // UserManagement page translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "user.title", Value = "Quản lý User", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.add_new", Value = "Thêm User", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.username_column", Value = "Tên đăng nhập", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.fullname_column", Value = "Họ và tên", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.email_column", Value = "Email", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.role_column", Value = "Vai trò", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.status_column", Value = "Trạng thái", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.created_column", Value = "Ngày tạo", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.last_login_column", Value = "Đăng nhập lần cuối", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.actions_column", Value = "Thao tác", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.active", Value = "Hoạt động", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.inactive", Value = "Ngưng hoạt động", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.never_login", Value = "Chưa đăng nhập", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.no_permission", Value = "Không có quyền", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "user.no_users_found", Value = "Không tìm thấy user nào.", Category = "user", CreatedAt = DateTime.UtcNow },

                // UserManagement page translations - English
                new Translation { LanguageCode = "en", Key = "user.title", Value = "User Management", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.add_new", Value = "Add User", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.username_column", Value = "Username", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.fullname_column", Value = "Full Name", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.email_column", Value = "Email", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.role_column", Value = "Role", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.status_column", Value = "Status", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.created_column", Value = "Created At", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.last_login_column", Value = "Last Login", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.actions_column", Value = "Actions", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.active", Value = "Active", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.inactive", Value = "Inactive", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.never_login", Value = "Never", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.no_permission", Value = "No permission", Category = "user", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "user.no_users_found", Value = "No users found.", Category = "user", CreatedAt = DateTime.UtcNow },

                // Languages page translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "language.title", Value = "Quản lý Ngôn ngữ", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.add_new", Value = "Thêm Ngôn ngữ", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.code_column", Value = "Mã", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.name_column", Value = "Tên tiếng Anh", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.native_name_column", Value = "Tên ngôn ngữ gốc", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.flag_column", Value = "Biểu tượng", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.default_column", Value = "Mặc định", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.active_column", Value = "Kích hoạt", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.actions_column", Value = "Thao tác", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.default_badge", Value = "Mặc định", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "language.no_languages_found", Value = "Chưa có ngôn ngữ nào. Click \"Thêm Ngôn ngữ\" để tạo ngôn ngữ đầu tiên.", Category = "language", CreatedAt = DateTime.UtcNow },

                // Languages page translations - English
                new Translation { LanguageCode = "en", Key = "language.title", Value = "Language Management", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.add_new", Value = "Add Language", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.code_column", Value = "Code", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.name_column", Value = "English Name", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.native_name_column", Value = "Native Name", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.flag_column", Value = "Flag Icon", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.default_column", Value = "Default", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.active_column", Value = "Active", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.actions_column", Value = "Actions", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.default_badge", Value = "Default", Category = "language", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "language.no_languages_found", Value = "No languages found. Click \"Add Language\" to create the first one.", Category = "language", CreatedAt = DateTime.UtcNow },

                // Translations page translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "translation.title", Value = "Quản lý Bản dịch", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.add_new", Value = "Thêm Bản dịch", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.language_filter", Value = "Ngôn ngữ:", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.category_filter", Value = "Phân loại:", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.all_categories", Value = "Tất cả", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.category_column", Value = "Phân loại", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.key_column", Value = "Key", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.value_column", Value = "Bản dịch", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.actions_column", Value = "Thao tác", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.language_label", Value = "Ngôn ngữ", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.category_label", Value = "Phân loại", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.key_label", Value = "Key", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.value_label", Value = "Bản dịch", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.select_category", Value = "-- Chọn phân loại --", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.key_example", Value = "Ví dụ: menu.dashboard, button.save, label.station_name", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "translation.value_placeholder", Value = "Nhập nội dung dịch...", Category = "translation", CreatedAt = DateTime.UtcNow },

                // Translations page translations - English
                new Translation { LanguageCode = "en", Key = "translation.title", Value = "Translation Management", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.add_new", Value = "Add Translation", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.language_filter", Value = "Language:", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.category_filter", Value = "Category:", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.all_categories", Value = "All", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.category_column", Value = "Category", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.key_column", Value = "Key", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.value_column", Value = "Translation", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.actions_column", Value = "Actions", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.language_label", Value = "Language", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.category_label", Value = "Category", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.key_label", Value = "Key", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.value_label", Value = "Translation", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.select_category", Value = "-- Select Category --", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.key_example", Value = "Example: menu.dashboard, button.save, label.station_name", Category = "translation", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "translation.value_placeholder", Value = "Enter translation content...", Category = "translation", CreatedAt = DateTime.UtcNow },

                // Login page translations - Vietnamese
                new Translation { LanguageCode = "vi", Key = "login.welcome_back", Value = "Chào mừng trở lại!", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "login.error_label", Value = "Lỗi!", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "login.username_placeholder", Value = "Tên đăng nhập", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "login.password_placeholder", Value = "Mật khẩu", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "login.remember_me", Value = "Ghi nhớ đăng nhập", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "login.logging_in", Value = "Đang đăng nhập...", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "login.login_button", Value = "Đăng nhập", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "login.default_accounts", Value = "Tài khoản mặc định:", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "login.failed_error", Value = "Đăng nhập thất bại. Vui lòng kiểm tra lại tên đăng nhập và mật khẩu.", Category = "login", CreatedAt = DateTime.UtcNow },

                // Login page translations - English
                new Translation { LanguageCode = "en", Key = "login.welcome_back", Value = "Welcome Back!", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "login.error_label", Value = "Error!", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "login.username_placeholder", Value = "Username", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "login.password_placeholder", Value = "Password", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "login.remember_me", Value = "Remember Me", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "login.logging_in", Value = "Logging in...", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "login.login_button", Value = "Login", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "login.default_accounts", Value = "Default Accounts:", Category = "login", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "login.failed_error", Value = "Login failed. Please check your username and password.", Category = "login", CreatedAt = DateTime.UtcNow },

                // Modal and form labels - Vietnamese
                new Translation { LanguageCode = "vi", Key = "modal.add_language", Value = "Thêm Ngôn ngữ Mới", Category = "modal", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "modal.edit_language", Value = "Chỉnh sửa Ngôn ngữ", Category = "modal", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "modal.add_translation", Value = "Thêm Bản dịch Mới", Category = "modal", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "modal.edit_translation", Value = "Chỉnh sửa Bản dịch", Category = "modal", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.language_code", Value = "Mã ngôn ngữ", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.language_code_hint", Value = "Mã ISO 639-1 (2-3 ký tự)", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.english_name", Value = "Tên tiếng Anh", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.native_name", Value = "Tên ngôn ngữ gốc", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.flag_icon", Value = "Biểu tượng cờ", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.flag_icon_hint", Value = "Mã quốc gia 2 ký tự (ISO 3166-1 alpha-2)", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.activate_language", Value = "Kích hoạt ngôn ngữ này", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.set_default", Value = "Đặt làm ngôn ngữ mặc định", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.default_hint", Value = "Chỉ có thể có 1 ngôn ngữ mặc định", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.language_select", Value = "Ngôn ngữ", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.category_select", Value = "Phân loại", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.select_category", Value = "-- Chọn phân loại --", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.key_label", Value = "Key", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.key_hint", Value = "Ví dụ: menu.dashboard, button.save, label.station_name", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.translation_label", Value = "Bản dịch", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.translation_placeholder", Value = "Nhập nội dung dịch...", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "form.required", Value = "*", Category = "form", CreatedAt = DateTime.UtcNow },

                // Modal and form labels - English
                new Translation { LanguageCode = "en", Key = "modal.add_language", Value = "Add New Language", Category = "modal", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "modal.edit_language", Value = "Edit Language", Category = "modal", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "modal.add_translation", Value = "Add New Translation", Category = "modal", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "modal.edit_translation", Value = "Edit Translation", Category = "modal", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.language_code", Value = "Language Code", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.language_code_hint", Value = "ISO 639-1 code (2-3 characters)", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.english_name", Value = "English Name", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.native_name", Value = "Native Name", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.flag_icon", Value = "Flag Icon", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.flag_icon_hint", Value = "2-character country code (ISO 3166-1 alpha-2)", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.activate_language", Value = "Activate this language", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.set_default", Value = "Set as default language", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.default_hint", Value = "Only one language can be default", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.language_select", Value = "Language", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.category_select", Value = "Category", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.select_category", Value = "-- Select category --", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.key_label", Value = "Key", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.key_hint", Value = "Example: menu.dashboard, button.save, label.station_name", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.translation_label", Value = "Translation", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.translation_placeholder", Value = "Enter translation content...", Category = "form", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "form.required", Value = "*", Category = "form", CreatedAt = DateTime.UtcNow },

                // Messages and errors - Vietnamese
                new Translation { LanguageCode = "vi", Key = "message.loading_languages", Value = "Lỗi khi tải danh sách ngôn ngữ:", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "message.loading_translations", Value = "Lỗi khi tải danh sách bản dịch:", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "message.saving_error", Value = "Lỗi khi lưu:", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "message.delete_error", Value = "Lỗi khi xóa:", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "message.confirm_delete_language", Value = "Bạn có chắc chắn muốn xóa ngôn ngữ", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "message.delete_language_warning", Value = "Cảnh báo: Tất cả bản dịch liên quan sẽ bị xóa!", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "message.confirm_delete_translation", Value = "Bạn có chắc chắn muốn xóa bản dịch này?", Category = "message", CreatedAt = DateTime.UtcNow },

                // Messages and errors - English
                new Translation { LanguageCode = "en", Key = "message.loading_languages", Value = "Error loading languages:", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "message.loading_translations", Value = "Error loading translations:", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "message.saving_error", Value = "Error saving:", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "message.delete_error", Value = "Error deleting:", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "message.confirm_delete_language", Value = "Are you sure you want to delete language", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "message.delete_language_warning", Value = "Warning: All related translations will be deleted!", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "message.confirm_delete_translation", Value = "Are you sure you want to delete this translation?", Category = "message", CreatedAt = DateTime.UtcNow },

                // Tooltip titles - Vietnamese
                new Translation { LanguageCode = "vi", Key = "tooltip.edit", Value = "Chỉnh sửa", Category = "tooltip", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "tooltip.delete", Value = "Xóa", Category = "tooltip", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "tooltip.manage_cameras", Value = "Quản lý Camera", Category = "tooltip", CreatedAt = DateTime.UtcNow },

                // Tooltip titles - English
                new Translation { LanguageCode = "en", Key = "tooltip.edit", Value = "Edit", Category = "tooltip", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "tooltip.delete", Value = "Delete", Category = "tooltip", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "tooltip.manage_cameras", Value = "Manage Cameras", Category = "tooltip", CreatedAt = DateTime.UtcNow },

                // Monitoring module - Vietnamese
                // Settings page
                new Translation { LanguageCode = "vi", Key = "monitoring.settings_page_title", Value = "Cấu hình Giám sát", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.settings_list_title", Value = "Cấu hình Giám sát Trạm", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.add_config_button", Value = "Thêm Cấu hình", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.add_config_title", Value = "Thêm Cấu hình Giám sát", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.edit_config_title", Value = "Chỉnh sửa Cấu hình Giám sát", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.edit_config_tooltip", Value = "Chỉnh sửa cấu hình", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.delete_config_tooltip", Value = "Xóa cấu hình", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.config_name_label", Value = "Tên Cấu hình:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.config_name_placeholder", Value = "Nhập tên cấu hình", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.config_station_label", Value = "Trạm:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.config_station_placeholder", Value = "Chọn trạm", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.config_profile_label", Value = "Profile Giám sát:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.config_profile_placeholder", Value = "Chọn profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.config_description_label", Value = "Mô tả:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.config_description_placeholder", Value = "Nhập mô tả", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.config_enabled_label", Value = "Bật Giám sát:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                
                // Profiles page
                new Translation { LanguageCode = "vi", Key = "monitoring.profiles_page_title", Value = "Profile Giám sát", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.profiles_list_title", Value = "Danh sách Profile Giám sát", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.add_profile_button", Value = "Thêm Profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.add_profile_title", Value = "Thêm Profile Mới", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.edit_profile_title", Value = "Chỉnh sửa Profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.edit_profile_tooltip", Value = "Chỉnh sửa profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.delete_profile_tooltip", Value = "Xóa profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.profile_name_label", Value = "Tên Profile:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.profile_name_placeholder", Value = "Nhập tên profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.profile_description_label", Value = "Mô tả:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.profile_description_placeholder", Value = "Nhập mô tả", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.profile_active_label", Value = "Kích hoạt:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                
                // TimeFrames
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframes_title", Value = "Khung giờ giám sát", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.add_timeframe_button", Value = "Thêm Khung giờ", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.add_timeframe_title", Value = "Thêm Khung giờ", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.edit_timeframe_title", Value = "Chỉnh sửa Khung giờ", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.edit_timeframe_tooltip", Value = "Chỉnh sửa", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.delete_timeframe_tooltip", Value = "Xóa", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_name_label", Value = "Tên:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_name_placeholder", Value = "Ví dụ: Ca sáng", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_start_label", Value = "Giờ bắt đầu:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_end_label", Value = "Giờ kết thúc:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_frequency_label", Value = "Tần suất kiểm tra (phút):", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_enabled_label", Value = "Bật:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_days_label", Value = "Ngày trong tuần:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_days_placeholder", Value = "1,2,3,4,5 (1=T2, 7=CN)", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_days_hint", Value = "1=Thứ Hai, 2=Thứ Ba, ..., 7=Chủ Nhật. Cách nhau bằng dấu phẩy.", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.no_timeframes", Value = "Chưa có khung giờ nào. Nhấn 'Thêm Khung giờ' để tạo.", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                
                // Column headers
                new Translation { LanguageCode = "vi", Key = "monitoring.name_column", Value = "Tên", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.station_column", Value = "Trạm", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.profile_column", Value = "Profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.description_column", Value = "Mô tả", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.status_column", Value = "Trạng thái", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.created_column", Value = "Ngày tạo", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.modified_column", Value = "Ngày sửa", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.actions_column", Value = "Thao tác", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_name_column", Value = "Tên", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_time_column", Value = "Thời gian", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_frequency_column", Value = "Tần suất", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_days_column", Value = "Ngày", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.timeframe_status_column", Value = "Trạng thái", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.search_placeholder", Value = "Tìm kiếm...", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "monitoring.minutes_unit", Value = "phút", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                
                // Messages
                new Translation { LanguageCode = "vi", Key = "message.confirm_delete_config", Value = "Bạn có chắc muốn xóa cấu hình này?", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "message.confirm_delete_profile", Value = "Bạn có chắc muốn xóa profile này? Tất cả khung giờ sẽ bị xóa.", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "vi", Key = "message.confirm_delete_timeframe", Value = "Bạn có chắc muốn xóa khung giờ này?", Category = "message", CreatedAt = DateTime.UtcNow },

                // Monitoring module - English
                // Settings page
                new Translation { LanguageCode = "en", Key = "monitoring.settings_page_title", Value = "Monitoring Settings", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.settings_list_title", Value = "Station Monitoring Configuration", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.add_config_button", Value = "Add Configuration", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.add_config_title", Value = "Add Monitoring Configuration", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.edit_config_title", Value = "Edit Monitoring Configuration", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.edit_config_tooltip", Value = "Edit configuration", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.delete_config_tooltip", Value = "Delete configuration", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.config_name_label", Value = "Configuration Name:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.config_name_placeholder", Value = "Enter configuration name", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.config_station_label", Value = "Station:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.config_station_placeholder", Value = "Select station", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.config_profile_label", Value = "Monitoring Profile:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.config_profile_placeholder", Value = "Select profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.config_description_label", Value = "Description:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.config_description_placeholder", Value = "Enter description", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.config_enabled_label", Value = "Enable Monitoring:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                
                // Profiles page
                new Translation { LanguageCode = "en", Key = "monitoring.profiles_page_title", Value = "Monitoring Profiles", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.profiles_list_title", Value = "Monitoring Profiles List", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.add_profile_button", Value = "Add Profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.add_profile_title", Value = "Add New Profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.edit_profile_title", Value = "Edit Profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.edit_profile_tooltip", Value = "Edit profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.delete_profile_tooltip", Value = "Delete profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.profile_name_label", Value = "Profile Name:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.profile_name_placeholder", Value = "Enter profile name", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.profile_description_label", Value = "Description:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.profile_description_placeholder", Value = "Enter description", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.profile_active_label", Value = "Active:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                
                // TimeFrames
                new Translation { LanguageCode = "en", Key = "monitoring.timeframes_title", Value = "Time Frames", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.add_timeframe_button", Value = "Add Time Frame", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.add_timeframe_title", Value = "Add Time Frame", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.edit_timeframe_title", Value = "Edit Time Frame", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.edit_timeframe_tooltip", Value = "Edit", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.delete_timeframe_tooltip", Value = "Delete", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_name_label", Value = "Name:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_name_placeholder", Value = "e.g., Morning Shift", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_start_label", Value = "Start Time:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_end_label", Value = "End Time:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_frequency_label", Value = "Check Frequency (minutes):", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_enabled_label", Value = "Enabled:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_days_label", Value = "Days of Week:", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_days_placeholder", Value = "1,2,3,4,5 (1=Mon, 7=Sun)", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_days_hint", Value = "1=Monday, 2=Tuesday, ..., 7=Sunday. Separate with commas.", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.no_timeframes", Value = "No time frames configured. Click 'Add Time Frame' to create one.", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                
                // Column headers
                new Translation { LanguageCode = "en", Key = "monitoring.name_column", Value = "Name", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.station_column", Value = "Station", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.profile_column", Value = "Profile", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.description_column", Value = "Description", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.status_column", Value = "Status", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.created_column", Value = "Created At", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.modified_column", Value = "Modified At", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.actions_column", Value = "Actions", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_name_column", Value = "Name", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_time_column", Value = "Time", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_frequency_column", Value = "Frequency", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_days_column", Value = "Days", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.timeframe_status_column", Value = "Status", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.search_placeholder", Value = "Search...", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "monitoring.minutes_unit", Value = "min", Category = "monitoring", CreatedAt = DateTime.UtcNow },
                
                // Messages
                new Translation { LanguageCode = "en", Key = "message.confirm_delete_config", Value = "Are you sure you want to delete this configuration?", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "message.confirm_delete_profile", Value = "Are you sure you want to delete this profile? All time frames will be deleted.", Category = "message", CreatedAt = DateTime.UtcNow },
                new Translation { LanguageCode = "en", Key = "message.confirm_delete_timeframe", Value = "Are you sure you want to delete this time frame?", Category = "message", CreatedAt = DateTime.UtcNow },
            };

            await context.Translations.AddRangeAsync(translations);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Seed real stations - removes test stations and creates 21 production stations
    /// </summary>
    public static async Task SeedStationsAsync(ApplicationDbContext context)
    {
        // Remove all test stations (stations with names starting with "trạm" or "Trạm")
        // Delete ALL existing stations (clean slate)
        var allStations = await context.Stations.ToListAsync();

        if (allStations.Any())
        {
            // Delete related data in correct order (respecting foreign key constraints)
            var stationIds = allStations.Select(s => s.Id).ToList();
            
            // Delete TimeFrameHistories first (references both TimeFrames and Stations)
            var relatedTimeFrameHistories = await context.TimeFrameHistories
                .Where(tfh => tfh.StationId.HasValue && stationIds.Contains(tfh.StationId.Value))
                .ToListAsync();
            
            context.TimeFrameHistories.RemoveRange(relatedTimeFrameHistories);
            
            // Delete TimeFrames (referenced by MotionAlerts)
            var relatedTimeFrames = await context.TimeFrames
                .Where(tf => tf.StationId.HasValue && stationIds.Contains(tf.StationId.Value))
                .ToListAsync();
            
            context.TimeFrames.RemoveRange(relatedTimeFrames);
            
            // Delete related MotionAlerts
            var relatedAlerts = await context.MotionAlerts
                .Where(ma => ma.StationId.HasValue && stationIds.Contains(ma.StationId.Value))
                .ToListAsync();
            
            context.MotionAlerts.RemoveRange(relatedAlerts);
            
            // Delete related MotionEvents
            var relatedEvents = await context.MotionEvents
                .Where(me => me.StationId.HasValue && stationIds.Contains(me.StationId.Value))
                .ToListAsync();
            
            context.MotionEvents.RemoveRange(relatedEvents);
            
            // Finally delete the stations
            context.Stations.RemoveRange(allStations);
            await context.SaveChangesAsync();
            
            Console.WriteLine($"[DbSeeder] Removed {allStations.Count} existing stations with their related data");
        }

        // Check if real stations already exist (should be 0 now)
        var existingCount = await context.Stations.CountAsync();
        if (existingCount >= 21)
        {
            Console.WriteLine("[DbSeeder] Stations already seeded, skipping...");
            return;
        }

        // Create 21 real stations
        var stations = new[]
        {
            new Station
            {
                StationCode = "ST000001",
                Name = "Trạm Lê Phan Gia",
                Address = "Lô E15 và E16, đường N4 và D1, KCN Nam Tân Uyên mở rộng, Thành phố Dĩ An, Tỉnh Bình Dương, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000002",
                Name = "Trạm Dielac 1",
                Address = "KCN Biên Hòa 1, Xa lộ Hà Nội, Thành phố Biên Hòa, Tỉnh Đồng Nai",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000003",
                Name = "Trạm Kolon",
                Address = "Lô C_5_CN, KCN Bàu Bàng mở rộng, Thị xã Bàu Bàng, Tỉnh Bình Dương, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000004",
                Name = "URC2",
                Address = "Số 42 VSIP Đại lộ Tự Do, KCN Việt Nam - Singapore I-A, Phường Vĩnh Tân, Thành phố Tân Uyên",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000005",
                Name = "Trạm Sài Gòn Cam",
                Address = "Lô A3, Nhóm NAY, Khu công nghiệp NAY Phước 2, Phường Bửu Hòa, Thành phố Biên Hòa, Tỉnh Đồng Nai, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000006",
                Name = "Trạm Lipol",
                Address = "Số 28-3, Khu phố 2, Đường An Phú Đông, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000007",
                Name = "Trạm Mỹ Phước 3",
                Address = "Lô D_2-CN, đường NE8, KCN Mỹ Phước 3, Đường Thới Hòa, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000008",
                Name = "Trạm Đại Thiên Lộc",
                Address = "Ô 13D, Lô CN 3, đường CN3, Phường Bình Đường, Tỉnh Bình Dương, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000009",
                Name = "Trạm URC1",
                Address = "Số 3C, đường số 6, Khu công nghiệp Việt Nam - Singapore I-A, Phường Vĩnh Tân, Thành phố Tân Uyên, Tỉnh Bình Dương, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000010",
                Name = "Trạm Tiên Thủ Đức",
                Address = "Tên đề 9 đường Võ Nguyên Giáp, Phường Thủ Đức, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000011",
                Name = "Trạm Lonat",
                Address = "Địa chỉ đường số 9, KCN Linh Xuân 3, Khu công nghiệp Mỹ Phước, Phường Thới Hòa, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000012",
                Name = "Trạm Hoàng Sen",
                Address = "Lô CN7, đường N4, Khu công nghiệp Song Thần 1, Phường Bình Phước, Tỉnh Bình Dương, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000013",
                Name = "Trạm Thuận Đạo",
                Address = "Lô 7, đường số II, KCN Tân Phú Trung, Xã Long Cang, Tỉnh Tiền Giang",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000014",
                Name = "Trạm Vinacoy",
                Address = "Số 5, đường số 11, KCN Tân Đô, Đức Hòa, Phường Vĩnh Tân, Tỉnh Thuận An, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000015",
                Name = "Trạm URC 3",
                Address = "Số 22 VSIP II-A, Đường số 25, KCN Việt Nam - Singapore II-A, Phường Vĩnh Tân, Thành phố Thuận An, Tỉnh Bình Dương, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000016",
                Name = "Trạm Dialoc2",
                Address = "Số 9 Đại Lộ 1A, Tỉnh Đồng Nai, KCN Việt Nam - Singapore, Khu B, Phường Vĩnh Tân, Thành phố Thuận An, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000017",
                Name = "Trị Thành",
                Address = "K12, 34 Tân Nhựt, Tỉnh Đồng Tháp",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000018",
                Name = "Zhyo Ziluc Industry",
                Address = "Lô 0-5, 61-3-1, đường N10, đường NE 1, đường Dĩ An, Phường Dĩ An, Tỉnh Tây Ninh, Thành phố Dĩ An",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000019",
                Name = "Nestle 1",
                Address = "Số 7, Đường Tân Bình Cong Nghiep Binh Bich 1, Phường Long Bình, Tỉnh Đồng Nai",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000020",
                Name = "Aceclo",
                Address = "Lô 311-312, đường số 17, KCN Việt Nam - Singapore II-A, Phường Vĩnh Tân, Thành phố Thuận An, Thành phố Hồ Chí Minh",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Station
            {
                StationCode = "ST000021",
                Name = "Nestle 2",
                Address = "Lô 311, Đường 09, KCN Amata, Phường Long Bình, Tỉnh Đồng Nai",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Stations.AddRangeAsync(stations);
        await context.SaveChangesAsync();

        Console.WriteLine($"[DbSeeder] Successfully seeded {stations.Length} real stations");
    }
}
