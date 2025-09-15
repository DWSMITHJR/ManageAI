using BotManagementSystem.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace BotManagementSystem.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSemanticKernelServices(
        this IServiceCollection services)
    {
        services.AddSingleton<ISemanticKernelService>(sp =>
        {
            var openAiSettings = sp.GetRequiredService<IOptions<OpenAiSettings>>();
            var logger = sp.GetRequiredService<ILogger<SemanticKernelService>>();
            return new SemanticKernelService(openAiSettings, logger);
        });

        return services;
    }

}
