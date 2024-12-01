using Microsoft.EntityFrameworkCore;
using WebShop.Domain.Entities;
using WebShop.Domain.InterfacesRepositories;
using WebShop.Infrastructure.Persistence.Repositories;

namespace WebShop.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WebShopDb _context;
        private readonly DbSet<ProductEntity> _dbSet;
        public IProductRepository Products { get; set; }
        public UnitOfWork(WebShopDb context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Products = new ProductRepository(_context);
        }

        public IGenericRepository<TEntity, TId> Repository<TEntity, TId>() where TEntity : BaseEntity<TId>
        {
            return new GenericRepository<TEntity, TId>(_context);

        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
            
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}