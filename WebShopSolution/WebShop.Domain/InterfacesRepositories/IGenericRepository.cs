using WebShop.Domain.Entities;

namespace WebShop.Domain.InterfacesRepositories;

public interface IGenericRepository<TEntity, TId> where TEntity : BaseEntity<TId>
{
    Task AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TId id);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TId id);
}