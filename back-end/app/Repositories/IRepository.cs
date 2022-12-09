using app.Entities;

namespace app.Repositories
{
    public interface IRepository<T> where T : IEntity
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task CreateAsync(T entity);
    }
}