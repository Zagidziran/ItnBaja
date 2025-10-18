using Microsoft.AspNetCore.Mvc;

namespace ITNBaja.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        
        public ScheduleController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        
        [HttpGet("yaml")]
        public async Task<IActionResult> GetYamlContent()
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "schedule.yaml");
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Schedule file not found");
                }
                
                var content = await System.IO.File.ReadAllTextAsync(filePath);
                return Ok(new { Content = content });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error reading file: {ex.Message}");
            }
        }
        
        [HttpPost("yaml")]
        public async Task<IActionResult> SaveYamlContent([FromBody] SaveYamlRequest request)
        {
            // Check session-based authentication
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                return Unauthorized(new { Message = "Authentication required" });
            }

            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "schedule.yaml");
                await System.IO.File.WriteAllTextAsync(filePath, request.Content);
                return Ok(new { Success = true, Message = "Schedule saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving file: {ex.Message}");
            }
        }
    }
    
    public class SaveYamlRequest
    {
        public string Content { get; set; } = "";
    }
}