# Device Registration System - Quick Testing Guide

## Prerequisites
- Application running on localhost (HTTPS: 55703, HTTP: 55704)
- Test user account: username=`testuser`, password=`testuser123`
- Admin account: username=`admin`, password=`admin123`

## Test Scenario 1: User Device Registration

### Step 1: Download Installer
1. Navigate to http://localhost:55704/devices (or https://localhost:55703/devices)
2. Authenticate as testuser/testuser123
3. Click "Download DeviceInstaller.exe" button
4. Save the executable

### Step 2: Generate Certificate
1. Run DeviceInstaller.exe
2. Enter Device Name: "MyTestDevice"
3. Enter Server URL: http://localhost:55704
4. Paste JWT token (from login response)
5. Click "Generate Certificate" button
6. Review certificate details displayed
7. Click "Register Device"
8. Note the Device ID returned

### Expected Results
✓ Certificate generated and stored in Windows certificate store
✓ Device registration shows success message
✓ Device appears in pending status on user's device page

---

## Test Scenario 2: Admin Approval

### Step 1: View Pending Devices
1. Authenticate as admin/admin123
2. Navigate to /device-approval page
3. View list of pending devices
4. Locate the device registered in Scenario 1

### Step 2: Approve Device
1. Click "Approve" button on the device
2. Confirm the action
3. See success message

### Step 3: Verify Approval
1. Navigate to /devices (as testuser)
2. Verify device now shows "Approved" status
3. View certificate details

### Expected Results
✓ Device marked as Approved
✓ User auto-assigned to device
✓ Audit log entry created
✓ Device count increases in admin dashboard

---

## Test Scenario 3: Admin Dashboard

### Step 1: Check Device Count
1. Authenticate as admin
2. Navigate to /users page
3. View user management grid
4. Check new "Số Thiết Bị" (Device Count) column
5. Verify testuser shows count=1 (or higher if multiple devices)

### Step 2: Device Management
1. Click on user row to edit
2. View other user details
3. See device count for each user

### Expected Results
✓ Device count column displays correctly
✓ Counts match actual approved devices
✓ Column updates when devices are approved/revoked

---

## Test Scenario 4: Device Revocation

### Step 1: Revoke Device (as User)
1. Navigate to /devices (as testuser)
2. Find the approved device
3. Click "Revoke" button
4. Confirm revocation

### Step 2: Verify Revocation
1. Device status changes to "Revoked"
2. Check admin dashboard - device count decreases
3. Admin cannot see device in device-assignments page

### Expected Results
✓ Device marked as Revoked
✓ User cannot use device
✓ Audit log entry created
✓ Statistics update

---

## Test Scenario 5: Admin Device Assignment

### Step 1: Assign Device to Another User
1. Authenticate as admin
2. Navigate to /device-assignments page
3. Find the approved device
4. Click "Assign to User"
5. Select another user from dropdown
6. Confirm assignment

### Step 2: Verify Assignment
1. Check list of assigned users
2. Device now appears accessible to both users
3. Audit log shows UserAssigned action

### Expected Results
✓ Device assigned to multiple users
✓ All assigned users can access device
✓ Owner user always has access
✓ Cannot assign to already-assigned users

---

## API Testing with PowerShell

### Login as User
```powershell
$response = Invoke-WebRequest -Uri "http://localhost:55704/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body (@{Username="testuser"; Password="testuser123"} | ConvertTo-Json)

$token = ($response.Content | ConvertFrom-Json).token
```

### Register Device
```powershell
$device = @{
    DeviceName = "TestDevice"
    CertificateThumbprint = "1234567890ABCDEF1234567890ABCDEF12345678"
    CertificateSubject = "CN=TestDevice"
    CertificateIssuer = "CN=TestCA"
    CertificateValidFrom = (Get-Date).ToUniversalTime().ToString("o")
    CertificateValidTo = (Get-Date).AddYears(1).ToUniversalTime().ToString("o")
    EKUOids = "1.3.6.1.5.5.7.3.2"
}

$response = Invoke-WebRequest -Uri "http://localhost:55704/api/auth/register-device" `
    -Method Post `
    -ContentType "application/json" `
    -Headers @{"Authorization" = "Bearer $token"} `
    -Body ($device | ConvertTo-Json)

$deviceData = $response.Content | ConvertFrom-Json
$deviceId = $deviceData.deviceId
```

