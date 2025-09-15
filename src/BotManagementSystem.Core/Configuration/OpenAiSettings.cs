using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace BotManagementSystem.Core.Configuration
{
    public class OpenAiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ModelId { get; set; } = string.Empty;
        public OpenAIPromptExecutionSettings PromptExecutionSettings { get; set; } = new();
    }
}
