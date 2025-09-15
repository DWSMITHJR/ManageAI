using BotManagementSystem.Core.Entities;
using Xunit;

namespace SimpleTestProject;

public class SimpleCoreTest
{
    [Fact]
    public void CanCreateBotEntity()
    {
        // Arrange & Act
        var bot = new Bot
        {
            Name = "Test Bot",
            Description = "Test Description",
            IsActive = true
        };

        // Assert
        Assert.NotNull(bot);
        Assert.Equal("Test Bot", bot.Name);
        Assert.Equal("Test Description", bot.Description);
        Assert.True(bot.IsActive);
        Assert.NotEqual(Guid.Empty, bot.Id);
    }
}
