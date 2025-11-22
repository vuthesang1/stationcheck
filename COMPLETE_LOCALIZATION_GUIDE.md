# 🌍 Complete Localization Implementation Guide

## 📋 Overview

This guide covers **removing ALL hardcoded Vietnamese text** from the StationCheck application and implementing proper internationalization (i18n) using the existing localization system.

## ✅ What Has Been Completed

1. ✅ **Comprehensive SQL Script Created**: `comprehensive-localization-keys.sql`
   - 400+ translation keys covering every page
   - Both Vietnamese (vi) and English (en) translations
   - Organized by page/component/category

2. ✅ **Localization Infrastructure Exists**:
   - `LocalizationStateService` with `GetText()` method
   - `Language` and `Translation` models
   - Database tables ready

## 🎯 Implementation Steps

### Step 1: Install Translation Keys

```powershell
# Run the comprehensive SQL script
sqlcmd -S localhost -d StationCheckDb -i Migrations/comprehensive-localization-keys.sql

# Or using SQL Server Management Studio:
# - Open comprehensive-localization-keys.sql
# - Execute against StationCheckDb database
```

**Verify Installation:**
```sql
SELECT COUNT(*) FROM Translations;
-- Should return 400+ rows

SELECT * FROM Translations WHERE [Key] LIKE 'email_simulator.%';
-- Should show all email simulator keys
```

---

### Step 2: Update Razor Files

#### **Pattern to Follow:**

**Before:**
```razor
<label class="form-label">Mã trạm (Station ID/Code)</label>
```

**After:**
```razor
<label class="form-label">@GetText("email_simulator.station_label", "Mã trạm (Station ID/Code)")</label>
```

#### **Files to Update:**

---

### 📄 **EmailSimulator.razor**

| Line | Old Code | New Code | Key |
|------|----------|----------|-----|
| 18 | `Mã trạm (Station ID/Code)` | `@GetText("email_simulator.station_label", "Mã trạm (Station ID/Code)")` | email_simulator.station_label |
| 24 | `Chọn mã trạm...` | `@GetText("email_simulator.station_placeholder", "Chọn mã trạm...")` | email_simulator.station_placeholder |
| 41 | `Gửi Email` | `@GetText("email_simulator.send_button", "Gửi Email")` | email_simulator.send_button |
| 86 | `"✅ Đã gửi email test thành công!"` | `GetText("email_simulator.success_message", "✅ Đã gửi email test thành công!")` | email_simulator.success_message |
| 91 | `$"❌ Lỗi gửi email: {ex.Message}"` | `string.Format(GetText("email_simulator.error_message", "❌ Lỗi gửi email: {0}"), ex.Message)` | email_simulator.error_message |
| 99 | `"Mã trạm là bắt buộc"` | `GetText("email_simulator.station_required", "Mã trạm là bắt buộc")` | email_simulator.station_required |
| 102 | `"Alarm Time là bắt buộc"` | `GetText("email_simulator.alarm_time_required", "Alarm Time là bắt buộc")` | email_simulator.alarm_time_required |

---

### 📄 **Reports.razor**

**Page Title:**
```razor
<!-- Line 13 -->
<PageTitle>@GetText("reports.page_title", "Báo cáo và Lịch sử")</PageTitle>

<!-- Line 119 -->
<h1 class="h3 mb-4 text-gray-800">@GetText("reports.page_title", "Báo cáo và Lịch sử")</h1>
```

**Tab Labels:**
```razor
<!-- Line 126 -->
<i class="fas fa-exclamation-triangle"></i> @GetText("reports.tab_alert_report", "Báo cáo Cảnh báo")

<!-- Line 132 -->
<i class="fas fa-chart-line"></i> @GetText("reports.tab_motion_report", "Báo cáo Chuyển động")

<!-- Line 138 -->
<i class="fas fa-history"></i> @GetText("reports.tab_config_history", "Lịch sử Thay đổi Cấu hình")
```

