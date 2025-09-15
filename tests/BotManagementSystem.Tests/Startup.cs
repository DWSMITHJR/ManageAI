using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace BotManagementSystem.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure any test services here
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddXunitOutput();
            loggingBuilder.SetMinimumLevel(LogLevel.Debug);
        });
    }
}
