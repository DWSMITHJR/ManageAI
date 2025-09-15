using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace BotManagementSystem.Tests.Base;

/// <summary>
/// Base class for all test classes providing common test functionality.
/// </summary>
public abstract class BaseTest : IAsyncLifetime
{
    protected readonly ITestOutputHelper Output;
    protected readonly ILogger Logger;
    protected readonly TestContext TestContext;

    protected BaseTest(ITestOutputHelper output)
    {
        Output = output ?? throw new ArgumentNullException(nameof(output));
        TestContext = new TestContext(GetType().Name);
        
        // Setup logging
        var loggerFactory = LoggerFactory.Create(builder => 
            builder.AddXUnit(output).SetMinimumLevel(LogLevel.Debug));
        
        Logger = loggerFactory.CreateLogger(GetType());
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;
    
    public virtual Task DisposeAsync() => Task.CompletedTask;
    
    protected void LogTestStep(string message)
    {
        var step = new TestStep(TestContext.CurrentTestName, message);
        TestContext.AddStep(step);
        Logger.LogInformation($"STEP: {message}");
    }
    
    protected void LogTestData(string dataName, object data)
    {
        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        var json = System.Text.Json.JsonSerializer.Serialize(data, options);
        Logger.LogInformation($"TEST DATA - {dataName}: {Environment.NewLine}{json}");
    }
}

public class TestContext
{
    private readonly List<TestStep> _steps = new();
    private string _currentTestName = string.Empty;
    
    public string TestClassName { get; }
    public IReadOnlyList<TestStep> Steps => _steps.AsReadOnly();
    public string CurrentTestName => _currentTestName;
    
    public TestContext(string testClassName)
    {
        TestClassName = testClassName;
    }
    
    public void SetCurrentTestName(string testName)
    {
        _currentTestName = testName;
    }
    
    public void AddStep(TestStep step)
    {
        _steps.Add(step);
    }
    
    public void ClearSteps()
    {
        _steps.Clear();
    }
}

public class TestStep
{
    public string TestName { get; }
    public string Message { get; }
    public DateTime Timestamp { get; }
    
    public TestStep(string testName, string message)
    {
        TestName = testName;
        Message = message;
        Timestamp = DateTime.UtcNow;
    }
    
    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss.fff}] {TestName} - {Message}";
    }
}
