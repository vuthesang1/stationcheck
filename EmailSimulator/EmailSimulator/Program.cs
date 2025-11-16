using System;
using System.Net;
using System.Net.Mail;

namespace StationCheck.EmailSimulator
{
    /// <summary>
    /// Email simulator để test motion detection email processing
    /// Giả lập email từ Dahua NVR gửi đến hệ thống
    /// </summary>
    public class EmailSimulator
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _fromPassword;
        private readonly string _toEmail;

        public EmailSimulator(
            string smtpServer = "smtp.gmail.com",
            int smtpPort = 587,
            string fromEmail = "",
            string fromPassword = "",
            string toEmail = "")
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _fromEmail = fromEmail;
            _fromPassword = fromPassword;
            _toEmail = toEmail;
        }

        /// <summary>
        /// Gửi email giả lập motion detection từ Dahua NVR
        /// </summary>
        /// <param name="stationId">Station ID hoặc Station Code (VD: "7" hoặc "ST000001")</param>
        /// <param name="deviceName">Tên thiết bị NVR (VD: "NVR-6C39")</param>
        /// <param name="channelName">Tên kênh camera (VD: "IPC", "Camera 1")</param>
        /// <param name="ipAddress">IP của NVR (VD: "192.168.1.200")</param>
        public void SendMotionDetectionEmail(
            string stationId,
            string deviceName = "NVR-TEST",
            string channelName = "IPC-TEST",
            string ipAddress = "192.168.1.100")
        {
            if (string.IsNullOrEmpty(_fromEmail) || string.IsNullOrEmpty(_fromPassword) || string.IsNullOrEmpty(_toEmail))
            {
                Console.WriteLine("❌ ERROR: Email credentials not configured!");
                Console.WriteLine("Please set SMTP credentials in EmailSimulator constructor.");
                return;
            }

            try
            {
                var alarmTime = DateTime.Now;
                
                // Subject format: [stm] {StationId}
                var subject = $"[stm] {stationId}";
                
                // Body format: Dahua NVR alarm email
                var body = $@"Alarm Type: Motion Detection Alarm

Alarm Event: Motion Detection

Alarm Input Channel No.: 1

Alarm Input Channel Name: {channelName}

Alarm Start Time (D/M/Y H:M:S): {alarmTime:dd/MM/yyyy HH:mm:ss}

Alarm Device Name: {deviceName}

Alarm Name: 

IP Address: {ipAddress}

Alarm Details: 
Motion detection triggered at {alarmTime:HH:mm:ss}";

                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_fromEmail, _fromPassword);
                    smtpClient.Timeout = 30000; // 30 seconds

                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress(_fromEmail, deviceName);
                        message.To.Add(_toEmail);
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = false;

                        Console.WriteLine("📧 Sending test email...");
                        Console.WriteLine($"   From: {_fromEmail}");
                        Console.WriteLine($"   To: {_toEmail}");
                        Console.WriteLine($"   Subject: {subject}");
                        Console.WriteLine($"   Time: {alarmTime:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine();

                        smtpClient.Send(message);

                        Console.WriteLine("✅ Email sent successfully!");
                        Console.WriteLine($"   Station ID: {stationId}");
                        Console.WriteLine($"   Device: {deviceName}");
                        Console.WriteLine($"   Channel: {channelName}");
                        Console.WriteLine($"   IP: {ipAddress}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR sending email: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner: {ex.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// Gửi nhiều email test liên tiếp (để test rate limiting hoặc bulk processing)
        /// </summary>
        public void SendBulkEmails(string stationId, int count = 5, int delaySeconds = 2)
        {
            Console.WriteLine($"📧 Sending {count} test emails with {delaySeconds}s delay...");
            Console.WriteLine();

            for (int i = 1; i <= count; i++)
            {
                Console.WriteLine($"[{i}/{count}]");
                SendMotionDetectionEmail(
                    stationId,
                    $"NVR-TEST-{i:D2}",
                    $"Camera-{i}",
                    $"192.168.1.{100 + i}"
                );
                
                if (i < count)
                {
                    Console.WriteLine($"⏳ Waiting {delaySeconds} seconds...");
                    Console.WriteLine();
                    System.Threading.Thread.Sleep(delaySeconds * 1000);
                }
            }

            Console.WriteLine();
            Console.WriteLine($"✅ Completed sending {count} emails!");
        }

        /// <summary>
        /// Interactive mode - nhập station ID từ console
        /// </summary>
        public void RunInteractive()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   StationCheck - Email Motion Detection Simulator     ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Options:");
                Console.WriteLine("  1. Send single test email");
                Console.WriteLine("  2. Send bulk test emails (5 emails)");
                Console.WriteLine("  3. Exit");
                Console.WriteLine();
                Console.Write("Select option (1-3): ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.WriteLine();
                        Console.Write("Enter Station ID (e.g., '7' or 'ST000001'): ");
                        var stationId = Console.ReadLine();
                        
                        if (!string.IsNullOrWhiteSpace(stationId))
                        {
                            Console.WriteLine();
                            SendMotionDetectionEmail(stationId.Trim());
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid Station ID!");
                        }
                        break;

                    case "2":
                        Console.WriteLine();
                        Console.Write("Enter Station ID for bulk test: ");
                        var bulkStationId = Console.ReadLine();
                        
                        if (!string.IsNullOrWhiteSpace(bulkStationId))
                        {
                            Console.WriteLine();
                            SendBulkEmails(bulkStationId.Trim(), 5, 2);
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid Station ID!");
                        }
                        break;

                    case "3":
                        Console.WriteLine();
                        Console.WriteLine("👋 Goodbye!");
                        return;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("❌ Invalid option! Please select 1, 2, or 3.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("─────────────────────────────────────────────────────────");
                Console.WriteLine();
            }
        }
    }

    /// <summary>
    /// Program để chạy simulator standalone
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // ⚠️ IMPORTANT: Cấu hình SMTP credentials ở đây
            // Đối với Gmail:
            // 1. Bật 2-Factor Authentication
            // 2. Tạo App Password tại: https://myaccount.google.com/apppasswords
            // 3. Sử dụng App Password thay vì password thật

            var simulator = new EmailSimulator(
                smtpServer: "smtp.gmail.com",
                smtpPort: 587,
                fromEmail: "huythucson01@gmail.com",
                fromPassword: "dtfn erbe wcjg mkrg",
                toEmail: "vuthesang4@gmail.com"
            );

            // Quick test mode nếu có args
            if (args.Length > 0)
            {
                var stationId = args[0];
                Console.WriteLine($"Quick test mode: Sending email for Station {stationId}");
                Console.WriteLine();
                simulator.SendMotionDetectionEmail(stationId);
            }
            else
            {
                // Interactive mode
                simulator.RunInteractive();
            }
        }
    }
}
