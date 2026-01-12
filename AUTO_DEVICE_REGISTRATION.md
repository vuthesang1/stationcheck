# Desktop Login App - Auto Device Registration Feature

## ✅ Hoàn thành Full Implementation

### 1. Backend API Endpoints

#### POST /api/auth/register-device
```csharp
// Request
{
  "username": "employee1",
  "password": "Employee@2025",
  "macAddress": "AA:BB:CC:DD:EE:FF",
  "deviceName": "DESKTOP-ABC",
  "certificateThumbprint": "..." // Optional
}

// Response
{
  "success": true,
  "status": 0, // 0=PendingApproval, 1=Approved, 2=Rejected
  "message": "Đăng ký thiết bị thành công! Vui lòng chờ Admin phê duyệt.",
  "deviceId": "guid"
}
```

**Logic**:
- Verify username/password
- Check if device already registered (by MAC or Certificate)
- If exists:
  - Approved + Assigned → Return "Login again"
  - Approved + Not Assigned → Return "Need assignment"
  - Pending → Return "Still pending"
  - Revoked → Return "Device revoked"
- If new:
  - Create UserDevice with PendingApproval status
  - Auto-create DeviceUserAssignment (inactive until approved)
  - Return device ID for status tracking

#### GET /api/auth/device-status/{deviceId}
```csharp
// Response
{
  "success": true,
  "status": 0,
  "message": "Thiết bị đang chờ phê duyệt",
  "isApproved": false,
  "isRevoked": false,
  "isUserAssigned": true,
  "deviceName": "DESKTOP-ABC",
  "macAddress": "AA:BB:CC:DD:EE:FF",
  "approvedAt": null,
  "approvedBy": null
}
```

### 2. Desktop Login App Changes

#### New Features:
1. **Auto Device Registration**
   - When login fails with "device not registered" → Auto call RegisterDevice API
   - Display registration status in real-time

2. **Device Status Panel**
   - Shows: Device Name, MAC Address, Status (Pending/Approved/Rejected)
   - Color-coded status:
     - ⏳ Orange = Pending Approval
     - ✅ Green = Approved
     - ❌ Red = Rejected

3. **Refresh Status Button**
   - Manual check if device has been approved
   - Auto-prompt to login again if approved + assigned

#### API Client Methods:
```csharp
// New methods in StationCheckApiClient.cs
Task<DeviceRegistrationResponse> RegisterDeviceAsync(username, password, mac, device)
Task<DeviceStatusResponse> GetDeviceStatusAsync(deviceId)
```

#### Login Flow:
```
1. User enters username/password
2. App gets MAC address
3. Try LoginAsync()
   ├─ Success + Token → Login successful ✅
   ├─ Error: "not registered" → Auto RegisterDeviceAsync()
   │   └─ Show status panel (Pending)
   ├─ Error: "not approved" → Show status panel (Pending)
   └─ Error: "not assigned" → Show error message
```

### 3. UI Components

**New XAML Elements**:
- `pnlDeviceStatus` - Border panel for device info
- `txtDeviceName` - Display device name
- `txtMacAddress` - Display MAC address (monospace font)
- `txtDeviceStatusText` - Status with emoji (⏳/✅/❌)
- `btnRefreshStatus` - Button to check status again

**Status Colors**:
```csharp
PendingApproval  → Orange (#FFC107)
Approved         → Green (#28A745)
Rejected         → Red (#DC3545)
```

### 4. Database Schema

**UserDevices Table** (already has columns):
```sql
MacAddress NVARCHAR(17) NULL        -- Format: XX:XX:XX:XX:XX:XX
DeviceStatus INT NOT NULL           -- 0=Pending, 1=Approved, 2=Rejected
```

**DeviceUserAssignments Table**:
- Auto-created when device registers
- `IsActive = false` until approved
- Links UserId ↔ DeviceId

### 5. Testing Flow

#### Scenario 1: New Employee First Login
```
1. Employee1 opens DesktopLoginApp
2. Enters: employee1 / Employee@2025
3. Click "Đăng nhập"
4. App detects MAC: AA:BB:CC:DD:EE:FF
5. Call LoginAsync() → Failed: "Device not registered"
6. Auto call RegisterDeviceAsync()
7. Show status panel:
   Device: DESKTOP-XYZ
   MAC: AA:BB:CC:DD:EE:FF
   Status: ⏳ Chờ phê duyệt
8. User waits for admin approval...
```

