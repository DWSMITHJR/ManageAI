using BotManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BotManagementSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Bot> Bots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                
                entity.OwnsMany(e => e.Integrations, i =>
                {
                    i.Property<Guid>("Id").ValueGeneratedOnAdd();
                    i.HasKey("Id");
                    i.WithOwner().HasForeignKey("BotId");
                    i.Property(x => x.Type).HasConversion<string>();
                    i.Property(x => x.ConfigurationJson).HasColumnName("Configuration");
                });
            });
        }
    }
}
