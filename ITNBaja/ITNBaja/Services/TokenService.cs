using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace ITNBaja.Services
{
    public class TokenService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _tokenExpiry = TimeSpan.FromHours(1);
        
        public TokenService(IMemoryCache cache)
        {
            _cache = cache;
        }
        
        public string GenerateToken(string username)
        {
            // Generate a secure random token
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            
            var token = Convert.ToBase64String(tokenBytes);
            
            // Store token in cache with expiry
            var tokenInfo = new TokenInfo
            {
                Username = username,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(_tokenExpiry)
            };
            
            _cache.Set($"token_{token}", tokenInfo, _tokenExpiry);
            
            return token;
        }
        
        public bool ValidateToken(string token, out string username)
        {
            username = "";
            
            if (string.IsNullOrEmpty(token))
                return false;
                
            if (_cache.TryGetValue($"token_{token}", out TokenInfo tokenInfo))
            {
                if (tokenInfo.ExpiresAt > DateTime.UtcNow)
                {
                    username = tokenInfo.Username;
                    return true;
                }
                else
                {
                    // Token expired, remove from cache
                    _cache.Remove($"token_{token}");
                }
            }
            
            return false;
        }
        
        public void RevokeToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _cache.Remove($"token_{token}");
            }
        }
        
        public void RevokeAllUserTokens(string username)
        {
            // Note: This is a simplified implementation
            // In production, you might want to maintain a user->tokens mapping
            // For now, we'll rely on natural expiration
        }
    }
    
    public class TokenInfo
    {
        public string Username { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}