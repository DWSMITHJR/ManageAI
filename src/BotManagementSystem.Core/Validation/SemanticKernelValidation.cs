using System;
using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;

namespace BotManagementSystem.Core.Validation;

public static class SemanticKernelValidation
{
    private const int MaxPromptLength = 4000;
    private const int MaxChatHistoryLength = 10;
    private static readonly string[] BannedPhrases = 
    { 
        "hack", 
        "password", 
        "secret", 
        "api key",
        "credit card",
        "ssn",
        "social security"
    };

    public static void ValidatePrompt(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be null or whitespace.", nameof(prompt));
        }

        if (prompt.Length > MaxPromptLength)
        {
            throw new ArgumentException($"Prompt length exceeds maximum allowed length of {MaxPromptLength} characters.", nameof(prompt));
        }

        var lowerPrompt = prompt.ToLowerInvariant();
        foreach (var phrase in BannedPhrases)
        {
            if (lowerPrompt.Contains(phrase))
            {
                throw new ArgumentException($"Prompt contains prohibited content: {phrase}", nameof(prompt));
            }
        }
    }

    public static void ValidateChatHistory(ChatHistory chatHistory)
    {
        if (chatHistory == null)
        {
            throw new ArgumentNullException(nameof(chatHistory));
        }

        if (chatHistory.Count == 0)
        {
            throw new ArgumentException("Chat history cannot be empty.", nameof(chatHistory));
        }

        if (chatHistory.Count > MaxChatHistoryLength)
        {
            throw new ArgumentException($"Chat history exceeds maximum allowed length of {MaxChatHistoryLength} messages.", nameof(chatHistory));
        }

        foreach (var message in chatHistory)
        {
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                throw new ArgumentException("Chat history contains empty or whitespace messages.", nameof(chatHistory));
            }
            
            var lowerContent = message.Content.ToLowerInvariant();
            foreach (var phrase in BannedPhrases)
            {
                if (lowerContent.Contains(phrase))
                {
                    throw new ArgumentException($"Chat history contains prohibited content: {phrase}", nameof(chatHistory));
                }
            }
        }
    }
}
