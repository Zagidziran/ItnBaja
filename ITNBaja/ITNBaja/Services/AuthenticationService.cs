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
}