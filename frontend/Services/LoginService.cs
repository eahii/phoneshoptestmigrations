using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace frontend.Services
{
    public interface ILoginService
    {
        Task<bool> Login(string email, string password);
        Task Logout();
    }

    public class LoginService : ILoginService
    {
        private readonly HttpClient _httpClient;
        private readonly CustomAuthenticationStateProvider _authStateProvider;
        private readonly IJSRuntime _jsRuntime;

        public LoginService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> Login(string email, string password)
        {
            var loginModel = new LoginModel { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
                    _authStateProvider.NotifyUserAuthentication(result.Token);
                    return true;
                }
            }

            return false;
        }

        public async Task Logout()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            _authStateProvider.NotifyUserLogout();
        }
    }

    public class LoginResult
    {
        public string Token { get; set; }
    }
}

namespace Shared.Models
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
