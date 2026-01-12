# IIS Deployment Guide for mTLS Client Certificate Authentication

## Overview

This guide covers deploying StationCheck with mTLS (mutual TLS) client certificate authentication on Plesk IIS.

**Key Components:**
- **IIS**: Requests client certificates and forwards to ASP.NET Core
- **ASP.NET Core**: Validates certificates with custom business logic (OID, database check)
- **DeviceInstaller**: Creates self-signed certificates on client machines
- **Browser Auto-Select**: Automatically sends matching certificate without prompts

---

## Architecture

```
┌─────────────┐     HTTPS + Client Cert      ┌─────────────┐
│   Browser   │ ──────────────────────────> │     IIS     │
│             │                              │             │
│ DeviceInst. │                              │ SslFlags=   │
│ Self-Signed │                              │ SslNegotiate│
│   Certificate│                              │ Cert        │
└─────────────┘                              └──────┬──────┘
                                                     │
                                                     │ X-ARR-ClientCert
                                                     │ (base64 cert)
                                                     ▼
                                              ┌─────────────┐
                                              │ ASP.NET Core│
                                              │             │
                                              │ • Forward   │
                                              │   cert      │
                                              │ • Validate  │
                                              │   OID       │
                                              │ • Check DB  │
                                              └─────────────┘
```

**Flow:**
1. Browser sends HTTPS request with client certificate
2. IIS receives certificate, doesn't validate chain (accepts self-signed)
3. IIS forwards certificate to ASP.NET Core via `X-ARR-ClientCert` header
4. ASP.NET Core `CertificateForwarding` middleware parses certificate
5. ASP.NET Core custom validation checks:
   - Custom OID (1.3.6.1.4.1.99999.1)
   - Client Authentication EKU (1.3.6.1.5.5.7.3.2)
   - Self-signed certificate
   - Database registration and approval

---

## Prerequisites

- Windows Server with IIS 10+
- Plesk (any version)
- .NET 8 Runtime (ASP.NET Core)
- SQL Server database
- Administrator access to server

---

## Step 1: Prepare Application Files

### 1.1 Build and Publish

On development machine:

```powershell
cd D:\station-c

# Clean previous builds
dotnet clean

# Publish for production
dotnet publish -c Release -o ./publish

# Verify web.config is included
ls ./publish/web.config
```

**Expected output:**
```
Directory: D:\station-c\publish

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a----        12/2/2025   3:45 PM           4567 web.config
```

### 1.2 Package for Deployment

```powershell
# Create ZIP for deployment
Compress-Archive -Path ./publish/* -DestinationPath StationCheck-Production.zip -Force
```

---

## Step 2: Deploy to Plesk

### 2.1 Upload Files

**Option A: Plesk File Manager**
1. Login to Plesk
2. Go to: Websites & Domains → yourdomain.com → File Manager
3. Navigate to `httpdocs` (or your application root)
4. Upload `StationCheck-Production.zip`
5. Extract files

**Option B: FTP**
1. Connect via FTP to your domain
2. Upload all files from `./publish/` to `httpdocs`
3. Ensure `web.config` is in the root

### 2.2 Verify File Structure

```
httpdocs/
├── web.config                    ← Required for IIS
├── StationCheck.dll
├── appsettings.json
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── DeviceInstallerPackage/
│       └── DeviceInstaller.exe   ← Client installer
└── ... (other files)
```

---

## Step 3: Configure IIS SSL Settings

### 3.1 Run PowerShell Script (Recommended)

On the **Plesk server**, run as **Administrator**:

```powershell
# Navigate to application directory
cd D:\host-station\httpdocs  # Or your actual path

# Run configuration script
.\Configure-IIS-ClientCert.ps1 -SiteName "pvgascng.vn"

# If in a subdirectory (not root):
.\Configure-IIS-ClientCert.ps1 -SiteName "pvgascng.vn" -ApplicationPath "stationcheck"
```

**Expected output:**
```
========================================
IIS Client Certificate Configuration
========================================

Target: IIS:\Sites\pvgascng.vn

Step 1: Checking current SSL settings...
Current SSL Flags: Ssl

Step 2: Configuring SSL settings...
✓ SSL Flags set to: Ssl, SslNegotiateCert

Step 3: Verifying configuration...
New SSL Flags: Ssl, SslNegotiateCert
✓ Client certificate negotiation enabled

Step 4: Restarting application pool...
Application Pool: pvgascng.vn
✓ Application pool restarted

========================================
Configuration Complete!
========================================
```

