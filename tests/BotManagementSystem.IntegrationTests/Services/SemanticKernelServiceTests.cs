using BotManagementSystem.Core.Extensions;
using BotManagementSystem.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace BotManagementSystem.IntegrationTests.Services;

public class SemanticKernelServiceTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ITestOutputHelper _output;
    private const string TestApiKey = "test-api-key";
    private const string TestModelId = "test-model";

    public SemanticKernelServiceTests(ITestOutputHelper output)
    {
        _output = output;
        
        // Setup configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenAI:ApiKey"] = TestApiKey,
                ["OpenAI:ModelId"] = TestModelId
            })
            .Build();

        // Setup services
        var services = new ServiceCollection()
            .AddLogging(builder => builder.AddXUnit(output).SetMinimumLevel(LogLevel.Debug))
            .AddSingleton<IConfiguration>(configuration);

        // Add SemanticKernelService with test configuration
        services.AddSemanticKernelServices(TestApiKey, TestModelId);
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task GetChatCompletionAsync_WithValidPrompt_ReturnsResponse()
    {
        // Skip test if no API key is configured
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENAI_API_KEY")))
        {
            _output.WriteLine("Skipping test: No OpenAI API key configured");
            return;
        }

        // Arrange
        var service = _serviceProvider.GetRequiredService<ISemanticKernelService>();
        var prompt = "Say 'Hello, World!'";

        // Act
        var result = await service.GetChatCompletionAsync(prompt);

        // Assert
        Assert.NotNull(result);
        _output.WriteLine($"AI Response: {result}");
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}
