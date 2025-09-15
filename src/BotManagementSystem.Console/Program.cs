using BotManagementSystem.Core.Entities;
using BotManagementSystem.Core.Configuration;
using BotManagementSystem.Core.Services;
using BotManagementSystem.Core.Interfaces;
using BotManagementSystem.Infrastructure.Data;
using BotManagementSystem.Infrastructure.Repositories;
using BotManagementSystem.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Text.Json;

// Create host builder
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Add database context
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("BotManagementDb"));
            
        // Add services
        services.Configure<OpenAiSettings>(hostContext.Configuration.GetSection("OpenAI"));
        services.AddScoped<IBotRepository, BotRepository>();
        services.AddScoped<IBotService, BotService>();
        services.AddScoped<ISemanticKernelService, SemanticKernelService>();
        services.AddSingleton<IMemoryCache, MemoryCache>();
        
        // Add the CLI service
        services.AddHostedService<BotManagementCli>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });
    });

// Build and run the host
await builder.Build().RunAsync();

public class BotManagementCli : IHostedService
{
    private readonly ILogger<BotManagementCli> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IBotService _botService;
    private readonly AppDbContext _dbContext;

    public BotManagementCli(
        ILogger<BotManagementCli> logger,
        IHostApplicationLifetime appLifetime,
        IBotService botService,
        AppDbContext dbContext)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _botService = botService;
        _dbContext = dbContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await InitializeDatabaseAsync();
            await ShowMainMenuAsync();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            _appLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task InitializeDatabaseAsync()
    {
        await _dbContext.Database.EnsureCreatedAsync();
    }

