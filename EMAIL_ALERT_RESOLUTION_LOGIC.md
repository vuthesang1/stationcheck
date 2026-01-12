# Email Alert Resolution Logic - Enhanced Version
**Cập nhật:** January 12, 2026

## ⚠️ Buffer Time Logic Change (Jan 2026)

**OLD Logic (Before Jan 2026):** Buffer time was ±BufferMinutes around checkpoint
- Example: Checkpoint 10:00, Buffer 30min → Window [09:30 - 10:30]
- Motion at 09:45 would resolve alert ✅

**NEW Logic (After Jan 2026):** Buffer time is +BufferMinutes AFTER checkpoint only
- Example: Checkpoint 10:00, Buffer 30min → Window [10:00 - 10:30]
- Motion at 09:45 will NOT resolve alert ❌
- Motion at 10:15 will resolve alert ✅

**Reason:** User wants to only accept motion AFTER checkpoint (late arrival), not before (early arrival).

---

## Tổng quan

EmailService giờ có trách nhiệm **resolve alert ngay lập tức** khi nhận được email motion thỏa mãn điều kiện, thay vì phải đợi AlertGenerationService chạy chu kỳ tiếp theo.

## Workflow Chi tiết

### 1. Nhận và Parse Email Motion

```
Email đến → Parse MotionEvent → Lưu DB → Update Station.LastMotionDetectedAt
```

### 2. Alert Resolution Strategy

Với mỗi trạm có motion mới, hệ thống áp dụng 1 trong 2 chiến lược:

#### **CASE 1: Tìm thấy alert chưa resolve** (Normal case)

```sql
-- Query để tìm alert chưa resolve gần nhất
SELECT TOP 1 *
FROM MotionAlerts
WHERE StationId = @StationId
  AND IsResolved = 0
  AND IsDeleted = 0
  AND TimeFrameId IS NOT NULL
ORDER BY AlertTime DESC
```

**Logic kiểm tra:**
```csharp
var windowStart = alert.AlertTime; // Checkpoint time
var windowEnd = alert.AlertTime.AddMinutes(+BufferMinutes);

// Check nếu motion nằm trong tolerance window
if (motion.DetectedAt >= windowStart && motion.DetectedAt <= windowEnd)
{
    // ✅ RESOLVE alert
    alert.IsResolved = true;
    alert.ResolvedAt = DateTime.UtcNow;
    alert.ResolvedBy = "System (Auto-resolved by email processing)";
}
else
{
    // ⚠️ Motion nằm NGOÀI window - Log để điều tra
    LogWarning("Motion OUTSIDE tolerance window");
}
```

**Ví dụ:**
- Alert: `AlertTime = 2025-12-21 02:00:00 UTC` (9:00 +7), BufferMinutes = 30
- Tolerance Window: `[01:30 - 02:30] UTC` ([8:30 - 9:30] +7)
- Motion: `DetectedAt = 2025-12-21 02:29:16 UTC` (9:29:16 +7)
- Kết quả: ✅ **Motion TRONG window** → Resolve alert
- UI: 🟢 **Bình thường (Online)**

#### **CASE 2: KHÔNG tìm thấy alert chưa resolve** (System missed checkpoint)

Có thể xảy ra khi:
- AlertGenerationService bị delay/crash
- Service chưa chạy tới checkpoint đó
- Database bị lỗi khi tạo alert

**Giải pháp: Tự động tạo alert RESOLVED**

```csharp
// 1. Tìm TimeFrame đang active cho trạm này
var activeTimeFrame = await _context.TimeFrames
    .Where(tf => tf.StationId == stationId && tf.IsEnabled)
    .FirstOrDefaultAsync();

// 2. Tính checkpoint gần nhất dựa trên thời gian motion
var motionLocalTime = motionTime.AddHours(7); // UTC → UTC+7
var elapsed = motionLocalTime.TimeOfDay - timeFrame.StartTime;
var checkpointsSinceStart = Math.Floor(elapsed.TotalMinutes / FrequencyMinutes);
var checkpointTime = timeFrame.StartTime.Add(checkpointsSinceStart * FrequencyMinutes);

// 3. Tạo alert RESOLVED mới
var newAlert = new MotionAlert
{
    AlertTime = checkpointDateTime,
    Message = $"Trạm {stationName} Online lúc {checkpointLocal}",
    IsResolved = true,  // ✅ Resolved ngay từ đầu
    ResolvedAt = DateTime.UtcNow,
    ResolvedBy = "System (Auto-created by email processing)",
    Notes = "Alert created by EmailService: No checkpoint alert found, motion detected at ...",
    IsDeleted = false  // Visible trong UI
};
```

