using BotManagementSystem.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace BotManagementSystem.Tests.Base;

/// <summary>
/// Base class for database tests that provides a fresh in-memory SQLite database for each test.
/// </summary>
public abstract class DatabaseTestBase : BaseTest, IAsyncLifetime
{
    private readonly SqliteConnection _connection;
    protected readonly AppDbContext DbContext;
    protected readonly IConfiguration Configuration;

    protected DatabaseTestBase(ITestOutputHelper output) : base(output)
    {
        // Set up configuration
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Create and open a connection. This creates the SQLite in-memory database
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        // Configure the context to use SQLite in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        // Create the database schema and seed data
        DbContext = new AppDbContext(options);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        LogTestStep("Initializing test database");
        
        // Ensure database is created and migrated
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();
        
        // Seed test data
        try 
        {
            await SeedTestDataAsync();
            await DbContext.SaveChangesAsync();
            LogTestStep("Test database initialized and seeded");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error seeding test data");
            throw;
        }
    }

    public override async Task DisposeAsync()
    {
        LogTestStep("Cleaning up test database");
        
        await DbContext.Database.EnsureDeletedAsync();
        await base.DisposeAsync();
        
        await _connection.DisposeAsync();
    }

    /// <summary>
    /// Override this method to seed test data specific to each test class.
    /// </summary>
    protected virtual Task SeedTestDataAsync() => Task.CompletedTask;
    
    /// <summary>
    /// Creates a test bot with the specified parameters.
    /// </summary>
    protected async Task<Core.Entities.Bot> CreateTestBotAsync(
        string name = "Test Bot", 
        string description = "Test Description", 
        bool isActive = true)
    {
        var bot = new Core.Entities.Bot
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };

        DbContext.Bots.Add(bot);
        await DbContext.SaveChangesAsync();
        
        LogTestData($"Created test bot: {name}", new { bot.Id, bot.Name, bot.IsActive });
        
        return bot;
    }
}
