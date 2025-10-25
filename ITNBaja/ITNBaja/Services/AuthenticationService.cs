using ITNBaja.Models;
using Microsoft.Extensions.Options;

namespace ITNBaja.Services
{
    public class AuthenticationService
    {
        private readonly AdminCredentialsOptions _adminCredentials;
        
        public AuthenticationService(IOptions<AdminCredentialsOptions> adminCredentials)
        {
            _adminCredentials = adminCredentials.Value;
        }
        
        public bool ValidateCredentials(string username, string password)
        {
            return _adminCredentials.Any(user => 
                user.Username == username && user.Password == password);
        }
    }
}