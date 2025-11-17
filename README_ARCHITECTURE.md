# Station Check - Face Recognition System

## Tổng quan
Hệ thống check-in tự động sử dụng camera và nhận diện khuôn mặt để xác định nhân viên.

**Production Ready:**
- ✅ EF Core Code First với SQL Server
- ✅ Bootstrap 5 + jQuery
- ✅ Auto migration on startup
- ✅ Proper dependency injection (Scoped services)

## Kiến trúc Module

### 1. **Data Layer** (`/Data`)
- `ApplicationDbContext.cs` - EF Core DbContext
  - Auto migration on application startup
  - Seed data cho testing
  - Index optimization cho queries

### 2. **Models** (`/Models`)
Các data model của hệ thống:
- `Employee.cs` - Thông tin nhân viên
- `CheckInRecord.cs` - Bản ghi check-in
- `CameraFrame.cs` - Frame từ camera và thông tin camera
- `FaceRecognitionResult.cs` - Kết quả nhận diện khuôn mặt

### 3. **Interfaces** (`/Interfaces`)
Định nghĩa contract cho các services:
- `IEmployeeService.cs` - Quản lý nhân viên
- `INvrService.cs` - Kết nối và lấy stream từ NVR/Camera
- `IFaceRecognitionService.cs` - Nhận diện khuôn mặt
- `ICheckInService.cs` - Logic check-in

### 4. **Services** (`/Services`)
Implementation các business logic:

#### EmployeeService ✅ Production
- **Sử dụng EF Core** - Async operations với database
- CRUD operations với proper error handling
- Search và filter với database indexes
- Soft delete pattern

#### MockNvrService (Mock - Cần thay thế)
- Interface chuẩn để kết nối Dahua NVR qua HTTP API
- **TODO khi có device thật:**
  - Implement HTTP API calls theo tài liệu `DAHUA_IPC_HTTP_API`
  - RTSP stream handling
  - Snapshot API integration

#### MockFaceRecognitionService (Mock - Cần thay thế)
- Interface chuẩn để tích hợp AI model
- **Options để implement:**
  1. Azure Face API
  2. AWS Rekognition
  3. OpenCV + dlib
  4. Custom trained model

#### CheckInService ✅ Production
- **Sử dụng EF Core** - Persist check-in records
- Transaction support
- Duplicate detection
- Manual override capability

### 5. **Components** (`/Components`)
Blazor components với Bootstrap 5:

#### CameraView
- Live stream visualization
- Real-time face recognition trigger
- Bootstrap 5 styled controls

#### EmployeeList
- Bootstrap 5 table với responsive design
- Real-time search
- Filter capabilities

#### CheckInHistory
- Bootstrap 5 cards layout
- Status badges
- Date filtering

## Database Schema

### Employees Table
```sql
- Id (PK, nvarchar(50))
- FullName (nvarchar(200), required)
- Email (nvarchar(200), unique index)
- Department (nvarchar(100))
- Position (nvarchar(100))
- FaceEmbedding (varbinary(max), nullable)
- PhotoUrl (nvarchar(500))
- IsActive (bit, indexed)
- CreatedAt (datetime2, default GETUTCDATE())
```

### CheckInRecords Table
```sql
- Id (PK, nvarchar(50))
- EmployeeId (nvarchar(50), indexed)
- EmployeeName (nvarchar(200))
- CheckInTime (datetime2, indexed, default GETDATE())
- CameraId (nvarchar(50))
- CameraName (nvarchar(200))
- Confidence (float, nullable)
- SnapshotUrl (nvarchar(500))
- Status (int enum, indexed)
- Notes (nvarchar(500))
```

