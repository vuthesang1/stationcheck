# Alert Resolution Logic Fix - December 21, 2025

## Vấn đề phát hiện

### Hiện tượng:
Trạm **THUẬN ĐẠO** có chuyển động lúc **9:29 sáng** (21/12/2025) nhưng vẫn hiển thị trạng thái **"Offline"** trên dashboard.

### Dữ liệu thực tế:

**MotionAlert (Unresolved):**
```
Id: 6f54449d-4236-4d54-9187-2ecdfe7b19c1
AlertTime: 2025-12-20 19:00:00 UTC (2:00 sáng +7)
Message: Trạm THUẬN ĐẠO Offline lúc 02:00
IsResolved: false (❌ Chưa resolve)
```

**MotionEvent (Mới nhận):**
```
Id: cb093856-e210-4308-ac72-c64ac5c89816
DetectedAt: 2025-12-21 02:29:16 UTC (9:29 sáng +7)
StationCode: ST000013 (THUẬN ĐẠO)
```

**Alert mới (Nên được tạo nhưng chưa có):**
```
AlertTime: 2025-12-21 02:00:00 UTC (9:00 sáng +7)
Message: Trạm THUẬN ĐẠO Online lúc 09:00
IsResolved: true (✅ Resolved)
```

---

## Root Cause Analysis

### Logic SAI trong EmailService.cs (Trước đây):

```csharp
// ❌ LOGIC SAI (OLD - Before Jan 2026): Cố gắng resolve alert cũ bằng motion mới với ±BufferMinutes
var windowStart = alert.AlertTime.AddMinutes(-alert.TimeFrame.BufferMinutes); // 18:30 UTC (1:30 +7)
var windowEnd = alert.AlertTime.AddMinutes(alert.TimeFrame.BufferMinutes);   // 19:30 UTC (2:30 +7)

// ✅ LOGIC MỚI (NEW - Jan 2026): Chỉ chấp nhận motion SAU checkpoint (+BufferMinutes)
var windowStart = alert.AlertTime; // 19:00 UTC (2:00 +7) - Checkpoint time
var windowEnd = alert.AlertTime.AddMinutes(alert.TimeFrame.BufferMinutes);   // 19:30 UTC (2:30 +7)

// Check motion lúc 02:29 UTC (9:29 +7) có nằm trong window [19:00-19:30] của ngày hôm trước?
// → KHÔNG → Alert không được resolve
var resolvingMotion = stationMotions.FirstOrDefault(m => 
    m.DetectedAt >= windowStart && m.DetectedAt <= windowEnd);
```

**Tại sao SAI:**
- Alert được tạo cho checkpoint **2:00 sáng ngày 20/12**
- Motion xảy ra lúc **9:29 sáng ngày 21/12** (7 giờ 29 phút sau)
- Motion nằm **NGOÀI** tolerance window (±30 phút) của alert cũ
- EmailService **KHÔNG NÊN** resolve alert cũ bằng motion mới

### Nguyên nhân gốc rễ:

**Trách nhiệm bị nhầm lẫn giữa 2 services:**

1. **EmailService:** 
   - ✅ Nhiệm vụ: Nhận email → Parse MotionEvent → Lưu DB → Update Station.LastMotionDetectedAt
   - ❌ KHÔNG NÊN: Resolve alert cũ (logic timing phức tạp, dễ sai)

2. **AlertGenerationService:**
   - ✅ Nhiệm vụ: Check chu kỳ → Tính checkpoint time → Check motion trong tolerance window → Tạo alert (resolved/unresolved)
   - ✅ Đây là nơi DUY NHẤT nên handle alert resolution

---

## Solution Implemented

### 1. XÓA logic resolve alert khỏi EmailService

**File:** `d:\station-c\Services\EmailService.cs` (Lines 223-227)

**Trước (SAI):**
```csharp
// ========== Auto-resolve alerts for all stations with new motion events ==========
if (stationIds.Any())
{
    var activeAlerts = await _context.MotionAlerts
        .Include(a => a.TimeFrame)
        .Where(a => stationIds.Contains(a.StationId!.Value)
                 && !a.IsResolved
                 && a.TimeFrameId.HasValue)
        .ToListAsync();
    
    // ... 50+ lines of complex window calculation logic
}
```

**Sau (ĐÚNG):**
```csharp
// ========== NOTE: Alert resolution is handled by AlertGenerationService ==========
// EmailService only saves MotionEvents and updates Station.LastMotionDetectedAt
// AlertGenerationService will check motion within tolerance window and create:
// - Resolved alert if motion detected (Station Online)
// - Unresolved alert if no motion (Station Offline)
// This ensures proper timing logic and avoids complex window calculations here
```

### 2. Giữ nguyên logic AlertGenerationService (ĐÃ ĐÚNG)

**File:** `d:\station-c\BackgroundServices\AlertGenerationService.cs`

**Logic tạo alert RESOLVED khi có motion:**

