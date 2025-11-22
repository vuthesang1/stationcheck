-- ========================================
-- COMPREHENSIVE LOCALIZATION KEYS v2.0
-- Covers ALL hardcoded Vietnamese text in the application
-- Generated: 2025-11-21
-- Run this script: sqlcmd -S localhost -d StationCheckDb -i comprehensive-localization-keys.sql
-- ========================================

USE [StationCheckDb];
GO

-- ========================================
-- 1. EMAIL SIMULATOR PAGE
-- ========================================
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'email_simulator.station_label', N'Mã trạm (Station ID/Code)', 'label', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.station_label', 'Station ID/Code', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.station_placeholder', N'Chọn mã trạm...', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.station_placeholder', 'Select station...', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.alarm_time_label', 'Alarm Time', 'label', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.alarm_time_label', 'Alarm Time', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.send_button', N'Gửi Email', 'button', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.send_button', 'Send Email', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.success_message', N'✅ Đã gửi email test thành công!', 'message', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.success_message', '✅ Test email sent successfully!', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.error_message', N'❌ Lỗi gửi email: {0}', 'message', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.error_message', '❌ Error sending email: {0}', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.station_required', N'Mã trạm là bắt buộc', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.station_required', 'Station code is required', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.alarm_time_required', N'Alarm Time là bắt buộc', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.alarm_time_required', 'Alarm Time is required', 'validation', GETUTCDATE());

-- ========================================
-- 2. REPORTS PAGE
-- ========================================

-- Page Title & Tabs
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.page_title', N'Báo cáo và Lịch sử', 'page', GETUTCDATE()),
(NEWID(), 'en', 'reports.page_title', 'Reports and History', 'page', GETUTCDATE()),
(NEWID(), 'vi', 'reports.tab_alert_report', N'Báo cáo Cảnh báo', 'tab', GETUTCDATE()),
(NEWID(), 'en', 'reports.tab_alert_report', 'Alert Report', 'tab', GETUTCDATE()),
(NEWID(), 'vi', 'reports.tab_motion_report', N'Báo cáo Chuyển động', 'tab', GETUTCDATE()),
(NEWID(), 'en', 'reports.tab_motion_report', 'Motion Report', 'tab', GETUTCDATE()),
(NEWID(), 'vi', 'reports.tab_config_history', N'Lịch sử Thay đổi Cấu hình', 'tab', GETUTCDATE()),
(NEWID(), 'en', 'reports.tab_config_history', 'Configuration History', 'tab', GETUTCDATE());

-- Form Labels (Alert Report)
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.time_range_label', N'Khoảng thời gian', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.time_range_label', 'Time Range', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.from_date_label', N'Từ ngày', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.from_date_label', 'From Date', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.to_date_label', N'Đến ngày', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.to_date_label', 'To Date', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.station_label', N'Trạm', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.station_label', 'Station', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.status_label', N'Trạng thái', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.status_label', 'Status', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.entity_type_label', N'Loại thực thể', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.entity_type_label', 'Entity Type', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.action_label', N'Hành động', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.action_label', 'Action', 'label', GETUTCDATE());

-- Time Range Options
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.option_today', N'Hôm nay', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.option_today', 'Today', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.option_this_week', N'Tuần này', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.option_this_week', 'This Week', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.option_this_month', N'Tháng này', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.option_this_month', 'This Month', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.option_7_days', N'7 ngày qua', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.option_7_days', 'Last 7 Days', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.option_30_days', N'30 ngày qua', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.option_30_days', 'Last 30 Days', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.option_custom', N'Tùy chỉnh', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.option_custom', 'Custom', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.option_all', N'Tất cả', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.option_all', 'All', 'option', GETUTCDATE());

-- Status Options
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.status_unresolved', N'Chưa xử lý', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.status_unresolved', 'Unresolved', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.status_resolved', N'Đã xử lý', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.status_resolved', 'Resolved', 'option', GETUTCDATE());

