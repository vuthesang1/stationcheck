namespace DesktopLoginApp;

/// <summary>
/// Application configuration constants
/// </summary>
public static class AppConfig
{
    // Change this URL when publishing for production
#if DEBUG
    public const string API_BASE_URL = "https://localhost:55703";
#else
    public const string API_BASE_URL = "https://pvgascng.vn";
#endif
}
