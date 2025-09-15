using BotManagementSystem.Core.Entities;
using BotManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SimpleTestProject;

public class MinimalDbTest : IDisposable
{
    private readonly AppDbContext _context;

    public MinimalDbTest()
    {
        // Setup in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public void CanCreateAndRetrieveBot()
    {
        // Arrange
        var bot = new Bot
        {
            Id = Guid.NewGuid(),
            Name = "Test Bot",
            Description = "Test Description",
            IsActive = true
        };

        // Act
        _context.Bots.Add(bot);
        _context.SaveChanges();

        // Assert
        var savedBot = _context.Bots.FirstOrDefault(b => b.Id == bot.Id);
        Assert.NotNull(savedBot);
        Assert.Equal(bot.Name, savedBot.Name);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