    private async Task ShowMainMenuAsync()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Bot Management System").Centered().Color(Color.Blue));
            
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nWhat would you like to do?")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Manage Bots",
                        "View Statistics",
                        "Exit"
                    }));

            switch (choice)
            {
                case "Manage Bots":
                    await ManageBotsAsync();
                    break;
                case "View Statistics":
                    await ShowStatisticsAsync();
                    break;
                case "Exit":
                    _appLifetime.StopApplication();
                    return;
            }
        }
    }

    private async Task ManageBotsAsync()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("Bot Management") { Justification = Justify.Left }.RuleStyle("blue"));
            
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nBot Management")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "List All Bots",
                        "View Bot Details",
                        "Create New Bot",
                        "Update Bot",
                        "Delete Bot",
                        "Toggle Bot Status",
                        "Back to Main Menu"
                    }));

            switch (choice)
            {
                case "List All Bots":
                    await ListAllBotsAsync();
                    break;
                case "View Bot Details":
                    await ViewBotDetailsAsync();
                    break;
                case "Create New Bot":
                    await CreateNewBotAsync();
                    break;
                case "Update Bot":
                    await UpdateBotAsync();
                    break;
                case "Delete Bot":
                    await DeleteBotAsync();
                    break;
                case "Toggle Bot Status":
                    await ToggleBotStatusAsync();
                    break;
                case "Back to Main Menu":
                    return;
            }
        }
    }

    private async Task ListAllBotsAsync()
    {
        var bots = await _botService.GetAllBotsAsync();
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("ID")
            .AddColumn("Name")
            .AddColumn("Type")
            .AddColumn("Status")
            .AddColumn("Last Active");

        foreach (var bot in bots)
        {
            table.AddRow(
                bot.Id.ToString(),
                bot.Name,
                bot.Type,
                bot.IsActive ? "[green]Active[/]" : "[red]Inactive[/]",
                bot.LastActive?.ToString("g") ?? "Never");
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewBotDetailsAsync()
    {
        var bots = await _botService.GetAllBotsAsync();
        if (!bots.Any())
        {
            AnsiConsole.MarkupLine("[red]No bots found.[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var botChoices = bots.Select(b => $"{b.Name} ({b.Type})").ToList();
        botChoices.Add("Back");

        while (true)
        {
            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a bot to view details:")
                    .AddChoices(botChoices));

            if (selected == "Back")
                return;

            var bot = bots.First(b => $"{b.Name} ({b.Type})" == selected);
            
            var panel = new Panel(GetBotDetails(bot))
                .Header($"[bold]Bot Details: {bot.Name}[/]")
                .BorderColor(Color.Blue);
            
            AnsiConsole.Write(panel);
            
            if (AnsiConsole.Confirm("View another bot?"))
                continue;
                
            break;
        }
    }

    private string GetBotDetails(Bot bot)
    {
        return $@"
[bold]ID:[/] {bot.Id}
[bold]Name:[/] {bot.Name}
[bold]Type:[/] {bot.Type}
[bold]Status:[/] {(bot.IsActive ? "[green]Active[/]" : "[red]Inactive[/]")}
[bold]Created:[/] {bot.CreatedAt:g}
[bold]Last Active:[/] {bot.LastActive?.ToString("g") ?? "Never"}
[bold]Description:[/] {bot.Description ?? "N/A"}
[bold]Configuration:[/] {(bot.Configuration != null && bot.Configuration.Any() ? JsonSerializer.Serialize(bot.Configuration) : "{}")}";
    }

    private async Task CreateNewBotAsync()
    {
        AnsiConsole.MarkupLine("[bold]Create New Bot[/]");
        AnsiConsole.MarkupLine("[grey]Press Ctrl+C to cancel at any time[/]");
        
        var name = AnsiConsole.Ask<string>("Bot Name:");
        var type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Bot Type:")
                .AddChoices(new[] { "Chat", "Automation", "Analytics", "Other" }));
                
        var description = AnsiConsole.Ask<string>("Description (optional):", "");
        var isActive = AnsiConsole.Confirm("Activate this bot?");

        var bot = new Bot
        {
            Name = name,
            Type = type,
            Description = string.IsNullOrWhiteSpace(description) ? null : description,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            var createdBot = await _botService.CreateBotAsync(bot);
            AnsiConsole.MarkupLine($"[green]✓ Bot '{createdBot.Name}' created successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error creating bot: {ex.Message}[/]");
        }
        
        AnsiConsole.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task UpdateBotAsync()
    {
        var bots = await _botService.GetAllBotsAsync();
        if (!bots.Any())
        {
            AnsiConsole.MarkupLine("[red]No bots found to update.[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var botChoices = bots.Select(b => $"{b.Name} ({b.Type})").ToList();
        botChoices.Add("Back");

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a bot to update:")
                .AddChoices(botChoices));

        if (selected == "Back")
            return;

        var bot = bots.First(b => $"{b.Name} ({b.Type})" == selected);
        
        AnsiConsole.MarkupLine($"[bold]Updating bot: {bot.Name}[/]");
        
        // In a real application, you would implement the update logic here
        // For now, we'll just show a message
        AnsiConsole.MarkupLine("[yellow]Update functionality will be implemented here.[/]");
        
        AnsiConsole.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task DeleteBotAsync()
    {
        var bots = await _botService.GetAllBotsAsync();
        if (!bots.Any())
        {
            AnsiConsole.MarkupLine("[red]No bots found to delete.[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var botChoices = bots.Select(b => $"{b.Name} ({b.Type})").ToList();
        botChoices.Add("Back");

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a bot to delete:")
                .AddChoices(botChoices));

        if (selected == "Back")
            return;

        var bot = bots.First(b => $"{b.Name} ({b.Type})" == selected);
        
        if (AnsiConsole.Confirm($"Are you sure you want to delete the bot '{bot.Name}'?", false))
        {
            try
            {
                var result = await _botService.DeleteBotAsync(bot.Id);
                if (result)
                {
                    AnsiConsole.MarkupLine($"[green]✓ Bot '{bot.Name}' deleted successfully![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Failed to delete the bot.[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error deleting bot: {ex.Message}[/]");
            }
        }
        
        AnsiConsole.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task ToggleBotStatusAsync()
    {
        var bots = await _botService.GetAllBotsAsync();
        if (!bots.Any())
        {
            AnsiConsole.MarkupLine("[red]No bots found.[/]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var botChoices = bots.Select(b => $"{b.Name} ({(b.IsActive ? "Active" : "Inactive")})").ToList();
        botChoices.Add("Back");

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a bot to toggle status:")
                .AddChoices(botChoices));

        if (selected == "Back")
            return;

        var bot = bots.ToList()[botChoices.IndexOf(selected)];
        var newStatus = !bot.IsActive;
        
        try
        {
            var result = await _botService.ToggleBotStatusAsync(bot.Id, newStatus);
            if (result)
            {
                AnsiConsole.MarkupLine($"[green]✓ Bot '{bot.Name}' is now {(newStatus ? "active" : "inactive")}![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Failed to update bot status.[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error toggling bot status: {ex.Message}[/]");
        }
        
        AnsiConsole.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task ShowStatisticsAsync()
    {
        var bots = await _botService.GetAllBotsAsync();
        var activeBots = bots.Count(b => b.IsActive);
        var inactiveBots = bots.Count() - activeBots;
        var botTypes = bots.GroupBy(b => b.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count);

        var chart = new BarChart()
            .Width(60)
            .Label("[green]Bot Statistics[/]")
            .CenterLabel()
            .AddItem("Total Bots", bots.Count(), Color.Blue)
            .AddItem("Active Bots", activeBots, Color.Green)
            .AddItem("Inactive Bots", inactiveBots, Color.Red);

        AnsiConsole.Write(chart);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("Bots by Type")
            .AddColumn("Type")
            .AddColumn("Count");

        foreach (var type in botTypes)
        {
            table.AddRow(type.Type, type.Count.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

} // End of BotManagementCli class
