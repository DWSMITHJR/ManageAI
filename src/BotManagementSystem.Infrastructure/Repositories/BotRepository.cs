using System.Linq.Expressions;
using BotManagementSystem.Core.Entities;
using BotManagementSystem.Core.Interfaces;
using BotManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BotManagementSystem.Infrastructure.Repositories
{
    public class BotRepository : IBotRepository
    {
        private readonly AppDbContext _context;

        public BotRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Bot?> GetByIdAsync(Guid id)
        {
            return await _context.Bots
                .Include(b => b.Integrations)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Bot>> GetAllAsync()
        {
            return await _context.Bots
                .Include(b => b.Integrations)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bot>> FindAsync(Expression<Func<Bot, bool>> predicate)
        {
            return await _context.Bots
                .Include(b => b.Integrations)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bot>> GetActiveBotsAsync()
        {
            return await _context.Bots
                .Include(b => b.Integrations)
                .Where(b => b.IsActive)
                .ToListAsync();
        }

        public async Task<Bot?> GetBotWithIntegrationsAsync(Guid id)
        {
            return await _context.Bots
                .Include(b => b.Integrations)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Bot entity)
        {
            await _context.Bots.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bot entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Bots.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<Bot, bool>> predicate)
        {
            return await _context.Bots.AnyAsync(predicate);
        }
    }
}
