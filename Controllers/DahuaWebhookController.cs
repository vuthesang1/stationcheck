using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace StationCheck.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DahuaWebhookController : ControllerBase
    {
        private readonly ILogger<DahuaWebhookController> _logger;
        private readonly string _logDirectory;

        public DahuaWebhookController(ILogger<DahuaWebhookController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _logDirectory = Path.Combine(env.ContentRootPath, "Logs", "DahuaWebhooks");
            
            // Tạo thư mục logs nếu chưa tồn tại
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        /// <summary>
        /// Nhận webhook từ đầu ghi Dahua qua HTTP Push
        /// Endpoint: POST /api/dahuawebhook/event
        /// </summary>
        [HttpPost("event")]
        public async Task<IActionResult> ReceiveEvent()
        {
            try
            {
                var timestamp = DateTime.Now;
                var fileName = $"dahua_event_{timestamp:yyyyMMdd_HHmmss_fff}.txt";
                var filePath = Path.Combine(_logDirectory, fileName);

                var sb = new StringBuilder();
                sb.AppendLine("=" .PadRight(80, '='));
                sb.AppendLine($"DAHUA WEBHOOK EVENT - {timestamp:yyyy-MM-dd HH:mm:ss.fff}");
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine();

                // 1. Log Request Headers
                sb.AppendLine("--- REQUEST HEADERS ---");
                foreach (var header in Request.Headers)
                {
                    sb.AppendLine($"{header.Key}: {header.Value}");
                }
                sb.AppendLine();

                // 2. Log Query String
                if (Request.QueryString.HasValue)
                {
                    sb.AppendLine("--- QUERY STRING ---");
                    sb.AppendLine(Request.QueryString.Value);
                    foreach (var query in Request.Query)
                    {
                        sb.AppendLine($"{query.Key} = {query.Value}");
                    }
                    sb.AppendLine();
                }

                // 3. Log Request Body (dynamic content)
                sb.AppendLine("--- REQUEST BODY ---");
                sb.AppendLine($"Content-Type: {Request.ContentType}");
                sb.AppendLine($"Content-Length: {Request.ContentLength}");
                sb.AppendLine();

                // Đọc body dưới dạng raw text
                Request.EnableBuffering(); // Cho phép đọc body nhiều lần
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    var bodyContent = await reader.ReadToEndAsync();
                    Request.Body.Position = 0; // Reset stream position

                    if (!string.IsNullOrWhiteSpace(bodyContent))
                    {
                        sb.AppendLine("Raw Body:");
                        sb.AppendLine(bodyContent);
                        sb.AppendLine();

                        // Thử parse JSON nếu có thể
                        if (Request.ContentType?.Contains("application/json") == true)
                        {
                            try
                            {
                                var jsonDoc = JsonDocument.Parse(bodyContent);
                                sb.AppendLine("Parsed JSON (Pretty Print):");
                                sb.AppendLine(JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions 
                                { 
                                    WriteIndented = true 
                                }));
                                sb.AppendLine();
                            }
                            catch (JsonException)
                            {
                                sb.AppendLine("(Body is not valid JSON)");
                                sb.AppendLine();
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine("(Empty body)");
                        sb.AppendLine();
                    }
                }

                // 4. Log Form Data (nếu có)
                if (Request.HasFormContentType)
                {
                    sb.AppendLine("--- FORM DATA ---");
                    foreach (var form in Request.Form)
                    {
                        sb.AppendLine($"{form.Key} = {form.Value}");
                    }
                    
                    if (Request.Form.Files.Count > 0)
                    {
                        sb.AppendLine();
                        sb.AppendLine("--- UPLOADED FILES ---");
                        foreach (var file in Request.Form.Files)
                        {
                            sb.AppendLine($"Name: {file.Name}");
                            sb.AppendLine($"FileName: {file.FileName}");
                            sb.AppendLine($"ContentType: {file.ContentType}");
                            sb.AppendLine($"Length: {file.Length} bytes");
                            sb.AppendLine();
                        }
                    }
                    sb.AppendLine();
                }

                // 5. Log Connection Info
                sb.AppendLine("--- CONNECTION INFO ---");
                sb.AppendLine($"Remote IP: {Request.HttpContext.Connection.RemoteIpAddress}");
                sb.AppendLine($"Remote Port: {Request.HttpContext.Connection.RemotePort}");
                sb.AppendLine($"Local IP: {Request.HttpContext.Connection.LocalIpAddress}");
                sb.AppendLine($"Local Port: {Request.HttpContext.Connection.LocalPort}");
                sb.AppendLine($"Protocol: {Request.Protocol}");
                sb.AppendLine($"Method: {Request.Method}");
                sb.AppendLine($"Path: {Request.Path}");
                sb.AppendLine();

                sb.AppendLine("=".PadRight(80, '='));

                // Ghi vào file
                await System.IO.File.WriteAllTextAsync(filePath, sb.ToString());

                // Log vào console
                _logger.LogInformation($"Dahua webhook received and logged to: {fileName}");

                // Trả về response thành công
                return Ok(new
                {
                    success = true,
                    message = "Event received successfully",
                    timestamp = timestamp,
                    logFile = fileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Dahua webhook");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Endpoint GET để test xem API có hoạt động không
        /// Endpoint: GET /api/dahuawebhook/test
        /// </summary>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                status = "API is running",
                endpoint = "/api/dahuawebhook/event",
                method = "POST",
                logDirectory = _logDirectory,
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Lấy danh sách các file log đã ghi
        /// Endpoint: GET /api/dahuawebhook/logs
        /// </summary>
        [HttpGet("logs")]
        public IActionResult GetLogs([FromQuery] int limit = 20)
        {
            try
            {
                var files = Directory.GetFiles(_logDirectory, "dahua_event_*.txt")
                    .OrderByDescending(f => System.IO.File.GetCreationTime(f))
                    .Take(limit)
                    .Select(f => new
                    {
                        fileName = Path.GetFileName(f),
                        createdAt = System.IO.File.GetCreationTime(f),
                        size = new FileInfo(f).Length
                    })
                    .ToList();

                return Ok(new
                {
                    total = files.Count,
                    logs = files
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving logs");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Đọc nội dung của một file log cụ thể
        /// Endpoint: GET /api/dahuawebhook/logs/{fileName}
        /// </summary>
        [HttpGet("logs/{fileName}")]
        public async Task<IActionResult> GetLogContent(string fileName)
        {
            try
            {
                // Validate filename để tránh path traversal
                if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                {
                    return BadRequest("Invalid file name");
                }

                var filePath = Path.Combine(_logDirectory, fileName);
                
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"Log file not found: {fileName}");
                }

                var content = await System.IO.File.ReadAllTextAsync(filePath);
                
                return Ok(new
                {
                    fileName = fileName,
                    content = content,
                    createdAt = System.IO.File.GetCreationTime(filePath)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading log file");
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}
