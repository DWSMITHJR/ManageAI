using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Logging;

namespace BotManagementSystem.Core.Services;

public interface ISemanticKernelService
{
    Task<string> GetChatCompletionAsync(string prompt);
    Task<string> GetChatCompletionAsync(ChatHistory chatHistory);
}

public class SemanticKernelService : ISemanticKernelService, IDisposable
{
    private readonly IKernelBuilder _kernelBuilder;
    private bool _disposed = false;
    private readonly ILogger<SemanticKernelService> _logger;

    public SemanticKernelService(
        string apiKey,
        string modelId = "gpt-35-turbo",
        string? endpoint = null,
        ILogger<SemanticKernelService>? logger = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _kernelBuilder = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(
                modelId: modelId,
                apiKey: apiKey,
                httpClient: null,
                endpoint: endpoint);

        _logger.LogInformation("SemanticKernelService initialized with model: {ModelId}", modelId);
    }

    public async Task<string> GetChatCompletionAsync(string prompt)
    {
        try
        {
            var kernel = _kernelBuilder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            
            var result = await chatCompletionService.GetChatMessageContentAsync(
                prompt,
                executionSettings: new OpenAIPromptExecutionSettings 
                { 
                    MaxTokens = 2000,
                    Temperature = 0.7,
                    TopP = 0.8
                });

            return result.Content ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat completion for prompt: {Prompt}", prompt);
            throw;
        }
    }

    public async Task<string> GetChatCompletionAsync(ChatHistory chatHistory)
    {
        try
        {
            var kernel = _kernelBuilder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            
            var result = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings: new OpenAIPromptExecutionSettings 
                { 
                    MaxTokens = 2000,
                    Temperature = 0.7,
                    TopP = 0.8
                });

            return result.Content ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat completion for chat history");
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects)
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
