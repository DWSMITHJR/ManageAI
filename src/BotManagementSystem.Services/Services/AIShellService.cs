using BotManagementSystem.Core.Interfaces;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;

namespace BotManagementSystem.Services.Services;

public class AIShellService : IAIShell
{
    private readonly AzureOpenAIClient _openAIClient;
    private readonly string _defaultModel;

    public AIShellService(AzureOpenAIClient openAIClient, IConfiguration configuration)
    {
        _openAIClient = openAIClient;
        _defaultModel = configuration["AIShell:DefaultModel"] ?? "gpt-3.5-turbo";
    }

    public async Task<string> CompletePromptAsync(string prompt)
    {
        var chatClient = _openAIClient.GetChatClient(_defaultModel);
        
        var messages = new List<ChatMessage>
        {
            new UserChatMessage(prompt)
        };

        var response = await chatClient.CompleteChatAsync(messages);
        return response.Value.Content[0].Text;
    }
}
