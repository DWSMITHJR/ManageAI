namespace BotManagementSystem.Core.Interfaces;

public interface IAIShell
{
    Task<string> CompletePromptAsync(string prompt);
}
