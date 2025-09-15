using BotManagementSystem.Core.Entities;
using BotManagementSystem.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BotManagementSystem.Tests;

public abstract class TestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly AppDbContext DbContext;
    protected readonly IConfiguration Configuration;

    protected TestBase()
    {
        // Set up configuration
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        // Create the schema and seed some data
        DbContext = new AppDbContext(options);
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
        
        // Seed test data if needed
        try 
        {
            SeedTestData();
            DbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding test data: {ex}");
            throw;
        }
    }

    protected virtual void SeedTestData()
    {
        // Override in test classes to seed specific test data
    }

    protected Bot CreateTestBot(string name = "Test Bot", string description = "Test Description", bool isActive = true)
    {
        var bot = new Bot
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };

        DbContext.Bots.Add(bot);
        DbContext.SaveChanges();
        
        return bot;
    }

    public void Dispose()
    {
        DbContext?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}
