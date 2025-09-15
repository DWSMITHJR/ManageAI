using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using BotManagementSystem.Core.Services;
using Microsoft.SemanticKernel;

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
        
        // Act
        var service = new SemanticKernelService(
            "test-api-key",
            "test-model",
            null,
            loggerMock.Object);
        
        // Assert
        Assert.NotNull(service);
    }
}