**Ví dụ:**
- Motion: `2025-12-21 02:29:16 UTC` (9:29:16 +7)
- TimeFrame: StartTime = 00:00, FrequencyMinutes = 60
- Checkpoint tính được: 02:00 UTC (9:00 +7)
- Kết quả: ✨ **Tạo alert mới**
  ```
  AlertTime: 2025-12-21 02:00:00 UTC
  Message: "Trạm THUẬN ĐẠO Online lúc 09:00"
  IsResolved: true
  ```
- UI: 🟢 **Bình thường (Online)**

## So sánh với AlertGenerationService

### AlertGenerationService (Background, định kỳ)

```
Chạy mỗi 1 giờ (hoặc theo cấu hình)
→ Check tất cả trạm active
→ Tính checkpoint hiện tại
→ Check motion trong tolerance window
→ Tạo alert (RESOLVED hoặc UNRESOLVED)
```

**Ưu điểm:**
- Tạo alert định kỳ, đầy đủ
- Phát hiện trạm OFFLINE (không có motion)

**Nhược điểm:**
- Chậm (phải đợi chu kỳ tiếp theo)
- Có thể bỏ lỡ checkpoint (nếu service delay)

### EmailService (Real-time, khi có email)

```
Nhận email motion
→ Parse và lưu MotionEvent
→ Update Station.LastMotionDetectedAt
→ Resolve alert ngay lập tức (nếu thỏa mãn)
→ Hoặc tạo alert RESOLVED mới (nếu không tìm thấy alert)
```

**Ưu điểm:**
- ⚡ **Real-time** - Resolve ngay khi có motion
- 🛡️ **Failsafe** - Tự tạo alert nếu system bị lỗi
- 🎯 **Chính xác** - Check exact tolerance window

**Nhược điểm:**
- Chỉ xử lý khi có email motion
- Không phát hiện OFFLINE (do không có email)

## Coordination giữa 2 Services

### Timeline Example:

```
08:55 (+7) | EmailService: Nhận email motion lúc 08:55
           | → Lưu MotionEvent(DetectedAt = 01:55 UTC)
           | → Update Station.LastMotionDetectedAt = 01:55 UTC
           | → Tìm alert chưa resolve... KHÔNG TÌM THẤY
           | → Tính checkpoint = 01:00 UTC (08:00 +7)
           | → ✨ TẠO alert RESOLVED:
           |    AlertTime = 01:00 UTC, Message = "Online lúc 08:00"
           |
09:00 (+7) | AlertGenerationService: Chạy checkpoint 09:00
           | → Check motion trong [01:30 - 02:30] UTC
           | → Không có motion (motion lúc 01:55 nằm ngoài window)
           | → Tạo alert UNRESOLVED:
           |    AlertTime = 02:00 UTC, Message = "Offline lúc 09:00"
           |
09:15 (+7) | EmailService: Nhận email motion lúc 09:15
           | → Lưu MotionEvent(DetectedAt = 02:15 UTC)
           | → Tìm alert chưa resolve... TÌM THẤY (alert lúc 02:00)
           | → Check window [01:30 - 02:30]? YES (02:15 trong window)
           | → ✅ RESOLVE alert:
           |    IsResolved = true, ResolvedAt = now
           |
09:29 (+7) | EmailService: Nhận email motion lúc 09:29
           | → Lưu MotionEvent(DetectedAt = 02:29 UTC)
           | → Tìm alert chưa resolve... KHÔNG (đã resolve ở 09:15)
           | → Tính checkpoint = 02:00 UTC (09:00 +7)
           | → Alert cho checkpoint này đã tồn tại → SKIP
           |
10:00 (+7) | AlertGenerationService: Chạy checkpoint 10:00
           | → Check motion trong [02:30 - 03:30] UTC
           | → Có motion lúc 02:29? KHÔNG (nằm ngoài 1 phút)
           | → Tạo alert UNRESOLVED:
           |    AlertTime = 03:00 UTC, Message = "Offline lúc 10:00"
```

## Logging Chi tiết

### ✅ Resolve alert thành công:

```
[EmailService] ✅ RESOLVED Alert {AlertId}
  | Station=THUẬN ĐẠO
  | AlertTime=02:00Z
  | Motion=02:15Z
  | Window=[01:30Z-02:30Z]
```

### ⚠️ Motion nằm ngoài window:

```
[EmailService] ⚠️ Motion OUTSIDE tolerance window
  | Station=THUẬN ĐẠO
  | AlertTime=02:00Z
  | LatestMotion=09:29Z
  | Window=[01:30Z-02:30Z]
  | Gap=447.5min
```