#### Scenario 2: Admin Approves Device
```
1. Admin logs into web UI
2. Goes to Device Approval page
3. Sees device: DESKTOP-XYZ (employee1)
4. Clicks "Approve"
5. Device status → Approved
6. DeviceUserAssignment.IsActive → true
```

#### Scenario 3: Employee Login After Approval
```
1. Employee1 clicks "🔄 Kiểm tra lại" button
2. Call GetDeviceStatusAsync()
3. Status: ✅ Đã phê duyệt
4. Popup: "Thiết bị đã được phê duyệt! Bạn có muốn đăng nhập ngay không?"
5. Click "Yes"
6. Call LoginAsync() → Success + Token ✅
7. MessageBox: "Login thành công! Chào mừng Employee 1"
```

### 6. Files Modified/Created

**Backend**:
- `Controllers/AuthController.cs` - Added RegisterDevice + GetDeviceStatus endpoints
- `Models/DeviceRegistrationDto.cs` - NEW DTOs

**Desktop App**:
- `Services/StationCheckApiClient.cs` - Added RegisterDeviceAsync, GetDeviceStatusAsync
- `MainWindow.xaml` - Added device status panel UI
- `MainWindow.xaml.cs` - Added auto-register logic + refresh button handler
- `Helpers/DeviceInfoHelper.cs` - Moved DeviceInfo class outside static class

**Deployed**:
- `wwwroot/DesktopLoginApp/DesktopLoginApp.exe` - Updated with new features

### 7. DeviceInstaller vs DesktopLoginApp

**OLD FLOW** (DeviceInstaller):
```
1. Run DeviceInstaller.exe
2. Generate certificate
3. Install certificate
4. Register device with certificate
5. Admin approves
6. Run DesktopLoginApp to login
```

**NEW FLOW** (DesktopLoginApp only):
```
1. Run DesktopLoginApp.exe
2. Login with username/password
3. Auto-detect MAC address
4. Auto-register device
5. Admin approves
6. Refresh status → Login successful
```

**DeviceInstaller is NO LONGER NEEDED** ✅

### 8. Benefits

1. **Simplified UX**: User only needs DesktopLoginApp
2. **No Certificate Management**: MAC address-based identification
3. **Real-time Status**: User can see approval status in app
4. **Auto-Assignment**: Device-User link created automatically
5. **Retry Logic**: Easy to check status and login again

### 9. Next Steps

- ✅ Backend API implemented
- ✅ Desktop App auto-register implemented
- ✅ Device status panel implemented
- ✅ Refresh status button implemented
- ⚠️ TODO: Main application window after successful login
- ⚠️ TODO: Remember Me functionality
- ⚠️ TODO: Token storage (registry/local file)

### 10. Test Commands

```powershell
# Run backend
cd d:\station-c
dotnet run

# Open browser
Start-Process "https://localhost:55703/login"

# Run Desktop Login App
Start-Process "d:\station-c\wwwroot\DesktopLoginApp\DesktopLoginApp.exe"

# Check database
sqlcmd -S "VUTHESANG\SQLEXPRESS01" -d StationCheckDb -E -Q "
SELECT 
  DeviceName, 
  MacAddress, 
  DeviceStatus,
  IsApproved,
  CreatedAt
FROM UserDevices
ORDER BY CreatedAt DESC
"
```

### 11. Screenshots Expected

**First Login (Not Registered)**:
```
┌─────────────────────────────────────┐
│ Chào mừng trở lại!                  │
│                                     │
│ [employee1        ]                 │
│ [••••••••••       ]                 │
│ □ Ghi nhớ đăng nhập                 │
│ ┌────────────────┐                  │
│ │  Đăng nhập     │                  │
│ └────────────────┘                  │
│                                     │
│ ℹ️ Thiết bị chưa đăng ký...         │
│                                     │
│ ┌─────────────────────────────────┐ │
│ │ Trạng thái thiết bị             │ │
│ │ Device: DESKTOP-ABC             │ │
│ │ MAC: AA:BB:CC:DD:EE:FF          │ │
│ │ Trạng thái: ⏳ Chờ phê duyệt    │ │
│ │ ┌────────────────┐               │ │
│ │ │ 🔄 Kiểm tra lại │               │ │
│ │ └────────────────┘               │ │
│ └─────────────────────────────────┘ │
└─────────────────────────────────────┘
```

**After Admin Approves**:
```
┌─────────────────────────────────────┐
│ ✅ Đã phê duyệt                     │
│                                     │
│ Popup: "Thiết bị đã được phê duyệt! │
│        Bạn có muốn đăng nhập ngay?" │
│        [Yes] [No]                   │
└─────────────────────────────────────┘
```
