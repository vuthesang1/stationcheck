using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopLoginApp.Services
{
    /// <summary>
    /// API client for StationCheck backend with certificate authentication
    /// </summary>
    public class StationCheckApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public StationCheckApiClient(string? baseUrl = null)
        {
            _baseUrl = (baseUrl ?? AppConfig.API_BASE_URL).TrimEnd('/');
            
            // Create HttpClientHandler with certificate
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Accept self-signed certs in dev
            };

            // Load client certificate from Windows Certificate Store
            var clientCert = LoadClientCertificate();
            if (clientCert != null)
            {
                handler.ClientCertificates.Add(clientCert);
            }

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        /// <summary>
        /// Request desktop login token (requires authenticated session)
        /// </summary>
        public async Task<DesktopLoginTokenResponse> RequestDesktopLoginTokenAsync(string jwtToken)
        {
            FileLogger.Log("=== REQUEST DESKTOP LOGIN TOKEN ===");
            
            try
            {
                // Add JWT token to request
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
                
                FileLogger.Log($"Sending POST to {_baseUrl}/api/auth/desktop-login-token");
                var response = await _httpClient.PostAsync("/api/auth/desktop-login-token", null);
                var rawContent = await response.Content.ReadAsStringAsync();
                
                FileLogger.Log($"Status: {response.StatusCode}");
                FileLogger.Log($"Response: {rawContent}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<DesktopLoginTokenResponse>(rawContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    FileLogger.Log("Desktop login token created successfully");
                    return result ?? new DesktopLoginTokenResponse { Success = false, Message = "Empty response" };
                }
                else
                {
                    return new DesktopLoginTokenResponse
                    {
                        Success = false,
                        Message = $"Server error ({response.StatusCode}): {rawContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Request desktop token failed", ex);
                return new DesktopLoginTokenResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    
        /// <summary>
        /// Login with username, password, and MAC address
        /// </summary>
        public async Task<LoginResponse> LoginAsync(string username, string password, string macAddress)
        {
            FileLogger.Log($"=== LOGIN ATTEMPT ===");
            FileLogger.Log($"Username: {username}");
            FileLogger.Log($"MAC Address: {macAddress}");
            
            try
            {
                var request = new LoginRequest
                {
                    Username = username,
                    Password = password,
                    MacAddress = macAddress
                };

                // Add MAC address to header as well
                _httpClient.DefaultRequestHeaders.Remove("X-Device-Mac");
                _httpClient.DefaultRequestHeaders.Add("X-Device-Mac", macAddress);

                FileLogger.Log($"Sending POST request to {_baseUrl}/api/auth/login");
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
                var rawContent = await response.Content.ReadAsStringAsync();
                
                // Log raw response for debugging
                FileLogger.Log($"=== LOGIN RESPONSE ===");
                FileLogger.Log($"Status Code: {response.StatusCode}");
                FileLogger.Log($"Content Type: {response.Content.Headers.ContentType}");
                FileLogger.Log($"Raw Content Length: {rawContent.Length} bytes");
                FileLogger.Log($"Raw Content: {rawContent}");
                FileLogger.Log($"=====================");
                
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        FileLogger.Log("Attempting to parse success response as JSON...");
                        var result = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(rawContent, 
                            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        
                        if (result != null)
                        {
                            FileLogger.Log($"Parse successful. Success={result.Success}, Message={result.Message}");
                        }
                        
                        return result ?? new LoginResponse { Success = false, Message = "Empty response from server" };
                    }
                    catch (Exception ex)
                    {
                        FileLogger.LogError("Failed to parse success response", ex);
                        return new LoginResponse 
                        { 
                            Success = false, 
                            Message = $"Failed to parse success response: {ex.Message}\n\nRaw: {rawContent}" 
                        };
                    }
                }
                else
                {
                    FileLogger.Log($"Non-success status code: {response.StatusCode}");
                    
                    // Try to parse JSON error response
                    try
                    {
                        FileLogger.Log("Attempting to parse error response as JSON...");
                        var errorResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(rawContent,
                            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        
                        if (errorResponse != null)
                        {
                            FileLogger.Log($"Error parse successful. Success={errorResponse.Success}, Message={errorResponse.Message}");
                            return errorResponse;
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLogger.LogError("JSON Parse Error", ex);
                    }
                    
                    return new LoginResponse
                    {
                        Success = false,
                        Message = $"Server error ({response.StatusCode}): {rawContent}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Network error: {ex.Message}\n\nMake sure the server is running at {_baseUrl}"
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Register device with username, password, MAC address
        /// </summary>
        public async Task<DeviceRegistrationResponse> RegisterDeviceAsync(string username, string password, string macAddress, string deviceName)
        {
            try
            {
                var request = new DeviceRegistrationRequest
                {
                    Username = username,
                    Password = password,
                    MacAddress = macAddress,
                    DeviceName = deviceName,
                    CertificateThumbprint = GetClientCertificateThumbprint()
                };

                var response = await _httpClient.PostAsJsonAsync("/api/auth/register-device", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<DeviceRegistrationResponse>();
                    return result ?? new DeviceRegistrationResponse { Success = false, Message = "Empty response from server" };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new DeviceRegistrationResponse
                    {
                        Success = false,
                        Message = $"Registration failed: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new DeviceRegistrationResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Check device registration status
        /// </summary>
        public async Task<DeviceStatusResponse> GetDeviceStatusAsync(Guid deviceId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/auth/device-status/{deviceId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<DeviceStatusResponse>();
                    return result ?? new DeviceStatusResponse { Success = false, Message = "Empty response" };
                }
                else
                {
                    return new DeviceStatusResponse
                    {
                        Success = false,
                        Message = $"Failed: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Network error checking device status", ex);
                return new DeviceStatusResponse
                {
                    Success = false,
                    Message = $"Network error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Load client certificate from Windows Certificate Store
        /// Looks for certificates with "StationCheck" in subject name
        /// </summary>
        private X509Certificate2? LoadClientCertificate()
        {
            try
            {
                using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);

                var certs = store.Certificates
                    .Find(X509FindType.FindBySubjectName, "StationCheck", false);

                if (certs.Count > 0)
                {
                    // Return the first valid certificate
                    foreach (X509Certificate2 cert in certs)
                    {
                        if (cert.HasPrivateKey && cert.NotAfter > DateTime.Now)
                        {
                            return cert;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading certificate: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Get thumbprint of loaded client certificate
        /// </summary>
        private string? GetClientCertificateThumbprint()
        {
            var cert = LoadClientCertificate();
            return cert?.Thumbprint;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    #region Request/Response Models

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? MacAddress { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public UserDto? User { get; set; }
        public bool RequiresDeviceRegistration { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Role { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public Guid? StationId { get; set; }
    }

    public class DeviceRegistrationRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string? DeviceName { get; set; }
        public string? CertificateThumbprint { get; set; }
    }

    public class DeviceRegistrationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DeviceStatus Status { get; set; }
        public Guid? DeviceId { get; set; }
    }

    public class DeviceStatusResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DeviceStatus Status { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUserAssigned { get; set; }
        public string? DeviceName { get; set; }
        public string? MacAddress { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
    }

    public enum DeviceStatus
    {
        PendingApproval = 0,
        Approved = 1,
        Rejected = 2
    }

    #endregion
}
