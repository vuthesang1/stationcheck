# Hướng dẫn Localization cho StationCheck

## Tổng quan
Tất cả các label, button text, message hardcode đã được chuyển sang hệ thống localization để hỗ trợ đa ngôn ngữ (Tiếng Việt và English).

## Cách sử dụng trong Razor Components

### 1. Inject LocalizationStateService
Thêm vào đầu file .razor:
```razor
@inject LocalizationStateService LocalizationState
```

### 2. Sử dụng GetText() method
```razor
<label class="form-label">@GetText("label_key", "Fallback Text")</label>
```

### 3. Category Structure
Keys được tổ chức theo category:

- **label**: Form labels (email_simulator.station_label, user.username_label)
- **column**: Grid column captions (reports.alert_time_column, user.username_column)
- **button**: Button text (button.save, button.cancel, button.export_excel)
- **message**: System messages (message.loading, message.success)
- **option**: Dropdown options (option.all, option.this_week)
- **page**: Page titles (page.reports, page.user_management)
- **tab**: Tab labels (tab.alert_report, tab.motion_report)

## Danh sách Translation Keys

### Email Simulator
```
email_simulator.station_label = "Mã trạm (Station ID/Code)" / "Station ID/Code"
email_simulator.alarm_time_label = "Alarm Time" / "Alarm Time"
email_simulator.send_button = "Gửi Email" / "Send Email"
```

### Reports Page
#### Labels
```
reports.time_range_label = "Khoảng thời gian" / "Time Range"
reports.from_date_label = "Từ ngày" / "From Date"
reports.to_date_label = "Đến ngày" / "To Date"
reports.station_label = "Trạm" / "Station"
reports.status_label = "Trạng thái" / "Status"
```

#### Alert Report Columns
```
reports.alert_time_column = "Thời gian" / "Time"
reports.station_column = "Trạm" / "Station"
reports.message_column = "Thông điệp" / "Message"
reports.severity_column = "Mức độ" / "Severity"
reports.status_column = "Trạng thái" / "Status"
reports.resolved_at_column = "Xử lý lúc" / "Resolved At"
reports.resolved_by_column = "Người xử lý" / "Resolved By"
reports.notes_column = "Ghi chú" / "Notes"
```

#### Motion Statistics Columns
```
reports.total_count_column = "Tổng số lần phát hiện" / "Total Detections"
reports.average_per_day_column = "Trung bình / ngày" / "Average / Day"
reports.max_date_column = "Ngày nhiều nhất" / "Peak Date"
reports.max_count_column = "Số lần (ngày nhiều nhất)" / "Peak Count"
```

### User Management
#### Labels
```
user.username_label = "Tên đăng nhập" / "Username"
user.fullname_label = "Họ và tên" / "Full Name"
user.email_label = "Email" / "Email"
user.password_label = "Mật khẩu" / "Password"
user.role_label = "Vai trò" / "Role"
user.station_label = "Trạm" / "Station"
user.is_active_label = "Kích hoạt" / "Active"
```

#### Columns (Existing)
```
user.username_column = "Username" / "Username"
user.fullname_column = "Họ và Tên" / "Full Name"
user.email_column = "Email" / "Email"
user.role_column = "Vai trò" / "Role"
user.status_column = "Trạng thái" / "Status"
user.created_column = "Ngày tạo" / "Created"
user.last_login_column = "Đăng nhập" / "Last Login"
user.actions_column = "Thao tác" / "Actions"
```

### Common Buttons
```
button.export_excel = "Xuất Excel" / "Export Excel"
button.exporting = "Đang xuất..." / "Exporting..."
button.search = "Tìm kiếm" / "Search"
button.add = "Thêm" / "Add"
button.edit = "Sửa" / "Edit"
button.delete = "Xóa" / "Delete"
button.save = "Lưu" / "Save"
button.cancel = "Hủy" / "Cancel"
button.close = "Đóng" / "Close"
```

### Common Messages
```
message.loading = "Đang tải dữ liệu..." / "Loading data..."
message.no_data = "Không có dữ liệu" / "No data available"
message.success = "Thành công" / "Success"
message.error = "Lỗi" / "Error"
message.confirm_delete = "Bạn có chắc chắn muốn xóa?" / "Are you sure you want to delete?"
```

### Dropdown Options
```
option.all = "Tất cả" / "All"
option.this_week = "Tuần này" / "This Week"
option.this_month = "Tháng này" / "This Month"
option.custom = "Tùy chỉnh" / "Custom"
option.resolved = "Đã xử lý" / "Resolved"
option.pending = "Chưa xử lý" / "Pending"
```

### Page Titles
```
page.reports = "Báo cáo và Lịch sử" / "Reports and History"
page.alert_report = "Báo cáo Cảnh báo" / "Alert Report"
page.motion_report = "Báo cáo Chuyển động" / "Motion Report"
page.config_history = "Lịch sử Thay đổi Cấu hình" / "Configuration Change History"
page.user_management = "Quản lý Người dùng" / "User Management"
page.email_simulator = "Email Motion Detection Simulator"
```

### Tab Labels
```
tab.alert_report = "Báo cáo Cảnh báo" / "Alert Report"
tab.motion_report = "Báo cáo Chuyển động" / "Motion Report"
tab.config_history = "Lịch sử Thay đổi Cấu hình" / "Configuration History"
```

## Ví dụ chuyển đổi code

### Before (Hardcoded):
```razor
<label class="form-label">Mã trạm (Station ID/Code)</label>
<DxGridDataColumn Caption="Thời gian" />
<button class="btn btn-primary">Xuất Excel</button>
```

### After (Localized):
```razor
<label class="form-label">@GetText("email_simulator.station_label", "Mã trạm (Station ID/Code)")</label>
<DxGridDataColumn Caption="@GetText("reports.alert_time_column", "Thời gian")" />
<button class="btn btn-primary">@GetText("button.export_excel", "Xuất Excel")</button>
```

## Cách thêm translation key mới

### 1. Thêm vào Database
```sql
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'your.key', 'Giá trị tiếng Việt', 'category', GETUTCDATE()),
(NEWID(), 'en', 'your.key', 'English Value', 'category', GETUTCDATE());
```

### 2. Sử dụng trong code
```razor
@GetText("your.key", "Default Value")
```

## Áp dụng vào project

### Bước 1: Chạy SQL Script
```bash
# Kết nối SQL Server và chạy file:
sqlcmd -S localhost -d StationCheckDb -i Migrations/add-localization-keys.sql
```

### Bước 2: Cập nhật code
Thay thế tất cả hardcoded text bằng `@GetText()` calls theo danh sách trên.

### Bước 3: Test
1. Chạy ứng dụng
2. Đổi ngôn ngữ qua menu Language
3. Kiểm tra các label đã đổi ngôn ngữ chưa

## Lưu ý quan trọng
- **Fallback text** trong GetText() nên giữ nguyên text tiếng Việt để đảm bảo hiển thị khi chưa load được translation
- **Key naming**: Dùng format `category.specific_name` (vd: reports.station_label)
- **Consistency**: Dùng cùng key cho cùng text ở nhiều nơi (vd: "Trạm" dùng `reports.station_label`)
