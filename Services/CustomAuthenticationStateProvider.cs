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

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                    new Claim(ClaimTypes.Name, userInfo.Username),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.Role, userInfo.Role),
                    new Claim("FullName", userInfo.FullName)
                };

                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
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
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.Role, userInfo.Role.ToString()),
                new Claim("FullName", userInfo.FullName)
            };

            var identity = new ClaimsIdentity(claims, "jwt");
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
