using BotManagementSystem.Core.Entities;
using BotManagementSystem.Core.Interfaces;
using BotManagementSystem.Infrastructure.Data;
using BotManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SimpleTestProject;

public class BotRepositoryTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IBotRepository _repository;

    public BotRepositoryTest()
    {
        // Setup in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new AppDbContext(options);
        
        // Ensure the database is created
        _context.Database.EnsureCreated();
        
        _repository = new BotRepository(_context);
        
        // Seed the database with test data
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        // Add test bots to the in-memory database
        var testBot1 = new Bot
        {
            Id = Guid.NewGuid(),
            Name = "Test Bot 1",
            Description = "Test Description 1",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var testBot2 = new Bot
        {
            Id = Guid.NewGuid(),
            Name = "Test Bot 2",
            Description = "Test Description 2",
            IsActive = false,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _context.Bots.AddRange(testBot1, testBot2);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        // Clean up the in-memory database after each test
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBot_WhenBotExists()
    {
        // Arrange
        var testBot = _context.Bots.First();
        
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
        // Act
        var result = await _repository.GetAllAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddAsync_AddsBotToDatabase()
    {
        // Arrange
        var newBot = new Bot
        {
            Id = Guid.NewGuid(),
            Name = "New Test Bot",
            Description = "New Test Description",
            IsActive = true
        };
        
        // Act
        await _repository.AddAsync(newBot);
        var result = await _repository.GetByIdAsync(newBot.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(newBot.Id, result.Id);
        Assert.Equal(newBot.Name, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBotInDatabase()
    {
        // Arrange
        var botToUpdate = _context.Bots.First();
        var originalName = botToUpdate.Name;
        var updatedName = "Updated Bot Name";
        
        botToUpdate.Name = updatedName;
        
        // Act
        await _repository.UpdateAsync(botToUpdate);
        var result = await _repository.GetByIdAsync(botToUpdate.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(botToUpdate.Id, result.Id);
        Assert.Equal(updatedName, result.Name);
        Assert.NotEqual(originalName, result.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesBotFromDatabase()
    {
        // Arrange
        var botToDelete = _context.Bots.First();
        
        // Act
        await _repository.DeleteAsync(botToDelete.Id);
        var result = await _repository.GetByIdAsync(botToDelete.Id);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveBotsAsync_ReturnsOnlyActiveBots()
    {
        // Act
        var result = await _repository.GetActiveBotsAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.First().IsActive);
    }
}
