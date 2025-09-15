using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BotManagementSystem.Core.Entities
{
    public class Bot
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastActive { get; set; }
        
        [NotMapped]
        public Dictionary<string, string> Configuration { get; set; } = new();
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Column("Configuration")]
        public string ConfigurationJson
        {
            get => JsonSerializer.Serialize(Configuration);
            set => Configuration = string.IsNullOrEmpty(value) 
                ? new Dictionary<string, string>() 
                : JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new Dictionary<string, string>();
        }
        
        public List<BotIntegration> Integrations { get; set; } = new();
    }

    public class BotIntegration
    {
        public IntegrationType Type { get; set; }
        public bool IsEnabled { get; set; }
        
        // This is the actual property used by the application
        [NotMapped]
        public Dictionary<string, string> Configuration { get; set; } = new();
        
        // This property is used by Entity Framework to store the dictionary as JSON
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Column("Configuration")]
        public string ConfigurationJson
        {
            get => JsonSerializer.Serialize(Configuration);
            set => Configuration = string.IsNullOrEmpty(value) 
                ? new Dictionary<string, string>() 
                : JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new Dictionary<string, string>();
        }
    }

    public enum IntegrationType
    {
        Google,
        Azure,
        SmartThings
    }
}
