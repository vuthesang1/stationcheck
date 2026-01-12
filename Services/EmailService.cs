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

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadWrite);

                // Get the station-motion label folder
                var stationMotionFolder = client.GetFolder("station-motion");
#if DEBUG
                stationMotionFolder = client.GetFolder("test");
#endif
                if (stationMotionFolder == null)
                {
                    _logger.LogWarning("[EmailService] Label 'station-motion' not found. Falling back to Inbox.");
                    stationMotionFolder = inbox;
                }
                else
                {
                    await stationMotionFolder.OpenAsync(FolderAccess.ReadWrite);
                    _logger.LogInformation("[EmailService] Opened folder: {FolderName}", stationMotionFolder.FullName);
                }

                // Search for UNREAD emails only, limit to 200
                SearchQuery searchQuery = SearchQuery.NotSeen;
                var allUnreadUids = await stationMotionFolder.SearchAsync(searchQuery);

                // Limit to 100 emails
                var limitedUids = allUnreadUids.Take(100).ToList();
                _logger.LogInformation("[EmailService] Found {Total} unread emails, processing {Limited} (limit 200)",
                    allUnreadUids.Count, limitedUids.Count);

                var uids = limitedUids;

                // ========== OPTIMIZATION: Batch fetch all messages first ==========
                _logger.LogInformation("[EmailService] 📥 Fetching {Count} messages in batch...", uids.Count);
                var messages = new Dictionary<UniqueId, MimeMessage>();
                foreach (var uid in uids)
                {
                    try
                    {
                        var message = await stationMotionFolder.GetMessageAsync(uid);
                        messages[uid] = message;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[EmailService] Error fetching message UID {Uid}", uid);
                    }
                }
                _logger.LogInformation("[EmailService] ✓ Fetched {Count} messages", messages.Count);

                // ========== OPTIMIZATION: Pre-load all stations into cache ==========
                var stationsCache = await _context.Stations
                    .ToDictionaryAsync(s => s.StationCode.ToUpper(), s => s);
                _logger.LogInformation("[EmailService] 💾 Cached {Count} stations", stationsCache.Count);

                // ========== OPTIMIZATION: Check existing MessageIds in batch ==========
                var messageIds = messages.Values.Select(m => m.MessageId).ToList();
                var existingMessageIds = await _context.MotionEvents
                    .Where(e => messageIds.Contains(e.EmailMessageId))
                    .Select(e => e.EmailMessageId)
                    .ToListAsync();
                var existingSet = new HashSet<string>(existingMessageIds);
                _logger.LogInformation("[EmailService] 🔍 Found {Count} already processed emails", existingSet.Count);

                // ========== OPTIMIZATION: Process emails in parallel ==========
                var semaphore = new SemaphoreSlim(10); // Max 10 concurrent processing
                var processingTasks = messages.Select(async kvp =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var uid = kvp.Key;
                        var message = kvp.Value;

                        var messageId = message.MessageId;
                        var emailSubject = message.Subject ?? string.Empty;
                        var emailFrom = message.From.ToString();
                        var receivedAt = message.Date.DateTime.ToUniversalTime();

                        // ✅ Filter: Only process emails with StationCode pattern (ST000001) in subject
                        var stationCodeMatch = Regex.Match(emailSubject, @"ST\d{6}");
                        if (!stationCodeMatch.Success)
                        {
                            _logger.LogDebug("[EmailService] Skipped email - no StationCode in subject: '{Subject}'", emailSubject);
                            return (uid, (MotionEvent?)null, false);
                        }

                        // Check if already processed (using pre-loaded set)
                        if (existingSet.Contains(messageId))
                        {
                            _logger.LogDebug("[EmailService] Email already processed: MessageId='{MessageId}'", messageId);
                            return (uid, (MotionEvent?)null, false);
                        }

                        _logger.LogInformation("[EmailService] Processing NEW email: Subject='{Subject}', From='{From}', MessageId='{MessageId}'",
                            emailSubject, emailFrom, messageId);

                        // Parse email to MotionEvent (with cached stations)
                        var motionEvent = await ParseEmailToMotionEventWithCacheAsync(
                            emailSubject,
                            message.TextBody ?? message.HtmlBody ?? string.Empty,
                            emailFrom,
                            receivedAt,
                            messageId,
                            stationsCache  // Pass cached stations
                        );

                        if (motionEvent != null)
                        {
                            _logger.LogInformation("[EmailService] Successfully processed email UID {Uid} - Ready to save", uid);
                            return (uid, motionEvent, true);
                        }
                        else
                        {
                            _logger.LogWarning("[EmailService] Failed to parse email UID {Uid}", uid);
                            return (uid, (MotionEvent?)null, false);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogWarning("[EmailService] Processing cancelled for email UID {Uid}", kvp.Key);
                        throw; // Re-throw cancellation
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[EmailService] ❌ CRITICAL ERROR processing email UID {Uid} - Details: {Message}", kvp.Key, ex.Message);
                        return (kvp.Key, (MotionEvent?)null, false);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }).ToList();

                _logger.LogInformation("[EmailService] ⏳ Waiting for {Count} parallel tasks to complete...", processingTasks.Count);
                var results = await Task.WhenAll(processingTasks);
                _logger.LogInformation("[EmailService] ✓ All {Count} tasks completed", results.Length);

                // ========== OPTIMIZATION: Batch save all MotionEvents with full logic ==========
                var validResults = results.Where(r => r.Item2 != null).ToList();
                if (validResults.Any())
                {
                    _logger.LogInformation("[EmailService] 💾 Saving {Count} MotionEvents in batch...", validResults.Count);
                    var eventsToSave = validResults.Select(r => r.Item2!).ToList();
                    
                    // Add all MotionEvents
                    await _context.MotionEvents.AddRangeAsync(eventsToSave);
                    
                    // ========== Update Station.LastMotionDetectedAt for each unique station ==========
                    var stationIds = eventsToSave.Where(e => e.StationId.HasValue).Select(e => e.StationId!.Value).Distinct().ToList();
                    if (stationIds.Any())
                    {
                        var stations = await _context.Stations.Where(s => stationIds.Contains(s.Id)).ToListAsync();
                        foreach (var station in stations)
                        {
                            var latestMotion = eventsToSave
                                .Where(e => e.StationId == station.Id)
                                .OrderByDescending(e => e.DetectedAt)
                                .FirstOrDefault();
                            
                            if (latestMotion != null)
                            {
                                station.LastMotionDetectedAt = latestMotion.DetectedAt;
                                _context.Stations.Update(station);
                                _logger.LogInformation("[EmailService] Updated Station {StationId} LastMotionDetectedAt to {Time}",
                                    station.Id, latestMotion.DetectedAt);
                            }
                        }
                        
                        // ========== Auto-resolve alerts or create resolved alert if missing ==========
                        // Strategy: For each station with new motion, either:
                        // 1. Resolve existing unresolved alert (normal case)
                        // 2. Create new resolved alert if no alert exists (system missed checkpoint)
                        int resolvedCount = 0;
                        int createdCount = 0;
                        
                        foreach (var stationId in stationIds)
                        {
                            var stationMotions = eventsToSave.Where(e => e.StationId == stationId).ToList();
                            if (!stationMotions.Any()) continue;
                            
                            var latestMotion = stationMotions.OrderByDescending(e => e.DetectedAt).First();
                            var station = stations.FirstOrDefault(s => s.Id == stationId);
                            if (station == null) continue;
                            
                            // Find active TimeFrame first to calculate current checkpoint
                            var activeTimeFrame = await _context.TimeFrames
                                .Where(tf => tf.StationId == stationId && tf.IsEnabled)
                                .OrderBy(tf => tf.StartTime)
                                .FirstOrDefaultAsync();
                            
                            if (activeTimeFrame == null)
                            {
                                _logger.LogWarning("[EmailService] No active TimeFrame found for StationId={StationId}", stationId);
                                continue;
                            }
                            
                            // Calculate which checkpoint this motion belongs to (all UTC)
                            var motionTimeUtc = latestMotion.DetectedAt;
                            var currentTimeOfDay = motionTimeUtc.TimeOfDay;
                            var elapsed = currentTimeOfDay - activeTimeFrame.StartTime;
                            
                            if (elapsed.TotalMinutes < 0)
                            {
                                _logger.LogDebug("[EmailService] Motion before TimeFrame StartTime, skipping");
                                continue;
                            }
                            
                            var checkpointsSinceStart = Math.Floor(elapsed.TotalMinutes / activeTimeFrame.FrequencyMinutes);
                            var checkpointTimeOfDay = activeTimeFrame.StartTime.Add(
                                TimeSpan.FromMinutes(checkpointsSinceStart * activeTimeFrame.FrequencyMinutes));
                            
                            var currentCheckpointDateTime = new DateTime(
                                motionTimeUtc.Year, motionTimeUtc.Month, motionTimeUtc.Day,
                                checkpointTimeOfDay.Hours, checkpointTimeOfDay.Minutes, 0,
                                DateTimeKind.Utc
                            );
                            
                            // Find alerts that could be relevant for this motion
                            // Strategy: Check if motion falls within any alert's tolerance window
                            // This handles config changes (e.g., frequency changed from 3min to 20min)
                            // Include both unresolved AND recently resolved alerts to avoid duplicates
                            // Look back 24 hours to handle delayed emails
                            var recentTimeWindow = motionTimeUtc.AddHours(-48);
                            var potentialAlerts = await _context.MotionAlerts
                                .Include(a => a.TimeFrame)
                                .Where(a => a.StationId == stationId 
                                         && !a.IsDeleted
                                         && a.TimeFrameId.HasValue
                                         && a.AlertTime >= recentTimeWindow) // Look back 24 hours
                                .OrderByDescending(a => a.AlertTime)
                                .ToListAsync();
                            
                            // First, check if motion matches any UNRESOLVED alert's tolerance window
                            var unresolvedAlert = potentialAlerts
                                .Where(a => !a.IsResolved)
                                .FirstOrDefault(a =>
                                {
                                    if (a.TimeFrame == null) return false;
                                    
                                    // Get buffer from TimeFrame, fallback to 1 minute for old alerts
                                    var bufferMinutes = a.TimeFrame.BufferMinutes > 0 
                                        ? a.TimeFrame.BufferMinutes 
                                        : 1; // Default buffer for legacy alerts
                                    
                                    var windowStart = a.AlertTime;
                                    var windowEnd = a.AlertTime.AddMinutes(bufferMinutes);
                                    return motionTimeUtc >= windowStart && motionTimeUtc <= windowEnd;
                                });
                            
                            // If no unresolved alert found, check if there's already a RESOLVED alert for this checkpoint
                            // This prevents creating duplicate alerts when AlertGenerationService has already processed
                            var existingResolvedAlert = potentialAlerts
                                .Where(a => a.IsResolved)
                                .FirstOrDefault(a =>
                                {
                                    if (a.TimeFrame == null) return false;
                                    
                                    // Get buffer from TimeFrame, fallback to 1 minute for old alerts
                                    var bufferMinutes = a.TimeFrame.BufferMinutes > 0 
                                        ? a.TimeFrame.BufferMinutes 
                                        : 1; // Default buffer for legacy alerts
                                    
                                    var windowStart = a.AlertTime;
                                    var windowEnd = a.AlertTime.AddMinutes(bufferMinutes);
                                    return motionTimeUtc >= windowStart && motionTimeUtc <= windowEnd;
                                });
                            
                            if (unresolvedAlert != null && unresolvedAlert.TimeFrame != null)
                            {
                                // CASE 1: Found unresolved alert where motion is within its tolerance window
                                var bufferMinutes = unresolvedAlert.TimeFrame.BufferMinutes;
                                var windowStart = unresolvedAlert.AlertTime;
                                var windowEnd = unresolvedAlert.AlertTime.AddMinutes(bufferMinutes);
                                
                                // Motion is within tolerance window - RESOLVE the alert
                                unresolvedAlert.IsResolved = true;
                                unresolvedAlert.ResolvedAt = DateTime.UtcNow;
                                unresolvedAlert.ResolvedBy = "System (Auto-resolved by email processing)";
                                unresolvedAlert.Notes = $"Auto-resolved: Motion at {motionTimeUtc:yyyy-MM-dd HH:mm:ss}Z within tolerance [{windowStart:HH:mm:ss}Z-{windowEnd:HH:mm:ss}Z]";
                                _context.MotionAlerts.Update(unresolvedAlert);
                                resolvedCount++;
                                
                                _logger.LogInformation(
                                    "[EmailService] ✅ RESOLVED Alert {AlertId} | Station={StationName} | AlertTime={AlertTime:yyyy-MM-dd HH:mm}Z | Motion={MotionTime:yyyy-MM-dd HH:mm}Z | Window=[{WindowStart:HH:mm}Z-{WindowEnd:HH:mm}Z] | ConfigFreq={Freq}min",
                                    unresolvedAlert.Id,
                                    station.Name,
                                    unresolvedAlert.AlertTime,
                                    motionTimeUtc,
                                    windowStart,
                                    windowEnd,
                                    bufferMinutes
                                );
                            }
                            else if (existingResolvedAlert != null)
                            {
                                // CASE 2: Found existing RESOLVED alert for this motion
                                // AlertGenerationService has already processed this checkpoint
                                // Skip creating duplicate alert
                                _logger.LogInformation(
                                    "[EmailService] ⏭️ SKIP: Alert already resolved | Station={StationName} | AlertTime={AlertTime:yyyy-MM-dd HH:mm}Z | Motion={MotionTime:yyyy-MM-dd HH:mm}Z | ResolvedBy={ResolvedBy}",
                                    station.Name,
                                    existingResolvedAlert.AlertTime,
                                    motionTimeUtc,
                                    existingResolvedAlert.ResolvedBy
                                );
                            }
                            else
                            {
                                // CASE 3: No alert found (unresolved or resolved) FOR THIS CHECKPOINT
                                // AlertGenerationService may have missed this checkpoint
                                // Create a new RESOLVED alert to indicate station is ONLINE
                                
                                // Create TimeFrameHistory if not exists
                                var timeFrameHistory = await _context.TimeFrameHistories
                                    .Where(h => h.TimeFrameId == activeTimeFrame.Id)
                                    .OrderByDescending(h => h.Version)
                                    .FirstOrDefaultAsync();
                                
                                if (timeFrameHistory == null)
                                {
                                    var tfSnapshot = new
                                    {
                                        TimeFrameId = activeTimeFrame.Id,
                                        Name = activeTimeFrame.Name,
                                        StartTime = activeTimeFrame.StartTime.ToString(),
                                        EndTime = activeTimeFrame.EndTime.ToString(),
                                        FrequencyMinutes = activeTimeFrame.FrequencyMinutes,
                                        BufferMinutes = activeTimeFrame.BufferMinutes,
                                        DaysOfWeek = activeTimeFrame.DaysOfWeek,
                                        IsEnabled = activeTimeFrame.IsEnabled
                                    };
                                    
                                    timeFrameHistory = new TimeFrameHistory
                                    {
                                        TimeFrameId = activeTimeFrame.Id,
                                        StationId = stationId,
                                        Version = 1,
                                        Action = "Create",
                                        ConfigurationSnapshot = System.Text.Json.JsonSerializer.Serialize(tfSnapshot),
                                        ChangeDescription = $"Auto-created by EmailService for checkpoint {currentCheckpointDateTime:yyyy-MM-dd HH:mm}Z",
                                        ChangedBy = "System (EmailService)",
                                        ChangedAt = DateTime.UtcNow
                                    };
                                    
                                    _context.TimeFrameHistories.Add(timeFrameHistory);
                                    await _context.SaveChangesAsync(); // Save to get ID
                                }
                                
                                // Create configuration snapshot
                                var configSnapshot = new
                                {
                                    TimeFrameId = activeTimeFrame.Id,
                                    TimeFrameName = activeTimeFrame.Name,
                                    StartTime = activeTimeFrame.StartTime.ToString(),
                                    EndTime = activeTimeFrame.EndTime.ToString(),
                                    FrequencyMinutes = activeTimeFrame.FrequencyMinutes,
                                    BufferMinutes = activeTimeFrame.BufferMinutes,
                                    DaysOfWeek = activeTimeFrame.DaysOfWeek,
                                    CheckedAt = currentCheckpointDateTime
                                };
                                
                                // Get last motion event details for CameraId/Name
                                var lastMotionEvent = stationMotions.OrderByDescending(m => m.DetectedAt).First();
                                
                                // Create RESOLVED alert (Station is Online at this checkpoint)
                                var newAlert = new MotionAlert
                                {
                                    StationId = stationId,
                                    StationName = station.Name,
                                    TimeFrameId = activeTimeFrame.Id,
                                    TimeFrameHistoryId = timeFrameHistory?.Id,
                                    ConfigurationSnapshot = System.Text.Json.JsonSerializer.Serialize(configSnapshot),
                                    AlertTime = currentCheckpointDateTime,
                                    Severity = AlertSeverity.Warning,
                                    Message = $"Trạm {station.Name} Online",
                                    ExpectedFrequencyMinutes = activeTimeFrame.FrequencyMinutes,
                                    LastMotionAt = latestMotion.DetectedAt,
                                    LastMotionCameraId = lastMotionEvent.CameraId,
                                    LastMotionCameraName = lastMotionEvent.CameraName,
                                    MinutesSinceLastMotion = 0,
                                    IsResolved = true,
                                    ResolvedAt = DateTime.UtcNow,
                                    ResolvedBy = "System (Auto-created by email processing)",
                                    Notes = $"Alert auto-created by EmailService: No alert found for checkpoint {currentCheckpointDateTime:yyyy-MM-dd HH:mm}Z, motion detected at {latestMotion.DetectedAt:yyyy-MM-dd HH:mm:ss}Z",
                                    IsDeleted = false,
                                    CreatedAt = DateTime.UtcNow
                                };
                                
                                _context.MotionAlerts.Add(newAlert);
                                createdCount++;
                                
                                _logger.LogInformation(
                                    "[EmailService] ✨ CREATED RESOLVED Alert | Station={StationName} | Checkpoint={Checkpoint:yyyy-MM-dd HH:mm}Z | Motion={MotionTime:yyyy-MM-dd HH:mm:ss}Z | Reason=No alert found for checkpoint",
                                    station.Name,
                                    currentCheckpointDateTime,
                                    motionTimeUtc
                                );
                            }
                        }
                        
                        if (resolvedCount > 0 || createdCount > 0)
                        {
                            _logger.LogInformation(
                                "[EmailService] 📊 Alert Summary: Resolved={Resolved}, Created={Created} | Total Stations={StationCount}",
                                resolvedCount,
                                createdCount,
                                stationIds.Count
                            );
                        }
                    } // End if (stationIds.Any())
                    
                    // Save all changes (MotionEvents + Station updates + Alert resolutions/creations)
                    await _context.SaveChangesAsync();
                    processedEvents.AddRange(eventsToSave);
                    _logger.LogInformation("[EmailService] ✓ Saved {Count} MotionEvents with full processing", eventsToSave.Count);
                }

                // ========== Mark as read / Delete (in batch) ==========
                var uidsToMarkRead = results.Where(r => r.Item3 || r.Item2 == null).Select(r => r.Item1).ToList();
                if (uidsToMarkRead.Any() && _emailSettings.MarkAsRead)
                {
                    _logger.LogInformation("[EmailService] ✓ Marking {Count} emails as read...", uidsToMarkRead.Count);
                    await stationMotionFolder.AddFlagsAsync(uidsToMarkRead, MessageFlags.Seen, true);
                }

                if (_emailSettings.DeleteAfterProcessing && validResults.Any())
                {
                    var uidsToDelete = validResults.Select(r => r.Item1).ToList();
                    _logger.LogInformation("[EmailService] 🗑️ Deleting {Count} processed emails...", uidsToDelete.Count);
                    await stationMotionFolder.AddFlagsAsync(uidsToDelete, MessageFlags.Deleted, true);
                }

                // Expunge deleted messages if any
                if (_emailSettings.DeleteAfterProcessing && processedEvents.Any())
                {
                    await stationMotionFolder.ExpungeAsync();
                }

                // Disconnect
                await client.DisconnectAsync(true);

                _logger.LogInformation("[EmailService] Processed {Count} email(s) successfully", processedEvents.Count);

                return processedEvents;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("[EmailService] ⚠️ Email check was cancelled");
                throw; // Re-throw to signal cancellation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EmailService] ❌ CRITICAL ERROR in CheckAndProcessNewEmailsAsync - Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}", 
                    ex.GetType().Name, ex.Message, ex.StackTrace);
                
                // Log inner exception if exists
                if (ex.InnerException != null)
                {
                    _logger.LogError("[EmailService] ❌ Inner Exception - Type: {InnerType}, Message: {InnerMessage}", 
                        ex.InnerException.GetType().Name, ex.InnerException.Message);
                }
                
                return processedEvents;
            }
        }

        // ========== NEW: Optimized version with cached stations ==========
        private async Task<MotionEvent?> ParseEmailToMotionEventWithCacheAsync(
            string subject, 
            string body, 
            string from, 
            DateTime receivedAt, 
            string messageId,
            Dictionary<string, Station> stationsCache)
        {
            try
            {
                // Extract StationCode from subject
                Guid? stationId = null;
                string? stationCode = null;

                var subjectTrimmed = (subject ?? "").Trim();
                var stationCodeMatch = Regex.Match(subjectTrimmed, @"ST\d{6}");

                if (stationCodeMatch.Success)
                {
                    stationCode = stationCodeMatch.Value.ToUpper();
                    _logger.LogInformation("[EmailService] Extracted StationCode from subject: '{StationCode}'", stationCode);

                    // Use cached stations (no DB query)
                    if (stationsCache.TryGetValue(stationCode, out var station))
                    {
                        stationId = station.Id;
                        _logger.LogInformation("[EmailService] ✓ Found station: Code='{StationCode}' → StationId={StationId}, Name='{StationName}'", 
                            stationCode, stationId, station.Name);
                    }
                    else
                    {
                        _logger.LogWarning("[EmailService] ✗ Station NOT FOUND for code: '{StationCode}'", stationCode);
                    }
                }
                else
                {
                    _logger.LogWarning("[EmailService] ✗ Could not extract StationCode pattern from subject: '{Subject}'", subject);
                }

                if (!stationId.HasValue)
                {
                    _logger.LogWarning("[EmailService] ✗ Could not find StationId from subject: '{Subject}'", subject);
                    return null;
                }

                // Filter: Only process motion detection events
                // if (body != null)
                // {
                //     bool isMotionDetection = body.Contains("Phát hiện chuyển động", StringComparison.OrdinalIgnoreCase) ||
                //                            body.Contains("Motion Detection", StringComparison.OrdinalIgnoreCase);

                //     if (!isMotionDetection)
                //     {
                //         _logger.LogInformation("[EmailService] ⏭️ Skipped - Not a motion detection event | Subject: '{Subject}'", subject);
                //         return null;
                //     }
                // }

                // Extract AlarmStartTime
                var alarmDetails = new Dictionary<string, object?>();
                DateTime? alarmStartTime = null;

                if (body != null)
                {
                    var startTimeMatch = Regex.Match(body, @"(Alarm Start Time|Thời gian bắt đầu cảnh báo).+?(\d{1,2}[/.]?\d{1,2}[/.]?\d{4}\s+\d{1,2}[:.]?\d{1,2}[:.]?\d{2})");

                    if (startTimeMatch.Success)
                    {
                        var timeStr = startTimeMatch.Groups[2].Value.Trim().Replace(".", ":");

                        if (DateTime.TryParseExact(
                            timeStr,
                            new[] { "dd/MM/yyyy HH:mm:ss", "d/M/yyyy H:m:s" },
                            null,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime parsedTime))
                        {
                            alarmStartTime = parsedTime.AddHours(-7); // UTC+7 to UTC
                            alarmDetails["AlarmStartTime"] = alarmStartTime;
                            _logger.LogInformation("[EmailService] ✅ Parsed AlarmStartTime: {LocalTime} → UTC={UtcTime}",
                                parsedTime, alarmStartTime);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("[EmailService] ⚠️ Could not extract AlarmStartTime from email body");
                        return null;
                    }
                }

                string cameraId = $"STATION_{stationId}_EMAIL";
                string cameraName = $"Ghi Nhận - Station {stationId}";

                var motionEvent = new MotionEvent
                {
                    StationId = stationId,
                    CameraId = cameraId,
                    CameraName = cameraName,
                    EventType = "Motion",
                    EmailMessageId = messageId,
                    EmailSubject = subject,
                    EmailBody = body,
                    Payload = JsonSerializer.Serialize(alarmDetails),
                    DetectedAt = alarmStartTime ?? receivedAt,
                    CreatedAt = DateTime.UtcNow,
                    IsProcessed = false
                };

                _logger.LogInformation(
                    "[EmailService] ✅ Parsed email: StationId={StationId}, StationCode={StationCode}, AlarmStartTime={AlarmTime}",
                    stationId, stationCode, alarmStartTime);

                return motionEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EmailService] Error parsing email");
                return null;
            }
        }

        public async Task<MotionEvent?> ParseEmailToMotionEventAsync(string subject, string body, string from, DateTime receivedAt, string messageId)
        {
            try
            {
                // Extract StationCode from subject
                // ✅ NEW FORMAT: Subject = "[stm] ST000001" (has [stm] prefix)
                // OLD FORMAT: Subject = "ST000001" (no prefix)
                Guid? stationId = null;
                string? stationCode = null;

                // Extract StationCode pattern: ST + 6 digits (with or without [stm] prefix)
                var subjectTrimmed = (subject ?? "").Trim();
                var stationCodeMatch = Regex.Match(subjectTrimmed, @"ST\d{6}");

                if (stationCodeMatch.Success)
                {
                    stationCode = stationCodeMatch.Value;
                    _logger.LogInformation("[EmailService] Extracted StationCode from subject: '{StationCode}'", stationCode);

                    // Query case-insensitive (handle both uppercase and lowercase variations)
                    var station = await _context.Stations
                        .FirstOrDefaultAsync(s => s.StationCode.ToUpper() == stationCode.ToUpper());

                    if (station != null)
                    {
                        stationId = station.Id;
                        _logger.LogInformation("[EmailService] ✓ Found station: Code='{StationCode}' → StationId={StationId}, Name='{StationName}'", stationCode, stationId, station.Name);
                    }
                    else
                    {
                        _logger.LogWarning("[EmailService] ✗ Station NOT FOUND for code: '{StationCode}'. Checking available stations...", stationCode);

                        // Log all available stations for debugging
                        var allStations = await _context.Stations.ToListAsync();
                        _logger.LogWarning("[EmailService] Available stations in database:");
                        foreach (var s in allStations)
                        {
                            _logger.LogWarning("[EmailService]   - StationCode: '{Code}' (Name: '{Name}')", s.StationCode, s.Name);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("[EmailService] ✗ Could not extract StationCode pattern (ST + 6 digits) from subject: '{Subject}'", subject);
                }

                if (!stationId.HasValue)
                {
                    _logger.LogWarning("[EmailService] ✗ Could not find StationId from subject: '{Subject}'", subject);
                    return null;
                }

                // ✅ Filter: Only process "Phát hiện nhấn nút" (Alert Start) emails
                // Skip "Xóa" or other event types (Alert End)
                if (body != null)
                {
                    // Check if this is a "Motion Detection" event (not an end/cancel event)
                    bool isMotionDetection = body.Contains("Phát hiện chuyển động", StringComparison.OrdinalIgnoreCase) ||
                                           body.Contains("Motion Detection", StringComparison.OrdinalIgnoreCase);

                    if (!isMotionDetection)
                    {
                        _logger.LogInformation("[EmailService] ⏭️ Skipped email - Not a motion detection event | Subject: '{Subject}'", subject);
                        return null;
                    }
                }

                // Extract only AlarmStartTime from body
                // ⚠️ SIMPLIFIED: Store full email body as-is, only parse AlarmStartTime
                var alarmDetails = new Dictionary<string, object?>();
                DateTime? alarmStartTime = null;

                if (body != null)
                {
                    // Alarm Start Time - Support both English and Vietnamese formats
                    // English: "Alarm Start Time (D/M/Y H:M:S): 12/11/2025 16:03:57"
                    // Vietnamese: "Thời gian bắt đầu cảnh báo(D/M/Y H:M:S): 29/11/2025 08:40:34"
                    // Handle both "/" and "." separators (Vietnamese emails sometimes use dots)
                    var startTimeMatch = Regex.Match(body, @"(Alarm Start Time|Thời gian bắt đầu cảnh báo).+?(\d{1,2}[/.]?\d{1,2}[/.]?\d{4}\s+\d{1,2}[:.]?\d{1,2}[:.]?\d{2})");

                    if (startTimeMatch.Success)
                    {
                        var timeStr = startTimeMatch.Groups[2].Value.Trim()
                            .Replace(".", ":");  // Normalize dots to colons for time parsing
                        if (DateTime.TryParseExact(
                            timeStr,
                            new[] { "dd/MM/yyyy HH:mm:ss", "d/M/yyyy H:m:s" },
                            null,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime parsedTime))
                        {
                            // Treat parsed time as UTC (already converted by camera/NVR)
                            alarmStartTime = DateTime.SpecifyKind(parsedTime, DateTimeKind.Utc);
                            alarmDetails["AlarmStartTime"] = alarmStartTime;
                            _logger.LogInformation("[EmailService] ✅ Parsed AlarmStartTime: {UtcTime}",
                                alarmStartTime);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("[EmailService] ⚠️ Could not extract AlarmStartTime from email body");
                        return null;
                    }
                }

                // Generate CameraId from Station info (required field in database)
                string cameraId = $"STATION_{stationId}_EMAIL";
                string cameraName = $"Ghi Nhận - Station {stationId}";

                // Create MotionEvent (store full email body for troubleshooting)
                var motionEvent = new MotionEvent
                {
                    StationId = stationId,
                    CameraId = cameraId,           // ✅ Required field - generated from station info
                    CameraName = cameraName,       // Optional but helpful
                    EventType = "Motion",
                    EmailMessageId = messageId,    // ✅ Store MessageId for duplicate checking
                    EmailSubject = subject,
                    EmailBody = body,              // ✅ NEW: Store full email content as-is
                    Payload = JsonSerializer.Serialize(alarmDetails),
                    // ✅ Use AlarmStartTime from email
                    DetectedAt = alarmStartTime ?? receivedAt,  // Fallback to received time if parse failed
                    CreatedAt = DateTime.UtcNow,
                    IsProcessed = false
                };

                _logger.LogInformation(
                    "[EmailService] ✅ Parsed email: StationId={StationId}, StationCode={StationCode}, AlarmStartTime={AlarmTime}",
                    stationId,
                    stationCode,
                    alarmStartTime
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
                    // AlertTime is stored in UTC in database
                    // DetectedAt (from email) is also in UTC
                    // Compare directly in UTC
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

                            // Both AlertTime and DetectedAt are in UTC
                            // Calculate tolerance window: +BufferMinutes after AlertTime (checkpoint)
                            var windowStart = alert.AlertTime;
                            var windowEnd = alert.AlertTime.AddMinutes(alert.TimeFrame.BufferMinutes);

                            // Check if motion falls within this alert's tolerance window (all times in UTC)
                            if (motionEvent.DetectedAt >= windowStart && motionEvent.DetectedAt <= windowEnd)
                            {
                                alert.IsResolved = true;
                                alert.ResolvedAt = DateTime.UtcNow;
                                alert.ResolvedBy = "System (Auto-resolved by email motion detection)";
                                alert.IsDeleted = true; // Hide resolved alert from display
                                alert.Notes = $"Auto-resolved: Motion at {motionEvent.DetectedAt:yyyy-MM-dd HH:mm:ss}Z (UTC) within tolerance [{windowStart:HH:mm:ss}Z-{windowEnd:HH:mm:ss}Z]";
                                _context.MotionAlerts.Update(alert);
                                resolvedCount++;

                                _logger.LogInformation(
                                    "[EmailService] ✅ RESOLVED Alert {AlertId} | AlertTime={AlertTimeUtc:HH:mm}Z (UTC) | Tolerance=[{WindowStart:HH:mm}Z-{WindowEnd:HH:mm}Z] | Motion={MotionTimeUtc:HH:mm}Z",
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
                                    "[EmailService] ❌ Alert {AlertId} NOT resolved | AlertTime={AlertTimeUtc:HH:mm}Z (UTC) | Tolerance=[{WindowStart:HH:mm}Z-{WindowEnd:HH:mm}Z] | Motion={MotionTimeUtc:HH:mm}Z (OUTSIDE)",
                                    alert.Id,
                                    alert.AlertTime,
                                    windowStart,
                                    windowEnd,
                                    motionEvent.DetectedAt
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
