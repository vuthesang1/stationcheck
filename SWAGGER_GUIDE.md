# Swagger API Documentation Guide

## 🚀 Truy cập Swagger UI

Sau khi chạy ứng dụng, truy cập Swagger UI tại:

**URL:** `https://localhost:55703/api/docs`

## 📚 Danh sách API Endpoints

### 1. **Dahua Webhook Controller** (`/api/DahuaWebhook`)

#### **POST /api/dahuawebhook/event**
Nhận webhook event từ đầu ghi Dahua qua chức năng HTTP Push.

**Mô tả:**
- API này có thể nhận mọi loại input (JSON, Form-data, XML, text, v.v.)
- Tự động ghi log toàn bộ request vào file text để phân tích
- Log bao gồm: Headers, Query String, Body (raw + parsed), Form Data, Files, Connection Info

**Response:**
```json
{
  "success": true,
  "message": "Event received successfully",
  "timestamp": "2025-11-03T20:50:00",
  "logFile": "dahua_event_20251103_205000_123.txt"
}
```

**Cấu hình trên đầu ghi Dahua:**
1. Vào **Event → HTTP Push**
2. Nhập URL: `https://your-server-ip:55703/api/dahuawebhook/event`
3. Method: **POST**
4. Chọn các event muốn gửi (Motion Detection, Line Crossing, Intrusion, Face Detection, v.v.)
5. Save và Test

---

#### **GET /api/dahuawebhook/test**
Kiểm tra xem API có hoạt động không.

**Response:**
```json
{
  "status": "API is running",
  "endpoint": "/api/dahuawebhook/event",
  "method": "POST",
  "logDirectory": "D:\\stationcheck\\Logs\\DahuaWebhooks",
  "timestamp": "2025-11-03T20:50:00"
}
```

---

#### **GET /api/dahuawebhook/logs?limit=20**
Lấy danh sách các file log đã ghi (mặc định 20 file gần nhất).

**Query Parameters:**
- `limit` (int, optional): Số lượng file log trả về (mặc định: 20)

**Response:**
```json
{
  "total": 5,
  "logs": [
    {
      "fileName": "dahua_event_20251103_205000_123.txt",
      "createdAt": "2025-11-03T20:50:00",
      "size": 2048
    }
  ]
}
```

---

#### **GET /api/dahuawebhook/logs/{fileName}**
Đọc nội dung của một file log cụ thể.

**Path Parameters:**
- `fileName` (string): Tên file log (VD: `dahua_event_20251103_205000_123.txt`)

**Response:**
```json
{
  "fileName": "dahua_event_20251103_205000_123.txt",
  "content": "================================================================================\nDAHUA WEBHOOK EVENT - 2025-11-03 20:50:00.123\n...",
  "createdAt": "2025-11-03T20:50:00"
}
```

---

## 🧪 Test API với Swagger UI

### Cách sử dụng Swagger UI:

1. **Truy cập:** `https://localhost:55703/api/docs`

2. **Test endpoint GET:**
   - Click vào endpoint muốn test (VD: `GET /api/dahuawebhook/test`)
   - Click nút **"Try it out"**
   - Click **"Execute"**
   - Xem kết quả ở phần **Response**

3. **Test endpoint POST với JSON:**
   - Click vào `POST /api/dahuawebhook/event`
   - Click **"Try it out"**
   - Nhập JSON test vào **Request body**:
   ```json
   {
     "eventType": "MotionDetection",
     "cameraId": "CAM001",
     "timestamp": "2025-11-03T20:50:00",
     "confidence": 0.95,
     "location": {
       "x": 100,
       "y": 200,
       "width": 50,
       "height": 50
     }
   }
   ```
   - Click **"Execute"**
   - Kiểm tra response và file log được tạo

4. **Test với Form-data:**
   - Chọn **Media type:** `multipart/form-data`
   - Click **"Try it out"**
   - Nhập các field và upload file
   - Click **"Execute"**

---

## 📂 Xem Log Files

Tất cả webhook events được ghi vào thư mục:

**📁 Location:** `D:\stationcheck\Logs\DahuaWebhooks\`

**📄 File format:** `dahua_event_yyyyMMdd_HHmmss_fff.txt`

**Nội dung log bao gồm:**
- ✅ Timestamp chi tiết (đến mili giây)
- ✅ Request Headers (User-Agent, Content-Type, v.v.)
- ✅ Query String
- ✅ Request Body (raw text + parsed JSON nếu có)
- ✅ Form Data (nếu là form submission)
- ✅ Uploaded Files (metadata)
- ✅ Connection Info (IP, Port, Protocol)

**Ví dụ log file:**
```
================================================================================
DAHUA WEBHOOK EVENT - 2025-11-03 20:50:00.123
================================================================================

--- REQUEST HEADERS ---
Content-Type: application/json
User-Agent: Dahua/1.0
Content-Length: 245

--- REQUEST BODY ---
Content-Type: application/json
Content-Length: 245

Raw Body:
{"eventType":"MotionDetection","cameraId":"CAM001",...}

Parsed JSON (Pretty Print):
{
  "eventType": "MotionDetection",
  "cameraId": "CAM001",
  ...
}

--- CONNECTION INFO ---
Remote IP: 192.168.1.100
Remote Port: 54321
Local IP: 192.168.1.10
Local Port: 55703
Protocol: HTTP/2
Method: POST
Path: /api/dahuawebhook/event

================================================================================
```

---

## 🔐 Authentication (Nếu cần)

Hiện tại API **không yêu cầu authentication** để đầu ghi Dahua có thể gửi webhook dễ dàng.

Nếu muốn bảo mật, có thể:
1. Thêm `[Authorize]` attribute vào controller
2. Sử dụng API Key trong Header
3. Whitelist IP của đầu ghi Dahua

---

## 🛠️ Troubleshooting

### Lỗi: "SSL certificate error"
- Đầu ghi Dahua có thể không tin tưởng self-signed certificate
- **Giải pháp:** Sử dụng HTTP thay vì HTTPS: `http://localhost:55704/api/dahuawebhook/event`

### Lỗi: "Connection refused"
- Kiểm tra firewall
- Đảm bảo app đang chạy: `dotnet run`
- Kiểm tra port: `netstat -ano | findstr :55703`

### Webhook không gửi từ Dahua
- Kiểm tra network giữa đầu ghi và server
- Thử test bằng Postman/curl trước
- Kiểm tra log trên đầu ghi Dahua

---

## 📖 Tài liệu bổ sung

- **Swagger JSON:** `https://localhost:55703/swagger/v1/swagger.json`
- **API Base URL:** `https://localhost:55703/api`
- **Log Directory:** `D:\stationcheck\Logs\DahuaWebhooks\`

---

## 💡 Tips

1. **Test với Postman/curl:**
   ```bash
   # Test với JSON
   curl -X POST https://localhost:55703/api/dahuawebhook/event \
     -H "Content-Type: application/json" \
     -d '{"test": "data"}'

   # Test với Form-data
   curl -X POST https://localhost:55703/api/dahuawebhook/event \
     -F "field1=value1" \
     -F "file=@image.jpg"
   ```

2. **Xem log realtime:**
   ```bash
   Get-Content "D:\stationcheck\Logs\DahuaWebhooks\dahua_event_*.txt" -Wait
   ```

3. **Đếm số event đã nhận:**
   ```bash
   (Get-ChildItem "D:\stationcheck\Logs\DahuaWebhooks\dahua_event_*.txt").Count
   ```

---

**✅ Ready to use! Truy cập https://localhost:55703/api/docs để bắt đầu.**
