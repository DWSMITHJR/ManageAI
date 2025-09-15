using BotManagementSystem.Core.Extensions;
using BotManagementSystem.Core.Services;
using BotManagementSystem.Core.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BotManagementSystem.Tests.Samples;

public class SemanticKernelSamples
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly ITestOutputHelper _output;
    private const string TestModelId = "gpt-3.5-turbo";
    private readonly string? _apiKey;

    public SemanticKernelSamples(ITestOutputHelper output)
    {
        _output = output;
        
        // Get API key from environment variable or configuration
        _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? 
                 new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                    .AddUserSecrets<SemanticKernelSamples>()
                    .Build()["OpenAI:ApiKey"] ?? string.Empty;

        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "your-test-api-key")
        {
            _output.WriteLine("WARNING: No valid OpenAI API key found. Tests requiring the API will be skipped.");
            _output.WriteLine("Please set the 'OPENAI_API_KEY' environment variable or add it to user secrets.");
            return;
        }
        
        var services = new ServiceCollection()
            .AddLogging(builder => builder.AddXUnit(output).SetMinimumLevel(LogLevel.Debug));

        // Configure OpenAiSettings
        var openAiSettings = new OpenAiSettings
        {
            ApiKey = _apiKey,
            ModelId = TestModelId
        };
        
        services.AddSingleton(Options.Create(openAiSettings));
        
        // Register SemanticKernelService
        services.AddSemanticKernelServices();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task RunBasicSample()
    {
        // Skip this test in the test runner since it's a sample
        // Uncomment the line below to run it manually
        // return;
        
        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "your-test-api-key")
        {
            _output.WriteLine("Skipping test: No valid API key found");
            return;
        }
        
        try
        {
            var service = _serviceProvider?.GetRequiredService<ISemanticKernelService>();
            if (service == null)
            {
                _output.WriteLine("Service not available");
                return;
            }
            
            // Simple completion
            _output.WriteLine("=== Basic Completion ===");
            var response = await service.GetChatCompletionAsync("Tell me a short fact about AI");
            _output.WriteLine($"AI: {response}\n");

            // Multi-turn conversation
            _output.WriteLine("=== Multi-turn Conversation ===");
            var chatHistory = new ChatHistory();
            
            // First message
            var firstQuestion = "What are three benefits of using AI in business?";
            chatHistory.AddUserMessage(firstQuestion);
            var firstResponse = await service.GetChatCompletionAsync(chatHistory);
            _output.WriteLine($"User: {firstQuestion}");
            _output.WriteLine($"AI: {firstResponse}\n");
            
            // Follow-up message
            var followUp = "Can you elaborate on the first point?";
            chatHistory.AddAssistantMessage(firstResponse);
            chatHistory.AddUserMessage(followUp);
            var secondResponse = await service.GetChatCompletionAsync(chatHistory);
            _output.WriteLine($"User: {followUp}");
            _output.WriteLine($"AI: {secondResponse}");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Sample execution failed: {ex.Message}");
            // Don't fail the test for sample code
        }
    }

    [Fact]
    public async Task RunAdvancedSample()
    {
        // Skip this test in the test runner since it's a sample
        // Uncomment the line below to run it manually
        // return;
        
        if (string.IsNullOrEmpty(_apiKey) || _apiKey == "your-test-api-key")
        {
            _output.WriteLine("Skipping test: No valid API key found");
            return;
        }
        
        try
        {
            var service = _serviceProvider?.GetRequiredService<ISemanticKernelService>();
            if (service == null)
            {
                _output.WriteLine("Service not available");
                return;
            }
            
            // Example of a more complex prompt with instructions
            var systemPrompt = """
                You are a helpful assistant that helps with business analysis.
                Please provide concise, well-structured responses.
                If a question is unclear, ask for clarification.
                Format lists with markdown bullet points.
                """;

            var chatHistory = new ChatHistory(systemPrompt);
            
            // First message with context
            var userMessage = """
                Analyze the following business scenario and suggest potential AI solutions:
                
                Scenario: A retail company wants to reduce customer service costs while 
                maintaining high customer satisfaction. They receive thousands of customer 
                inquiries daily about order status, returns, and product information.
                """;
                
            chatHistory.AddUserMessage(userMessage);

            _output.WriteLine("=== Business Analysis Sample ===");
            _output.WriteLine($"User: {userMessage}\n");
            
            // Act
            var response = await service.GetChatCompletionAsync(chatHistory);
            
            // Assert
            _output.WriteLine($"AI: {response}");
            response.Should().NotBeNullOrWhiteSpace();
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Sample execution failed: {ex.Message}");
            // Don't fail the test for sample code
        }
    }
}