```csharp
// Lines 463-509: Calculate checkpoint time and check motion
var checkWindowStart = checkpointDateTime.AddMinutes(-timeFrame.BufferMinutes);
var checkWindowEnd = checkpointDateTime.AddMinutes(timeFrame.BufferMinutes);

var hasRecentMotion = await context.MotionEvents
    .Where(me => me.StationId == station.Id 
              && me.DetectedAt >= checkWindowStart 
              && me.DetectedAt <= checkWindowEnd)
    .AnyAsync(cancellationToken);

// Lines 680-697: Create alert with appropriate status
var alert = new MotionAlert
{
    AlertTime = checkpointDateTime,
    IsResolved = hasRecentMotion,  // ✅ true nếu có motion trong window
    ResolvedAt = hasRecentMotion ? checkpointDateTime : null,
    ResolvedBy = hasRecentMotion ? "System" : null,
    IsDeleted = false,  // ✅ QUAN TRỌNG: Không xóa alert resolved
    Message = hasRecentMotion 
        ? $"Trạm {station.Name} Online lúc {checkpointLocal}" 
        : $"Trạm {station.Name} Offline lúc {checkpointLocal}"
};
```

### 3. UI logic (ĐÃ ĐÚNG)

**File:** `d:\station-c\Components\StationStatusPanel.razor`

**Logic hiển thị trạng thái:**

```csharp
// Lines 1010-1019: GetStationStatus()
var latestAlert = latestStationAlerts[station.Id];  // Alert mới nhất theo AlertTime
if (!latestAlert.IsResolved)  // ✅ Check resolved status
{
    return StationStatus.Alert;  // Offline
}
return StationStatus.Normal;  // Online
```

---

## Luồng hoạt động ĐÚNG

### Ví dụ cụ thể: Trạm THUẬN ĐẠO

**Timeline:**

1. **20/12 2:00 sáng (+7) = 19:00 UTC:**
   - AlertGenerationService chạy
   - Tính checkpoint = 19:00 UTC (2:00 +7)
   - Check motion trong window [18:30-19:30] UTC
   - Không có motion → Tạo alert UNRESOLVED:
     ```
     AlertTime: 2025-12-20 19:00:00 UTC
     Message: "Trạm THUẬN ĐẠO Offline lúc 02:00"
     IsResolved: false
     ```
   - UI hiển thị: **🔴 Cảnh báo (Offline)**

2. **21/12 9:29 sáng (+7) = 02:29 UTC:**
   - Email motion đến
   - EmailService parse và lưu MotionEvent:
     ```
     DetectedAt: 2025-12-21 02:29:16 UTC
     StationId: 67EF1667-3ACF-4D00-B60A-08DE2A82BD24
     ```
   - Update `Station.LastMotionDetectedAt = 02:29:16 UTC`
   - **KHÔNG** resolve alert cũ (19:00 UTC) vì motion nằm ngoài window

3. **21/12 10:00 sáng (+7) = 03:00 UTC:**
   - AlertGenerationService chạy lần tiếp theo
   - Tính checkpoint = 03:00 UTC (10:00 +7)
   - Check motion trong window [02:30-03:30] UTC
   - **Có motion lúc 02:29:16** → **KHÔNG nằm trong [02:30-03:30]**
   - → Tạo alert UNRESOLVED:
     ```
     AlertTime: 2025-12-21 03:00:00 UTC
     Message: "Trạm THUẬN ĐẠO Offline lúc 10:00"
     IsResolved: false
     ```

4. **21/12 11:00 sáng (+7) = 04:00 UTC:**
   - AlertGenerationService chạy
   - Tính checkpoint = 04:00 UTC (11:00 +7)
   - Check motion trong window [03:30-04:30] UTC
   - **Không có motion mới**
   - → Tạo alert UNRESOLVED

**❗ Vấn đề:**

Motion lúc **02:29:16 UTC** nằm **NGOÀI** tất cả các checkpoint window:
- Window 02:00: [01:30 - 02:30] → Motion lúc 02:29:16 ✅ **TRONG WINDOW**
- Window 03:00: [02:30 - 03:30] → Motion lúc 02:29:16 ❌ NGOÀI (sớm hơn 1 phút)
- Window 04:00: [03:30 - 04:30] → Motion lúc 02:29:16 ❌ NGOÀI

**Giải pháp:**

Motion lúc **02:29:16** phải được catch bởi checkpoint **02:00** (9:00 sáng +7):
- Window: [01:30 - 02:30] UTC (8:30 - 9:30 +7)
- Motion: 02:29:16 UTC (9:29 +7) → ✅ **TRONG WINDOW**

**Nếu AlertGenerationService chưa chạy checkpoint 02:00:**
- Service chạy với `toleranceMinutes = 3` phút
- Checkpoint 02:00, chỉ chạy trong khoảng 02:00:00 - 02:03:00
- Nếu service chạy lúc 02:05 → **BỎ LỠ checkpoint 02:00**

---

## Kiểm tra lại AlertGenerationService Schedule

**Interval hiện tại:** 1 giờ (3600 giây)

