using WebShop.Domain.Entities;

namespace WebShop.Application.InterfacesServices;

public interface IProductService
{
    Task<ProductEntity> AddAsync(ProductEntity entity);
    Task<IEnumerable<ProductEntity>> GetProductsByNameAsync(string name);
}