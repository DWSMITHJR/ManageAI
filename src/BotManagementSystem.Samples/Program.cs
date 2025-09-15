using BotManagementSystem.Core.Extensions;
using BotManagementSystem.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Create host builder
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddUserSecrets<Program>();
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        
        // Add Semantic Kernel service
        services.AddSemanticKernelServices(
            configuration["OpenAI:ApiKey"] ?? 
            throw new ArgumentNullException("OpenAI:ApiKey is not configured"),
            configuration["OpenAI:ModelId"] ?? "gpt-3.5-turbo");
            
        // Register the sample app
        services.AddHostedService<SemanticKernelSample>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });
    });

// Build and run the host
await builder.Build().RunAsync();

public class SemanticKernelSample : IHostedService
{
    private readonly ISemanticKernelService _kernelService;
    private readonly ILogger<SemanticKernelSample> _logger;

    public SemanticKernelSample(
        ISemanticKernelService kernelService,
        ILogger<SemanticKernelSample> logger)
    {
        _kernelService = kernelService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Semantic Kernel Sample...");
        
        try
        {
            // Simple completion
            _logger.LogInformation("Testing simple completion...");
            var response = await _kernelService.GetChatCompletionAsync("Tell me a short joke about AI");
            _logger.LogInformation("AI Response: {Response}", response);

            // Multi-turn conversation
            _logger.LogInformation("\nTesting multi-turn conversation...");
            var chatHistory = new Microsoft.SemanticKernel.ChatCompletion.ChatHistory();
            
            // First message
            chatHistory.AddUserMessage("What's the capital of France?");
            var firstResponse = await _kernelService.GetChatCompletionAsync(chatHistory);
            _logger.LogInformation("\nUser: What's the capital of France?");
            _logger.LogInformation("AI: {Response}", firstResponse);
            
            // Follow-up message
            chatHistory.AddAssistantMessage(firstResponse);
            chatHistory.AddUserMessage("What about the capital of Japan?");
            var secondResponse = await _kernelService.GetChatCompletionAsync(chatHistory);
            _logger.LogInformation("\nUser: What about the capital of Japan?");
            _logger.LogInformation("AI: {Response}", secondResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Semantic Kernel sample execution");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
