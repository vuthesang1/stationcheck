using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DesktopLoginApp.Services;

/// <summary>
/// Manages secure storage and retrieval of authentication tokens
/// </summary>
public static class TokenManager
{
    private static readonly string TokenDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "StationCheck"
    );
    
    private static readonly string TokenFilePath = Path.Combine(TokenDirectory, "token.dat");
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("StationCheck-Desktop-App-v1.0");

    public class TokenData
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Role { get; set; }
        public string MacAddress { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; }
    }

    /// <summary>
    /// Save token data securely using Windows DPAPI
    /// </summary>
    public static void SaveToken(TokenData tokenData)
    {
        try
        {
            // Ensure directory exists
            Directory.CreateDirectory(TokenDirectory);

            // Serialize to JSON
            var json = JsonSerializer.Serialize(tokenData);
            var plainBytes = Encoding.UTF8.GetBytes(json);

            // Encrypt using DPAPI (Windows Data Protection API)
            var encryptedBytes = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.CurrentUser);

            // Save to file
            File.WriteAllBytes(TokenFilePath, encryptedBytes);
            
            FileLogger.Log($"Token saved for user: {tokenData.Username}");
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Failed to save token", ex);
            throw;
        }
    }

    /// <summary>
    /// Load token data from secure storage
    /// </summary>
    public static TokenData? LoadToken()
    {
        try
        {
            if (!File.Exists(TokenFilePath))
            {
                FileLogger.Log("No saved token found");
                return null;
            }

            // Read encrypted file
            var encryptedBytes = File.ReadAllBytes(TokenFilePath);

            // Decrypt using DPAPI
            var plainBytes = ProtectedData.Unprotect(encryptedBytes, Entropy, DataProtectionScope.CurrentUser);
            var json = Encoding.UTF8.GetString(plainBytes);

            // Deserialize
            var tokenData = JsonSerializer.Deserialize<TokenData>(json);
            
            if (tokenData != null)
            {
                FileLogger.Log($"Token loaded for user: {tokenData.Username} (saved at: {tokenData.SavedAt})");
            }

            return tokenData;
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Failed to load token", ex);
            return null;
        }
    }

    /// <summary>
    /// Clear saved token
    /// </summary>
    public static void ClearToken()
    {
        try
        {
            if (File.Exists(TokenFilePath))
            {
                File.Delete(TokenFilePath);
                FileLogger.Log("Token cleared");
            }
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Failed to clear token", ex);
        }
    }

    /// <summary>
    /// Check if token exists
    /// </summary>
    public static bool HasSavedToken()
    {
        return File.Exists(TokenFilePath);
    }
}
