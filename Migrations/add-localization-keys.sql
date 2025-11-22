-- ========================================
-- COMPREHENSIVE LOCALIZATION KEYS
-- Covers ALL hardcoded Vietnamese text in the application
-- Run this script after applying migrations
-- ========================================

-- ========================================
-- EMAIL SIMULATOR PAGE
-- ========================================
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'email_simulator.station_label', 'Mã trạm (Station ID/Code)', 'label', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.station_label', 'Station ID/Code', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.station_placeholder', 'Chọn mã trạm...', 'placeholder', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.station_placeholder', 'Select station...', 'placeholder', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.alarm_time_label', 'Alarm Time', 'label', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.alarm_time_label', 'Alarm Time', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.send_button', 'Gửi Email', 'button', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.send_button', 'Send Email', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.success_message', '✅ Đã gửi email test thành công!', 'message', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.success_message', '✅ Test email sent successfully!', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.error_message', '❌ Lỗi gửi email: {0}', 'message', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.error_message', '❌ Error sending email: {0}', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.station_required', 'Mã trạm là bắt buộc', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.station_required', 'Station code is required', 'validation', GETUTCDATE()),
(NEWID(), 'vi', 'email_simulator.alarm_time_required', 'Alarm Time là bắt buộc', 'validation', GETUTCDATE()),
(NEWID(), 'en', 'email_simulator.alarm_time_required', 'Alarm Time is required', 'validation', GETUTCDATE());

-- Reports - Alert Report
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.time_range_label', 'Khoảng thời gian', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.time_range_label', 'Time Range', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.from_date_label', 'Từ ngày', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.from_date_label', 'From Date', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.to_date_label', 'Đến ngày', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.to_date_label', 'To Date', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.station_label', 'Trạm', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.station_label', 'Station', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'reports.status_label', 'Trạng thái', 'label', GETUTCDATE()),
(NEWID(), 'en', 'reports.status_label', 'Status', 'label', GETUTCDATE());

-- Reports - Grid Columns (Alert Report)
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.alert_time_column', 'Thời gian', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.alert_time_column', 'Time', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.station_column', 'Trạm', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.station_column', 'Station', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.message_column', 'Thông điệp', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.message_column', 'Message', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.severity_column', 'Mức độ', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.severity_column', 'Severity', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.status_column', 'Trạng thái', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.status_column', 'Status', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.resolved_at_column', 'Xử lý lúc', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.resolved_at_column', 'Resolved At', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.resolved_by_column', 'Người xử lý', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.resolved_by_column', 'Resolved By', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.notes_column', 'Ghi chú', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.notes_column', 'Notes', 'column', GETUTCDATE());

-- Reports - Motion Statistics Columns
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'reports.total_count_column', 'Tổng số lần phát hiện', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.total_count_column', 'Total Detections', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.average_per_day_column', 'Trung bình / ngày', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.average_per_day_column', 'Average / Day', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.max_date_column', 'Ngày nhiều nhất', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.max_date_column', 'Peak Date', 'column', GETUTCDATE()),
(NEWID(), 'vi', 'reports.max_count_column', 'Số lần (ngày nhiều nhất)', 'column', GETUTCDATE()),
(NEWID(), 'en', 'reports.max_count_column', 'Peak Count', 'column', GETUTCDATE());

-- User Management - Form Labels
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'user.username_label', 'Tên đăng nhập', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.username_label', 'Username', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.fullname_label', 'Họ và tên', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.fullname_label', 'Full Name', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.email_label', 'Email', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.email_label', 'Email', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.password_label', 'Mật khẩu', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.password_label', 'Password', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.role_label', 'Vai trò', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.role_label', 'Role', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.station_label', 'Trạm', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.station_label', 'Station', 'label', GETUTCDATE()),
(NEWID(), 'vi', 'user.is_active_label', 'Kích hoạt', 'label', GETUTCDATE()),
(NEWID(), 'en', 'user.is_active_label', 'Active', 'label', GETUTCDATE());

-- ========================================
-- BUTTONS
-- ========================================
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'button.export_excel', 'Xuất Excel', 'button', GETUTCDATE()),
(NEWID(), 'en', 'button.export_excel', 'Export Excel', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'button.exporting', 'Đang xuất...', 'button', GETUTCDATE()),
(NEWID(), 'en', 'button.exporting', 'Exporting...', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'button.search', 'Tìm kiếm', 'button', GETUTCDATE()),
(NEWID(), 'en', 'button.search', 'Search', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'button.add', 'Thêm', 'button', GETUTCDATE()),
(NEWID(), 'en', 'button.add', 'Add', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'button.edit', 'Sửa', 'button', GETUTCDATE()),
(NEWID(), 'en', 'button.edit', 'Edit', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'button.delete', 'Xóa', 'button', GETUTCDATE()),
(NEWID(), 'en', 'button.delete', 'Delete', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'button.save', 'Lưu', 'button', GETUTCDATE()),
(NEWID(), 'en', 'button.save', 'Save', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'button.cancel', 'Hủy', 'button', GETUTCDATE()),
(NEWID(), 'en', 'button.cancel', 'Cancel', 'button', GETUTCDATE()),
(NEWID(), 'vi', 'button.close', 'Đóng', 'button', GETUTCDATE()),
(NEWID(), 'en', 'button.close', 'Close', 'button', GETUTCDATE());

