using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace BotManagementSystem.Tests.Samples;

public class SemanticKernelTest
{
    private readonly ITestOutputHelper _output;

    public SemanticKernelTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestDependencyInjection()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging(builder => 
                builder.AddXUnit(_output)
                       .SetMinimumLevel(LogLevel.Debug));

        // Act & Assert
        // This test just verifies that we can create a service provider
        // with the required logging configuration
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<SemanticKernelTest>>();
        
        logger.LogInformation("Test message from SemanticKernelTest");
        
        Assert.NotNull(logger);
    }
}
