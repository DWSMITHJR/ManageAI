using BotManagementSystem.Core.Entities;
using BotManagementSystem.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BotManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BotsController : ControllerBase
    {
        private readonly IBotService _botService;
        private readonly ILogger<BotsController> _logger;

        public BotsController(IBotService botService, ILogger<BotsController> logger)
        {
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBots()
        {
            try
            {
                var bots = await _botService.GetAllBotsAsync();
                return Ok(bots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all bots");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBotById(Guid id)
        {
            try
            {
                var bot = await _botService.GetBotByIdAsync(id);
                if (bot == null)
                {
                    return NotFound();
                }
                return Ok(bot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting bot with ID {BotId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBot([FromBody] Bot bot)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdBot = await _botService.CreateBotAsync(bot);
                return CreatedAtAction(nameof(GetBotById), new { id = createdBot.Id }, createdBot);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new bot");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBot(Guid id, [FromBody] Bot bot)
        {
            try
            {
                if (id != bot.Id)
                {
                    return BadRequest("ID in the URL does not match the bot ID.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _botService.UpdateBotAsync(bot);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating bot with ID {BotId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBot(Guid id)
        {
            try
            {
                var success = await _botService.DeleteBotAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting bot with ID {BotId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{id}/toggle-status")]
        public async Task<IActionResult> ToggleBotStatus(
            Guid id, 
            [FromQuery, Required] bool isActive)
        {
            try
            {
                var success = await _botService.ToggleBotStatusAsync(id, isActive);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling status for bot with ID {BotId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
