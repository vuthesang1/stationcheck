# Hướng dẫn sử dụng Dahua Webhook API

## Tổng quan
API này được tạo để nhận webhook từ đầu ghi Dahua qua chức năng **HTTP Push**. Tất cả request data sẽ được ghi vào file text để phân tích cấu trúc dữ liệu.

---

## Endpoints

### 1. **POST /api/dahuawebhook/event** (Endpoint chính)
Nhận webhook từ đầu ghi Dahua.

**Đặc điểm:**
- ✅ Nhận mọi loại input: JSON, Form Data, Raw Text, Binary
- ✅ Ghi log đầy đủ: Headers, Query String, Body, Connection Info
- ✅ Tự động parse JSON nếu có thể
- ✅ Lưu vào file text với timestamp

**URL đầy đủ:**
```
https://your-server.com/api/dahuawebhook/event
```

**Ví dụ local:**
```
https://localhost:55703/api/dahuawebhook/event
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Event received successfully",
  "timestamp": "2025-11-03T10:30:45.123",
  "logFile": "dahua_event_20251103_103045_123.txt"
}
```

---

### 2. **GET /api/dahuawebhook/test**
Kiểm tra API có hoạt động không.

**Response:**
```json
{
  "status": "API is running",
  "endpoint": "/api/dahuawebhook/event",
  "method": "POST",
  "logDirectory": "D:\\stationcheck\\Logs\\DahuaWebhooks",
  "timestamp": "2025-11-03T10:30:45.123"
}
```

---

### 3. **GET /api/dahuawebhook/logs?limit=20**
Lấy danh sách các file log đã ghi.

**Query Parameters:**
- `limit` (optional): Số lượng file tối đa (default: 20)

**Response:**
```json
{
  "total": 5,
  "logs": [
    {
      "fileName": "dahua_event_20251103_103045_123.txt",
      "createdAt": "2025-11-03T10:30:45",
      "size": 2048
    }
  ]
}
```

---

### 4. **GET /api/dahuawebhook/logs/{fileName}**
Đọc nội dung của một file log cụ thể.

**Ví dụ:**
```
GET /api/dahuawebhook/logs/dahua_event_20251103_103045_123.txt
```

**Response:**
```json
{
  "fileName": "dahua_event_20251103_103045_123.txt",
  "content": "================================================================================\nDAHUA WEBHOOK EVENT - 2025-11-03 10:30:45.123\n...",
  "createdAt": "2025-11-03T10:30:45"
}
```

---

## Cấu hình Dahua NVR/DVR

### Bước 1: Đăng nhập vào Web Interface
1. Mở trình duyệt, truy cập IP của đầu ghi Dahua
2. Đăng nhập với tài khoản Admin

### Bước 2: Cấu hình HTTP Push
1. Vào menu: **Setup** → **Event** → **Abnormity**
2. Hoặc: **Setup** → **Event** → **Video Detection**
3. Chọn loại event bạn muốn (Motion Detection, Video Loss, etc.)

### Bước 3: Enable HTTP Push
1. Tìm phần **Linkage Method** hoặc **Alarm Output**
2. Check vào **HTTP** hoặc **HTTP Push**
3. Nhập URL của API:
   ```
   https://your-server.com/api/dahuawebhook/event
   ```

### Bước 4: Cấu hình nâng cao (nếu có)
- **Method**: POST
- **Content-Type**: application/json (hoặc để mặc định)
- **Authentication**: Basic Auth (nếu cần)
- **Interval**: Khoảng thời gian giữa các lần gửi

### Bước 5: Test
1. Click **Test** (nếu có) hoặc kích hoạt event thật
2. Kiểm tra log tại: `D:\stationcheck\Logs\DahuaWebhooks\`

---

## Cấu trúc File Log

Mỗi file log sẽ có cấu trúc như sau:

```
================================================================================
DAHUA WEBHOOK EVENT - 2025-11-03 10:30:45.123
================================================================================

--- REQUEST HEADERS ---
User-Agent: DahuaEventPusher/1.0
Content-Type: application/json
Content-Length: 256
Authorization: Basic xxxxx

--- QUERY STRING ---
?channel=1&event=motion

--- REQUEST BODY ---
Content-Type: application/json
Content-Length: 256