### 3.2 Manual Configuration (If Script Fails)

If the PowerShell script fails due to locked sections:

#### Unlock Configuration Section

1. Open: `C:\Windows\System32\inetsrv\config\applicationHost.config`
2. Find:
   ```xml
   <section name="access" overrideModeDefault="Deny" />
   ```
3. Change to:
   ```xml
   <section name="access" overrideModeDefault="Allow" />
   ```
4. Save and restart IIS:
   ```powershell
   iisreset
   ```

#### Configure SSL Settings

```powershell
# Set SSL flags manually
Import-Module WebAdministration
Set-WebConfigurationProperty `
    -PSPath "IIS:\Sites\pvgascng.vn" `
    -Filter "system.webServer/security/access" `
    -Name "sslFlags" `
    -Value "Ssl,SslNegotiateCert"

# Restart app pool
Restart-WebAppPool -Name "pvgascng.vn"
```

---

## Step 4: Configure Database Connection

Edit `appsettings.json` on the server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=StationCheckDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "YourVerySecureSecretKeyForJWT_MinimumLength32Characters!",
    "Issuer": "StationCheck",
    "Audience": "StationCheckApp",
    "ExpiryMinutes": 1440
  }
}
```

**Security Note:** Use Windows Authentication or Managed Identity in production instead of SQL authentication.

---

## Step 5: Deploy DeviceInstaller

### 5.1 Build DeviceInstaller for Production

```powershell
cd D:\station-c\DeviceInstaller

# Build with production server URL
.\publish.bat

# Output: D:\station-c\wwwroot\DeviceInstallerPackage\DeviceInstaller.exe
```

Verify `DeviceInstaller.exe` is included in the deployment package at:
```
httpdocs/wwwroot/DeviceInstallerPackage/DeviceInstaller.exe
```

### 5.2 Test Download

1. Open browser: `https://pvgascng.vn/DeviceInstallerPackage/DeviceInstaller.exe`
2. File should download (154MB approx)

---

## Step 6: Test mTLS Authentication

### 6.1 Install Client Certificate

On a client machine:

1. Download `DeviceInstaller.exe` from server
2. Run as Administrator
3. Enter device name (e.g., "Office-PC-01")
4. Server URL is pre-configured: `https://pvgascng.vn`
5. Click "Đăng ký"
6. Wait for success message

**Expected output:**
```
✓ Chứng chỉ đã được tạo và lưu
✓ Chrome/Edge đã được cấu hình tự động chọn chứng chỉ
✓ Đăng ký thiết bị thành công
```

### 6.2 Verify Registry Configuration

```powershell
# Check Chrome auto-select policy
Get-ItemProperty "HKCU:\Software\Policies\Google\Chrome\AutoSelectCertificateForUrls"

# Expected:
# 1 : {"pattern":"https://pvgascng.vn","filter":{"ISSUER":{"CN":"Office-PC-01","OU":"pvgascng.vn"}}}
```

### 6.3 Test Browser Authentication

1. Close **ALL** Chrome/Edge windows
2. Reopen Chrome
3. Navigate to: `https://pvgascng.vn`
4. **Expected behavior:**
   - No certificate selection dialog
   - Certificate automatically sent
   - Login page appears (if not approved yet)
   - After admin approves, automatic login

### 6.4 Admin Approval

1. Admin logs in with username/password
2. Go to: Device Approval page
3. Approve the new device
4. User on client machine refreshes browser
5. **Expected:** Automatic login without password

---

## Step 7: Verify Server Logs

Check application logs to verify certificate forwarding:

### 7.1 View Logs in Plesk

Plesk → Websites & Domains → yourdomain.com → Logs

Or check file:
```
D:\host-station\httpdocs\Logs\webapp-YYYYMMDD.log
```

### 7.2 Expected Log Entries

**Successful certificate authentication:**
```
2025-12-02 15:30:00.123 [INF] StationEmployee login: User='employee1', Certificate Thumbprint from client='A1B2C3D4E5F6...'
2025-12-02 15:30:00.124 [INF] Device authentication successful for device: Office-PC-01
```

**Certificate not sent (issue):**
```
2025-12-02 15:30:00.123 [WRN] Login failed: StationEmployee user 'employee1' attempted login without device certificate
```

**Certificate forwarding failure (issue):**
```
2025-12-02 15:30:00.123 [WRN] Failed to parse client certificate from IIS header
```

---

## Troubleshooting

### Issue 1: HTTP 403.16 - Forbidden

**Symptom:** Browser shows "HTTP Error 403.16 - Forbidden" when accessing site.