### ✨ Tạo alert mới (không tìm thấy):

```
[EmailService] ✨ CREATED RESOLVED Alert
  | Station=THUẬN ĐẠO
  | Checkpoint=2025-12-21 02:00:00Z
  | Motion=02:29Z
  | Reason=No unresolved alert found (system may have missed checkpoint)
```

### 📊 Summary:

```
[EmailService] 📊 Alert Summary:
  Resolved=2, Created=1 | Total Stations=3
```

## Kiểm tra và Debugging

### 1. Kiểm tra alert có được resolve không:

```sql
-- Alert chưa resolve
SELECT Id, StationName, AlertTime, Message, IsResolved
FROM MotionAlerts
WHERE StationId = @StationId
  AND IsResolved = 0
ORDER BY AlertTime DESC;

-- Alert vừa resolve
SELECT Id, StationName, AlertTime, ResolvedAt, ResolvedBy, Notes
FROM MotionAlerts
WHERE StationId = @StationId
  AND IsResolved = 1
  AND ResolvedBy LIKE '%email processing%'
ORDER BY ResolvedAt DESC;
```

### 2. Kiểm tra motion trong window:

```sql
SELECT 
    a.AlertTime,
    a.Message,
    a.IsResolved,
    tf.BufferMinutes,
    a.AlertTime AS WindowStart, -- NEW: Window starts at checkpoint (not before)
    DATEADD(MINUTE, tf.BufferMinutes, a.AlertTime) AS WindowEnd,
    (SELECT TOP 1 DetectedAt FROM MotionEvents 
     WHERE StationId = a.StationId 
       AND DetectedAt >= a.AlertTime -- NEW: Motion must be AFTER checkpoint
       AND DetectedAt <= DATEADD(MINUTE, tf.BufferMinutes, a.AlertTime)
     ORDER BY DetectedAt DESC) AS MotionInWindow
FROM MotionAlerts a
JOIN TimeFrames tf ON a.TimeFrameId = tf.Id
WHERE a.StationId = @StationId
ORDER BY a.AlertTime DESC;
```

### 3. Kiểm tra log trong Logs/app-{date}.txt:

```bash
# Tìm email processing events
grep "\[EmailService\]" Logs/app-20251221.txt | grep -E "(RESOLVED|CREATED|OUTSIDE)"

# Tìm alert summary
grep "Alert Summary" Logs/app-20251221.txt
```

## Best Practices

### 1. Cấu hình BufferMinutes hợp lý:

- **Khuyến nghị:** 30-60 phút
- Quá nhỏ (< 10 phút): Dễ miss motion, nhiều false positive
- Quá lớn (> 120 phút): Alert không chính xác, mất ý nghĩa checkpoint

### 2. Monitor alert creation rate:

```sql
-- Số alert được tạo bởi EmailService vs AlertGenerationService
SELECT 
    CASE 
        WHEN ResolvedBy LIKE '%email%' THEN 'EmailService'
        WHEN ResolvedBy = 'System' THEN 'AlertGenerationService'
        ELSE 'Other'
    END AS Source,
    COUNT(*) AS AlertCount
FROM MotionAlerts
WHERE CreatedAt >= DATEADD(DAY, -7, GETDATE())
  AND IsResolved = 1
GROUP BY CASE 
    WHEN ResolvedBy LIKE '%email%' THEN 'EmailService'
    WHEN ResolvedBy = 'System' THEN 'AlertGenerationService'
    ELSE 'Other'
END;
```

Nếu **EmailService tạo nhiều alert** (> 20% total) → AlertGenerationService có vấn đề (delay, miss checkpoint)

### 3. Alert audit trail:

Tất cả alert (resolved/unresolved) đều có `IsDeleted = false` để:
- Audit trail đầy đủ
- Phân tích trạng thái trạm theo thời gian
- Debugging khi có vấn đề

## Kết luận

**EmailService giờ có 2 trách nhiệm chính:**

1. ✅ **Lưu MotionEvent** và update Station.LastMotionDetectedAt
2. ✅ **Resolve alert ngay lập tức** hoặc tạo alert RESOLVED mới

**Kết quả:**
- ⚡ Real-time alert resolution (không phải đợi chu kỳ)
- 🛡️ Failsafe khi AlertGenerationService bỏ lỡ checkpoint
- 🎯 Chính xác với tolerance window checking
- 🟢 UI hiển thị "Online" ngay khi có motion

---

**Files Modified:**
- `d:\station-c\Services\EmailService.cs` (Lines 200-430)

**Next Steps:**
1. Deploy và monitor logs
2. Verify alert resolution real-time
3. Check alert creation rate (EmailService vs AlertGenerationService)