### Get Pending Devices (Admin)
```powershell
$adminResponse = Invoke-WebRequest -Uri "http://localhost:55704/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body (@{Username="admin"; Password="admin123"} | ConvertTo-Json)

$adminToken = ($adminResponse.Content | ConvertFrom-Json).token

$pending = Invoke-WebRequest -Uri "http://localhost:55704/api/auth/pending-devices" `
    -Method Get `
    -Headers @{"Authorization" = "Bearer $adminToken"}

$pending.Content | ConvertFrom-Json
```

### Approve Device (Admin)
```powershell
$approve = Invoke-WebRequest -Uri "http://localhost:55704/api/auth/approve-device/$deviceId" `
    -Method Post `
    -Headers @{"Authorization" = "Bearer $adminToken"} `
    -ContentType "application/json"
```

### Get Device Counts (Admin)
```powershell
$counts = Invoke-WebRequest -Uri "http://localhost:55704/api/auth/user-device-counts" `
    -Method Get `
    -Headers @{"Authorization" = "Bearer $adminToken"}

$counts.Content | ConvertFrom-Json
```

---

## Verification Checklist

### Database
- [ ] UserDevice records created
- [ ] DeviceUserAssignment records created
- [ ] DeviceCertificateAuditLog entries created
- [ ] IsDeleted flag working (soft deletes)

### API
- [ ] All 10 endpoints responding
- [ ] Authorization working (401 for missing token)
- [ ] Role-based access (403 for non-admin on admin endpoints)
- [ ] Validation errors returned properly

### UI
- [ ] DeviceInstaller.exe downloads successfully
- [ ] Device Registration page loads
- [ ] Device Approval page shows pending devices
- [ ] User Management grid shows device count
- [ ] Device Assignment page works

### Security
- [ ] Certificates validated properly
- [ ] EKU validation enforced
- [ ] Thumbprint format validated
- [ ] JWT token required for registration
- [ ] Admin-only endpoints protected

### Audit
- [ ] Actions logged to DeviceCertificateAuditLog
- [ ] User ID recorded
- [ ] Timestamp recorded
- [ ] IP address captured
- [ ] Action type set correctly

---

## Common Issues & Solutions

### DeviceInstaller.exe Not Found
**Solution:** Verify file exists at `d:\station-c\wwwroot\DeviceInstaller.exe`

### Certificate Not Generating
**Solution:** 
- Ensure Windows Forms SDK is installed
- Check System.Security.Cryptography package
- Verify Windows certificate store is accessible

### Device Registration Failed
**Solution:**
- Verify JWT token is valid and not expired
- Check certificate thumbprint format (40 hex chars)
- Verify server URL is correct
- Check firewall allows connection

### API Endpoints Return 401
**Solution:**
- Verify Bearer token is valid
- Check token has not expired
- Ensure Authorization header format: "Bearer <token>"

### Admin Endpoints Return 403
**Solution:**
- Verify user has Admin role
- Check authentication is successful
- Verify token contains correct role claim

---

## Performance Testing

### Load Test
1. Register 100 devices
2. Approve all devices
3. Query /api/auth/user-device-counts
4. Measure response time (should be <500ms)

### Stress Test
1. Rapid API calls to register-device
2. Verify duplicate prevention works
3. Check database consistency

### Database
1. Check device-related table sizes
2. Verify indexes are used
3. Monitor query performance

---

## Documentation References

- **Implementation Summary:** DEVICE_REGISTRATION_SUMMARY.md
- **Complete Guide:** IMPLEMENTATION_COMPLETE.md
- **Database Schema:** See Models/ directory
- **API Design:** AuthController.cs
- **UI Components:** Pages/Device*.razor
