using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace ITNBaja.Services
{
    public class AuthStateService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigation;
        private bool _isAuthenticated = false;
        private string _username = "";
        
        public AuthStateService(HttpClient httpClient, NavigationManager navigation)
        {
            _httpClient = httpClient;
            _navigation = navigation;
        }
        
        public bool IsAuthenticated => _isAuthenticated;
        public string Username => _username;
        
        public event Action? OnAuthStateChanged;
        
        public async Task CheckAuthStatusAsync()
        {
            try
            {
                var baseUri = _navigation.BaseUri;
                var fullUrl = new Uri(new Uri(baseUri), "api/auth/status").ToString();
                Console.WriteLine($"Checking auth status at: {fullUrl}");
                
                // Create request with token if available
                var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
                
                // Try to get token from localStorage (this won't work from server-side)
                // For now, we'll rely on session-based auth for server-side components
                
                var response = await _httpClient.SendAsync(request);
                var authResponse = await response.Content.ReadFromJsonAsync<AuthStatusResponse>();
                
                if (authResponse != null)
                {
                    var wasAuthenticated = _isAuthenticated;
                    _isAuthenticated = authResponse.IsAuthenticated;
                    _username = authResponse.Username;
                    
                    Console.WriteLine($"Auth status: {_isAuthenticated}, Username: {_username}");
                    
                    // Only trigger event if state actually changed
                    if (wasAuthenticated != _isAuthenticated)
                    {
                        OnAuthStateChanged?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking auth status: {ex.Message}");
                _isAuthenticated = false;
                _username = "";
                OnAuthStateChanged?.Invoke();
            }
        }
        
        public async Task LogoutAsync()
        {
            try
            {
                var baseUri = _navigation.BaseUri;
                var fullUrl = new Uri(new Uri(baseUri), "api/auth/logout").ToString();
                
                await _httpClient.PostAsync(fullUrl, null);
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
    
    public class AuthStatusResponse
    {
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; } = "";
    }
}