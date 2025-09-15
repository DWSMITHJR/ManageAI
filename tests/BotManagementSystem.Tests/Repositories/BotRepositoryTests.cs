using BotManagementSystem.Core.Entities;
using BotManagementSystem.Core.Interfaces;
using BotManagementSystem.Infrastructure.Data;
using BotManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BotManagementSystem.Tests.Repositories;

public class BotRepositoryTests : TestBase
{
    private readonly IBotRepository _repository;

    public BotRepositoryTests()
    {
        _repository = new BotRepository(DbContext);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBot_WhenBotExists()
    {
        // Arrange
        var testBot = CreateTestBot();

        // Act
        var result = await _repository.GetByIdAsync(testBot.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testBot.Id, result.Id);
        Assert.Equal(testBot.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenBotDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBots()
    {
        // Arrange
        var bot1 = CreateTestBot("Bot 1");
        var bot2 = CreateTestBot("Bot 2", isActive: false);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, b => b.Id == bot1.Id);
        Assert.Contains(result, b => b.Id == bot2.Id);
    }

    [Fact]
    public async Task AddAsync_AddsBotToDatabase()
    {
        try
        {
            Console.WriteLine("Starting AddAsync_AddsBotToDatabase test...");
            
            // Arrange
            var newBot = new Bot
            {
                Id = Guid.NewGuid(),
                Name = "New Bot",
                Description = "New Description",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Configuration = new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                }
            };

            Console.WriteLine("Created test bot with ID: " + newBot.Id);

            // Act
            Console.WriteLine("Calling AddAsync...");
            await _repository.AddAsync(newBot);
            Console.WriteLine("AddAsync completed");

            Console.WriteLine("Calling GetByIdAsync...");
            var result = await _repository.GetByIdAsync(newBot.Id);
            Console.WriteLine("GetByIdAsync completed");

            // Assert
            Console.WriteLine("Starting assertions...");
            Assert.NotNull(result);
            Console.WriteLine("Result is not null");
            
            Assert.Equal(newBot.Id, result.Id);
            Console.WriteLine("ID matches");
            
            Assert.Equal(newBot.Name, result.Name);
            Console.WriteLine("Name matches");
            
            Assert.NotNull(result.Configuration);
            Console.WriteLine("Configuration is not null");
            Assert.Equal(2, result.Configuration.Count);
            Console.WriteLine("Configuration has 2 items");
            
            Console.WriteLine("Test completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test failed with exception: {ex}");
            throw;
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBotInDatabase()
    {
        // Arrange
        var bot = CreateTestBot();
        var updatedName = "Updated Bot Name";
        bot.Name = updatedName;

        // Act
        await _repository.UpdateAsync(bot);
        var result = await _repository.GetByIdAsync(bot.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedName, result.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesBotFromDatabase()
    {
        // Arrange
        var bot = CreateTestBot();

        // Act
        await _repository.DeleteAsync(bot.Id);
        var result = await _repository.GetByIdAsync(bot.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveBotsAsync_ReturnsOnlyActiveBots()
    {
        // Arrange
        var activeBot = CreateTestBot("Active Bot", isActive: true);
        var inactiveBot = CreateTestBot("Inactive Bot", isActive: false);

        // Act
        var result = await _repository.GetActiveBotsAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Single(resultList);
        Assert.Equal(activeBot.Id, resultList[0].Id);
    }

    [Fact]
    public async Task GetBotWithIntegrationsAsync_ReturnsBotWithIntegrations()
    {
        // Arrange
        var bot = CreateTestBot();
        var integration = new BotIntegration
        {
            Type = IntegrationType.Google,
            IsEnabled = true,
            Configuration = new Dictionary<string, string> { { "key", "value" } }
        };
        
        bot.Integrations.Add(integration);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetBotWithIntegrationsAsync(bot.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bot.Id, result.Id);
        Assert.Single(result.Integrations);
        Assert.Equal(IntegrationType.Google, result.Integrations.First().Type);
    }
}
