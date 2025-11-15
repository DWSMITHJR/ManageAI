using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace BotManagementSystem.Tests;

public class IntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly TestReportGenerator _reportGenerator;

    public IntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _reportGenerator = new TestReportGenerator();
    }

    [Fact]
    public async Task Test_OpenAI_API_Endpoint()
    {
        var stopwatch = Stopwatch.StartNew();
        var testName = "OpenAI_API_Endpoint";
        
        try
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/OpenAITest/test");
            
            stopwatch.Stop();
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _reportGenerator.AddTestResult(
                    testName, 
                    true, 
                    $"Successfully called OpenAI test endpoint. Status: {response.StatusCode}. Response: {content}",
                    null,
                    stopwatch.Elapsed
                );
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _reportGenerator.AddTestResult(
                    testName, 
                    false, 
                    $"Failed to call OpenAI test endpoint. Status: {response.StatusCode}",
                    errorContent,
                    stopwatch.Elapsed
                );
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _reportGenerator.AddTestResult(
                testName, 
                false, 
                "Exception occurred while testing OpenAI endpoint",
                ex.ToString(),
                stopwatch.Elapsed
            );
        }
        
        _reportGenerator.GenerateIndividualReports();
    }

    [Fact]
    public async Task Test_AIShell_API_Endpoint()
    {
        var stopwatch = Stopwatch.StartNew();
        var testName = "AIShell_API_Endpoint";
        
        try
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/AIShellTest/test");
            
            stopwatch.Stop();
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _reportGenerator.AddTestResult(
                    testName, 
                    true, 
                    $"Successfully called AIShell test endpoint. Status: {response.StatusCode}. Response: {content}",
                    null,
                    stopwatch.Elapsed
                );
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _reportGenerator.AddTestResult(
                    testName, 
                    false, 
                    $"Failed to call AIShell test endpoint. Status: {response.StatusCode}",
                    errorContent,
                    stopwatch.Elapsed
                );
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _reportGenerator.AddTestResult(
                testName, 
                false, 
                "Exception occurred while testing AIShell endpoint",
                ex.ToString(),
                stopwatch.Elapsed
            );
        }
        
        _reportGenerator.GenerateIndividualReports();
    }

    [Fact]
    public async Task Test_API_Health_Check()
    {
        var stopwatch = Stopwatch.StartNew();
        var testName = "API_Health_Check";
        
        try
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/health");
            
            stopwatch.Stop();
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _reportGenerator.AddTestResult(
                    testName, 
                    true, 
                    $"API health check passed. Status: {response.StatusCode}. Response: {content}",
                    null,
                    stopwatch.Elapsed
                );
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _reportGenerator.AddTestResult(
                    testName, 
                    false, 
                    $"API health check failed. Status: {response.StatusCode}",
                    errorContent,
                    stopwatch.Elapsed
                );
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _reportGenerator.AddTestResult(
                testName, 
                false, 
                "Exception occurred during API health check",
                ex.ToString(),
                stopwatch.Elapsed
            );
        }
        
        _reportGenerator.GenerateIndividualReports();
    }

    [Fact]
    public async Task Test_API_Swagger_Endpoint()
    {
        var stopwatch = Stopwatch.StartNew();
        var testName = "API_Swagger_Endpoint";
        
        try
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/swagger");
            
            stopwatch.Stop();
            
            if (response.IsSuccessStatusCode)
            {
                _reportGenerator.AddTestResult(
                    testName, 
                    true, 
                    $"Swagger endpoint is accessible. Status: {response.StatusCode}",
                    null,
                    stopwatch.Elapsed
                );
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _reportGenerator.AddTestResult(
                    testName, 
                    false, 
                    $"Swagger endpoint is not accessible. Status: {response.StatusCode}",
                    errorContent,
                    stopwatch.Elapsed
                );
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _reportGenerator.AddTestResult(
                testName, 
                false, 
                "Exception occurred while testing Swagger endpoint",
                ex.ToString(),
                stopwatch.Elapsed
            );
        }
        
        _reportGenerator.GenerateIndividualReports();
    }
}
