using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using BotManagementSystem.Core.Services;
using BotManagementSystem.Core.Configuration;
using Microsoft.Extensions.Options;

namespace BotManagementSystem.Tests;

public class SimpleTest
{
    [Fact]
    public void SimpleTest_Passes()
    {
        // Arrange
        var expected = true;
        
        // Act
        var actual = true;
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanCreateSemanticKernelService()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<SemanticKernelService>>();
        var openAiSettings = new OpenAiSettings
        {
            ApiKey = "test-api-key",
            ModelId = "test-model"
        };
        var options = Options.Create(openAiSettings);
        
        // Act
        var service = new SemanticKernelService(options, loggerMock.Object);
        
        // Assert
        Assert.NotNull(service);
    }
}
