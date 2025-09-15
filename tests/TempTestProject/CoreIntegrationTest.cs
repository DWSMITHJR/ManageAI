using BotManagementSystem.Core.Entities;
using Xunit;

namespace TempTestProject;

public class CoreIntegrationTest
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
