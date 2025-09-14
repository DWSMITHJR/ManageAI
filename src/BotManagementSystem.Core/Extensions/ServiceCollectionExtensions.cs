using BotManagementSystem.Core.Services;
using Microsoft.Cloud.MCP.SDK;
using Microsoft.Extensions.DependencyInjection;
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
            return new SemanticKernelService(apiKey, modelId, endpoint, logger);
        });

        return services;
    }

    public static IServiceCollection AddMCPClient(
        this IServiceCollection services,
        string mcpApiKey,
        string baseUrl = "https://api.mcp.microsoft.com")
    {
        services.AddHttpClient<IMCPClient, MCPClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", mcpApiKey);
        });

        return services;
    }
}
