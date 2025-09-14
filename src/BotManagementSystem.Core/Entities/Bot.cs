using System;
using System.Collections.Generic;

namespace BotManagementSystem.Core.Entities
{
    public class Bot
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastActive { get; set; }
        public Dictionary<string, string> Configuration { get; set; } = new();
        public List<BotIntegration> Integrations { get; set; } = new();
    }

    public class BotIntegration
    {
        public IntegrationType Type { get; set; }
        public bool IsEnabled { get; set; }
        public Dictionary<string, string> Configuration { get; set; } = new();
    }

    public enum IntegrationType
    {
        Google,
        Azure,
        SmartThings
    }
}
