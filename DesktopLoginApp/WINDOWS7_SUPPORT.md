# Hỗ trợ Windows 7 - Desktop Login App

## ⚠️ Vấn đề

Desktop Login App được phát triển trên **.NET 8.0**, framework này **KHÔNG hỗ trợ Windows 7**.

Khi chạy trên Windows 7, ứng dụng sẽ:
- Không khởi động được
- Bị treo (hang/freeze)
- Báo lỗi thiếu dependencies
- Hoặc không có phản hồi gì

## ✅ Giải pháp

### Tùy chọn 1: Nâng cấp hệ điều hành (Khuyến nghị)

Nâng cấp lên một trong các hệ điều hành được hỗ trợ:
- **Windows 10** (phiên bản 1607 trở lên)
- **Windows 11**
- **Windows Server 2012 R2** trở lên

**Lý do khuyến nghị:**
- Windows 7 đã hết hỗ trợ từ Microsoft (14/01/2020)
- Không còn nhận bản vá bảo mật
- Nhiều ứng dụng hiện đại không tương thích
- Rủi ro bảo mật cao

### Tùy chọn 2: Sử dụng trình duyệt Web

Nếu không thể nâng cấp OS, nhân viên có thể:

1. **Sử dụng trình duyệt hỗ trợ chứng chỉ client**:
   - Chrome (phiên bản cũ tương thích Win7)
   - Firefox ESR
   - Edge Legacy (nếu có)

2. **Cài đặt chứng chỉ thủ công**:
   - Download chứng chỉ từ quản trị viên
   - Import vào Certificate Store của Windows
   - Restart trình duyệt
   - Truy cập https://pvgascng.vn
   - Trình duyệt sẽ tự động sử dụng chứng chỉ đã cài

3. **Quy trình cài chứng chỉ thủ công**:

```powershell
# Bước 1: Admin tạo chứng chỉ cho user
# (Thực hiện trên máy có Desktop Login App hoặc server)

# Bước 2: Copy file .pfx về máy Windows 7
# Ví dụ: user_certificate.pfx

# Bước 3: Import vào Certificate Store
# - Double click file .pfx
# - Chọn "Current User"
# - Nhập password (nếu có)
# - Chọn "Automatically select the certificate store"
# - Finish

# Bước 4: Restart browser và truy cập web
```

### Tùy chọn 3: Build lại app cho .NET Framework 4.8 (Cho Dev)

**Yêu cầu kỹ thuật:**
- Rebuild Desktop Login App với .NET Framework 4.8
- .NET Framework 4.8 hỗ trợ Windows 7 SP1
- Cần sửa code do API khác biệt giữa .NET Framework và .NET 8.0

**Bước thực hiện:**

1. Tạo project mới với .NET Framework 4.8:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <OutputType>WinExe</OutputType>
  </PropertyGroup>
</Project>
```

2. Cài đặt dependencies tương thích:
```bash
dotnet add package System.Net.Http
dotnet add package Newtonsoft.Json
```

3. Sửa code không tương thích:
   - Thay `HttpClient` patterns
   - Sửa async/await patterns
   - Cập nhật certificate handling APIs

## 📋 Hướng dẫn cho User

### Bước 1: Kiểm tra phiên bản Windows

1. Nhấn phím **Windows + R**
2. Gõ: `winver`
3. Nhấn **Enter**
4. Xem phiên bản Windows

### Bước 2: Nếu là Windows 7

**Liên hệ quản trị viên IT để:**
- Nâng cấp lên Windows 10/11, HOẶC
- Nhận file chứng chỉ (.pfx) để cài đặt thủ công
- Hướng dẫn sử dụng web thay vì Desktop App

### Bước 3: Cài chứng chỉ thủ công (nếu không nâng cấp được)

1. Nhận file chứng chỉ từ admin (ví dụ: `nhanvien_A.pfx`)
2. Double-click file `.pfx`
3. Chọn **"Current User"** → Next
4. Next (giữ nguyên đường dẫn file)
5. Nhập password (nếu admin cung cấp) → Next
6. Chọn **"Automatically select the certificate store"** → Next
7. Click **Finish**
8. Thấy thông báo "The import was successful"

### Bước 4: Sử dụng trình duyệt để đăng nhập

1. Mở Chrome/Firefox
2. Truy cập: `https://pvgascng.vn`
3. Trình duyệt sẽ tự động nhận chứng chỉ
4. Đăng nhập bình thường với username/password

## 🔧 Troubleshooting

### Lỗi: "This application requires .NET 8.0"
**Nguyên nhân:** Windows 7 không hỗ trợ .NET 8.0  
**Giải pháp:** Sử dụng Tùy chọn 2 hoặc 3 ở trên

### Lỗi: "Certificate not found" trên trình duyệt
**Nguyên nhân:** Chứng chỉ chưa được import đúng  
**Giải pháp:**
1. Mở `certmgr.msc`
2. Kiểm tra: Personal → Certificates
3. Xem có chứng chỉ của user không
4. Nếu không có, import lại

### Lỗi: Chrome không nhận chứng chỉ
**Giải pháp:**
1. Restart Chrome hoàn toàn (đóng tất cả cửa sổ)
2. Hoặc restart máy
3. Hoặc thử Firefox

## 📞 Liên hệ hỗ trợ

Nếu gặp vấn đề, liên hệ IT Support với thông tin:
- Phiên bản Windows (chạy `winver`)
- Screenshot lỗi (nếu có)
- Tên đăng nhập
- Trạm làm việc

## 📚 Tài liệu tham khảo

- [.NET 8.0 Supported OS](https://github.com/dotnet/core/blob/main/release-notes/8.0/supported-os.md)
- [Windows 7 End of Life](https://support.microsoft.com/en-us/windows/windows-7-support-ended-on-january-14-2020-b75d4580-2cc7-895a-2c9c-1466d9a53962)
- [Client Certificate Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth)