Raw Body:
{
  "eventType": "MotionDetection",
  "channelId": 1,
  "timestamp": "2025-11-03T10:30:45"
}

Parsed JSON (Pretty Print):
{
  "eventType": "MotionDetection",
  "channelId": 1,
  "timestamp": "2025-11-03T10:30:45"
}

--- CONNECTION INFO ---
Remote IP: 192.168.1.100
Remote Port: 54321
Local IP: 192.168.1.50
Local Port: 55703
Protocol: HTTP/1.1
Method: POST
Path: /api/dahuawebhook/event

================================================================================
```

---

## Test với Postman

### Test 1: JSON Body
```http
POST https://localhost:55703/api/dahuawebhook/event
Content-Type: application/json

{
  "eventType": "MotionDetection",
  "channelId": 1,
  "timestamp": "2025-11-03T10:30:45",
  "metadata": {
    "sensitivity": 80,
    "region": "Zone1"
  }
}
```

### Test 2: Form Data
```http
POST https://localhost:55703/api/dahuawebhook/event
Content-Type: application/x-www-form-urlencoded

eventType=MotionDetection&channelId=1&timestamp=2025-11-03T10:30:45
```

### Test 3: Query Parameters
```http
POST https://localhost:55703/api/dahuawebhook/event?channel=1&event=motion
```

---

## Test với PowerShell

```powershell
# Test 1: JSON
$body = @{
    eventType = "MotionDetection"
    channelId = 1
    timestamp = (Get-Date).ToString("o")
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:55703/api/dahuawebhook/event" `
    -Method POST `
    -Body $body `
    -ContentType "application/json" `
    -SkipCertificateCheck

# Test 2: Form Data
$form = @{
    eventType = "MotionDetection"
    channelId = 1
}

Invoke-RestMethod -Uri "https://localhost:55703/api/dahuawebhook/event" `
    -Method POST `
    -Body $form `
    -SkipCertificateCheck
```

---

## Xem Log Files

### Qua API
```bash
# Lấy danh sách logs
curl https://localhost:55703/api/dahuawebhook/logs

# Xem nội dung log cụ thể
curl https://localhost:55703/api/dahuawebhook/logs/dahua_event_20251103_103045_123.txt
```

### Trực tiếp trong Windows Explorer
```
D:\stationcheck\Logs\DahuaWebhooks\
```

---

## Lưu ý bảo mật

⚠️ **QUAN TRỌNG:** API này đang ở chế độ phát triển, chưa có authentication!

### Để deploy production, cần thêm:

1. **API Key Authentication**
2. **IP Whitelist** (chỉ cho phép IP của Dahua NVR)
3. **HTTPS bắt buộc**
4. **Rate Limiting**

### Ví dụ thêm API Key (sửa Controller):

```csharp
[HttpPost("event")]
public async Task<IActionResult> ReceiveEvent([FromHeader(Name = "X-API-Key")] string apiKey)
{
    // Validate API key
    if (apiKey != "your-secret-api-key-here")
    {
        return Unauthorized("Invalid API key");
    }
    
    // ... rest of code
}
```

Sau đó cấu hình Dahua gửi header:
```
X-API-Key: your-secret-api-key-here
```

---

## Troubleshooting

### Không nhận được webhook
1. Kiểm tra Dahua có thể ping được server không
2. Kiểm tra firewall đã mở port 55703 (hoặc 443 nếu production)
3. Kiểm tra URL đã đúng chưa
4. Xem log của ASP.NET Core để debug

### File log không được tạo
1. Kiểm tra quyền ghi vào thư mục `Logs/DahuaWebhooks/`
2. Xem log error trong console

### Muốn xem log real-time
```powershell
# Theo dõi thư mục log
Get-ChildItem D:\stationcheck\Logs\DahuaWebhooks\ -Filter *.txt | Sort-Object LastWriteTime -Descending | Select-Object -First 1 | Get-Content -Tail 50 -Wait
```

---

## Tiếp theo

Sau khi có được cấu trúc dữ liệu từ Dahua:

1. Phân tích file log để hiểu format
2. Tạo Model class tương ứng
3. Parse JSON thành object
4. Lưu vào database
5. Xử lý logic (gửi thông báo, trigger action, etc.)

**Chúc bạn thành công!** 🚀
