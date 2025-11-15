using Microsoft.AspNetCore.Mvc;
using BotManagementSystem.Core.Interfaces;

namespace BotManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIShellTestController : ControllerBase
{
    private readonly IAIShell _aiShell;

    public AIShellTestController(IAIShell aiShell)
    {
        _aiShell = aiShell;
    }

    [HttpGet("test")]
    public async Task<IActionResult> TestAIShell()
    {
        try
        {
            var response = await _aiShell.CompletePromptAsync("Hello, AIShell! Are you working?");
            return Ok(new { 
                Success = true, 
                Response = response,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                Success = false, 
                Error = ex.Message,
                Details = ex.ToString(),
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
