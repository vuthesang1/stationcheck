using System.Text.RegularExpressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MimeKit;
using StationCheck.Data;
using StationCheck.Interfaces;
using StationCheck.Models;

namespace StationCheck.Services
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(
            ApplicationDbContext context, 
            ILogger<EmailService> logger,
            IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public async Task<List<MotionEvent>> CheckAndProcessNewEmailsAsync()
        {
            _logger.LogInformation("[EmailService] Checking for new emails...");
            
            var processedEvents = new List<MotionEvent>();
            
            try
            {
                if (string.IsNullOrEmpty(_emailSettings.EmailAddress) || 
                    string.IsNullOrEmpty(_emailSettings.Password))
                {
                    _logger.LogWarning("[EmailService] Email settings not configured. Skipping email check.");
                    return processedEvents;
                }

                using var client = new ImapClient();
                
                // Connect to IMAP server
                _logger.LogInformation("[EmailService] Connecting to {Server}:{Port}", _emailSettings.ImapServer, _emailSettings.ImapPort);
                await client.ConnectAsync(_emailSettings.ImapServer, _emailSettings.ImapPort, _emailSettings.UseSsl);
                
                // Authenticate
                await client.AuthenticateAsync(_emailSettings.EmailAddress, _emailSettings.Password);
                _logger.LogInformation("[EmailService] Successfully authenticated as {Email}", _emailSettings.EmailAddress);
                
                // Search in ALL MAIL folder (includes all labels/folders)
                var allMail = client.GetFolder(SpecialFolder.All);
                await allMail.OpenAsync(FolderAccess.ReadWrite);
                _logger.LogInformation("[EmailService] Opened All Mail folder");
                
                // Search for UNREAD emails containing StationCode pattern
                var searchQuery = SearchQuery.SubjectContains("ST").And(SearchQuery.NotSeen);
                var uids = await allMail.SearchAsync(searchQuery);
                _logger.LogInformation("[EmailService] Found {Count} UNREAD email(s) with 'ST' in subject", uids.Count);
                
                foreach (var uid in uids)
                {
                    try
                    {
                        var message = await allMail.GetMessageAsync(uid);
                        
                        // Use MessageId for duplicate checking (more reliable than subject)
                        var messageId = message.MessageId;
                        var emailSubject = message.Subject ?? string.Empty;
                        var emailFrom = message.From.ToString();
                        var receivedAt = message.Date.DateTime;
                        
                        // Check if MotionEvent with this MessageId already exists
                        var existingMotionEvent = await _context.MotionEvents
                            .Where(e => e.EmailMessageId == messageId)
                            .FirstOrDefaultAsync();
                        
                        if (existingMotionEvent != null)
                        {
                            _logger.LogDebug("[EmailService] Email already processed: MessageId='{MessageId}'", messageId);
                            continue; // Skip this email
                        }
                        
                        _logger.LogInformation("[EmailService] Processing NEW email: Subject='{Subject}', From='{From}', MessageId='{MessageId}'", 
                            emailSubject, emailFrom, messageId);
                        
                        // Parse email to MotionEvent
                        var motionEvent = await ParseEmailToMotionEventAsync(
                            emailSubject,
                            message.TextBody ?? message.HtmlBody ?? string.Empty,
                            emailFrom,
                            receivedAt,
                            messageId  // ✅ Pass MessageId
                        );
                        
                        if (motionEvent != null)
                        {
                            // Save MotionEvent
                            await SaveMotionEventAsync(motionEvent);
                            processedEvents.Add(motionEvent);
                            
                            _logger.LogInformation("[EmailService] Successfully processed email UID {Uid} - Saved to MotionEvents", uid);
                            
                            // Mark as read if configured
                            if (_emailSettings.MarkAsRead)
                            {
                                await allMail.AddFlagsAsync(uid, MessageFlags.Seen, true);
                            }
                            
                            // Delete if configured
                            if (_emailSettings.DeleteAfterProcessing)
                            {
                                await allMail.AddFlagsAsync(uid, MessageFlags.Deleted, true);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("[EmailService] Failed to parse email UID {Uid}", uid);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[EmailService] Error processing email UID {Uid}", uid);
                    }
                }
                
                // Expunge deleted messages if any
                if (_emailSettings.DeleteAfterProcessing && processedEvents.Any())
                {
                    await allMail.ExpungeAsync();
                }
                
                // Disconnect
                await client.DisconnectAsync(true);
                
                _logger.LogInformation("[EmailService] Processed {Count} email(s) successfully", processedEvents.Count);
                
                return processedEvents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EmailService] Error checking emails");
                return processedEvents;
            }
        }

        public async Task<MotionEvent?> ParseEmailToMotionEventAsync(string subject, string body, string from, DateTime receivedAt, string messageId)
        {
            try
            {
                // Extract StationCode from subject
                // Format: Subject contains StationCode directly (e.g., "ST000001", "ST000002")
                // No [stm] prefix anymore
                Guid? stationId = null;
                string? stationCode = null;
                
                // Extract StationCode pattern: ST + 6 digits
                var subjectTrimmed = (subject ?? "").Trim();
                var stationCodeMatch = Regex.Match(subjectTrimmed, @"ST\d{6}");
                
                if (stationCodeMatch.Success)
                {
                    stationCode = stationCodeMatch.Value;
                    var station = await _context.Stations
                        .FirstOrDefaultAsync(s => s.StationCode == stationCode);
                    
                    if (station != null)
                    {
                        stationId = station.Id;
                        _logger.LogInformation("[EmailService] Found station by code: {StationCode} -> StationId={StationId}", stationCode, stationId);
                    }
                    else
                    {
                        _logger.LogWarning("[EmailService] Station not found for code: {StationCode}", stationCode);
                    }
                }
                else
                {
                    _logger.LogWarning("[EmailService] Could not extract StationCode from subject: {Subject}", subject);
                }

                if (!stationId.HasValue)
                {
                    _logger.LogWarning("[EmailService] Could not extract StationId from subject: {Subject}", subject);
                    return null;
                }

                // Parse alarm details from body
                var alarmDetails = new Dictionary<string, object?>();

                if (body != null)
                {
                    // Alarm Event: Motion Detection
                    var alarmEventMatch = Regex.Match(body, @"Alarm Event:\s*(.+?)(?:\r?\n|$)");
                    if (alarmEventMatch.Success)
                    {
                        alarmDetails["AlarmEvent"] = alarmEventMatch.Groups[1].Value.Trim();
                    }

                    // Alarm Input Channel No.: 2
                    var channelNoMatch = Regex.Match(body, @"Alarm Input Channel No\.:\s*(\d+)");
                    if (channelNoMatch.Success && int.TryParse(channelNoMatch.Groups[1].Value, out int channelNo))
                    {
                        alarmDetails["AlarmInputChannelNo"] = channelNo;
                    }

                    // Alarm Input Channel Name: IPC
                    var channelNameMatch = Regex.Match(body, @"Alarm Input Channel Name:\s*(.+?)(?:\r?\n|$)");
                    if (channelNameMatch.Success)
                    {
                        alarmDetails["AlarmInputChannelName"] = channelNameMatch.Groups[1].Value.Trim();
                    }

                    // Alarm Start Time (D/M/Y H:M:S): 12/11/2025 16:03:57
                    // ⚠️ Time from email is UTC+7 (Vietnam timezone), need to convert to UTC for database
                    var startTimeMatch = Regex.Match(body, @"Alarm Start Time \(D/M/Y H:M:S\):\s*(.+?)(?:\r?\n|$)");
                    if (startTimeMatch.Success)
                    {
                        if (DateTime.TryParseExact(
                            startTimeMatch.Groups[1].Value.Trim(),
                            new[] { "dd/MM/yyyy HH:mm:ss", "d/M/yyyy H:m:s" },
                            null,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime alarmStartTime))
                        {
                            // Convert from UTC+7 to UTC by subtracting 7 hours
                            var alarmStartTimeUtc = alarmStartTime.AddHours(-7);
                            alarmDetails["AlarmStartTime"] = alarmStartTimeUtc;
                        }
                    }

                    // Alarm Device Name: NVR-6C39
                    var deviceNameMatch = Regex.Match(body, @"Alarm Device Name:\s*(.+?)(?:\r?\n|$)");
                    if (deviceNameMatch.Success)
                    {
                        alarmDetails["AlarmDeviceName"] = deviceNameMatch.Groups[1].Value.Trim();
                    }

                    // Alarm Name:
                    var alarmNameMatch = Regex.Match(body, @"Alarm Name:\s*(.+?)(?:\r?\n|$)");
                    if (alarmNameMatch.Success)
                    {
                        var name = alarmNameMatch.Groups[1].Value.Trim();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            alarmDetails["AlarmName"] = name;
                        }
                    }

                    // IP Address: 192.168.1.200
                    var ipMatch = Regex.Match(body, @"IP Address:\s*(.+?)(?:\r?\n|$)");
                    if (ipMatch.Success)
                    {
                        alarmDetails["IpAddress"] = ipMatch.Groups[1].Value.Trim();
                    }

                    // Alarm Details:
                    var detailsMatch = Regex.Match(body, @"Alarm Details:\s*(.+?)(?:$)", RegexOptions.Singleline);
                    if (detailsMatch.Success)
                    {
                        var details = detailsMatch.Groups[1].Value.Trim();
                        if (!string.IsNullOrWhiteSpace(details))
                        {
                            alarmDetails["AlarmDetails"] = details;
                        }
                    }
                }

                // Add email metadata
                alarmDetails["EmailSubject"] = subject;
                alarmDetails["EmailFrom"] = from;
                alarmDetails["EmailReceivedAt"] = receivedAt;

                // Generate CameraId from Station info (required field in database)
                // Format: STATION_{StationId}_EMAIL or use AlarmInputChannelName if available
                string cameraId;
                string? cameraName = null;
                
                if (alarmDetails.ContainsKey("AlarmInputChannelName"))
                {
                    // Sanitize camera name: trim, remove newlines, limit length to avoid truncation
                    var rawCameraName = alarmDetails["AlarmInputChannelName"]?.ToString() ?? "";
                    cameraName = rawCameraName
                        .Replace("\r", "")
                        .Replace("\n", "")
                        .Trim();
                    
                    // Limit camera name to reasonable length to prevent CameraId overflow
                    // CameraId format: "STATION_{Guid}_" = ~50 chars, leave room for camera name
                    if (cameraName.Length > 100)
                    {
                        cameraName = cameraName.Substring(0, 100);
                    }
                    
                    cameraId = $"STATION_{stationId}_{cameraName}";
                    
                    // Final safety check: ensure CameraId doesn't exceed 200 chars
                    if (cameraId.Length > 200)
                    {
                        cameraId = cameraId.Substring(0, 200);
                    }
                }
                else
                {
                    cameraId = $"STATION_{stationId}_EMAIL";
                    cameraName = $"Email Detector - Station {stationId}";
                }

                // Create MotionEvent (no need for separate EmailEvent table)
                var motionEvent = new MotionEvent
                {
                    StationId = stationId,
                    CameraId = cameraId,      // ✅ Required field - generated from station info
                    CameraName = cameraName,  // Optional but helpful
                    EventType = "Motion",
                    EmailMessageId = messageId,  // ✅ Store MessageId for duplicate checking
                    EmailSubject = subject,
                    Payload = JsonSerializer.Serialize(alarmDetails),
                    DetectedAt = alarmDetails.ContainsKey("AlarmStartTime") 
                        ? (DateTime)alarmDetails["AlarmStartTime"]! 
                        : receivedAt,
                    CreatedAt = DateTime.UtcNow,
                    IsProcessed = false
                };

                _logger.LogInformation(
                    "[EmailService] Parsed email: StationId={StationId}, StationCode={StationCode}, AlarmEvent={AlarmEvent}",
                    stationId,
                    stationCode,
                    alarmDetails.GetValueOrDefault("AlarmEvent")
                );

                return motionEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EmailService] Error parsing email");
                return null;
            }
        }

        public async Task SaveMotionEventAsync(MotionEvent motionEvent)
        {
            try
            {
                // Save MotionEvent
                _context.MotionEvents.Add(motionEvent);
                _logger.LogInformation(
                    "[EmailService] Saving MotionEvent for StationId={StationId}",
                    motionEvent.StationId
                );
                
                // Update Station.LastMotionDetectedAt
                if (motionEvent.StationId.HasValue)
                {
                    var station = await _context.Stations.FindAsync(motionEvent.StationId.Value);
                    if (station != null)
                    {
                        station.LastMotionDetectedAt = motionEvent.DetectedAt;
                        _context.Stations.Update(station);
                        
                        _logger.LogInformation(
                            "[EmailService] Updated Station {StationId} LastMotionDetectedAt to {Time}",
                            station.Id,
                            motionEvent.DetectedAt
                        );
                    }
                    
                    // ✅ Auto-resolve alerts within tolerance window (±BufferMinutes around AlertTime)
                    // Logic: Alert at 10h00, buffer 5min
                    //   → Tolerance: [09h55, 10h05] (AlertTime ± buffer)
                    //   → Motion at 10h03 resolves this alert
                    var activeAlerts = await _context.MotionAlerts
                        .Include(a => a.TimeFrame) // Need TimeFrame for BufferMinutes
                        .Where(a => a.StationId == motionEvent.StationId 
                                 && !a.IsResolved 
                                 && a.TimeFrameId.HasValue)
                        .ToListAsync();
                    
                    if (activeAlerts.Any())
                    {
                        int resolvedCount = 0;
                        foreach (var alert in activeAlerts)
                        {
                            if (alert.TimeFrame == null) continue;

                            // Calculate tolerance window: ± BufferMinutes around AlertTime
                            var windowStart = alert.AlertTime.AddMinutes(-alert.TimeFrame.BufferMinutes);
                            var windowEnd = alert.AlertTime.AddMinutes(alert.TimeFrame.BufferMinutes);

                            // Check if motion falls within this alert's tolerance window
                            if (motionEvent.DetectedAt >= windowStart && motionEvent.DetectedAt <= windowEnd)
                            {
                                alert.IsResolved = true;
                                alert.ResolvedAt = DateTime.UtcNow;
                                alert.ResolvedBy = "System (Auto-resolved by email motion detection)";
                                alert.IsDeleted = true; // Hide resolved alert from display
                                alert.Notes = $"Auto-resolved: Motion at {motionEvent.DetectedAt:yyyy-MM-dd HH:mm:ss} within tolerance [{windowStart:HH:mm}-{windowEnd:HH:mm}]";
                                _context.MotionAlerts.Update(alert);
                                resolvedCount++;

                                _logger.LogInformation(
                                    "[EmailService] Resolved alert {AlertId} (AlertTime={AlertTime:HH:mm}, Tolerance=[{WindowStart:HH:mm}-{WindowEnd:HH:mm}], Motion={MotionTime:HH:mm})",
                                    alert.Id,
                                    alert.AlertTime,
                                    windowStart,
                                    windowEnd,
                                    motionEvent.DetectedAt
                                );
                            }
                            else
                            {
                                _logger.LogDebug(
                                    "[EmailService] Alert {AlertId} NOT resolved - Motion {MotionTime:HH:mm} outside tolerance [{WindowStart:HH:mm}-{WindowEnd:HH:mm}]",
                                    alert.Id,
                                    motionEvent.DetectedAt,
                                    windowStart,
                                    windowEnd
                                );
                            }
                        }

                        if (resolvedCount > 0)
                        {
                            _logger.LogInformation(
                                "[EmailService] Auto-resolved {Count}/{Total} alert(s) for StationId={StationId}",
                                resolvedCount,
                                activeAlerts.Count,
                                motionEvent.StationId
                            );
                        }
                    }
                    else
                    {
                        _logger.LogDebug(
                            "[EmailService] No unresolved alerts found for StationId={StationId}",
                            motionEvent.StationId
                        );
                    }
                }
                
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "[EmailService] ✅ Saved MotionEvent ID={MotionEventId} for StationId={StationId}",
                    motionEvent.Id,
                    motionEvent.StationId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EmailService] ❌ Error saving MotionEvent");
                throw;
            }
        }
    }
}
