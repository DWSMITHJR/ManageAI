using BotManagementSystem.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace BotManagementSystem.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSemanticKernelServices(
        this IServiceCollection services,
        string apiKey,
        string modelId = "gpt-35-turbo",
        string? endpoint = null)
    {
        services.AddSingleton<ISemanticKernelService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<SemanticKernelService>>();
            Uri? endpointUri = null;
            if (!string.IsNullOrEmpty(endpoint))
            {
                endpointUri = new Uri(endpoint);
            }
            return new SemanticKernelService(apiKey, modelId, endpoint, logger);
        });

        return services;
    }

}
