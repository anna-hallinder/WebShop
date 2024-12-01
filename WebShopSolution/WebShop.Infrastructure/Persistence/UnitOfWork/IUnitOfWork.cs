using WebShop.Domain.Entities;
using WebShop.Domain.InterfacesRepositories;

namespace WebShop.Infrastructure.Persistence.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity, TId> Repository<TEntity, TId>() where TEntity : BaseEntity<TId>;

        IProductRepository Products { get; }

        Task<int> CompleteAsync();
    }
}