**Form Labels (Alert Report Section):**
```razor
<!-- Line 153 -->
<label class="form-label">@GetText("reports.time_range_label", "Khoảng thời gian")</label>

<!-- Line 164 -->
<label class="form-label">@GetText("reports.from_date_label", "Từ ngày")</label>

<!-- Line 168 -->
<label class="form-label">@GetText("reports.to_date_label", "Đến ngày")</label>

<!-- Line 173 -->
<label class="form-label">@GetText("reports.station_label", "Trạm")</label>

<!-- Line 183 -->
<label class="form-label">@GetText("reports.status_label", "Trạng thái")</label>
```

**Dropdown Options:**
```razor
<!-- Time Range Options (Lines 155-158) -->
<option value="today">@GetText("reports.option_today", "Hôm nay")</option>
<option value="week">@GetText("reports.option_this_week", "Tuần này")</option>
<option value="month">@GetText("reports.option_this_month", "Tháng này")</option>
<option value="custom">@GetText("reports.option_custom", "Tùy chỉnh")</option>

<!-- Status Options (Lines 185-187) -->
<option value="all">@GetText("reports.option_all", "Tất cả")</option>
<option value="active">@GetText("reports.status_unresolved", "Chưa xử lý")</option>
<option value="resolved">@GetText("reports.status_resolved", "Đã xử lý")</option>
```

**Grid Columns:**
```razor
<!-- Line 229 -->
<DxGridDataColumn FieldName="AlertTime" Caption="@GetText("reports.alert_time_column", "Thời gian")" Width="160px">

<!-- Line 237 -->
<DxGridDataColumn FieldName="StationName" Caption="@GetText("reports.station_column", "Trạm")" Width="200px" />

<!-- Line 238 -->
<DxGridDataColumn FieldName="Message" Caption="@GetText("reports.message_column", "Thông điệp")" MinWidth="300" />

<!-- Line 239 -->
<DxGridDataColumn FieldName="Severity" Caption="@GetText("reports.severity_column", "Mức độ")" Width="100px" />

<!-- Line 240 -->
<DxGridDataColumn FieldName="IsResolved" Caption="@GetText("reports.status_column", "Trạng thái")" Width="120px">

<!-- Line 246 (Status Display) -->
@(alert.IsResolved ? GetText("reports.status_resolved", "Đã xử lý") : GetText("reports.status_unresolved", "Chưa xử lý"))

<!-- Line 250 -->
<DxGridDataColumn FieldName="ResolvedAt" Caption="@GetText("reports.resolved_at_column", "Xử lý lúc")" Width="180px">

<!-- Line 258 -->
<DxGridDataColumn FieldName="ResolvedBy" Caption="@GetText("reports.resolved_by_column", "Người xử lý")" Width="150px" />

<!-- Line 259 -->
<DxGridDataColumn FieldName="Notes" Caption="@GetText("reports.notes_column", "Ghi chú")" MinWidth="200">
```

**Buttons:**
```razor
<!-- Line 200 -->
<i class="fas fa-file-excel me-1 pr-1"></i>@(isExporting ? GetText("reports.exporting_text", "Đang xuất...") : GetText("reports.export_excel_button", "Xuất Excel"))

<!-- Line 319 -->
<i class="fas fa-search me-1"></i>@GetText("reports.search_button", "Tìm kiếm")
```

**Loading/Summary Messages:**
```razor
<!-- Line 209 -->
<p>@GetText("reports.loading_message", "Đang tải dữ liệu...")</p>

<!-- Lines 280-282 -->
<strong>@GetText("reports.total_alerts", "Tổng số cảnh báo:"):</strong> @alerts.Count |
<strong>@GetText("reports.unresolved_alerts", "Chưa xử lý:"):</strong> @alerts.Count(a => !a.IsResolved) |
<strong>@GetText("reports.resolved_alerts", "Đã xử lý:"):</strong> @alerts.Count(a => a.IsResolved)
```

---

### 📄 **UserManagement.razor**

**Already Partially Localized** - Just verify these keys exist:

```razor
<!-- Page Title (Line 13) -->
<PageTitle>@GetText("user.title", "Quản Lý User")</PageTitle>

<!-- Column Captions (Bottom of file) -->
private string FullNameColumnCaption => GetText("user.fullname_column", "Họ và Tên");
private string RoleColumnCaption => GetText("user.role_column", "Vai trò");
private string StatusColumnCaption => GetText("user.status_column", "Trạng thái");
// ... etc
```

