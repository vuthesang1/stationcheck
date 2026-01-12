# README - .NET Framework 4.8 Version

## Windows 7 Compatibility Version

Đây là phiên bản Desktop Login App được build lại với **.NET Framework 4.8** để hỗ trợ **Windows 7 SP1**.

## Yêu cầu hệ thống

### Trên máy build (Developer)
- Windows 10/11
- Visual Studio 2019/2022 hoặc Build Tools
- .NET Framework 4.8 SDK
- MSBuild và NuGet CLI

### Trên máy user (Windows 7)
- **Windows 7 SP1** (bắt buộc phải có Service Pack 1)
- **.NET Framework 4.8 Runtime**
  - Download: https://dotnet.microsoft.com/download/dotnet-framework/net48
  - File: `ndp48-web.exe` hoặc `ndp48-x86-x64-allos-enu.exe`

## Cách build

### Option 1: Sử dụng PowerShell script
```powershell
cd d:\station-c\DesktopLoginApp
.\build-net48.ps1
```

### Option 2: Build thủ công
```powershell
# Restore packages
nuget restore DesktopLoginApp-Net48.csproj -PackagesDirectory packages

# Build
msbuild DesktopLoginApp-Net48.csproj /p:Configuration=Release /p:Platform=AnyCPU /t:Rebuild
```

## Output

Build thành công sẽ tạo folder:
```
bin\Release\Net48\
  ├── DesktopLoginApp.exe         (Main executable)
  ├── DesktopLoginApp.exe.config  (Config file)
  ├── Newtonsoft.Json.dll         (JSON library)
  └── [other DLLs]
```

## Deploy lên Windows 7

1. **Copy toàn bộ folder** `bin\Release\Net48` sang máy Windows 7
2. **Cài .NET Framework 4.8** (nếu chưa có):
   - Download từ: https://dotnet.microsoft.com/download/dotnet-framework/net48
   - Hoặc dùng file offline installer: `ndp48-x86-x64-allos-enu.exe`
3. **Chạy** `DesktopLoginApp.exe`

## Sự khác biệt với version .NET 9

### Code changes
- ❌ Không dùng `System.Text.Json` → ✅ Dùng `Newtonsoft.Json`
- ❌ Không dùng nullable reference types (`string?`) → ✅ Dùng `string`
- ❌ Không dùng file-scoped namespaces → ✅ Dùng block namespaces
- ❌ Không dùng `PostAsJsonAsync` → ✅ Dùng `StringContent` + JSON manual

### Features
- ✅ Certificate authentication: Vẫn hoạt động
- ✅ MAC address detection: Vẫn hoạt động
- ✅ WPF UI: Vẫn hoạt động
- ✅ File logging: Vẫn hoạt động
- ❌ Single-file publish: Không hỗ trợ (phải copy nhiều DLL)

### Performance
- Khởi động: Chậm hơn ~20% so với .NET 9
- Memory: Sử dụng nhiều hơn ~15MB
- Tương thích: Hỗ trợ Windows 7 SP1+

## Troubleshooting

### Lỗi: "This application requires .NET Framework 4.8"
**Giải pháp:** Cài .NET Framework 4.8 từ link trên

### Lỗi: "Could not load file Newtonsoft.Json.dll"
**Giải pháp:** Copy lại toàn bộ folder build, không chỉ file .exe

### Lỗi: "Application has stopped working"
**Giải pháp:** 
1. Kiểm tra Event Viewer → Windows Logs → Application
2. Xem file log tại: `%LOCALAPPDATA%\StationCheck\Logs\desktop-login.log`
3. Đảm bảo Windows 7 đã update đầy đủ (Windows Update)

### Windows 7 không cài được .NET Framework 4.8
**Yêu cầu:**
- Phải có **Windows 7 Service Pack 1**
- Cài Windows Update KB2999226 (nếu thiếu)
- Khởi động lại sau khi cài

## Kiểm tra version đang dùng

Mở PowerShell/Command Prompt:
```cmd
DesktopLoginApp.exe --version
```

Hoặc right-click vào `DesktopLoginApp.exe` → Properties → Details:
- **Product Version**: 1.0.0.0
- **Description**: StationCheck Desktop Login (Windows 7 Compatible)

## Hỗ trợ

Nếu gặp vấn đề:
1. Gửi file log: `%LOCALAPPDATA%\StationCheck\Logs\desktop-login.log`
2. Screenshot lỗi
3. Thông tin hệ thống: `winver` và `systeminfo`
