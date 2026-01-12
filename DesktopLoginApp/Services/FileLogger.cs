using System;
using System.IO;

namespace DesktopLoginApp.Services;

public static class FileLogger
{
    private static readonly string LogDirectory = @"d:\station-c\Logs\DesktopApp";
    private static readonly object LockObject = new object();

    static FileLogger()
    {
        try
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }
        catch { }
    }

    public static void Log(string message)
    {
        try
        {
            lock (LockObject)
            {
                var logFile = Path.Combine(LogDirectory, $"app-{DateTime.Now:yyyyMMdd}.txt");
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                
                File.AppendAllText(logFile, logMessage + Environment.NewLine);
                
                // Also write to debug output
                System.Diagnostics.Debug.WriteLine(logMessage);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to write log: {ex.Message}");
        }
    }

    public static void LogError(string message, Exception? ex = null)
    {
        var errorMessage = ex != null 
            ? $"ERROR: {message}\nException: {ex.GetType().Name}: {ex.Message}\nStackTrace: {ex.StackTrace}"
            : $"ERROR: {message}";
        
        Log(errorMessage);
    }
}