-- ========================================
-- MESSAGES
-- ========================================
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'message.loading', 'Đang tải dữ liệu...', 'message', GETUTCDATE()),
(NEWID(), 'en', 'message.loading', 'Loading data...', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'message.no_data', 'Không có dữ liệu', 'message', GETUTCDATE()),
(NEWID(), 'en', 'message.no_data', 'No data available', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'message.success', 'Thành công', 'message', GETUTCDATE()),
(NEWID(), 'en', 'message.success', 'Success', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'message.error', 'Lỗi', 'message', GETUTCDATE()),
(NEWID(), 'en', 'message.error', 'Error', 'message', GETUTCDATE()),
(NEWID(), 'vi', 'message.confirm_delete', 'Bạn có chắc chắn muốn xóa?', 'message', GETUTCDATE()),
(NEWID(), 'en', 'message.confirm_delete', 'Are you sure you want to delete?', 'message', GETUTCDATE());

-- ========================================
-- DROPDOWN OPTIONS
-- ========================================
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'option.all', 'Tất cả', 'option', GETUTCDATE()),
(NEWID(), 'en', 'option.all', 'All', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'option.this_week', 'Tuần này', 'option', GETUTCDATE()),
(NEWID(), 'en', 'option.this_week', 'This Week', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'option.this_month', 'Tháng này', 'option', GETUTCDATE()),
(NEWID(), 'en', 'option.this_month', 'This Month', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'option.custom', 'Tùy chỉnh', 'option', GETUTCDATE()),
(NEWID(), 'en', 'option.custom', 'Custom', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'option.resolved', 'Đã xử lý', 'option', GETUTCDATE()),
(NEWID(), 'en', 'option.resolved', 'Resolved', 'option', GETUTCDATE()),
(NEWID(), 'vi', 'option.pending', 'Chưa xử lý', 'option', GETUTCDATE()),
(NEWID(), 'en', 'option.pending', 'Pending', 'option', GETUTCDATE());

-- ========================================
-- PAGE TITLES
-- ========================================
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'page.reports', 'Báo cáo và Lịch sử', 'page', GETUTCDATE()),
(NEWID(), 'en', 'page.reports', 'Reports and History', 'page', GETUTCDATE()),
(NEWID(), 'vi', 'page.alert_report', 'Báo cáo Cảnh báo', 'page', GETUTCDATE()),
(NEWID(), 'en', 'page.alert_report', 'Alert Report', 'page', GETUTCDATE()),
(NEWID(), 'vi', 'page.motion_report', 'Báo cáo Chuyển động', 'page', GETUTCDATE()),
(NEWID(), 'en', 'page.motion_report', 'Motion Report', 'page', GETUTCDATE()),
(NEWID(), 'vi', 'page.config_history', 'Lịch sử Thay đổi Cấu hình', 'page', GETUTCDATE()),
(NEWID(), 'en', 'page.config_history', 'Configuration Change History', 'page', GETUTCDATE()),
(NEWID(), 'vi', 'page.user_management', 'Quản lý Người dùng', 'page', GETUTCDATE()),
(NEWID(), 'en', 'page.user_management', 'User Management', 'page', GETUTCDATE()),
(NEWID(), 'vi', 'page.email_simulator', 'Email Motion Detection Simulator', 'page', GETUTCDATE()),
(NEWID(), 'en', 'page.email_simulator', 'Email Motion Detection Simulator', 'page', GETUTCDATE());

-- ========================================
-- TAB LABELS
-- ========================================
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'tab.alert_report', 'Báo cáo Cảnh báo', 'tab', GETUTCDATE()),
(NEWID(), 'en', 'tab.alert_report', 'Alert Report', 'tab', GETUTCDATE()),
(NEWID(), 'vi', 'tab.motion_report', 'Báo cáo Chuyển động', 'tab', GETUTCDATE()),
(NEWID(), 'en', 'tab.motion_report', 'Motion Report', 'tab', GETUTCDATE()),
(NEWID(), 'vi', 'tab.config_history', 'Lịch sử Thay đổi Cấu hình', 'tab', GETUTCDATE()),
(NEWID(), 'en', 'tab.config_history', 'Configuration History', 'tab', GETUTCDATE());