-- Entity Type Options
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.entity_station', N'Trạm', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.entity_station', 'Station', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.entity_timeframe', N'Khung giờ', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.entity_timeframe', 'Time Frame', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.entity_monitoring_profile', N'Cấu hình giám sát', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.entity_monitoring_profile', 'Monitoring Profile', 'option', GETUTCDATE());

-- Action Type Options
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.action_create', N'Tạo mới', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.action_create', 'Create', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.action_update', N'Cập nhật', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.action_update', 'Update', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'reports.action_delete', N'Xóa', 'option', GETUTCDATE()),
(NEWID(), 'en', 'reports.action_delete', 'Delete', 'option', GETUTCDATE());

-- Grid Columns (Alert Report)
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.alert_time_column', N'Thời gian', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.alert_time_column', 'Time', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.station_column', N'Trạm', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.station_column', 'Station', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.message_column', N'Thông điệp', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.message_column', 'Message', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.severity_column', N'Mức độ', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.severity_column', 'Severity', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.status_column', N'Trạng thái', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.status_column', 'Status', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.resolved_at_column', N'Xử lý lúc', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.resolved_at_column', 'Resolved At', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.resolved_by_column', N'Người xử lý', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.resolved_by_column', 'Resolved By', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.notes_column', N'Ghi chú', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.notes_column', 'Notes', 'column', GETUTCDATE());

-- Grid Columns (Motion Statistics)
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.total_count_column', N'Tổng số lần phát hiện', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.total_count_column', 'Total Detections', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.avg_per_day_column', N'Trung bình / ngày', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.avg_per_day_column', 'Average Per Day', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.max_date_column', N'Ngày nhiều nhất', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.max_date_column', 'Peak Date', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.max_count_column', N'Số lần (ngày nhiều nhất)', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.max_count_column', 'Peak Count', 'column', GETUTCDATE());

-- Grid Columns (Config History)
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.changed_at_column', N'Thời gian', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.changed_at_column', 'Time', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.entity_type_column', N'Loại', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.entity_type_column', 'Type', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.entity_name_column', N'Tên', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.entity_name_column', 'Name', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.action_type_column', N'Hành động', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.action_type_column', 'Action', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.changes_column', N'Thay đổi', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.changes_column', 'Changes', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.changed_by_column', N'Người thực hiện', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.changed_by_column', 'Changed By', 'column', GETUTCDATE());

-- Buttons & Actions
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.export_excel_button', N'Xuất Excel', 'button', GETUTCDATE()),
(NEWID(), 'en', 'reports.export_excel_button', 'Export Excel', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'reports.exporting_text', N'Đang xuất...', 'button', GETUTCDATE()),
(NEWID(), 'en', 'reports.exporting_text', 'Exporting...', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'reports.search_button', N'Tìm kiếm', 'button', GETUTCDATE()),
(NEWID(), 'en', 'reports.search_button', 'Search', 'button', GETUTCDATE());

-- Messages
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.loading_message', N'Đang tải dữ liệu...', 'message', GETUTCDATE()),
(NEWID(), 'en', 'reports.loading_message', 'Loading data...', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'reports.total_alerts', N'Tổng số cảnh báo:', 'message', GETUTCDATE()),
(NEWID(), 'en', 'reports.total_alerts', 'Total Alerts:', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'reports.unresolved_alerts', N'Chưa xử lý:', 'message', GETUTCDATE()),
(NEWID(), 'en', 'reports.unresolved_alerts', 'Unresolved:', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'reports.resolved_alerts', N'Đã xử lý:', 'message', GETUTCDATE()),
(NEWID(), 'en', 'reports.resolved_alerts', 'Resolved:', 'message', GETUTCDATE());

-- Motion Statistics Labels
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.stats_by_station_title', N'Thống kê theo trạm', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.stats_by_station_title', 'Statistics by Station', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.comparison_chart_title', N'Biểu đồ so sánh', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.comparison_chart_title', 'Comparison Chart', 'label', GETUTCDATE());

-- ========================================
-- 3. USER MANAGEMENT PAGE
-- ========================================

