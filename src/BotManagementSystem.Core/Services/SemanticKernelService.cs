using BotManagementSystem.Core.Validation;
using BotManagementSystem.Core.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.SemanticKernel.Embeddings;
using Polly;
using Polly.Retry;

namespace BotManagementSystem.Core.Services;

public interface ISemanticKernelService
{
    Task<string> GetChatCompletionAsync(string prompt);
    Task<string> GetChatCompletionAsync(ChatHistory chatHistory);
}

public class SemanticKernelService : ISemanticKernelService
{
    private readonly Kernel _kernel;
    
    private readonly ILogger<SemanticKernelService> _logger;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly AsyncRetryPolicy _retryPolicy;

    private readonly OpenAIPromptExecutionSettings _promptExecutionSettings;

    public SemanticKernelService(
        IOptions<OpenAiSettings> openAiSettings,
        ILogger<SemanticKernelService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var settings = openAiSettings.Value;
        _promptExecutionSettings = settings.PromptExecutionSettings;
        
        try
        {
            if (string.IsNullOrWhiteSpace(settings.ApiKey))
            {
                throw new ArgumentException("API key cannot be null or whitespace.", nameof(settings.ApiKey));
            }

            var builder = Kernel.CreateBuilder();
            
            builder.AddOpenAIChatCompletion(
                modelId: settings.ModelId,
                apiKey: settings.ApiKey);
            
            _kernel = builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            // Configure retry policy with exponential backoff
            _retryPolicy = Policy
                .Handle<HttpRequestException>(ex => ex.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
                .Or<InvalidOperationException>(ex => ex.Message.Contains("The AI service is currently unavailable"))
                .Or<TimeoutException>()
                .Or<OperationCanceledException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, delay, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Retry {RetryCount} after {Delay}ms due to: {Message}", 
                            retryCount, delay.TotalMilliseconds, exception.Message);
                    });

            _logger.LogInformation("SemanticKernelService initialized with model: {ModelId}", settings.ModelId);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            _logger.LogCritical(ex, "Failed to initialize SemanticKernelService");
            throw new InvalidOperationException("Failed to initialize the AI service. Please check your configuration and try again.", ex);
        }
    }

    public async Task<string> GetChatCompletionAsync(string prompt)
    {
        try
        {
            // Input validation
            SemanticKernelValidation.ValidatePrompt(prompt);

            _logger.LogDebug("Sending prompt to AI service. Length: {Length} characters", prompt.Length);
            
            return await _retryPolicy.ExecuteAsync(async () => 
            {
                try
                {
                    var result = await _chatCompletionService.GetChatMessageContentsAsync(
                        prompt,
                        _promptExecutionSettings,
                        _kernel);

                    var response = result?.FirstOrDefault()?.Content ?? string.Empty;
                    _logger.LogDebug("Received AI response. Length: {Length} characters", response.Length);
                    return response;
                }
                catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized || httpEx.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogError("Authentication failed. Please check your API key and permissions.");
                    throw new UnauthorizedAccessException("Authentication failed. Please check your API key and permissions.", httpEx);
                }
                catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("Rate limit exceeded. Please wait before making more requests.");
                    throw new InvalidOperationException("Rate limit exceeded. Please wait before making more requests.", httpEx);
                }
                catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError("The requested resource was not found. Please check the model ID and endpoint.");
                    throw new KeyNotFoundException("The requested AI model was not found. Please check your configuration.", httpEx);
                }
                catch (HttpRequestException httpEx) when (httpEx.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogWarning("The AI service is currently unavailable. Please try again later.");
                    throw new InvalidOperationException("The AI service is currently unavailable. Please try again later.", httpEx);
                }
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid input: {Message}", ex.Message);
            throw;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning("The operation was canceled: {Message}", ex.Message);
            throw new TimeoutException("The request timed out. Please check your connection and try again.", ex);
        }
        catch (Exception ex)
        {
            if (ex is UnauthorizedAccessException || ex is KeyNotFoundException || ex is InvalidOperationException)
            {
                throw;
            }
            _logger.LogError(ex, "Error getting chat completion for prompt. Length: {Length} characters", prompt?.Length ?? 0);
            throw new InvalidOperationException("An error occurred while processing your request. Please try again later.", ex);
        }
    }

    public async Task<string> GetChatCompletionAsync(ChatHistory chatHistory)
    {
        try
        {
            // Input validation
            SemanticKernelValidation.ValidateChatHistory(chatHistory);

            _logger.LogDebug("Sending chat history to AI service. Message count: {Count}", chatHistory.Count);
            
            return await _retryPolicy.ExecuteAsync(async () => 
            {
                try
                {
                    var result = await _chatCompletionService.GetChatMessageContentsAsync(
                        chatHistory,
                        _promptExecutionSettings,
                        _kernel);

                    var response = result?.FirstOrDefault()?.Content ?? string.Empty;
                    _logger.LogDebug("Received AI response. Length: {Length} characters", response.Length);
                    return response;
                }
                catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized || httpEx.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    _logger.LogError("Authentication failed. Please check your API key and permissions.");
                    throw new UnauthorizedAccessException("Authentication failed. Please check your API key and permissions.", httpEx);
                }
                catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("Rate limit exceeded. Please wait before making more requests.");
                    throw new InvalidOperationException("Rate limit exceeded. Please wait before making more requests.", httpEx);
                }
                catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError("The requested resource was not found. Please check the model ID and endpoint.");
                    throw new KeyNotFoundException("The requested AI model was not found. Please check your configuration.", httpEx);
                }
                catch (HttpRequestException httpEx) when (httpEx.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogWarning("The AI service is currently unavailable. Please try again later.");
                    throw new InvalidOperationException("The AI service is currently unavailable. Please try again later.", httpEx);
                }
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid input: {Message}", ex.Message);
            throw;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning("The operation was canceled: {Message}", ex.Message);
            throw new TimeoutException("The request timed out. Please check your connection and try again.", ex);
        }
        catch (Exception ex)
        {
            if (ex is UnauthorizedAccessException || ex is KeyNotFoundException || ex is InvalidOperationException)
            {
                throw;
            }
            _logger.LogError(ex, "Error getting chat completion for chat history. Message count: {Count}", chatHistory?.Count ?? 0);
            throw new InvalidOperationException("An error occurred while processing your request. Please try again later.", ex);
        }
    }

}
