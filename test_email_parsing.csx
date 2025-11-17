using Microsoft.EntityFrameworkCore;
using StationCheck.Data;
using StationCheck.Services;

// Test Email Parsing
var testEmailSubject = "ST000001 - Alarm Notification";
var testEmailBody = @"Alarm Event: Motion Detection
Alarm Input Channel No.: 2
Alarm Input Channel Name: IPC
Alarm Start Time (D/M/Y H:M:S): 12/11/2025 16:03:57
Alarm Device Name: NVR-6C39
Alarm Name:
IP Address: 192.168.1.200
Alarm Details:
";
var testEmailFrom = "nvr@monitoring.local";
var testReceivedAt = DateTime.Now;

Console.WriteLine("=== Testing Email Parsing ===");
Console.WriteLine($"Subject: {testEmailSubject}");
Console.WriteLine($"From: {testEmailFrom}");
Console.WriteLine($"Received: {testReceivedAt}");
Console.WriteLine();
Console.WriteLine("Email Body:");
Console.WriteLine(testEmailBody);
Console.WriteLine();

// Create service instances (for testing purposes)
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer("Server=.\\SQLEXPRESS;Database=StationCheckDb;Trusted_Connection=true;TrustServerCertificate=true;")
    .Options;

using var context = new ApplicationDbContext(options);
var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<EmailService>.Instance;
var emailService = new EmailService(context, logger);

// Parse email
var emailEvent = await emailService.ParseEmailToEventAsync(
    testEmailSubject, 
    testEmailBody, 
    testEmailFrom, 
    testReceivedAt
);

Console.WriteLine("=== Parsed Result ===");
Console.WriteLine($"StationCode: {emailEvent.StationCode}");
Console.WriteLine($"StationId: {emailEvent.StationId}");
Console.WriteLine($"AlarmEvent: {emailEvent.AlarmEvent}");
Console.WriteLine($"AlarmInputChannelNo: {emailEvent.AlarmInputChannelNo}");
Console.WriteLine($"AlarmInputChannelName: {emailEvent.AlarmInputChannelName}");
Console.WriteLine($"AlarmStartTime: {emailEvent.AlarmStartTime}");
Console.WriteLine($"AlarmDeviceName: {emailEvent.AlarmDeviceName}");
Console.WriteLine($"AlarmName: {emailEvent.AlarmName ?? "(empty)"}");
Console.WriteLine($"IpAddress: {emailEvent.IpAddress}");
Console.WriteLine($"AlarmDetails: {emailEvent.AlarmDetails ?? "(empty)"}");
Console.WriteLine();

// Save to database
Console.WriteLine("Saving to database...");
await emailService.SaveEmailEventAsync(emailEvent);
Console.WriteLine($"✓ Saved EmailEvent with ID: {emailEvent.Id}");