**Form Labels to Update:**
```razor
<!-- Line 188 -->
placeholder="@GetText("user.username_placeholder", "Nhập username...")"

<!-- Line 207 -->
placeholder="@GetText("user.fullname_placeholder", "Nhập họ và tên...")"

<!-- Line 226 -->
placeholder="@GetText("user.password_placeholder", "Tối thiểu 6 ký tự...")"
```

**Validation Messages (Lines 354-362):**
```csharp
[Required(ErrorMessage = "Họ tên là bắt buộc")]
// CHANGE TO:
[Required(ErrorMessage = "validation_fullname_required")] // Will be resolved via data annotations validator

// OR use custom validation with GetText() in code
```

**Success/Error Messages (Lines 563, 577, 604, 609):**
```csharp
// Line 563
successMessage = GetText("user.success_create", "Tạo user thành công");

// Line 577
successMessage = GetText("user.success_update", "Cập nhật user thành công");

// Line 604
successMessage = GetText("user.success_delete", "Xóa user thành công");

// Line 609
errorMessage = string.Format(GetText("user.error_delete", "Lỗi khi xóa user: {0}"), ex.Message);
```

**Confirm Dialog (Line 598):**
```csharp
if (!await JS.InvokeAsync<bool>("confirm", string.Format(GetText("user.confirm_delete", "Bạn có chắc muốn xóa user '{0}'?"), user.Username)))
```

---

### 📄 **TimeFrameForm.razor (Component)**

**Modal Title:**
```razor
<!-- Line 11 -->
HeaderText="@(TimeFrame.Id == Guid.Empty ? GetText("timeframe.add_title", "➕ Thêm khung thời gian") : GetText("timeframe.edit_title", "✏️ Sửa khung thời gian"))"
```

**Form Labels:**
```razor
<!-- Line 27 -->
<label class="form-label">@GetText("timeframe.name_label", "Tên khung thời gian") <span class="text-danger">*</span></label>

<!-- Line 28 -->
placeholder="@GetText("timeframe.name_placeholder", "Ví dụ: Ca sáng, Ca chiều...")"

<!-- Line 35 -->
<label class="form-label">@GetText("timeframe.start_time_label", "Bắt đầu") <span class="text-danger">*</span></label>

<!-- Line 39 -->
<label class="form-label">@GetText("timeframe.end_time_label", "Kết thúc") <span class="text-danger">*</span></label>

<!-- Line 52 -->
<label class="form-label">@GetText("timeframe.frequency_label", "Tần suất kiểm tra (phút)") <span class="text-danger">*</span></label>
```

**Days of Week (Lines 106-130):**
```razor
<label class="form-check-label" for="day1">@GetText("timeframe.day_monday", "Thứ 2")</label>
<label class="form-check-label" for="day2">@GetText("timeframe.day_tuesday", "Thứ 3")</label>
<label class="form-check-label" for="day3">@GetText("timeframe.day_wednesday", "Thứ 4")</label>
<label class="form-check-label" for="day4">@GetText("timeframe.day_thursday", "Thứ 5")</label>
<label class="form-check-label" for="day5">@GetText("timeframe.day_friday", "Thứ 6")</label>
<label class="form-check-label" for="day6">@GetText("timeframe.day_saturday", "Thứ 7")</label>
<label class="form-check-label" for="day7">@GetText("timeframe.day_sunday", "Chủ nhật")</label>
```

**Validation Error Messages (Lines 228, 261):**
```csharp
// Line 228
timeRangeError = GetText("timeframe.validation_end_after_start", "Thời gian kết thúc phải lớn hơn thời gian bắt đầu!");

// Line 261
timeRangeError = GetText("timeframe.validation_invalid_format", "Định dạng thời gian không hợp lệ!");
```