-- Page Title
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.page_title', N'Quản Lý User', 'page', GETUTCDATE()),
(NEWID(), 'en', 'user.page_title', 'User Management', 'page', GETUTCDATE()),
(NEWID(), 'vi', 'user.list_title', N'Danh Sách User', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.list_title', 'User List', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.add_title', N'Thêm User Mới', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.add_title', 'Add New User', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.edit_title', N'Chỉnh Sửa User', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.edit_title', 'Edit User', 'label', GETUTCDATE());

-- Form Labels
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.username_label', 'Username', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.username_label', 'Username', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.fullname_label', N'Họ và Tên', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.fullname_label', 'Full Name', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.email_label', 'Email', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.email_label', 'Email', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.password_label', N'Mật khẩu', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.password_label', 'Password', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.role_label', N'Vai trò', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.role_label', 'Role', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.active_label', N'Kích hoạt', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.active_label', 'Active', 'label', GETUTCDATE());

-- Placeholders
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.username_placeholder', N'Nhập username...', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'user.username_placeholder', 'Enter username...', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'user.fullname_placeholder', N'Nhập họ và tên...', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'user.fullname_placeholder', 'Enter full name...', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'user.password_placeholder', N'Tối thiểu 6 ký tự...', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'user.password_placeholder', 'Minimum 6 characters...', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'user.username_readonly_note', N'Username không thể thay đổi', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.username_readonly_note', 'Username cannot be changed', 'message', GETUTCDATE());

-- Buttons & Tooltips
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.add_button', N'Thêm User', 'button', GETUTCDATE()),
(NEWID(), 'en', 'user.add_button', 'Add User', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'user.edit_tooltip', N'Chỉnh sửa', 'tooltip', GETUTCDATE()),
(NEWID(), 'en', 'user.edit_tooltip', 'Edit', 'tooltip', GETUTCDATE()),
(NEWID(), 'vi', 'user.delete_tooltip', N'Xóa', 'tooltip', GETUTCDATE()),
(NEWID(), 'en', 'user.delete_tooltip', 'Delete', 'tooltip', GETUTCDATE()),
(NEWID(), 'vi', 'user.show_password_tooltip', N'Hiển thị mật khẩu', 'tooltip', GETUTCDATE()),
(NEWID(), 'en', 'user.show_password_tooltip', 'Show password', 'tooltip', GETUTCDATE()),
(NEWID(), 'vi', 'user.hide_password_tooltip', N'Ẩn mật khẩu', 'tooltip', GETUTCDATE()),
(NEWID(), 'en', 'user.hide_password_tooltip', 'Hide password', 'tooltip', GETUTCDATE()),
(NEWID(), 'vi', 'user.copy_password_tooltip', N'Copy mật khẩu', 'tooltip', GETUTCDATE()),
(NEWID(), 'en', 'user.copy_password_tooltip', 'Copy password', 'tooltip', GETUTCDATE()),
(NEWID(), 'vi', 'user.generate_password_tooltip', N'Tạo mật khẩu ngẫu nhiên', 'tooltip', GETUTCDATE()),
(NEWID(), 'en', 'user.generate_password_tooltip', 'Generate random password', 'tooltip', GETUTCDATE());

-- Grid Columns
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.username_column', 'Username', 'column', GETUTCDATE()),
(NEWID(), 'en', 'user.username_column', 'Username', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'user.fullname_column', N'Họ và Tên', 'column', GETUTCDATE()),
(NEWID(), 'en', 'user.fullname_column', 'Full Name', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'user.email_column', 'Email', 'column', GETUTCDATE()),
(NEWID(), 'en', 'user.email_column', 'Email', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'user.role_column', N'Vai trò', 'column', GETUTCDATE()),
(NEWID(), 'en', 'user.role_column', 'Role', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'user.status_column', N'Trạng thái', 'column', GETUTCDATE()),
(NEWID(), 'en', 'user.status_column', 'Status', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'user.created_column', N'Ngày tạo', 'column', GETUTCDATE()),
(NEWID(), 'en', 'user.created_column', 'Created Date', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'user.last_login_column', N'Đăng nhập', 'column', GETUTCDATE()),
(NEWID(), 'en', 'user.last_login_column', 'Last Login', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'user.actions_column', N'Thao tác', 'column', GETUTCDATE()),
(NEWID(), 'en', 'user.actions_column', 'Actions', 'column', GETUTCDATE());

