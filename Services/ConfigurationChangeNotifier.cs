namespace StationCheck.Services
{
    /// <summary>
    /// Notifies background services when configuration changes
    /// </summary>
    public class ConfigurationChangeNotifier
    {
        private readonly Dictionary<string, List<Action>> _subscribers = new();
        private readonly ILogger<ConfigurationChangeNotifier> _logger;

        public ConfigurationChangeNotifier(ILogger<ConfigurationChangeNotifier> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Subscribe to configuration changes for a specific key
        /// </summary>
        public void Subscribe(string key, Action callback)
        {
            if (!_subscribers.ContainsKey(key))
            {
                _subscribers[key] = new List<Action>();
            }
            
            _subscribers[key].Add(callback);
            _logger.LogDebug("Added subscriber for configuration key: {Key}", key);
        }

        /// <summary>
        /// Notify all subscribers that a configuration value has changed
        /// </summary>
        public void NotifyChange(string key)
        {
            if (_subscribers.TryGetValue(key, out var callbacks))
            {
                _logger.LogInformation("Notifying {Count} subscriber(s) for configuration key: {Key}", callbacks.Count, key);
                
                foreach (var callback in callbacks)
                {
                    try
                    {
                        callback.Invoke();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error invoking subscriber callback for key: {Key}", key);
                    }
                }
            }
        }
    }
}
