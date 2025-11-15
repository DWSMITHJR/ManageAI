using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace BotManagementSystem.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHostBuilder CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TestStartup>();
            });

        return builder;
    }
}

public class TestStartup
{
    public TestStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add minimal services for testing
        services.AddControllers();
        
        // Mock OpenAI service
        services.AddSingleton<Azure.AI.OpenAI.AzureOpenAIClient>(sp =>
        {
            // Return a mock client or use test configuration
            var apiKey = Configuration["OpenAI:ApiKey"] ?? "test-key";
            return new Azure.AI.OpenAI.AzureOpenAIClient(
                new Uri("https://api.openai.com/"), 
                new Azure.AzureKeyCredential(apiKey));
        });

        // Add AIShell service
        services.AddScoped<BotManagementSystem.Core.Interfaces.IAIShell, BotManagementSystem.Services.Services.AIShellService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

// Make Program accessible for testing
public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        // Add services to the container.
        builder.Services.AddApplicationInsightsTelemetry();

        // Optional: Add Application Insights telemetry processor for filtering
        builder.Services.AddApplicationInsightsTelemetryProcessor<CustomTelemetryProcessor>();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Add Infrastructure Layer
        builder.Services.AddInfrastructure(configuration);

        // Add Services Layer
        builder.Services.AddScoped<IBotService, BotService>();
        builder.Services.AddScoped<BotManagementSystem.Core.Interfaces.IAIShell, BotManagementSystem.Services.Services.AIShellService>();

        // Add OpenAI service
        builder.Services.AddSingleton<AzureOpenAIClient>(sp =>
        {
            var apiKey = configuration["OpenAI:ApiKey"];
            return new AzureOpenAIClient(new Uri("https://api.openai.com/"), new Azure.AzureKeyCredential(apiKey));
        });

        // Configure JWT Authentication
        var jwtSettings = configuration.GetSection("Jwt");
        var jwtKey = jwtSettings["Key"];

        if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
        {
            // Generate a secure key if not configured or too short
            jwtKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            Console.WriteLine($"Generated JWT Key: {jwtKey}");
        }

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"] ?? "BotManagementSystem",
                    ValidAudience = jwtSettings["Audience"] ?? "BotManagementSystem",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bot Management System API", Version = "v1" });
            
            // Add JWT Authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Add Health Checks
        builder.Services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");

        // Add Health Checks UI
        builder.Services.AddHealthChecksUI(settings =>
        {
            settings.SetEvaluationTimeInSeconds(30);
            settings.MaximumHistoryEntriesPerEndpoint(50);
            settings.AddHealthCheckEndpoint("Database Health", "/health");
        })
        .AddInMemoryStorage();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bot Management System API V1");
                c.RoutePrefix = string.Empty; // Sets Swagger UI at app's root
            });
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        // Health Check endpoint
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckResponseUI
        });

        // Health Check UI endpoint
        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
            options.ApiPath = "/health-ui-api";
        });

        app.Run();
    }
}
