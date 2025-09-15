using BotManagementSystem.Core.Entities;
using BotManagementSystem.Core.Interfaces;
using BotManagementSystem.Core.Validation;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BotManagementSystem.Services.Services
{
    public interface IBotService
    {
        Task<Bot> CreateBotAsync(Bot bot);
        Task<Bot?> GetBotByIdAsync(Guid id);
        Task<IEnumerable<Bot>> GetAllBotsAsync();
        Task<IEnumerable<Bot>> GetActiveBotsAsync();
        Task<bool> UpdateBotAsync(Bot bot);
        Task<bool> DeleteBotAsync(Guid id);
        Task<bool> ToggleBotStatusAsync(Guid id, bool isActive);
    }

    public class BotService : IBotService
    {
        private readonly IBotRepository _botRepository;
        private readonly BotValidator _botValidator;
        private readonly IDistributedCache? _cache;
        private const string CachePrefix = "Bot_";
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public BotService(IBotRepository botRepository, IDistributedCache? cache = null)
        {
            _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
            _botValidator = new BotValidator();
            _cache = cache;
        }

        public async Task<Bot> CreateBotAsync(Bot bot)
        {
            await _botValidator.ValidateAndThrowAsync(bot);
            
            if (await _botRepository.ExistsAsync(b => b.Name == bot.Name))
            {
                throw new ValidationException("A bot with this name already exists.");
            }

            bot.CreatedAt = DateTime.UtcNow;
            bot.LastActive = null;
            
            await _botRepository.AddAsync(bot);
            return bot;
        }

        public async Task<Bot?> GetBotByIdAsync(Guid id)
        {
            var cacheKey = $"{CachePrefix}{id}";
            
            // Try to get from cache first
            if (_cache != null)
            {
                var cachedBot = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedBot))
                {
                    return JsonSerializer.Deserialize<Bot>(cachedBot, _jsonOptions);
                }
            }

            // Get from database
            var bot = await _botRepository.GetByIdAsync(id);
            
            // Cache the result
            if (bot != null && _cache != null)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(bot, _jsonOptions),
                    cacheOptions);
            }

            return bot;
        }

        public async Task<IEnumerable<Bot>> GetAllBotsAsync()
        {
            return await _botRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Bot>> GetActiveBotsAsync()
        {
            return await _botRepository.GetActiveBotsAsync();
        }

        public async Task<bool> UpdateBotAsync(Bot bot)
        {
            await _botValidator.ValidateAndThrowAsync(bot);
            
            var existingBot = await _botRepository.GetByIdAsync(bot.Id);
            if (existingBot == null)
            {
                return false;
            }

            // Check for duplicate name
            if (await _botRepository.ExistsAsync(b => b.Id != bot.Id && b.Name == bot.Name))
            {
                throw new ValidationException("A bot with this name already exists.");
            }

            // Update properties
            existingBot.Name = bot.Name;
            existingBot.Description = bot.Description;
            existingBot.Configuration = bot.Configuration;
            existingBot.Integrations = bot.Integrations;

            await _botRepository.UpdateAsync(existingBot);
            
            // Invalidate cache
            if (_cache != null)
            {
                await _cache.RemoveAsync($"{CachePrefix}{bot.Id}");
            }

            return true;
        }

        public async Task<bool> DeleteBotAsync(Guid id)
        {
            var bot = await _botRepository.GetByIdAsync(id);
            if (bot == null)
            {
                return false;
            }

            await _botRepository.DeleteAsync(id);
            
            // Invalidate cache
            if (_cache != null)
            {
                await _cache.RemoveAsync($"{CachePrefix}{id}");
            }

            return true;
        }

        public async Task<bool> ToggleBotStatusAsync(Guid id, bool isActive)
        {
            var bot = await _botRepository.GetByIdAsync(id);
            if (bot == null)
            {
                return false;
            }

            if (bot.IsActive == isActive)
            {
                return true; // No change needed
            }

            bot.IsActive = isActive;
            bot.LastActive = isActive ? DateTime.UtcNow : bot.LastActive;

            await _botRepository.UpdateAsync(bot);
            
            // Invalidate cache
            if (_cache != null)
            {
                await _cache.RemoveAsync($"{CachePrefix}{id}");
            }

            return true;
        }
    }
}
