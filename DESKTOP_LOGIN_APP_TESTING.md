# Desktop Login App - Testing Guide

## Hoàn thành Full Luồng Login với MAC Address Authentication

### 1. Database Setup ✅
```sql
-- Đã thêm 2 cột vào bảng UserDevices:
- MacAddress NVARCHAR(17) NULL    -- Format: XX:XX:XX:XX:XX:XX
- DeviceStatus INT NOT NULL       -- 0: PendingApproval, 1: Approved, 2: Rejected
```

### 2. Backend Changes ✅
- **AuthService.cs**: Xóa try-catch không cần thiết vì database đã có cột
- **LoginRequest**: Đã có field MacAddress và DeviceName
- **UserDevice model**: Đã có properties MacAddress và DeviceStatus

### 3. Desktop Login App Features ✅

#### DeviceInfoHelper.cs
- Lấy MAC address của network adapter đầu tiên
- Lấy device name (computer name)
- Format MAC: XX:XX:XX:XX:XX:XX

#### StationCheckApiClient.cs
- HTTP client với certificate authentication
- Auto-load certificate từ Windows Certificate Store
- POST /api/auth/login với username, password, MAC address, device name

#### MainWindow.xaml.cs
- Validate input (username, password)
- Get device info (MAC address, device name)
- Call API login
- Hiển thị response message

### 4. Testing Steps

#### Test 1: Employee Web Login (Desktop App Required)
```
1. Mở trình duyệt: https://localhost:55703/login
2. Login với: employee1 / Employee@2025
3. Kết quả: Hiển thị thông báo "Vui lòng tải Desktop Login App..."
4. Click nút "Tải Desktop Login App"
5. Download file DesktopLoginApp.zip
```

#### Test 2: Desktop App Login (Device Not Registered)
```
1. Giải nén DesktopLoginApp.zip
2. Chạy DesktopLoginApp.exe
3. Nhập: employee1 / Employee@2025
4. Click "Đăng nhập"
5. Kết quả: 
   - Nếu chưa có certificate: "Thiết bị chưa được đăng ký. Vui lòng tải Device Installer..."
   - Nếu có certificate nhưng chưa approve: "Thiết bị chưa được phê duyệt..."
   - Nếu approved nhưng chưa assign: "Bạn chưa được phân quyền đăng nhập từ thiết bị này..."
```

#### Test 3: Full Flow with Device Registration
```
1. Chạy DeviceInstaller.exe (từ folder DeviceInstaller/bin/Release/...)
2. Nhập Station Code và click Register
3. Admin approve device trong web UI (Device Approval page)
4. Admin assign user to device (User Management page)
5. Chạy DesktopLoginApp.exe
6. Login với employee1 / Employee@2025
7. Kết quả: "Đăng nhập thành công! Chào mừng..."
```

### 5. Flow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     Employee Login Flow                      │
└─────────────────────────────────────────────────────────────┘

                        ┌──────────────┐
                        │  Web Login   │
                        │  employee1   │
                        └──────┬───────┘
                               │
                    ┌──────────▼──────────┐
                    │ Check Role          │
                    │ = StationEmployee?  │
                    └──────────┬──────────┘
                               │ Yes
                    ┌──────────▼──────────────┐
                    │ Has Certificate?        │
                    │ (from browser)          │
                    └──────────┬──────────────┘
                               │ No
                    ┌──────────▼──────────────┐
                    │ Return Message:         │
                    │ "Download Desktop App"  │
                    └──────────┬──────────────┘
                               │
                    ┌──────────▼──────────────┐
                    │ User Downloads App      │
                    │ DesktopLoginApp.exe     │
                    └──────────┬──────────────┘
                               │
                    ┌──────────▼──────────────┐
                    │ Desktop App Gets:       │
                    │ - MAC Address           │
                    │ - Device Name           │
                    │ - Certificate (if any)  │
                    └──────────┬──────────────┘
                               │
                    ┌──────────▼──────────────┐
                    │ POST /api/auth/login    │
                    │ {username, password,    │
                    │  macAddress, device}    │
                    └──────────┬──────────────┘
                               │
                    ┌──────────▼──────────────┐
                    │ Backend Checks:         │
                    │ 1. Device registered?   │
                    │ 2. Device approved?     │
                    │ 3. User assigned?       │
                    │ 4. Device not revoked?  │
                    └──────────┬──────────────┘
                               │
                    ┌──────────▼──────────────┐
                    │ All OK?                 │
                    └──────────┬──────────────┘
                               │ Yes
                    ┌──────────▼──────────────┐
                    │ Return JWT Token        │
                    │ Login Success!          │
                    └─────────────────────────┘
```

### 6. Current Status

✅ **Hoàn thành**:
- Database có cột MacAddress và DeviceStatus
- Desktop App có UI hoàn chỉnh
- Desktop App tích hợp API client
- Helper lấy MAC address
- Backend xử lý MAC address authentication

⚠️ **Cần làm tiếp**:
- Tạo main application window sau khi login thành công
- Lưu token vào local storage/registry
- Auto-login nếu có token hợp lệ
- Remember Me functionality
- Device registration flow trong Desktop App (hoặc dùng DeviceInstaller riêng)

### 7. Files Modified/Created

**Backend**:
- `add-mac-device-columns.sql` - SQL script thêm cột
- `Services/AuthService.cs` - Clean up try-catch

**Desktop App**:
- `Helpers/DeviceInfoHelper.cs` - NEW
- `Services/StationCheckApiClient.cs` - NEW
- `MainWindow.xaml.cs` - Updated with API integration
- `DesktopLoginApp.csproj` - Added System.Management reference

**Published**:
- `wwwroot/DesktopLoginApp/DesktopLoginApp.exe` - Updated with API integration

### 8. Testing Commands

```powershell
# Test MAC address detection
cd DesktopLoginApp
dotnet run

# Check database columns
sqlcmd -S "VUTHESANG\SQLEXPRESS01" -d StationCheckDb -E -Q "SELECT TOP 5 DeviceName, MacAddress, DeviceStatus, IsApproved FROM UserDevices"

# Run web app
cd d:\station-c
dotnet run

# Check logs for login attempts
Get-Content Logs\app-*.txt -Tail 50
```

### 9. Error Messages Reference

| Message | Meaning | Action |
|---------|---------|--------|
| "Vui lòng tải Desktop Login App..." | Web login for Employee | Download Desktop App |
| "Không thể lấy thông tin MAC address..." | No network adapter found | Check network connection |
| "Thiết bị chưa được đăng ký..." | Device not in database | Run DeviceInstaller.exe |
| "Thiết bị chưa được phê duyệt..." | Device pending approval | Admin approve in web UI |
| "Bạn chưa được phân quyền..." | User not assigned to device | Admin assign user |
| "Thiết bị đã bị thu hồi..." | Device revoked | Contact admin |
| "Đăng nhập thành công!" | Login OK | Token received |

### 10. Next Steps

1. **Test Desktop App**: Chạy app, login với employee1, xem response
2. **Register Device**: Dùng DeviceInstaller để đăng ký thiết bị mới
3. **Approve & Assign**: Admin approve device và assign user
4. **Full Test**: Login lại để test full flow
5. **Build Main Window**: Tạo màn hình chính sau login thành công
