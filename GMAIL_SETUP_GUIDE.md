# Hướng dẫn cấu hình Email để fetch từ Gmail

## Bước 1: Tạo App Password cho Gmail

Gmail yêu cầu bạn sử dụng **App Password** thay vì mật khẩu thông thường khi kết nối qua IMAP.

### Các bước tạo App Password:

1. **Bật xác thực 2 bước (2-Step Verification)**:
   - Truy cập: https://myaccount.google.com/security
   - Tìm mục "2-Step Verification" và bật nó lên
   - Làm theo hướng dẫn để hoàn tất

2. **Tạo App Password**:
   - Sau khi bật 2-Step Verification, truy cập: https://myaccount.google.com/apppasswords
   - Hoặc từ trang Security, tìm "App passwords"
   - Chọn "Select app" → "Other (Custom name)"
   - Đặt tên: "StationCheck Email Monitor"
   - Click "Generate"
   - Gmail sẽ hiển thị một mật khẩu 16 ký tự (có dạng: `xxxx xxxx xxxx xxxx`)
   - **LƯU Ý**: Copy mật khẩu này ngay vì bạn sẽ không thể xem lại

## Bước 2: Cấu hình trong appsettings.json

Mở file `appsettings.json` và tìm section `EmailSettings`:

```json
{
  "EmailSettings": {
    "ImapServer": "imap.gmail.com",
    "ImapPort": 993,
    "UseSsl": true,
    "EmailAddress": "vuthesang4@gmail.com",
    "Password": "YOUR_APP_PASSWORD_HERE",
    "CheckIntervalMinutes": 1,
    "MarkAsRead": true,
    "DeleteAfterProcessing": false
  }
}
```

### Thay thế `YOUR_APP_PASSWORD_HERE` bằng App Password vừa tạo

**VÍ DỤ**:
```json
"Password": "abcd efgh ijkl mnop"
```

Hoặc bỏ dấu cách:
```json
"Password": "abcdefghijklmnop"
```

## Bước 3: Giải thích các tham số

| Tham số | Mô tả | Giá trị đề xuất |
|---------|-------|-----------------|
| `ImapServer` | Địa chỉ IMAP server của Gmail | `imap.gmail.com` |
| `ImapPort` | Cổng IMAP SSL | `993` |
| `UseSsl` | Sử dụng SSL/TLS | `true` |
| `EmailAddress` | Email của bạn | `vuthesang4@gmail.com` |
| `Password` | App Password (16 ký tự) | App Password từ Gmail |
| `CheckIntervalMinutes` | Tần suất check email (phút) | `1` (mỗi 1 phút) |
| `MarkAsRead` | Đánh dấu email đã đọc sau khi xử lý | `true` |
| `DeleteAfterProcessing` | Xóa email sau khi xử lý | `false` (nên để false để giữ email) |

## Bước 4: Kiểm tra hoạt động

1. **Khởi động ứng dụng**:
   ```powershell
   dotnet run
   ```

2. **Xem log**:
   - Ứng dụng sẽ check email mỗi 1 phút (theo `CheckIntervalMinutes`)
   - Log sẽ hiển thị:
     ```
     [EmailService] Checking for new emails...
     [EmailService] Connecting to imap.gmail.com:993
     [EmailService] Successfully authenticated as vuthesang4@gmail.com
     [EmailService] Found 2 unread email(s)
     [EmailService] Processing email: Subject='4', From='...
     [EmailService] Successfully processed email UID 123
     [EmailService] Processed 2 email(s) successfully
     ```

3. **Gửi email test**:
   - Gửi email đến `vuthesang4@gmail.com`
   - Subject: `4` hoặc `ST000004` (Station ID hoặc Station Code)
   - Body: Nội dung theo format của Dahua NVR (xem mẫu bên dưới)

## Mẫu email từ Dahua NVR

```
Alarm Event: Motion Detection
Alarm Input Channel No.: 2
Alarm Input Channel Name: IPC
Alarm Start Time (D/M/Y H:M:S): 12/11/2025 16:03:57
Alarm Device Name: NVR-6C39
Alarm Name: 
IP Address: 192.168.1.200
Alarm Details:
```

## Troubleshooting

### Lỗi "Authentication failed"
- Kiểm tra App Password đã đúng chưa (16 ký tự, không có dấu cách hoặc có dấu cách)
- Đảm bảo đã bật 2-Step Verification
- Thử tạo lại App Password mới

### Lỗi "Could not connect"
- Kiểm tra kết nối internet
- Đảm bảo `ImapServer` là `imap.gmail.com`
- Đảm bảo `ImapPort` là `993`
- Đảm bảo `UseSsl` là `true`

### Email không được xử lý
- Kiểm tra Subject có chứa Station ID hoặc Station Code không
- Kiểm tra Station đã được tạo trong database chưa
- Xem log để biết lý do cụ thể

## Bảo mật

⚠️ **QUAN TRỌNG**:
- **KHÔNG** commit file `appsettings.json` có chứa App Password lên Git
- Thêm vào `.gitignore`:
  ```
  appsettings.json
  appsettings.*.json
  ```
- Sử dụng environment variables hoặc Azure Key Vault cho production

### Sử dụng Environment Variable (Khuyến nghị cho Production)

```powershell
# Windows PowerShell
$env:EmailSettings__Password = "your-app-password"
```

Hoặc trong `appsettings.Production.json`:
```json
{
  "EmailSettings": {
    "Password": ""  // Sẽ được override bởi environment variable
  }
}
```

## Kiến trúc hoạt động

```
Gmail IMAP Server (imap.gmail.com:993)
         ↓
    [EmailMonitoringService] (Background Service - Chạy mỗi 1 phút)
         ↓
    [EmailService.CheckAndProcessNewEmailsAsync()]
         ↓
    Fetch unread emails → Parse → Create MotionEvent → Save to DB
         ↓
    [MonitoringService] → Check timeframes → Generate alerts if needed
```

## Tóm tắt

1. ✅ Bật 2-Step Verification trên Gmail
2. ✅ Tạo App Password
3. ✅ Cấu hình `appsettings.json` với App Password
4. ✅ Chạy ứng dụng
5. ✅ Gửi email test và xem log

Ứng dụng sẽ tự động fetch email từ Gmail mỗi 1 phút và xử lý các email có chứa thông tin motion detection! 🎉
