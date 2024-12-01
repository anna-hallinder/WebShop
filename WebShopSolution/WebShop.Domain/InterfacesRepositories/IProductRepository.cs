using WebShop.Domain.Entities;

namespace WebShop.Domain.InterfacesRepositories;

public interface IProductRepository : IGenericRepository<ProductEntity, int>
{
    Task<IEnumerable<ProductEntity>> GetProductsByNameAsync(string name);
}