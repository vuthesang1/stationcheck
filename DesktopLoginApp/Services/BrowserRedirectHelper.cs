using System;
using System.Diagnostics;
using System.Web;

namespace DesktopLoginApp.Services;

/// <summary>
/// Handles redirecting back to browser with authentication token
/// </summary>
public static class BrowserRedirectHelper
{
    /// <summary>
    /// Open browser and redirect to web application with token
    /// </summary>
    public static void RedirectToBrowser(string accessToken, string? baseUrl = null)
    {
        try
        {
            var url = baseUrl ?? AppConfig.API_BASE_URL;
            FileLogger.Log($"Redirecting to browser: {url}");
            
            // URL encode the token
            var encodedToken = HttpUtility.UrlEncode(accessToken);
            
            // Build callback URL with token
            var callbackUrl = $"{url}/desktop-login-callback?token={encodedToken}";
            
            FileLogger.Log($"Callback URL: {callbackUrl}");

            // Open default browser
            var psi = new ProcessStartInfo
            {
                FileName = callbackUrl,
                UseShellExecute = true
            };
            
            Process.Start(psi);
            
            FileLogger.Log("Browser launched successfully");
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Failed to redirect to browser", ex);
            throw;
        }
    }

    /// <summary>
    /// Open browser to specific page
    /// </summary>
    public static void OpenBrowserUrl(string url)
    {
        try
        {
            FileLogger.Log($"Opening URL in browser: {url}");
            
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            FileLogger.LogError($"Failed to open URL: {url}", ex);
            throw;
        }
    }
}