**Day Names in Code (Lines 324-330):**
```csharp
private string GetDayName(DayOfWeek day) => day switch
{
    DayOfWeek.Monday => GetText("timeframe.day_monday", "Thứ 2"),
    DayOfWeek.Tuesday => GetText("timeframe.day_tuesday", "Thứ 3"),
    DayOfWeek.Wednesday => GetText("timeframe.day_wednesday", "Thứ 4"),
    DayOfWeek.Thursday => GetText("timeframe.day_thursday", "Thứ 5"),
    DayOfWeek.Friday => GetText("timeframe.day_friday", "Thứ 6"),
    DayOfWeek.Saturday => GetText("timeframe.day_saturday", "Thứ 7"),
    DayOfWeek.Sunday => GetText("timeframe.day_sunday", "Chủ nhật"),
    _ => day.ToString()
};
```

---

### 📄 **Login.razor**

```razor
<!-- Title -->
<PageTitle>@GetText("login.page_title", "Đăng nhập")</PageTitle>

<!-- Form -->
<label>@GetText("login.username_label", "Username")</label>
<InputText @bind-Value="loginRequest.Username" 
           placeholder="@GetText("login.username_placeholder", "Nhập username...")" />

<label>@GetText("login.password_label", "Mật khẩu")</label>
<InputPassword @bind-Value="loginRequest.Password" 
               placeholder="@GetText("login.password_placeholder", "Nhập mật khẩu...")" />

<!-- Button -->
<button>@(isLoggingIn ? GetText("login.logging_in", "Đang đăng nhập...") : GetText("login.login_button", "Đăng nhập"))</button>

<!-- Error Message (Line 172) -->
errorMessage = loginResponse.Message ?? GetText("login.error_default", "Đăng nhập thất bại. Vui lòng kiểm tra lại tên đăng nhập và mật khẩu.");
```

---

### 📄 **Translations.razor**

```razor
<!-- Line 34 -->
<span class="sr-only">@GetText("translations.loading", "Đang tải...")</span>

<!-- Line 106 -->
title="@GetText("translations.edit_tooltip", "Chỉnh sửa")"

<!-- Line 109 -->
title="@GetText("translations.delete_tooltip", "Xóa")"

<!-- Line 130 -->
@(editingTranslation == null ? GetText("translations.add_new_title", "Thêm Bản dịch Mới") : GetText("translations.edit_title", "Chỉnh sửa Bản dịch"))

<!-- Error messages (Lines 273, 286, 362, 372) -->
errorMessage = string.Format(GetText("translations.error_load_languages", "Lỗi khi tải danh sách ngôn ngữ: {0}"), ex.Message);
errorMessage = string.Format(GetText("translations.error_load_translations", "Lỗi khi tải danh sách bản dịch: {0}"), ex.Message);
errorMessage = string.Format(GetText("translations.error_save", "Lỗi khi lưu bản dịch: {0}"), ex.Message);

bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", GetText("translations.confirm_delete", "Bạn có chắc chắn muốn xóa bản dịch này?"));
```

---

### Step 3: Update C# Services

#### **MonitoringService.cs**

**Line 342 & 415 - Timeframe Overlap Exception:**
```csharp
// BEFORE:
throw new ArgumentException($"Khung thời gian bị trùng với '{existing.Name}' ({existing.StartTime:hh\\:mm} - {existing.EndTime:hh\\:mm})");

// AFTER:
// Inject ILocalizationService in constructor
private readonly ILocalizationService _localizationService;

public MonitoringService(..., ILocalizationService localizationService)
{
    _localizationService = localizationService;
}

// Then use:
var message = string.Format(
    _localizationService.GetText("service.timeframe_overlap_error", "Khung thời gian bị trùng với '{0}' ({1} - {2})"),
    existing.Name,
    existing.StartTime.ToString(@"hh\:mm"),
    existing.EndTime.ToString(@"hh\:mm")
);
throw new ArgumentException(message);
```

**Line 686 - Alert Message:**
```csharp
// BEFORE:
Message = $"Trạm '{station.Name}' không phát hiện chuyển động trong {minutesSinceLastMotion} phút (ngưỡng: {timeFrame.FrequencyMinutes} phút)",

// AFTER:
Message = string.Format(
    _localizationService.GetText("service.no_motion_alert_message", "Trạm '{0}' không phát hiện chuyển động trong {1} phút (ngưỡng: {2} phút)"),
    station.Name,
    minutesSinceLastMotion,
    timeFrame.FrequencyMinutes
),
```

