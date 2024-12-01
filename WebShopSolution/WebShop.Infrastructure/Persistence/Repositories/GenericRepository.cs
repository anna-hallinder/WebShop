using Microsoft.EntityFrameworkCore;
using WebShop.Domain.Entities;
using WebShop.Domain.InterfacesRepositories;

namespace WebShop.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> where TEntity : BaseEntity<TId>
    {
        private readonly WebShopDb _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(WebShopDb context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<TEntity> GetByIdAsync(TId id)
        {
            return await _context.Set<TEntity>()
                .AsNoTracking() // Lägg till detta
                .FirstOrDefaultAsync(e => e.Id.Equals(id));
        }


        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public async Task DeleteAsync(TId id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}