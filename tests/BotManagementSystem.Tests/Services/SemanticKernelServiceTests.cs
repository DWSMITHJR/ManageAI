using BotManagementSystem.Core.Services;
using BotManagementSystem.Core.Validation;
using BotManagementSystem.Tests.Base;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace BotManagementSystem.Tests.Services
{
    [Trait("Category", "Unit")]
    [Trait("Service", "SemanticKernel")]
    public class SemanticKernelServiceTests : BaseTest, IDisposable
    {
        // Test constants
        private const string TestApiKey = "test-api-key";
        private const string TestModelId = "test-model";
        private const string TestEndpoint = "https://test-endpoint.com";
        private const string TestResponse = "Test response";
        private const string TestQuestion = "What is the capital of France?";
        private const string TestAnswer = "The capital of France is Paris.";
        private const int MaxRetryCount = 3;

        // Mocks and services
        private readonly Mock<ILogger<SemanticKernelService>> _mockLogger;
        private readonly Kernel _kernel;
        private readonly ServiceCollection _services;
        private readonly ServiceProvider _serviceProvider;
        private readonly TestChatCompletionService _testChatCompletionService;
        private bool _disposed;

        // Test data
        public static IEnumerable<object[]> InvalidPrompts => new List<object[]>
        {
            new object[] { null! },
            new object[] { string.Empty },
            new object[] { "   " },
            new object[] { new string('a', 5001) } // Exceeds max length
        };

        public static IEnumerable<object[]> GetHttpErrorTestCases()
        {
            yield return new object[] { HttpStatusCode.Unauthorized, "Authentication failed" };
            yield return new object[] { HttpStatusCode.Forbidden, "Authentication failed" };
            yield return new object[] { HttpStatusCode.TooManyRequests, "Rate limit exceeded" };
            yield return new object[] { HttpStatusCode.NotFound, "The requested resource was not found" };
            yield return new object[] { HttpStatusCode.InternalServerError, "The AI service is currently unavailable" };
            yield return new object[] { HttpStatusCode.BadGateway, "The AI service is currently unavailable" };
            yield return new object[] { HttpStatusCode.ServiceUnavailable, "The AI service is currently unavailable" };
            yield return new object[] { HttpStatusCode.GatewayTimeout, "The AI service is currently unavailable" };
        }

        public SemanticKernelServiceTests(ITestOutputHelper output) : base(output)
        {
            LogTestStep("Initializing SemanticKernelServiceTests");
            
            // Setup mock logger
            _mockLogger = new Mock<ILogger<SemanticKernelService>>();

            // Create a test chat completion service
            _testChatCompletionService = new TestChatCompletionService();

            // Setup service collection
            _services = new ServiceCollection();
            _services.AddSingleton<IChatCompletionService>(_testChatCompletionService);
            _services.AddLogging(builder => builder.AddXUnit(Output).SetMinimumLevel(LogLevel.Debug));
            
            // Build the service provider
            _serviceProvider = _services.BuildServiceProvider();
            
            // Create a test kernel with our test chat completion service
            var builder = Kernel.CreateBuilder();
            builder.Services.AddSingleton<IChatCompletionService>(_testChatCompletionService);
            _kernel = builder.Build();
            
            LogTestStep("Test initialization completed");
        }

        private void SetupMockChatCompletion(string response, bool useChatHistory = true)
        {
            var chatMessageContent = new ChatMessageContent(AuthorRole.Assistant, response);
            _testChatCompletionService.SetResponse(new[] { chatMessageContent });
            LogTestStep($"Test chat completion set up with response: {response}");
        }

        private void SetupMockException(Exception exception, bool useChatHistory = true)
        {
            _testChatCompletionService.SetException(exception);
            LogTestStep($"Test chat completion set up to throw {exception.GetType().Name}");
        }

        private SemanticKernelService CreateService(
            string? endpoint = null,
            bool setupMocks = true,
            bool useChatHistory = true)
        {
            LogTestStep("Creating SemanticKernelService instance");
            
            var service = new SemanticKernelService(
                apiKey: TestApiKey,
                modelId: TestModelId,
                endpoint: endpoint,
                logger: _mockLogger.Object);
            
            if (setupMocks)
            {
                // Use reflection to inject our test chat completion service
                var chatCompletionField = typeof(SemanticKernelService).GetField(
                    "_chatCompletionService",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                var kernelField = typeof(SemanticKernelService).GetField(
                    "_kernel",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (chatCompletionField != null && kernelField != null)
                {
                    chatCompletionField.SetValue(service, _testChatCompletionService);
                    kernelField.SetValue(service, _kernel);
                }
                else
                {
                    throw new InvalidOperationException("Failed to inject test dependencies into SemanticKernelService");
                }
            }
            
            LogTestStep("SemanticKernelService instance created successfully");
            return service;
        }

        #region Constructor Tests
        
        [Fact]
        [Trait("Category", "Initialization")]
        public void Constructor_WithValidParameters_InitializesSuccessfully()
        {
            // Arrange & Act
            var service = new SemanticKernelService(
                apiKey: TestApiKey, 
                modelId: TestModelId,
                endpoint: null,
                logger: _mockLogger.Object);
            
            // Assert
            service.Should().NotBeNull();
            
        }
        
        [Theory]
        [InlineData(null, "modelId")]
        [InlineData("", "modelId")]
        [InlineData("   ", "modelId")]
        [InlineData("apiKey", null)]
        [InlineData("apiKey", "")]
        [InlineData("apiKey", "   ")]
        [Trait("Category", "Validation")]
        public void Constructor_WithInvalidParameters_ThrowsArgumentException(string apiKey, string modelId)
        {
            // Act
            Action act = () => new SemanticKernelService(apiKey, modelId, logger: _mockLogger.Object);
            
            // Assert
            act.Should().Throw<ArgumentException>();
        }
        
        [Fact]
        [Trait("Category", "Initialization")]
        public void Constructor_WithInvalidEndpoint_ThrowsArgumentException()
        {
            // Arrange
            var invalidEndpoint = "not-a-valid-uri";
            
            // Act
            Action act = () => new SemanticKernelService(TestApiKey, TestModelId, invalidEndpoint, logger: _mockLogger.Object);
            
            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid endpoint URL format.*")
                .WithParameterName("endpoint");
        }
        
        #endregion
        
        #region GetChatCompletionAsync(string prompt) Tests
        
        [Fact]
        [Trait("Category", "ChatCompletion")]
        public async Task GetChatCompletionAsync_WithValidPrompt_ReturnsExpectedResponse()
        {
            // Arrange
            SetupMockChatCompletion(TestResponse, useChatHistory: false);
            var service = CreateService();
            
            // Act
            var result = await service.GetChatCompletionAsync(TestQuestion);
            
            // Assert
            result.Should().Be(TestResponse);
            
            // Verify the chat history was updated correctly
            _testChatCompletionService.ChatHistories.Should().HaveCount(1);
            var chatHistory = _testChatCompletionService.ChatHistories[0];
            chatHistory.Should().HaveCount(1);
            chatHistory[0].Role.Should().Be(AuthorRole.User);
            chatHistory[0].Content.Should().Be(TestQuestion);
            
            // Note: We can't directly verify the execution settings with the test service
            // as they're not exposed in the interface. The behavior is tested through the response.
                
            
        }
        
        [Theory]
        [MemberData(nameof(InvalidPrompts))]
        [Trait("Category", "Validation")]
        public async Task GetChatCompletionAsync_WithInvalidPrompt_ThrowsArgumentException(string prompt)
        {
            // Arrange
            var service = CreateService();
            
            // Act
            Func<Task> act = () => service.GetChatCompletionAsync(prompt);
            
            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
        
        [Theory]
        [MemberData(nameof(GetHttpErrorTestCases))]
        [Trait("Category", "ErrorHandling")]
        public async Task GetChatCompletionAsync_WithHttpError_ThrowsAppropriateException(
            HttpStatusCode statusCode, string _) // _ indicates we're not using the expectedMessage parameter
        {
            // Arrange
            var httpEx = new HttpRequestException("Test exception", null, statusCode);
            SetupMockException(httpEx, useChatHistory: false);
            
            var service = CreateService();
            
            // Act
            Func<Task> act = () => service.GetChatCompletionAsync(TestQuestion);
            
            // Assert
            if (statusCode == HttpStatusCode.Unauthorized || statusCode == HttpStatusCode.Forbidden)
            {
                await act.Should().ThrowAsync<UnauthorizedAccessException>()
                    .WithMessage("Authentication failed*");
            }
            else if (statusCode == HttpStatusCode.TooManyRequests)
            {
                await act.Should().ThrowAsync<InvalidOperationException>()
                    .WithMessage("Rate limit exceeded*");
            }
            else if (statusCode == HttpStatusCode.NotFound)
            {
                await act.Should().ThrowAsync<KeyNotFoundException>()
                    .WithMessage("The requested AI model was not found*");
            }
            else
            {
                await act.Should().ThrowAsync<InvalidOperationException>()
                    .WithMessage("The AI service is currently unavailable*");
            }
        }
        
        [Fact]
        [Trait("Category", "ErrorHandling")]
        public async Task GetChatCompletionAsync_WithOperationCanceled_ThrowsTimeoutException()
        {
            // Arrange
            var canceledEx = new OperationCanceledException();
            SetupMockException(canceledEx, useChatHistory: false);
            
            var service = CreateService();
            
            // Act
            Func<Task> act = () => service.GetChatCompletionAsync(TestQuestion);
            
            // Assert
            await act.Should().ThrowAsync<TimeoutException>()
                .WithMessage("The request timed out*");
        }
        
        [Fact]
        [Trait("Category", "Resilience")]
        public async Task GetChatCompletionAsync_WithTransientFailure_RetriesAndSucceeds()
        {
            // Arrange
            var httpEx = new HttpRequestException("Temporary failure", null, HttpStatusCode.InternalServerError);
            
            // Setup the test service to fail twice, then succeed
            _testChatCompletionService.QueueResponse(() => Task.FromException<IReadOnlyList<ChatMessageContent>>(httpEx));
            _testChatCompletionService.QueueResponse(() => Task.FromException<IReadOnlyList<ChatMessageContent>>(httpEx));
            _testChatCompletionService.QueueResponse(() => Task.FromResult<IReadOnlyList<ChatMessageContent>>(
                new[] { new ChatMessageContent(AuthorRole.Assistant, TestResponse) }));
            
            var service = CreateService();
            
            // Act
            var result = await service.GetChatCompletionAsync(TestQuestion);
            
            // Assert
            result.Should().Be(TestResponse);
            _testChatCompletionService.CallCount.Should().Be(3);
            
            // Note: We can't directly verify LogWarning calls to ILogger extension methods with Moq
            // Instead, we verify the behavior through the retry count
        }
        
        #endregion
        
        #region GetChatCompletionAsync(ChatHistory chatHistory) Tests
        
        [Fact]
        [Trait("Category", "ChatCompletion")]
        public async Task GetChatCompletionAsync_WithChatHistory_ReturnsExpectedResponse()
        {
            // Arrange
            var chatHistory = new ChatHistory
            {
                new ChatMessageContent(AuthorRole.User, "Hello!"),
                new ChatMessageContent(AuthorRole.Assistant, "Hi there!"),
                new ChatMessageContent(AuthorRole.User, TestQuestion)
            };
            
            SetupMockChatCompletion(TestAnswer);
            var service = CreateService();
            
            // Act
            var result = await service.GetChatCompletionAsync(chatHistory);
            
            // Assert
            result.Should().Be(TestAnswer);
            
            _testChatCompletionService.ChatHistories.Should().HaveCount(1);
            var capturedHistory = _testChatCompletionService.ChatHistories[0];
            capturedHistory.Should().HaveCount(3);
            capturedHistory[0].Role.Should().Be(AuthorRole.User);
            capturedHistory[0].Content.Should().Be("Hello!");
            capturedHistory[1].Role.Should().Be(AuthorRole.Assistant);
            capturedHistory[1].Content.Should().Be("Hi there!");
            capturedHistory[2].Role.Should().Be(AuthorRole.User);
            capturedHistory[2].Content.Should().Be(TestQuestion);
        }
        
        [Fact]
        [Trait("Category", "Validation")]
        public async Task GetChatCompletionAsync_WithNullChatHistory_ThrowsArgumentNullException()
        {
            // Arrange
            var service = CreateService();
            
            // Act
            Func<Task> act = () => service.GetChatCompletionAsync((ChatHistory)null!);
            
            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
        
        [Fact]
        [Trait("Category", "Validation")]
        public async Task GetChatCompletionAsync_WithEmptyChatHistory_ThrowsArgumentException()
        {
            // Arrange
            var service = CreateService();
            
            // Act
            Func<Task> act = () => service.GetChatCompletionAsync(new ChatHistory());
            
            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Chat history cannot be empty*");
        }
        
        [Fact]
        [Trait("Category", "Validation")]
        public async Task GetChatCompletionAsync_WithTooManyMessages_ThrowsArgumentException()
        {
            // Arrange
            var chatHistory = new ChatHistory();
            for (int i = 0; i < 11; i++) // Exceeds the 10-message limit
            {
                chatHistory.AddUserMessage($"Message {i}");
            }
            
            var service = CreateService();
            
            // Act
            Func<Task> act = () => service.GetChatCompletionAsync(chatHistory);
            
            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Chat history exceeds maximum allowed length*");
        }
        
        #endregion

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    _serviceProvider?.Dispose();
                    _disposed = true;
                }
                catch (Exception ex)
                {
                    // Don't throw in dispose
                    _mockLogger.Object.LogError(ex, "Error during test cleanup");
                }
            }
            
            GC.SuppressFinalize(this);
        }
    }
    
    // Test implementation of IChatCompletionService that doesn't rely on mocking extension methods
    public class TestChatCompletionService : IChatCompletionService
    {
        private IReadOnlyList<ChatMessageContent>? _response;
        private Exception? _exception;
        private readonly List<ChatHistory> _chatHistories = new();
        private readonly List<string> _prompts = new();
        private readonly Queue<Func<Task<IReadOnlyList<ChatMessageContent>>>> _responseQueue = new();
        
        public IReadOnlyList<ChatHistory> ChatHistories => _chatHistories.AsReadOnly();
        public IReadOnlyList<string> Prompts => _prompts.AsReadOnly();
        public int CallCount { get; private set; }
        
        public void SetResponse(IReadOnlyList<ChatMessageContent> response)
        {
            _response = response;
            _exception = null;
            _responseQueue.Clear();
        }
        
        public void QueueResponse(Func<Task<IReadOnlyList<ChatMessageContent>>> responseFactory)
        {
            _responseQueue.Enqueue(responseFactory);
        }
        
        public void SetException(Exception exception)
        {
            _exception = exception;
            _response = null;
        }
        
        public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>(StringComparer.Ordinal);
        
        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            _chatHistories.Add(chatHistory);
            
            if (_exception != null)
                throw _exception;
                
            if (_response == null)
                throw new InvalidOperationException("Response not set");
            
            return GetStreamingChatMessageContentsAsyncImpl();
            
            async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsyncImpl(
                [EnumeratorCancellation] CancellationToken ct = default)
            {
                foreach (var message in _response!)
                {
                    ct.ThrowIfCancellationRequested();
                    
                    yield return new StreamingChatMessageContent(
                        message.Role,
                        message.Content ?? string.Empty,
                        message.ModelId);
                    
                    await Task.Delay(10, ct).ConfigureAwait(false);
                }
            }
        }
        
        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            _chatHistories.Add(chatHistory);
            
            if (_exception != null)
                throw _exception;
                
            if (_responseQueue.Count > 0)
            {
                var responseFactory = _responseQueue.Dequeue();
                return await responseFactory();
            }
            
            return _response ?? new List<ChatMessageContent>();
        }
        
        public IAsyncEnumerable<StreamingTextContent> GetStreamingContentAsync(
            string prompt,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            _prompts.Add(prompt);
            
            if (_exception != null)
                throw _exception;
                
            if (_response == null)
                throw new InvalidOperationException("Response not set");
            
            return GetStreamingContentAsyncImpl();
            
            async IAsyncEnumerable<StreamingTextContent> GetStreamingContentAsyncImpl(
                [EnumeratorCancellation] CancellationToken ct = default)
            {
                foreach (var message in _response!)
                {
                    ct.ThrowIfCancellationRequested();
                    
                    yield return new StreamingTextContent(
                        message.Content ?? string.Empty,
                        modelId: message.ModelId);
                    
                    await Task.Delay(10, ct).ConfigureAwait(false);
                }
            }
        }
        
        public Task<IReadOnlyList<TextContent>> GetTextContentsAsync(
            string prompt,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            _prompts.Add(prompt);
            
            if (_exception != null)
                return Task.FromException<IReadOnlyList<TextContent>>(_exception);
                
            if (_response == null)
                throw new InvalidOperationException("Response not set");
            
            var result = _response.Select(m => new TextContent(
                m.Content ?? string.Empty,
                m.ModelId)).ToList();
                
            return Task.FromResult<IReadOnlyList<TextContent>>(result);
        }
    }
}