---

#### **UserService.cs**

**Lines 73, 77, 90, 94, 137, 141 - Exception Messages:**

```csharp
// Inject ILocalizationService
private readonly ILocalizationService _localizationService;

public UserService(..., ILocalizationService localizationService)
{
    _localizationService = localizationService;
}

// Line 73
throw new InvalidOperationException(
    string.Format(
        _localizationService.GetText("service.email_exists_deleted", "Email '{0}' đã tồn tại trong hệ thống (user đã bị xóa trước đó). Vui lòng sử dụng email khác hoặc liên hệ quản trị viên để khôi phục tài khoản."),
        request.Email
    )
);

// Line 77
throw new InvalidOperationException(
    string.Format(
        _localizationService.GetText("service.email_exists", "Email '{0}' đã được sử dụng bởi user khác."),
        request.Email
    )
);

// Line 90
throw new InvalidOperationException(
    string.Format(
        _localizationService.GetText("service.username_exists_deleted", "Username '{0}' đã tồn tại trong hệ thống (user đã bị xóa trước đó). Vui lòng sử dụng username khác hoặc liên hệ quản trị viên để khôi phục tài khoản."),
        request.Username
    )
);

// Line 94
throw new InvalidOperationException(
    string.Format(
        _localizationService.GetText("service.username_exists", "Username '{0}' đã được sử dụng."),
        request.Username
    )
);

// Similar pattern for lines 137, 141
```

---

#### **MotionDetectionService.cs**

**Line 115 - Alert Message:**
```csharp
// Inject ILocalizationService
private readonly ILocalizationService _localizationService;

// Line 115
Message = string.Format(
    _localizationService.GetText("service.motion_detection_alert", "Không phát hiện chuyển động trong {0} phút (mong đợi: {1} phút)"),
    minutesSinceLastMotion,
    currentRule.IntervalMinutes
),
```

---

### Step 4: Language Switching

**SBAdminLayout.razor** already has language dropdown (Lines 257-273):
```razor
<select @onchange="ChangeLanguage" class="form-control form-control-sm">
    <option value="vi" selected="@(LocalizationState.CurrentLanguage == "vi")">
        Tiếng Việt
    </option>
    <option value="en" selected="@(LocalizationState.CurrentLanguage == "en")">
        English
    </option>
</select>
```

Update language display names:
```razor
<option value="vi" selected="@(LocalizationState.CurrentLanguage == "vi")">
    @GetText("language.vietnamese", "Tiếng Việt")
</option>
<option value="en" selected="@(LocalizationState.CurrentLanguage == "en")">
    @GetText("language.english", "English")
</option>
```

---

## 🧪 Testing Checklist

### 1. Database Verification
```sql
-- Check total translations
SELECT COUNT(*) FROM Translations;
-- Expected: 400+

-- Check categories distribution
SELECT Category, COUNT(*) as Count
FROM Translations
GROUP BY Category
ORDER BY Count DESC;

-- Check for missing translations
SELECT DISTINCT [Key]
FROM Translations
WHERE LanguageCode = 'vi'
AND [Key] NOT IN (SELECT [Key] FROM Translations WHERE LanguageCode = 'en');
-- Should return 0 rows (all keys have both languages)
```

### 2. UI Testing

**Test Each Page:**
- [ ] EmailSimulator: All labels, placeholders, success/error messages
- [ ] Reports: All tabs, form labels, grid columns, buttons, summary text
- [ ] UserManagement: Form labels, validation messages, success/error messages
- [ ] Login: Form labels, error messages
- [ ] Translations: All UI text
- [ ] TimeFrameForm: Modal title, labels, validation messages, day names

**Language Switching:**
1. Switch to English using language dropdown
2. Verify all text changes to English
3. Navigate through all pages
4. Switch back to Vietnamese
5. Verify all text returns to Vietnamese

### 3. Functional Testing

**Test Data Entry:**
- [ ] Create new station (should see localized validation errors)
- [ ] Create new user (validation messages in current language)
- [ ] Submit forms with errors (error messages localized)

**Test Services:**
- [ ] Trigger timeframe overlap error → Exception message should be localized
- [ ] Create user with duplicate email → Exception message localized
- [ ] Generate motion detection alert → Alert message localized

