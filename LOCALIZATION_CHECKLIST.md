# Danh sách tất cả các chỗ cần thay đổi từ hardcode sang localization

## EmailSimulator.razor
| Line | Current | Replace With |
|------|---------|--------------|
| 18 | `<label class="form-label">Mã trạm (Station ID/Code)</label>` | `<label class="form-label">@GetText("email_simulator.station_label", "Mã trạm (Station ID/Code)")</label>` |
| 32 | `<label class="form-label">Alarm Time</label>` | `<label class="form-label">@GetText("email_simulator.alarm_time_label", "Alarm Time")</label>` |
| 43 | `Gửi Email` | `@GetText("email_simulator.send_button", "Gửi Email")` |

## Reports.razor - Alert Report Section

### Labels
| Line | Current | Replace With |
|------|---------|--------------|
| 153 | `<label class="form-label">Khoảng thời gian</label>` | `<label class="form-label">@GetText("reports.time_range_label", "Khoảng thời gian")</label>` |
| 164 | `<label class="form-label">Từ ngày</label>` | `<label class="form-label">@GetText("reports.from_date_label", "Từ ngày")</label>` |
| 168 | `<label class="form-label">Đến ngày</label>` | `<label class="form-label">@GetText("reports.to_date_label", "Đến ngày")</label>` |
| 173 | `<label class="form-label">Trạm</label>` | `<label class="form-label">@GetText("reports.station_label", "Trạm")</label>` |
| 183 | `<label class="form-label">Trạng thái</label>` | `<label class="form-label">@GetText("reports.status_label", "Trạng thái")</label>` |

### Grid Columns
| Line | Current | Replace With |
|------|---------|--------------|
| 229 | `Caption="Thời gian"` | `Caption="@GetText("reports.alert_time_column", "Thời gian")"` |
| 237 | `Caption="Trạm"` | `Caption="@GetText("reports.station_column", "Trạm")"` |
| 238 | `Caption="Thông điệp"` | `Caption="@GetText("reports.message_column", "Thông điệp")"` |
| 239 | `Caption="Mức độ"` | `Caption="@GetText("reports.severity_column", "Mức độ")"` |
| 240 | `Caption="Trạng thái"` | `Caption="@GetText("reports.status_column", "Trạng thái")"` |
| 250 | `Caption="Xử lý lúc"` | `Caption="@GetText("reports.resolved_at_column", "Xử lý lúc")"` |
| 258 | `Caption="Người xử lý"` | `Caption="@GetText("reports.resolved_by_column", "Người xử lý")"` |
| 259 | `Caption="Ghi chú"` | `Caption="@GetText("reports.notes_column", "Ghi chú")"` |

### Buttons
| Line | Current | Replace With |
|------|---------|--------------|
| 200 | `"Xuất Excel"` hoặc `"Đang xuất..."` | `@(isExporting ? GetText("button.exporting", "Đang xuất...") : GetText("button.export_excel", "Xuất Excel"))` |

## Reports.razor - Motion Statistics Section

### Labels
| Line | Current | Replace With |
|------|---------|--------------|
| 299 | `<label class="form-label">Khoảng thời gian</label>` | `<label class="form-label">@GetText("reports.time_range_label", "Khoảng thời gian")</label>` |
| 310 | `<label class="form-label">Từ ngày</label>` | `<label class="form-label">@GetText("reports.from_date_label", "Từ ngày")</label>` |
| 314 | `<label class="form-label">Đến ngày</label>` | `<label class="form-label">@GetText("reports.to_date_label", "Đến ngày")</label>` |

### Grid Columns
| Line | Current | Replace With |
|------|---------|--------------|
| 350 | `Caption="Trạm"` | `Caption="@GetText("reports.station_column", "Trạm")"` |
| 351 | `Caption="Tổng số lần phát hiện"` | `Caption="@GetText("reports.total_count_column", "Tổng số lần phát hiện")"` |
| 352 | `Caption="Trung bình / ngày"` | `Caption="@GetText("reports.average_per_day_column", "Trung bình / ngày")"` |
| 353 | `Caption="Ngày nhiều nhất"` | `Caption="@GetText("reports.max_date_column", "Ngày nhiều nhất")"` |
| 354 | `Caption="Số lần (ngày nhiều nhất)"` | `Caption="@GetText("reports.max_count_column", "Số lần (ngày nhiều nhất)")"` |

## UserManagement.razor

### Form Labels (trong modal)
| Line | Current | Replace With |
|------|---------|--------------|
| 186 | Sau `<label class="form-label">` | `@GetText("user.username_label", "Tên đăng nhập")` |
| 196 | Sau `<label class="form-label">` | `@GetText("user.fullname_label", "Họ và tên")` |
| 205 | Sau `<label class="form-label">` | `@GetText("user.email_label", "Email")` |
| 213 | Sau `<label class="form-label">` | `@GetText("user.password_label", "Mật khẩu")` |
| 223 | Sau `<label class="form-label">` | `@GetText("user.role_label", "Vai trò")` |
| 243 | Sau `<label class="form-label">` | `@GetText("user.station_label", "Trạm")` |
| 260 | Sau `<label class="form-check-label">` | `@GetText("user.is_active_label", "Kích hoạt")` |

### Buttons (trong modal)
Tìm các button trong UserManagement.razor:
- "Thêm User" → `@GetText("button.add", "Thêm") User`
- "Lưu" → `@GetText("button.save", "Lưu")`
- "Hủy" → `@GetText("button.cancel", "Hủy")`
- "Xóa" → `@GetText("button.delete", "Xóa")`

## Common Patterns để tìm

### Pattern 1: Form Labels
```razor
<!-- Before -->
<label class="form-label">Text tiếng Việt</label>

<!-- After -->
<label class="form-label">@GetText("category.key", "Text tiếng Việt")</label>
```

### Pattern 2: Grid Columns
```razor
<!-- Before -->
<DxGridDataColumn Caption="Text tiếng Việt" />

<!-- After -->
<DxGridDataColumn Caption="@GetText("category.key", "Text tiếng Việt")" />
```

### Pattern 3: Button Text
```razor
<!-- Before -->
<button>Text tiếng Việt</button>

<!-- After -->
<button>@GetText("category.key", "Text tiếng Việt")</button>
```

### Pattern 4: Conditional Button Text
```razor
<!-- Before -->
@(isLoading ? "Đang tải..." : "Tải")

<!-- After -->
@(isLoading ? GetText("message.loading", "Đang tải...") : GetText("button.load", "Tải"))
```

## Lệnh tìm kiếm để phát hiện hardcoded strings

### Tìm tất cả label tags
```bash
grep -n '<label.*>' Pages/*.razor
```

### Tìm Caption attributes
```bash
grep -n 'Caption="[^@]' Pages/*.razor
```

### Tìm button text (không phải icon)
```bash
grep -n '<button.*>[^<]*[A-Za-zÀ-ỹ]' Pages/*.razor
```

### Tìm h1-h6 headers
```bash
grep -n '<h[1-6].*>[^<@]*[A-Za-zÀ-ỹ]' Pages/*.razor
```

## Thứ tự ưu tiên thực hiện

1. **Cao nhất**: Form labels và grid columns (ảnh hưởng UX trực tiếp)
2. **Trung bình**: Button text và messages
3. **Thấp**: Placeholders và tooltips

## Notes
- Luôn giữ fallback text = text tiếng Việt gốc
- Test sau mỗi file được update
- Commit sau mỗi module hoàn thành