### Cameras Table
```sql
- Id (PK, nvarchar(50))
- Name (nvarchar(200), required)
- IpAddress (nvarchar(50), indexed)
- Port (int)
- Username (nvarchar(100))
- Password (nvarchar(100))
- StreamUrl (nvarchar(500))
- Type (int enum)
```

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StationCheckDb;..."
  },
  "FaceRecognition": {
    "ConfidenceThreshold": 0.75,
    "MaxFaceSize": 5242880
  }
}
```

## Dependency Injection

```csharp
// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Services - Scoped for proper DbContext lifecycle
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICheckInService, CheckInService>();
builder.Services.AddScoped<IFaceRecognitionService, MockFaceRecognitionService>();
builder.Services.AddSingleton<INvrService, MockNvrService>();
```

## Tech Stack

### Backend
- .NET 8.0
- Entity Framework Core 8.0
- SQL Server (LocalDB for development)
- Blazor Server

### Frontend
- Bootstrap 5.3.2
- jQuery 3.7.1
- Bootstrap Icons 1.11.1

## Workflow

1. **Kết nối camera** → NVR Service → HTTP/RTSP to Dahua camera
2. **Stream frames** → Real-time video processing
3. **Face detection** → AI model detects faces in frame
4. **Face recognition** → Compare with employee database
5. **Check-in** → Save to database with confidence score
6. **Real-time UI** → Update components via SignalR

## Development Commands

```bash
# Build
dotnet build StationCheck.csproj

# Run (auto migrate database)
dotnet run --project StationCheck.csproj

# Add migration
dotnet ef migrations add MigrationName

# Update database manually
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## Roadmap - Production Integration

### Phase 1: NVR Integration ⏳
- [ ] Implement Dahua HTTP API authentication
- [ ] RTSP stream decoding (FFmpeg.NET)
- [ ] Snapshot capture API
- [ ] Connection pooling và error recovery

### Phase 2: Face Recognition ⏳
- [ ] Select AI provider (Azure/AWS/Local)
- [ ] Train/register employee faces
- [ ] Real-time inference optimization
- [ ] Confidence threshold tuning

### Phase 3: Production Features 📋
- [ ] User authentication (ASP.NET Identity)
- [ ] Role-based authorization
- [ ] Admin dashboard
- [ ] Reports & exports (Excel, PDF)
- [ ] Email/SMS notifications
- [ ] Audit logging

### Phase 4: Scalability 🚀
- [ ] Redis caching
- [ ] Load balancing
- [ ] Docker containerization
- [ ] CI/CD pipeline
- [ ] Monitoring (Application Insights)

## URLs

- Development: https://localhost:55703
- Database: SQL Server LocalDB `StationCheckDb`

## Notes

- ✅ Production ready với proper database persistence
- ✅ Bootstrap 5 responsive design
- ✅ Auto migration eliminates manual database setup
- ⏳ Mock NVR và Face Recognition services cần replace khi có device
- 🔒 Thêm authentication/authorization trước khi deploy production


### 1. **Models** (`/Models`)
Các data model của hệ thống:
- `Employee.cs` - Thông tin nhân viên
- `CheckInRecord.cs` - Bản ghi check-in
- `CameraFrame.cs` - Frame từ camera và thông tin camera
- `FaceRecognitionResult.cs` - Kết quả nhận diện khuôn mặt

### 2. **Interfaces** (`/Interfaces`)
Định nghĩa contract cho các services:
- `IEmployeeService.cs` - Quản lý nhân viên
- `INvrService.cs` - Kết nối và lấy stream từ NVR/Camera
- `IFaceRecognitionService.cs` - Nhận diện khuôn mặt
- `ICheckInService.cs` - Logic check-in

### 3. **Services** (`/Services`)
Implementation các business logic:

#### EmployeeService
- Quản lý CRUD cho nhân viên
- Tìm kiếm và lọc nhân viên
- Hiện tại: In-memory storage
- Tương lai: Kết nối database (SQL Server, PostgreSQL, etc.)

#### MockNvrService (Implementation của INvrService)
- **Mock implementation** - Sẵn sàng để thay thế bằng integration thật
- Interface chuẩn để kết nối Dahua NVR qua HTTP API
- Các method chính:
  - `ConnectAsync()` - Kết nối tới camera/NVR
  - `StartStreamAsync()` - Bắt đầu stream video
  - `GetCurrentFrameAsync()` - Lấy frame hiện tại
