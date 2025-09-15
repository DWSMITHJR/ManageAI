using BotManagementSystem.Core.Validation;
using Microsoft.SemanticKernel.ChatCompletion;
using Xunit;
using Xunit.Abstractions;

namespace BotManagementSystem.Tests.Validation;

public class SemanticKernelValidationTests
{
    private readonly ITestOutputHelper _output;

    public SemanticKernelValidationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidatePrompt_WhenEmptyOrWhitespace_ThrowsArgumentException(string prompt)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => SemanticKernelValidation.ValidatePrompt(prompt));
        _output.WriteLine(exception.Message);
        Assert.Equal("prompt", exception.ParamName);
    }

    [Fact]
    public void ValidatePrompt_WhenExceedsMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var longPrompt = new string('a', 4001);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => SemanticKernelValidation.ValidatePrompt(longPrompt));
        _output.WriteLine(exception.Message);
        Assert.Equal("prompt", exception.ParamName);
        Assert.Contains("exceeds maximum", exception.Message);
    }

    [Theory]
    [InlineData("This contains hack attempt")]
    [InlineData("My password is 1234")]
    [InlineData("Don't share your API key")]
    [InlineData("Credit card number 1234-5678-9012-3456")]
    [InlineData("My SSN is 123-45-6789")]
    public void ValidatePrompt_WhenContainsBannedPhrase_ThrowsArgumentException(string prompt)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => SemanticKernelValidation.ValidatePrompt(prompt));
        _output.WriteLine($"Banned phrase detected: {exception.Message}");
        Assert.Equal("prompt", exception.ParamName);
        Assert.Contains("prohibited content", exception.Message);
    }

    [Fact]
    public void ValidateChatHistory_WhenNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => SemanticKernelValidation.ValidateChatHistory(null!));
        _output.WriteLine(exception.Message);
        Assert.Equal("chatHistory", exception.ParamName);
    }

    [Fact]
    public void ValidateChatHistory_WhenEmpty_ThrowsArgumentException()
    {
        // Arrange
        var chatHistory = new ChatHistory();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => SemanticKernelValidation.ValidateChatHistory(chatHistory));
        _output.WriteLine(exception.Message);
        Assert.Equal("chatHistory", exception.ParamName);
        Assert.Contains("cannot be empty", exception.Message);
    }

    [Fact]
    public void ValidateChatHistory_WhenExceedsMaxMessageCount_ThrowsArgumentException()
    {
        // Arrange
        var chatHistory = new ChatHistory();
        for (int i = 0; i < 11; i++)
        {
            chatHistory.AddUserMessage($"Message {i + 1}");
        }

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => SemanticKernelValidation.ValidateChatHistory(chatHistory));
        _output.WriteLine(exception.Message);
        Assert.Equal("chatHistory", exception.ParamName);
        Assert.Contains("exceeds maximum", exception.Message);
    }

    [Fact]
    public void ValidateChatHistory_WhenContainsEmptyMessage_ThrowsArgumentException()
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Valid message");
        chatHistory.AddUserMessage("   "); // Empty message

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => SemanticKernelValidation.ValidateChatHistory(chatHistory));
        _output.WriteLine(exception.Message);
        Assert.Equal("chatHistory", exception.ParamName);
        Assert.Contains("empty or whitespace", exception.Message);
    }

    [Fact]
    public void ValidateChatHistory_WhenContainsBannedPhrase_ThrowsArgumentException()
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("This is a normal message");
        chatHistory.AddUserMessage("But this contains a hack attempt");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => SemanticKernelValidation.ValidateChatHistory(chatHistory));
        _output.WriteLine($"Banned phrase detected in chat history: {exception.Message}");
        Assert.Equal("chatHistory", exception.ParamName);
        Assert.Contains("prohibited content", exception.Message);
    }

    [Fact]
    public void ValidatePrompt_WhenValid_DoesNotThrow()
    {
        // Arrange
        var validPrompt = "This is a valid prompt for testing.";

        // Act & Assert (should not throw)
        var exception = Record.Exception(() => SemanticKernelValidation.ValidatePrompt(validPrompt));
        Assert.Null(exception);
    }

    [Fact]
    public void ValidateChatHistory_WhenValid_DoesNotThrow()
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Hello");
        chatHistory.AddAssistantMessage("Hi there! How can I help you today?");

        // Act & Assert (should not throw)
        var exception = Record.Exception(() => SemanticKernelValidation.ValidateChatHistory(chatHistory));
        Assert.Null(exception);
    }
}
