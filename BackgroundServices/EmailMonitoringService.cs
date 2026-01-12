using StationCheck.Interfaces;
using StationCheck.Services;

namespace StationCheck.BackgroundServices
{
    public class EmailMonitoringService : BackgroundService
    {
        private readonly ILogger<EmailMonitoringService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConfigurationChangeNotifier _notifier;
        private TimeSpan _checkInterval = TimeSpan.FromMinutes(3); // Default: 3 minutes
        private CancellationTokenSource? _delayCts;

        public EmailMonitoringService(
            ILogger<EmailMonitoringService> logger,
            IServiceProvider serviceProvider,
            ConfigurationChangeNotifier notifier)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _notifier = notifier;
            
            // ✅ Subscribe to configuration changes
            _notifier.Subscribe("EmailMonitorInterval", OnConfigurationChanged);
        }
        
        private void OnConfigurationChanged()
        {
            _logger.LogInformation("[EmailMonitor] Configuration changed, reloading interval immediately...");
            // Cancel the current delay to trigger immediate reload
            _delayCts?.Cancel();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[EmailMonitor] ⚙️ Service starting...");

            try
            {
                // Load interval from database
                await LoadIntervalAsync();

                // Wait a bit before starting to ensure app is fully initialized
                _logger.LogInformation("[EmailMonitor] ⏳ Waiting 10s for app initialization...");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                
                _logger.LogInformation("[EmailMonitor] ✅ Service fully started, entering monitoring loop");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation("[EmailMonitor] 🔄 Starting email check cycle...");
                        await CheckEmailsAsync(stoppingToken);
                        _logger.LogInformation("[EmailMonitor] ✓ Email check cycle completed");
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("[EmailMonitor] ⚠️ Check cancelled due to shutdown request");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[EmailMonitor] ❌ ERROR in monitoring loop - Type: {Type}, Message: {Message}", 
                            ex.GetType().Name, ex.Message);
                    }

                    // Reload interval from DB every cycle
                    try
                    {
                        await LoadIntervalAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[EmailMonitor] ❌ Error loading interval, using current value: {Interval}s", 
                            _checkInterval.TotalSeconds);
                    }

                    // Wait for next check interval (can be cancelled by config change)
                    try
                    {
                        _logger.LogInformation("[EmailMonitor] ⏱️ Waiting {Interval} seconds until next check...", 
                            _checkInterval.TotalSeconds);
                        _delayCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                        await Task.Delay(_checkInterval, _delayCts.Token);
                    }
                    catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
                    {
                        // Config changed, reload immediately
                        _logger.LogInformation("[EmailMonitor] ⚡ Delay cancelled due to configuration change");
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("[EmailMonitor] ⚠️ Delay cancelled due to shutdown request");
                        break;
                    }
                }

                _logger.LogInformation("[EmailMonitor] 🛑 Service stopped gracefully");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("[EmailMonitor] ⚠️ Service cancelled during startup");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "[EmailMonitor] 💥 CRITICAL ERROR in ExecuteAsync - Service will terminate! Type: {Type}, Message: {Message}, StackTrace: {StackTrace}", 
                    ex.GetType().Name, ex.Message, ex.StackTrace);
                throw; // Re-throw to crash the app and force restart
            }
        }

        private async Task LoadIntervalAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var configService = scope.ServiceProvider.GetRequiredService<SystemConfigurationService>();
                
                var interval = await configService.GetTimeSpanValueAsync("EmailMonitorInterval");
                
                if (interval.HasValue)
                {
                    _checkInterval = interval.Value;
                    _logger.LogInformation(
                        "[EmailMonitor] Loaded interval from config: {Interval} seconds",
                        _checkInterval.TotalSeconds
                    );
                }
                else
                {
                    _logger.LogWarning(
                        "[EmailMonitor] Using default interval: {Interval} seconds",
                        _checkInterval.TotalSeconds
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EmailMonitor] Error loading interval config, using default");
            }
        }

        private async Task CheckEmailsAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[EmailMonitor] Running check at {Time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                var processedEvents = await emailService.CheckAndProcessNewEmailsAsync();

                if (processedEvents.Any())
                {
                    _logger.LogInformation(
                        "[EmailMonitor] Processed {Count} new motion events from emails",
                        processedEvents.Count
                    );

                    foreach (var motionEvent in processedEvents)
                    {
                        _logger.LogInformation(
                            "[EmailMonitor] MotionEvent: ID={EventId}, StationId={StationId}, DetectedAt={DetectedAt}",
                            motionEvent.Id,
                            motionEvent.StationId,
                            motionEvent.DetectedAt
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EmailMonitor] Error checking emails");
            }
        }
    }
}