---

## 📊 Translation Key Reference

### Quick Lookup Table

| Page/Component | Prefix | Example Key |
|----------------|--------|-------------|
| Email Simulator | `email_simulator.` | `email_simulator.station_label` |
| Reports | `reports.` | `reports.alert_time_column` |
| User Management | `user.` | `user.fullname_label` |
| TimeFrame Form | `timeframe.` | `timeframe.frequency_label` |
| Login | `login.` | `login.username_label` |
| Translations Page | `translations.` | `translations.edit_title` |
| Common Buttons | `common.` | `common.save` |
| Services | `service.` | `service.email_exists` |
| Language Names | `language.` | `language.vietnamese` |

### Categories

- **label**: Form labels, section titles
- **column**: Grid column captions
- **button**: Button text, action labels
- **message**: System messages, alerts, notifications
- **option**: Dropdown options, checkbox labels
- **validation**: Validation error messages
- **tooltip**: Button/icon tooltips
- **placeholder**: Input placeholders
- **page**: Page titles
- **tab**: Tab labels

---

## 🚀 Deployment Process

### Development Environment
1. Run SQL script on local database
2. Update Razor files page by page
3. Test each page after update
4. Update services with localization
5. Full regression test

### Staging/Production
1. **Backup database** before applying SQL script
2. Run comprehensive-localization-keys.sql
3. Verify translations loaded:
   ```sql
   SELECT TOP 10 * FROM Translations ORDER BY CreatedAt DESC;
   ```
4. Deploy updated code
5. Test language switching
6. Monitor for any missed hardcoded text

---

## ⚠️ Common Pitfalls

### 1. String Interpolation
**WRONG:**
```razor
<label>Tổng số: @count</label>
```

**RIGHT:**
```razor
<label>@string.Format(GetText("label.total_count", "Tổng số: {0}"), count)</label>
```

### 2. Validation Attributes
**WRONG (Won't update with language):**
```csharp
[Required(ErrorMessage = "Field is required")]
```

**RIGHT:**
```csharp
// Use custom validation or resolve message in code
if (string.IsNullOrEmpty(value))
{
    error = GetText("validation.required", "Field is required");
}
```

### 3. JavaScript Confirm/Alert
**WRONG:**
```csharp
await JS.InvokeAsync<bool>("confirm", "Bạn có chắc không?");
```

**RIGHT:**
```csharp
await JS.InvokeAsync<bool>("confirm", GetText("confirm.delete", "Bạn có chắc không?"));
```

---

## 📝 File Summary

| File | Purpose | Keys Added |
|------|---------|------------|
| comprehensive-localization-keys.sql | Complete translation database | 400+ |
| COMPLETE_LOCALIZATION_GUIDE.md | Implementation guide | N/A |

---

## ✅ Sign-off Checklist

Before considering localization complete:

- [ ] SQL script executed successfully
- [ ] All 9 sections updated (Email Simulator, Reports, User Management, TimeFrame Form, Login, Translations, Services, Common, Languages)
- [ ] No grep search finds hardcoded Vietnamese text in Razor files
- [ ] No grep search finds hardcoded Vietnamese text in Service exception messages
- [ ] Language dropdown works on all pages
- [ ] All form validation messages localized
- [ ] All success/error toasts localized
- [ ] All confirm dialogs localized
- [ ] All grid columns localized
- [ ] All button text localized
- [ ] Full regression test passed in both Vietnamese and English

---

## 🆘 Support

If you encounter missing translations:
1. Add key to SQL script
2. Re-run script or INSERT manually
3. Update code to use new key
4. Test in both languages

**Example:**
```sql
INSERT INTO [Translations] ([Id], [LanguageCode], [Key], [Value], [Category], [CreatedAt]) VALUES
(NEWID(), 'vi', 'new.key.name', N'Giá trị tiếng Việt', 'label', GETUTCDATE()),
(NEWID(), 'en', 'new.key.name', 'English value', 'label', GETUTCDATE());
```

---

**Last Updated:** 2025-11-21
**Version:** 2.0
**Author:** StationCheck Localization Team