**Cause:** IIS is set to "Require" client certificates and validates chain, rejecting self-signed certs.

**Solution:**
```powershell
# Change from "Require" to "Accept"
Set-WebConfigurationProperty `
    -PSPath "IIS:\Sites\pvgascng.vn" `
    -Filter "system.webServer/security/access" `
    -Name "sslFlags" `
    -Value "Ssl,SslNegotiateCert"  # NOT "SslRequireCert"
```

### Issue 2: Browser Not Sending Certificate

**Symptom:** Server log shows "attempted login without device certificate".

**Causes & Solutions:**

**A. IIS not requesting certificate**
```powershell
# Verify SSL flags
Get-WebConfigurationProperty `
    -PSPath "IIS:\Sites\pvgascng.vn" `
    -Filter "system.webServer/security/access" `
    -Name "sslFlags"

# Should show: Ssl, SslNegotiateCert
```

**B. Certificate not installed on client**
```powershell
# Check certificate store
Get-ChildItem Cert:\CurrentUser\My | Where-Object {
    $_.Subject -like "*pvgascng.vn*"
}
```

**C. Browser not restarted after DeviceInstaller**
- Close ALL Chrome/Edge windows
- Reopen browser

**D. Registry policy not configured**
```powershell
# Check registry
Get-ItemProperty "HKCU:\Software\Policies\Google\Chrome\AutoSelectCertificateForUrls"
```

### Issue 3: Certificate Forwarding Not Working

**Symptom:** ASP.NET Core receives null certificate despite IIS requesting it.

**Solution:**

Verify `web.config` has certificate forwarding enabled:
```xml
<environmentVariables>
  <environmentVariable name="ASPNETCORE_FORWARDCLIENTCERTIFICATE" value="true" />
</environmentVariables>
```

Check `Program.cs` has certificate forwarding middleware:
```csharp
// After app.UseRouting()
app.UseCertificateForwarding();
```

### Issue 4: Application Not Starting

**Symptom:** HTTP 500 error or blank page.

**Solutions:**

**A. Check .NET Runtime**
```powershell
# Verify .NET 8 Runtime installed
dotnet --list-runtimes

# Should show: Microsoft.AspNetCore.App 8.0.x
```

**B. Enable detailed errors in `web.config`**
```xml
<aspNetCore ... stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout">
```

Then check: `D:\host-station\httpdocs\logs\stdout_*.log`

**C. Check application pool**
- Plesk → Websites & Domains → Application Pool
- Ensure: .NET CLR Version = "No Managed Code" (for .NET Core)
- Managed Pipeline Mode = "Integrated"

### Issue 5: Database Connection Fails

**Symptom:** Application starts but crashes on database access.

**Solution:**

1. Verify connection string in `appsettings.json`
2. Test SQL connectivity from server:
   ```powershell
   Test-NetConnection -ComputerName YOUR_SQL_SERVER -Port 1433
   ```
3. Check SQL Server allows remote connections
4. Verify SQL login credentials
5. Check firewall rules

### Issue 6: DeviceInstaller Download Not Working

**Symptom:** 404 error when downloading DeviceInstaller.exe.

**Solutions:**

**A. Verify file exists**
```powershell
Test-Path "D:\host-station\httpdocs\wwwroot\DeviceInstallerPackage\DeviceInstaller.exe"
```

**B. Check IIS MIME type**
```powershell
# Add .exe MIME type if needed
Add-WebConfigurationProperty `
    -PSPath "IIS:\Sites\pvgascng.vn" `
    -Filter "system.webServer/staticContent" `
    -Name "." `
    -Value @{fileExtension='.exe'; mimeType='application/octet-stream'}
```

---

## Security Considerations

### 1. HTTPS Only

Ensure site is HTTPS-only:
- Valid SSL certificate installed
- HTTP to HTTPS redirect enabled
- HSTS header configured

### 2. Certificate Validation

ASP.NET Core validates:
- Custom OID (1.3.6.1.4.1.99999.1) - DeviceInstaller marker
- Client Authentication EKU (1.3.6.1.5.5.7.3.2)
- Self-signed certificate (ISSUER = SUBJECT)
- Device registered in database
- Device approved by admin

### 3. Database Security

- Use Windows Authentication for SQL Server
- Enable SQL Server encryption
- Regular database backups
- Audit logging enabled

### 4. Network Security

- Firewall rules: Allow HTTPS (443) only
- No direct SQL Server access from internet
- VPN for admin access

---

## Maintenance

### Update Application

```powershell
# 1. Build new version
dotnet publish -c Release -o ./publish

