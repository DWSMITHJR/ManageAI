using System.Text;
using System.Text.Json;

namespace BotManagementSystem.Tests;

public class TestReportGenerator
{
    private readonly string _reportDirectory;
    private readonly List<TestResult> _testResults;

    public TestReportGenerator()
    {
        _reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestReports");
        _testResults = new List<TestResult>();
        
        if (!Directory.Exists(_reportDirectory))
        {
            Directory.CreateDirectory(_reportDirectory);
        }
    }

    public void AddTestResult(string testName, bool passed, string details, string? error = null, TimeSpan? duration = null)
    {
        _testResults.Add(new TestResult
        {
            TestName = testName,
            Passed = passed,
            Details = details,
            Error = error,
            Duration = duration ?? TimeSpan.Zero,
            Timestamp = DateTime.UtcNow
        });
    }

    public void GenerateIndividualReports()
    {
        foreach (var result in _testResults)
        {
            var report = GenerateMarkdownReport(result);
            var fileName = $"{result.TestName}_{result.Timestamp:yyyyMMdd_HHmmss}.md";
            var filePath = Path.Combine(_reportDirectory, fileName);
            File.WriteAllText(filePath, report);
        }
    }

    public void GenerateSummaryReport()
    {
        var summary = GenerateSummaryMarkdown();
        var filePath = Path.Combine(_reportDirectory, $"TestSummary_{DateTime.UtcNow:yyyyMMdd_HHmmss}.md");
        File.WriteAllText(filePath, summary);
    }

    private string GenerateMarkdownReport(TestResult result)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"# Test Report: {result.TestName}");
        sb.AppendLine();
        sb.AppendLine($"**Timestamp:** {result.Timestamp:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"**Status:** {(result.Passed ? "✅ PASSED" : "❌ FAILED")}");
        sb.AppendLine($"**Duration:** {result.Duration.TotalMilliseconds:F2} ms");
        sb.AppendLine();
        
        sb.AppendLine("## Test Details");
        sb.AppendLine(result.Details);
        sb.AppendLine();
        
        if (!string.IsNullOrEmpty(result.Error))
        {
            sb.AppendLine("## Error Details");
            sb.AppendLine("```");
            sb.AppendLine(result.Error);
            sb.AppendLine("```");
            sb.AppendLine();
        }
        
        sb.AppendLine("---");
        sb.AppendLine($"*Report generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC*");
        
        return sb.ToString();
    }

    private string GenerateSummaryMarkdown()
    {
        var sb = new StringBuilder();
        var totalTests = _testResults.Count;
        var passedTests = _testResults.Count(r => r.Passed);
        var failedTests = totalTests - passedTests;
        var totalDuration = _testResults.Sum(r => r.Duration.TotalMilliseconds);
        
        sb.AppendLine("# Test Suite Summary Report");
        sb.AppendLine();
        sb.AppendLine($"**Generated:** {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"**Total Tests:** {totalTests}");
        sb.AppendLine($"**Passed:** {passedTests} ✅");
        sb.AppendLine($"**Failed:** {failedTests} ❌");
        sb.AppendLine($"**Success Rate:** {(totalTests > 0 ? (passedTests * 100.0 / totalTests):0):F1}%");
        sb.AppendLine($"**Total Duration:** {totalDuration:F2} ms");
        sb.AppendLine();
        
        sb.AppendLine("## Test Results");
        sb.AppendLine("| Test Name | Status | Duration | Timestamp |");
        sb.AppendLine("|-----------|--------|----------|-----------|");
        
        foreach (var result in _testResults.OrderByDescending(r => r.Timestamp))
        {
            var status = result.Passed ? "✅ PASSED" : "❌ FAILED";
            sb.AppendLine($"| {result.TestName} | {status} | {result.Duration.TotalMilliseconds:F2} ms | {result.Timestamp:HH:mm:ss} |");
        }
        
        sb.AppendLine();
        
        if (failedTests > 0)
        {
            sb.AppendLine("## Failed Tests");
            foreach (var result in _testResults.Where(r => !r.Passed))
            {
                sb.AppendLine($"### {result.TestName}");
                if (!string.IsNullOrEmpty(result.Error))
                {
                    sb.AppendLine("```");
                    sb.AppendLine(result.Error);
                    sb.AppendLine("```");
                }
                sb.AppendLine();
            }
        }
        
        sb.AppendLine("---");
        sb.AppendLine($"*Report generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC*");
        
        return sb.ToString();
    }
}

public class TestResult
{
    public string TestName { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string Details { get; set; } = string.Empty;
    public string? Error { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime Timestamp { get; set; }
}