**Checkpoint tolerance:** 3 phút

**Vấn đề tiềm ẩn:**

Nếu service chạy lúc **02:05** (muộn hơn tolerance 3 phút):
```csharp
// Line 337-351: Check if within checkpoint window
var remainder = elapsed.TotalMinutes % timeFrame.FrequencyMinutes;

if (remainder > toleranceMinutes)  // 5 > 3 → Skip
{
    _logger.LogDebug("NOT in checkpoint window");
    return false;
}
```

→ **BỎ LỠ checkpoint 02:00** → Không tạo alert RESOLVED cho motion 02:29

---

## Khuyến nghị tiếp theo

### 1. Tăng tolerance window (RECOMMENDED)

**File:** `AlertGenerationService.cs` Line 312

```csharp
// TRƯỚC:
const double toleranceMinutes = 3.0;

// SAU (KHUYẾN NGHỊ):
const double toleranceMinutes = 10.0;  // Tăng lên 10 phút
```

**Lý do:**
- Service chạy 1 giờ/lần có thể bị delay 5-10 phút (server load, GC, etc.)
- Tolerance 3 phút quá ngắn → dễ bỏ lỡ checkpoint
- Tolerance 10 phút vẫn đủ chính xác, tránh alert duplicate

### 2. Hoặc: Giảm interval xuống 30 phút

**Database:** SystemConfiguration table

```sql
UPDATE SystemConfiguration 
SET ConfigValue = '1800'  -- 30 phút = 1800 giây
WHERE ConfigKey = 'AlertGenerationInterval';
```

**Lý do:**
- Check 30 phút/lần → Nhanh hơn, ít bỏ lỡ checkpoint
- Nhưng tốn tài nguyên hơn (gấp đôi số lần query DB)

---

## Testing Plan

### Test Case 1: Motion trong window

1. Mark email có motion lúc **9:29 sáng** as unread
2. Chờ EmailService xử lý (cập nhật LastMotionDetectedAt)
3. Chờ AlertGenerationService chạy checkpoint **9:00** (trong vòng 10 phút sau 9:00)
4. Kiểm tra DB:
   - Alert mới với `AlertTime = 02:00 UTC`, `IsResolved = true`, `Message = "Online lúc 09:00"`
5. Kiểm tra UI:
   - Trạm THUẬN ĐẠO hiển thị **🟢 Bình thường (Online)**

### Test Case 2: Không có motion

1. Không có email motion mới
2. Chờ AlertGenerationService chạy checkpoint **10:00**
3. Check motion trong window [09:30-10:30] → Không có
4. Kiểm tra DB:
   - Alert mới với `AlertTime = 03:00 UTC`, `IsResolved = false`, `Message = "Offline lúc 10:00"`
5. Kiểm tra UI:
   - Trạm THUẬN ĐẠO hiển thị **🔴 Cảnh báo (Offline)**

---

## Summary

### Thay đổi chính:

1. ✅ **XÓA logic resolve alert khỏi EmailService**
   - EmailService chỉ lo nhận email, lưu MotionEvent, update Station.LastMotionDetectedAt
   - Không can thiệp vào alert resolution

2. ✅ **Giữ nguyên AlertGenerationService** (đã đúng)
   - Tạo alert RESOLVED khi có motion trong tolerance window
   - Tạo alert UNRESOLVED khi không có motion
   - Alert RESOLVED có `IsDeleted = false` (visible trong UI)

3. ✅ **UI logic đã đúng**
   - Lấy alert mới nhất theo AlertTime
   - Check `IsResolved` để hiển thị trạng thái

### Kết quả mong đợi:

- ✅ Motion lúc 9:29 sẽ được catch bởi checkpoint 9:00 (window 8:30-9:30)
- ✅ Alert "Online lúc 09:00" sẽ được tạo với `IsResolved = true`
- ✅ UI hiển thị trạm **🟢 Bình thường (Online)**
- ✅ Alert cũ (2:00 ngày 20/12) không bị động chạm (đúng vì motion nằm ngoài window)

### Lưu ý:

- Nếu vẫn thấy "Offline" sau khi có motion, kiểm tra:
  1. AlertGenerationService có chạy đúng schedule không? (log `[AlertGeneration] 🔄`)
  2. Checkpoint có bị skip do tolerance quá nhỏ không? (log `NOT in checkpoint window`)
  3. Nếu có, tăng `toleranceMinutes` lên 10 phút (khuyến nghị)

---

## Files Modified

1. **d:\station-c\Services\EmailService.cs**
   - Removed: 50+ lines of alert resolution logic
   - Added: Comment explaining AlertGenerationService handles resolution

2. **d:\station-c\BackgroundServices\AlertGenerationService.cs**
   - No changes (already correct)

3. **d:\station-c\Components\StationStatusPanel.razor**
   - No changes (already correct)

---

**Ngày:** December 21, 2025  
**Người thực hiện:** AI Assistant  
**Trạng thái:** ✅ Hoàn thành, chờ testing trên production