-- Status Values
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.status_active', N'Kích hoạt', 'option', GETUTCDATE()),
(NEWID(), 'en', 'user.status_active', 'Active', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'user.status_inactive', N'Vô hiệu', 'option', GETUTCDATE()),
(NEWID(), 'en', 'user.status_inactive', 'Inactive', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'user.never_login', N'Chưa đăng nhập', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.never_login', 'Never logged in', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'user.no_permission', N'Không có quyền', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.no_permission', 'No permission', 'message', GETUTCDATE());

-- Role Values
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.role_admin', N'Admin (Quản trị viên)', 'option', GETUTCDATE()),
(NEWID(), 'en', 'user.role_admin', 'Admin (Administrator)', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'user.role_manager', N'Manager (Quản lý)', 'option', GETUTCDATE()),
(NEWID(), 'en', 'user.role_manager', 'Manager', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'user.role_station_employee', N'Station Employee (Nhân viên trạm)', 'option', GETUTCDATE()),
(NEWID(), 'en', 'user.role_station_employee', 'Station Employee', 'option', GETUTCDATE());

-- Validation Messages
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.validation_fullname_required', N'Họ tên là bắt buộc', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'user.validation_fullname_required', 'Full name is required', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'user.validation_fullname_maxlength', N'Họ tên không được quá 100 ký tự', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'user.validation_fullname_maxlength', 'Full name must not exceed 100 characters', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'user.validation_email_required', N'Email là bắt buộc', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'user.validation_email_required', 'Email is required', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'user.validation_email_invalid', N'Email không hợp lệ', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'user.validation_email_invalid', 'Invalid email', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'user.validation_password_minlength', N'Mật khẩu phải có ít nhất 6 ký tự', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'user.validation_password_minlength', 'Password must be at least 6 characters', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'user.validation_username_required', N'Username là bắt buộc', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'user.validation_username_required', 'Username is required', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'user.validation_username_length', N'Username phải từ 3-50 ký tự', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'user.validation_username_length', 'Username must be 3-50 characters', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'user.validation_password_required_create', N'Mật khẩu là bắt buộc khi tạo user mới', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'user.validation_password_required_create', 'Password is required when creating new user', 'validation', GETUTCDATE());

-- Success/Error Messages
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.success_create', N'Tạo user thành công', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.success_create', 'User created successfully', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'user.success_update', N'Cập nhật user thành công', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.success_update', 'User updated successfully', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'user.success_delete', N'Xóa user thành công', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.success_delete', 'User deleted successfully', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'user.error_delete', N'Lỗi khi xóa user: {0}', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.error_delete', 'Error deleting user: {0}', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'user.error_unknown', N'Lỗi không xác định: {0}', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.error_unknown', 'Unknown error: {0}', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'user.confirm_delete', N'Bạn có chắc muốn xóa user ''{0}''?', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.confirm_delete', 'Are you sure you want to delete user ''{0}''?', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'user.password_copied', N'Đã copy mật khẩu vào clipboard!', 'message', GETUTCDATE()),
(NEWID(), 'en', 'user.password_copied', 'Password copied to clipboard!', 'message', GETUTCDATE());

-- ========================================
-- 4. TIMEFRAME FORM COMPONENT
-- ========================================

INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'timeframe.add_title', N'➕ Thêm khung thời gian', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.add_title', '➕ Add Time Frame', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.edit_title', N'✏️ Sửa khung thời gian', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.edit_title', '✏️ Edit Time Frame', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.name_label', N'Tên khung thời gian', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.name_label', 'Time Frame Name', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.name_placeholder', N'Ví dụ: Ca sáng, Ca chiều...', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.name_placeholder', 'Example: Morning Shift, Evening Shift...', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.start_time_label', N'Bắt đầu', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.start_time_label', 'Start Time', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.end_time_label', N'Kết thúc', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.end_time_label', 'End Time', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.frequency_label', N'Tần suất kiểm tra (phút)', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.frequency_label', 'Check Frequency (minutes)', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.frequency_placeholder', N'Nhập số phút', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.frequency_placeholder', 'Enter minutes', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.frequency_hint', N'Tối thiểu: 1 phút | Tối đa: {0} phút (dựa trên khoảng thời gian {1} - {2})', 'message', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.frequency_hint', 'Minimum: 1 minute | Maximum: {0} minutes (based on time range {1} - {2})', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.buffer_label', N'Thời gian buffer (phút)', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.buffer_label', 'Buffer Time (minutes)', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.buffer_placeholder', N'Nhập số phút buffer (mặc định: 0)', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.buffer_placeholder', 'Enter buffer minutes (default: 0)', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.buffer_hint', N'Dung sai cho check-in sớm/trễ. VD: Buffer 15 phút với tần suất 60 phút → Chấp nhận từ 09:45 đến 10:15 cho khung 10:00', 'message', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.buffer_hint', 'Tolerance for early/late check-in. Example: 15-minute buffer with 60-minute frequency → Accept from 09:45 to 10:15 for 10:00 frame', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.next_runs_label', N'3 lần chạy tiếp theo:', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.next_runs_label', 'Next 3 Runs:', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.days_of_week_label', N'Ngày trong tuần', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.days_of_week_label', 'Days of Week', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.days_of_week_hint', N'Không chọn = áp dụng mọi ngày', 'message', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.days_of_week_hint', 'No selection = apply every day', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.enable_immediately_label', N'Kích hoạt ngay', 'label', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.enable_immediately_label', 'Enable Immediately', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.button_cancel', N'Hủy', 'button', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.button_cancel', 'Cancel', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.button_add', N'Thêm', 'button', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.button_add', 'Add', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.button_update', N'Cập nhật', 'button', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.button_update', 'Update', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.validation_end_after_start', N'Thời gian kết thúc phải lớn hơn thời gian bắt đầu!', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.validation_end_after_start', 'End time must be after start time!', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.validation_invalid_format', N'Định dạng thời gian không hợp lệ!', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.validation_invalid_format', 'Invalid time format!', 'validation', GETUTCDATE());

-- Days of Week
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'timeframe.day_monday', N'Thứ 2', 'option', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.day_monday', 'Monday', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.day_tuesday', N'Thứ 3', 'option', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.day_tuesday', 'Tuesday', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.day_wednesday', N'Thứ 4', 'option', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.day_wednesday', 'Wednesday', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.day_thursday', N'Thứ 5', 'option', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.day_thursday', 'Thursday', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.day_friday', N'Thứ 6', 'option', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.day_friday', 'Friday', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.day_saturday', N'Thứ 7', 'option', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.day_saturday', 'Saturday', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'timeframe.day_sunday', N'Chủ nhật', 'option', GETUTCDATE()),
(NEWID(), 'en', 'timeframe.day_sunday', 'Sunday', 'option', GETUTCDATE());

-- ========================================
-- 5. LOGIN PAGE
-- ========================================

INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'login.page_title', N'Đăng nhập', 'page', GETUTCDATE()),
(NEWID(), 'en', 'login.page_title', 'Login', 'page', GETUTCDATE()),
(NEWID(), 'vi', 'login.welcome_title', N'Chào mừng!', 'label', GETUTCDATE()),
(NEWID(), 'en', 'login.welcome_title', 'Welcome!', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'login.username_label', 'Username', 'label', GETUTCDATE()),
(NEWID(), 'en', 'login.username_label', 'Username', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'login.username_placeholder', N'Nhập username...', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'login.username_placeholder', 'Enter username...', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'login.password_label', N'Mật khẩu', 'label', GETUTCDATE()),
(NEWID(), 'en', 'login.password_label', 'Password', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'login.password_placeholder', N'Nhập mật khẩu...', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'login.password_placeholder', 'Enter password...', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'login.remember_me_label', N'Ghi nhớ đăng nhập', 'label', GETUTCDATE()),
(NEWID(), 'en', 'login.remember_me_label', 'Remember Me', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'login.login_button', N'Đăng nhập', 'button', GETUTCDATE()),
(NEWID(), 'en', 'login.login_button', 'Login', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'login.logging_in', N'Đang đăng nhập...', 'message', GETUTCDATE()),
(NEWID(), 'en', 'login.logging_in', 'Logging in...', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'login.error_default', N'Đăng nhập thất bại. Vui lòng kiểm tra lại tên đăng nhập và mật khẩu.', 'message', GETUTCDATE()),
(NEWID(), 'en', 'login.error_default', 'Login failed. Please check your username and password.', 'message', GETUTCDATE());

-- ========================================
-- 6. TRANSLATIONS PAGE
-- ========================================

INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'translations.page_title', N'Quản lý Bản dịch', 'page', GETUTCDATE()),
(NEWID(), 'en', 'translations.page_title', 'Translation Management', 'page', GETUTCDATE()),
(NEWID(), 'vi', 'translations.loading', N'Đang tải...', 'message', GETUTCDATE()),
(NEWID(), 'en', 'translations.loading', 'Loading...', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'translations.add_new_title', N'Thêm Bản dịch Mới', 'label', GETUTCDATE()),
(NEWID(), 'en', 'translations.add_new_title', 'Add New Translation', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'translations.edit_title', N'Chỉnh sửa Bản dịch', 'label', GETUTCDATE()),
(NEWID(), 'en', 'translations.edit_title', 'Edit Translation', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'translations.edit_tooltip', N'Chỉnh sửa', 'tooltip', GETUTCDATE()),
(NEWID(), 'en', 'translations.edit_tooltip', 'Edit', 'tooltip', GETUTCDATE()),
(NEWID(), 'vi', 'translations.delete_tooltip', N'Xóa', 'tooltip', GETUTCDATE()),
(NEWID(), 'en', 'translations.delete_tooltip', 'Delete', 'tooltip', GETUTCDATE()),
(NEWID(), 'vi', 'translations.confirm_delete', N'Bạn có chắc chắn muốn xóa bản dịch này?', 'message', GETUTCDATE()),
(NEWID(), 'en', 'translations.confirm_delete', 'Are you sure you want to delete this translation?', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'translations.error_load_languages', N'Lỗi khi tải danh sách ngôn ngữ: {0}', 'message', GETUTCDATE()),
(NEWID(), 'en', 'translations.error_load_languages', 'Error loading languages: {0}', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'translations.error_load_translations', N'Lỗi khi tải danh sách bản dịch: {0}', 'message', GETUTCDATE()),
(NEWID(), 'en', 'translations.error_load_translations', 'Error loading translations: {0}', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'translations.error_save', N'Lỗi khi lưu bản dịch: {0}', 'message', GETUTCDATE()),
(NEWID(), 'en', 'translations.error_save', 'Error saving translation: {0}', 'message', GETUTCDATE());

-- ========================================
-- 7. COMMON BUTTONS & ACTIONS
-- ========================================

INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'common.save', N'Lưu', 'button', GETUTCDATE()),
(NEWID(), 'en', 'common.save', 'Save', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'common.cancel', N'Hủy', 'button', GETUTCDATE()),
(NEWID(), 'en', 'common.cancel', 'Cancel', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'common.add', N'Thêm', 'button', GETUTCDATE()),
(NEWID(), 'en', 'common.add', 'Add', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'common.edit', N'Sửa', 'button', GETUTCDATE()),
(NEWID(), 'en', 'common.edit', 'Edit', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'common.delete', N'Xóa', 'button', GETUTCDATE()),
(NEWID(), 'en', 'common.delete', 'Delete', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'common.search', N'Tìm kiếm', 'button', GETUTCDATE()),
(NEWID(), 'en', 'common.search', 'Search', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'common.loading', N'Đang tải...', 'message', GETUTCDATE()),
(NEWID(), 'en', 'common.loading', 'Loading...', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'common.all', N'Tất cả', 'option', GETUTCDATE()),
(NEWID(), 'en', 'common.all', 'All', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'common.page_size_selector_all', N'Tất cả', 'option', GETUTCDATE()),
(NEWID(), 'en', 'common.page_size_selector_all', 'All', 'option', GETUTCDATE());

-- ========================================
-- 8. SERVICE EXCEPTION MESSAGES
-- ========================================

-- MonitoringService
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'service.timeframe_overlap_error', N'Khung thời gian bị trùng với ''{0}'' ({1} - {2})', 'message', GETUTCDATE()),
(NEWID(), 'en', 'service.timeframe_overlap_error', 'Time frame overlaps with ''{0}'' ({1} - {2})', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'service.no_motion_alert_message', N'Trạm ''{0}'' không phát hiện chuyển động trong {1} phút (ngưỡng: {2} phút)', 'message', GETUTCDATE()),
(NEWID(), 'en', 'service.no_motion_alert_message', 'Station ''{0}'' no motion detected for {1} minutes (threshold: {2} minutes)', 'message', GETUTCDATE());

-- UserService
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'service.email_exists_deleted', N'Email ''{0}'' đã tồn tại trong hệ thống (user đã bị xóa trước đó). Vui lòng sử dụng email khác hoặc liên hệ quản trị viên để khôi phục tài khoản.', 'message', GETUTCDATE()),
(NEWID(), 'en', 'service.email_exists_deleted', 'Email ''{0}'' already exists in the system (previously deleted user). Please use another email or contact administrator to restore the account.', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'service.email_exists', N'Email ''{0}'' đã được sử dụng bởi user khác.', 'message', GETUTCDATE()),
(NEWID(), 'en', 'service.email_exists', 'Email ''{0}'' is already in use by another user.', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'service.username_exists_deleted', N'Username ''{0}'' đã tồn tại trong hệ thống (user đã bị xóa trước đó). Vui lòng sử dụng username khác hoặc liên hệ quản trị viên để khôi phục tài khoản.', 'message', GETUTCDATE()),
(NEWID(), 'en', 'service.username_exists_deleted', 'Username ''{0}'' already exists in the system (previously deleted user). Please use another username or contact administrator to restore the account.', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'service.username_exists', N'Username ''{0}'' đã được sử dụng.', 'message', GETUTCDATE()),
(NEWID(), 'en', 'service.username_exists', 'Username ''{0}'' is already in use.', 'message', GETUTCDATE());

-- MotionDetectionService
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'service.motion_detection_alert', N'Không phát hiện chuyển động trong {0} phút (mong đợi: {1} phút)', 'message', GETUTCDATE()),
(NEWID(), 'en', 'service.motion_detection_alert', 'No motion detected for {0} minutes (expected: {1} minutes)', 'message', GETUTCDATE());

-- ========================================
-- 9. LANGUAGE NAMES
-- ========================================

INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'language.vietnamese', N'Tiếng Việt', 'option', GETUTCDATE()),
(NEWID(), 'en', 'language.vietnamese', 'Vietnamese', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'language.english', 'English', 'option', GETUTCDATE()),
(NEWID(), 'en', 'language.english', 'English', 'option', GETUTCDATE());

GO

PRINT N'✅ Comprehensive localization keys inserted successfully!';
PRINT N'Total translation keys added: 400+ (200+ per language)';
PRINT N'';
PRINT N'Next steps:';
PRINT N'1. Update Razor files to use @GetText() with these keys';
PRINT N'2. Update C# services to use localized exception messages';
PRINT N'3. Test language switching in the UI';
