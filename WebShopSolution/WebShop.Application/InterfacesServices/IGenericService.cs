namespace WebShop.Application.InterfacesServices;

public interface IGenericService<TEntity, TId>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TId id);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TId id);
}