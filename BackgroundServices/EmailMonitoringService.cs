using StationCheck.Interfaces;

namespace StationCheck.BackgroundServices
{
    public class EmailMonitoringService : BackgroundService
    {
        private readonly ILogger<EmailMonitoringService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // ✅ Changed from 1 to 5 minutes

        public EmailMonitoringService(
            ILogger<EmailMonitoringService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[EmailMonitor] Service started");

            // Wait a bit before starting to ensure app is fully initialized
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckEmailsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[EmailMonitor] Error in monitoring loop");
                }

                // Wait for next check interval
                try
                {
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Service is stopping
                    break;
                }
            }

            _logger.LogInformation("[EmailMonitor] Service stopped");
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