- **TODO khi có device thật:**
  - Implement HTTP API calls theo tài liệu `DAHUA_IPC_HTTP_API`
  - RTSP stream handling (rtsp://username:password@ip:port/...)
  - Snapshot API: GET http://{ip}:{port}/cgi-bin/snapshot.cgi

#### MockFaceRecognitionService (Implementation của IFaceRecognitionService)
- **Mock implementation** - Random nhận diện để test UI/UX
- Interface chuẩn để tích hợp AI model
- **Options để implement thật:**
  1. **Azure Face API** - Cloud-based, dễ sử dụng
  2. **AWS Rekognition** - Cloud-based, scalable
  3. **OpenCV + dlib** - Local processing, privacy-focused
  4. **Face-api.js** - Browser-based recognition
  5. **Custom trained model** - TensorFlow/PyTorch

#### CheckInService
- Xử lý logic check-in dựa trên kết quả nhận diện
- Quản lý lịch sử check-in
- Kiểm tra duplicate check-in
- Hỗ trợ manual override

### 4. **Components** (`/Components`)
Các Blazor components cho UI:

#### CameraView
- Hiển thị live stream từ camera
- Controls: Start/Stop stream, Trigger recognition
- Hiển thị kết quả nhận diện real-time

#### EmployeeList
- Danh sách nhân viên
- Tìm kiếm và filter
- Hiển thị thông tin chi tiết

#### CheckInHistory
- Lịch sử check-in (hôm nay / tất cả)
- Filter theo nhân viên
- Hiển thị status, confidence, timestamp

## Dependency Injection Setup

```csharp
// Program.cs
builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
builder.Services.AddSingleton<INvrService, MockNvrService>();
builder.Services.AddSingleton<IFaceRecognitionService, MockFaceRecognitionService>();
builder.Services.AddSingleton<ICheckInService, CheckInService>();
```

## Workflow

1. **Kết nối camera** → NVR Service → Kết nối tới Dahua camera qua HTTP/RTSP
2. **Lấy video stream** → NVR Service → Stream frames liên tục
3. **Phát hiện khuôn mặt** → Face Recognition Service → Detect faces trong frame
4. **Nhận diện** → Face Recognition Service → So sánh với database nhân viên
5. **Check-in** → CheckIn Service → Tạo bản ghi check-in nếu confidence đủ cao
6. **Hiển thị** → UI Components → Update real-time

## Roadmap - Tích hợp device thật

### Phase 1: NVR Integration (Cần device Dahua)
- [ ] Implement HTTP API authentication
- [ ] Implement snapshot capture
- [ ] Implement RTSP stream handling
- [ ] Test với Dahua camera/NVR thật
- [ ] Handle connection errors và reconnection

### Phase 2: Face Recognition Integration (Chọn 1 solution)
- [ ] Chọn AI service/library (Azure, AWS, hoặc local)
- [ ] Implement face detection
- [ ] Implement face encoding/embedding
- [ ] Build face database từ employee photos
- [ ] Fine-tune confidence threshold
- [ ] Optimize performance

### Phase 3: Database Integration
- [ ] Setup database (SQL Server/PostgreSQL)
- [ ] Create schema cho employees, check-ins
- [ ] Replace in-memory storage
- [ ] Add proper logging

### Phase 4: Production Features
- [ ] Authentication & Authorization
- [ ] Admin dashboard
- [ ] Reports & Analytics
- [ ] Notification system
- [ ] Mobile app (optional)

## Chạy project

```bash
dotnet build
dotnet run
```

Mở browser: https://localhost:55703

## Dependencies

- .NET 8.0
- Blazor Server
- QRCoder (legacy, có thể remove)

## Notes

- Mock services sử dụng random data để demo UI/UX
- Interface design cho phép swap implementation dễ dàng khi có device thật
- Code có TODO comments đánh dấu nơi cần implement real logic
- Thread-safe cho concurrent access (sử dụng proper locking khi cần)
