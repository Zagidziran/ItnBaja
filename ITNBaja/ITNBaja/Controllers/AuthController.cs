using ITNBaja.Attributes;
using ITNBaja.Controllers.Requests;
using ITNBaja.Controllers.Responses;
using ITNBaja.Services;
using Microsoft.AspNetCore.Mvc;
using YamlDotNet.Core.Tokens;

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
                return BadRequest("Пожалуйста, заполните все поля.");
            }
            
            if (_authService.ValidateCredentials(request.Username, request.Password))
            {
                // Generate token
                var token = _tokenService.GenerateToken(request.Username);

                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("Username", request.Username); 
                HttpContext.Session.SetString("Token", token);
                
                return Ok(new LoginResponse(token));
            }
            
            return Unauthorized("Неверное имя пользователя или пароль.");
        }
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var token = HttpContext.Session.GetString("Token");
            _tokenService.RevokeToken(token);
            HttpContext.Session.Clear();

            return Ok();
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
                    return Ok(new AuthStatusReponse(tokenUsername));
                }
            }
            
            // Fallback to session
            var sessionToken = HttpContext.Session.GetString("Token");
            if (_tokenService.ValidateToken(sessionToken, out string sessionTokenUsername))
            {
                return Ok(new AuthStatusReponse(sessionTokenUsername));
            }

            return Unauthorized();
        }
    }
}