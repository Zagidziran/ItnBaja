namespace ITNBaja.Services
{
    public class AuthenticationService
    {
        private const string ADMIN_USERNAME = "admin";
        private const string ADMIN_PASSWORD = "aBajaYou<3";
        
        public bool ValidateCredentials(string username, string password)
        {
            return username == ADMIN_USERNAME && password == ADMIN_PASSWORD;
        }
    }
    
    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
    
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public string Token { get; set; } = "";
    }
}