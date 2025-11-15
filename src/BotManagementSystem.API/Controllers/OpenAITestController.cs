using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using Azure;
using OpenAI.Chat;

namespace BotManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpenAITestController : ControllerBase
{
    private readonly AzureOpenAIClient _openAIClient;
    private readonly IConfiguration _configuration;

    public OpenAITestController(AzureOpenAIClient openAIClient, IConfiguration configuration)
    {
        _openAIClient = openAIClient;
        _configuration = configuration;
    }

    [HttpGet("test")]
    public async Task<IActionResult> TestOpenAI()
    {
        try
        {
            var modelName = _configuration["OpenAI:ModelId"] ?? "gpt-3.5-turbo";
            var chatClient = _openAIClient.GetChatClient(modelName);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful AI assistant."),
                new UserChatMessage("Hello, can you hear me?")
            };

            var response = await chatClient.CompleteChatAsync(messages);
            var message = response.Value.Content[0].Text;
            
            return Ok(new
            {
                Success = true,
                Response = message,
                Model = modelName,
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
