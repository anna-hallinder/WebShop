using WebShop.Application.InterfacesServices;
using WebShop.Domain.Entities;
using WebShop.Infrastructure.Persistence.UnitOfWork;

public class GenericService<TEntity, TId> : IGenericService<TEntity, TId> where TEntity : BaseEntity<TId>
{
    protected readonly IUnitOfWork _unitOfWork;

    public GenericService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        var repository = _unitOfWork.Repository<TEntity, TId>();
        await repository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();
        return entity;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _unitOfWork.Repository<TEntity, TId>().GetAllAsync();
    }

    public async Task<TEntity?> GetByIdAsync(TId id)
    {
        return await _unitOfWork.Repository<TEntity, TId>().GetByIdAsync(id);
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        var repository = _unitOfWork.Repository<TEntity, TId>();

        var existingEntity = await repository.GetByIdAsync(entity.Id);
        if (existingEntity == null)
        {
            return false;
        }

        await repository.UpdateAsync(entity);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(TId id)
    {
        var repository = _unitOfWork.Repository<TEntity, TId>();
        var entity = await repository.GetByIdAsync(id);

        if (entity == null)
        {
            return false;
        }

        await repository.DeleteAsync(id);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}