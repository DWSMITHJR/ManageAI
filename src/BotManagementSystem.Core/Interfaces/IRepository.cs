using System.Linq.Expressions;
using BotManagementSystem.Core.Entities;

namespace BotManagementSystem.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }

    public interface IBotRepository : IRepository<Bot>
    {
        Task<IEnumerable<Bot>> GetActiveBotsAsync();
        Task<Bot?> GetBotWithIntegrationsAsync(Guid id);
    }
}
