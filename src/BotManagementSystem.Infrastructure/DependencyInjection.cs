using BotManagementSystem.Core.Interfaces;
using BotManagementSystem.Infrastructure.Data;
using BotManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BotManagementSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName = "DefaultConnection")
        {
            // Database Context
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(connectionStringName),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // Redis Caching
            var redisConnection = configuration.GetSection("Redis:ConnectionString").Value;
            if (!string.IsNullOrEmpty(redisConnection))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                    options.InstanceName = "BotManagementSystem_";
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            // Repositories
            services.AddScoped<IBotRepository, BotRepository>();

            return services;
        }
    }
}
