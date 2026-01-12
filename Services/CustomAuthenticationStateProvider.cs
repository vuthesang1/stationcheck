using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace StationCheck.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt_token");
                
                if (string.IsNullOrEmpty(token))
                {
                    return new AuthenticationState(_anonymous);
                }

                var userInfoJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "user_info");
                
                if (string.IsNullOrEmpty(userInfoJson))
                {
                    return new AuthenticationState(_anonymous);
                }

                var userInfo = JsonSerializer.Deserialize<UserInfoDto>(userInfoJson);
                
                if (userInfo == null)
                {
                    return new AuthenticationState(_anonymous);
                }

                var claimsList = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                    new Claim(ClaimTypes.Name, userInfo.Username),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.Role, userInfo.Role),
                    new Claim("FullName", userInfo.FullName)
                };

                // Decode JWT token to extract CertificateThumbprint
                var certificateThumbprint = ExtractCertificateThumbprintFromJwt(token);
                if (!string.IsNullOrEmpty(certificateThumbprint))
                {
                    claimsList.Add(new Claim("CertificateThumbprint", certificateThumbprint));
                }

                var identity = new ClaimsIdentity(claimsList, "jwt");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        private string? ExtractCertificateThumbprintFromJwt(string token)
        {
            try
            {
                // JWT format: header.payload.signature
                var parts = token.Split('.');
                if (parts.Length != 3)
                    return null;

                // Decode the payload (second part)
                var payload = parts[1];
                
                // Base64Url decode
                var base64 = payload.Replace('-', '+').Replace('_', '/');
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }

                var bytes = Convert.FromBase64String(base64);
                var json = System.Text.Encoding.UTF8.GetString(bytes);

                // Parse JSON to extract CertificateThumbprint
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("CertificateThumbprint", out var thumbprintElement))
                {
                    return thumbprintElement.GetString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task NotifyUserAuthentication(string token, Models.UserInfo userInfo)
        {
            // Save to localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwt_token", token);
            var userInfoDto = new UserInfoDto
            {
                Id = userInfo.Id,
                Username = userInfo.Username,
                Email = userInfo.Email,
                FullName = userInfo.FullName,
                Role = userInfo.Role.ToString()
            };
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user_info", JsonSerializer.Serialize(userInfoDto));
            
            var claimsList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.Role, userInfo.Role.ToString()),
                new Claim("FullName", userInfo.FullName)
            };

            // Extract certificate thumbprint from JWT
            var certificateThumbprint = ExtractCertificateThumbprintFromJwt(token);
            if (!string.IsNullOrEmpty(certificateThumbprint))
            {
                claimsList.Add(new Claim("CertificateThumbprint", certificateThumbprint));
            }

            var identity = new ClaimsIdentity(claimsList, "jwt");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task NotifyUserLogout()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwt_token");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user_info");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refresh_token");
            
            var authState = Task.FromResult(new AuthenticationState(_anonymous));
            NotifyAuthenticationStateChanged(authState);
        }

        public class UserInfoDto
        {
            public string Id { get; set; } = "";
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string FullName { get; set; } = "";
            public string Role { get; set; } = "";
        }
    }
}