# 2. Stop application pool in Plesk
Stop-WebAppPool -Name "pvgascng.vn"

# 3. Upload new files (overwrite existing)
# Via FTP or Plesk File Manager

# 4. Start application pool
Start-WebAppPool -Name "pvgascng.vn"
```

### Update DeviceInstaller

```powershell
# 1. Rebuild DeviceInstaller
cd D:\station-c\DeviceInstaller
.\publish.bat

# 2. Upload new DeviceInstaller.exe to:
# httpdocs/wwwroot/DeviceInstallerPackage/DeviceInstaller.exe

# 3. Notify users to download and reinstall
```

### View Logs

```powershell
# Real-time log viewing
Get-Content "D:\host-station\httpdocs\Logs\webapp-*.log" -Wait -Tail 50

# Search for errors
Get-Content "D:\host-station\httpdocs\Logs\webapp-*.log" | Select-String "ERROR"
```

---

## Performance Tuning

### IIS Settings

```powershell
# Increase connection timeout
Set-WebConfigurationProperty `
    -PSPath "IIS:\Sites\pvgascng.vn" `
    -Filter "system.webServer/asp" `
    -Name "limits.requestTimeout" `
    -Value "00:05:00"

# Enable compression
Set-WebConfigurationProperty `
    -PSPath "IIS:\Sites\pvgascng.vn" `
    -Filter "system.webServer/httpCompression" `
    -Name "doDynamicCompression" `
    -Value $true
```

### Application Pool

- Dedicated app pool for StationCheck
- Recycling: Time-based (e.g., 2 AM daily)
- Idle timeout: 20 minutes (default)
- Maximum worker processes: 1 (default for non-web-farm)

---

## Monitoring

### Key Metrics

1. **Application Pool Status**
   - Should be "Running"
   - Monitor recycling frequency

2. **Certificate Authentication Rate**
   - Check logs for "Certificate Thumbprint from client" entries
   - Monitor "attempted login without device certificate" warnings

3. **Database Performance**
   - Query execution times
   - Connection pool usage

4. **Disk Space**
   - Log file growth
   - Database size

### Alerts

Set up monitoring for:
- Application pool stopped
- High error rate in logs
- Database connection failures
- Disk space < 20%

---

## Support

### Logs to Collect

When reporting issues:
1. Application logs: `Logs/webapp-YYYYMMDD.log`
2. IIS stdout logs: `logs/stdout_*.log`
3. Windows Event Viewer: Application logs
4. Client certificate details:
   ```powershell
   Get-ChildItem Cert:\CurrentUser\My | Where-Object {
       $_.Subject -like "*pvgascng.vn*"
   } | Format-List Subject, Issuer, Thumbprint, NotBefore, NotAfter
   ```

### Diagnostic Commands

```powershell
# Check IIS SSL settings
Get-WebConfigurationProperty -PSPath "IIS:\Sites\pvgascng.vn" -Filter "system.webServer/security/access" -Name "sslFlags"

# Check application pool status
Get-WebAppPoolState -Name "pvgascng.vn"

# Test database connection
Test-NetConnection -ComputerName YOUR_SQL_SERVER -Port 1433

# View recent errors
Get-Content "Logs\webapp-*.log" -Tail 100 | Select-String "ERROR|WARN"
```

---

## Appendix: web.config Reference

Complete `web.config` for StationCheck:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      
      <aspNetCore processPath="dotnet" 
                  arguments=".\StationCheck.dll" 
                  stdoutLogEnabled="false" 
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="inprocess"
                  forwardWindowsAuthToken="false">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_FORWARDCLIENTCERTIFICATE" value="true" />
        </environmentVariables>
      </aspNetCore>
      
      <modules>
        <remove name="WebDAVModule" />
      </modules>
    </system.webServer>
  </location>
</configuration>
```

---

## Conclusion

After completing this guide:
- ✅ IIS requests client certificates without validating chain
- ✅ ASP.NET Core receives certificates via forwarding
- ✅ Custom validation logic checks DeviceInstaller certs
- ✅ Browser automatically sends matching certificate
- ✅ Users authenticate without passwords

**Key Points:**
- Use `SslNegotiateCert` (not `SslRequireCert`) to accept self-signed certs
- Certificate forwarding enabled via `ASPNETCORE_FORWARDCLIENTCERTIFICATE`
- ASP.NET Core validates custom OID and database registration
- Browser auto-select requires registry configuration + browser restart
