using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using ITNBaja.Controllers.Responses;

namespace ITNBaja.Services
{
    public class AuthStateService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NavigationManager _navigation;
        private bool _isAuthenticated = false;
        private string _username = "";

        public AuthStateService(IHttpClientFactory httpClientFactory, NavigationManager navigation)
        {
            _httpClientFactory = httpClientFactory;
            _navigation = navigation;
        }

        public bool IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }
        public string Username => _username;

        public event Action? OnAuthStateChanged;

        public async Task CheckAuthStatusAsync()
        {
            var wasAuthenticated = _isAuthenticated;
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                
                var baseUri = _navigation.BaseUri;
                var fullUrl = new Uri(new Uri(baseUri), "api/auth/status").ToString();
                Console.WriteLine($"Checking auth status at: {fullUrl}");

                var response = await httpClient.GetAsync(fullUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthStatusResponse>();
                    _isAuthenticated = true;
                    _username = result?.Username ?? "";
                }
                else
                {
                    _isAuthenticated = false;
                    _username = "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking auth status: {ex.Message}");
                _isAuthenticated = false;
                _username = "";
            }

            if (wasAuthenticated != _isAuthenticated)
            {
                OnAuthStateChanged?.Invoke();
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                
                var baseUri = _navigation.BaseUri;
                var fullUrl = new Uri(new Uri(baseUri), "api/auth/logout").ToString();

                await httpClient.PostAsync(fullUrl, null);
                _isAuthenticated = false;
                _username = "";
                OnAuthStateChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during logout: {ex.Message}");
            }
        }
    }
}