using Microsoft.AspNetCore.Mvc;
using ITNBaja.Services;
using ITNBaja.Attributes;

namespace ITNBaja.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authService;
        private readonly TokenService _tokenService;
        
        public AuthController(AuthenticationService authService, TokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }
        
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Ok(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Пожалуйста, заполните все поля." 
                });
            }
            
            if (_authService.ValidateCredentials(request.Username, request.Password))
            {
                // Generate token
                var token = _tokenService.GenerateToken(request.Username);
                
                // Also set session for backward compatibility
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("Username", request.Username);
                HttpContext.Session.SetString("Token", token);
                
                return Ok(new LoginResponse 
                { 
                    Success = true, 
                    Message = "Успешный вход в систему.",
                    Token = token
                });
            }
            
            return Ok(new LoginResponse 
            { 
                Success = false, 
                Message = "Неверное имя пользователя или пароль." 
            });
        }
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Get token from session or header
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader != null && authHeader.StartsWith("Token "))
                {
                    token = authHeader.Substring("Token ".Length).Trim();
                }
            }
            
            // Revoke token
            if (!string.IsNullOrEmpty(token))
            {
                _tokenService.RevokeToken(token);
            }
            
            HttpContext.Session.Clear();
            return Ok(new { Success = true });
        }
        
        [HttpGet("status")]
        public IActionResult GetAuthStatus()
        {
            // Check token first
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Token "))
            {
                var token = authHeader.Substring("Token ".Length).Trim();
                if (_tokenService.ValidateToken(token, out string tokenUsername))
                {
                    return Ok(new { IsAuthenticated = true, Username = tokenUsername });
                }
            }
            
            // Fallback to session
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated") == "true";
            var username = HttpContext.Session.GetString("Username") ?? "";
            
            return Ok(new { IsAuthenticated = isAuthenticated, Username = username });
        }
        
        [HttpGet("validate")]
        [TokenAuth]
        public IActionResult ValidateToken()
        {
            var username = HttpContext.Items["Username"]?.ToString() ?? "";
            return Ok(new { IsValid = true, Username = username });
        }
    }
}