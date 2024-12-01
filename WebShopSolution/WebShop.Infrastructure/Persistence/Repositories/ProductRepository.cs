using Microsoft.EntityFrameworkCore;
using WebShop.Domain.Entities;
using WebShop.Domain.InterfacesRepositories;

namespace WebShop.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<ProductEntity, int>, IProductRepository
    {
        private readonly WebShopDb _context;

        public ProductRepository(WebShopDb context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductEntity>> GetProductsByNameAsync(string name)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }
    }
}