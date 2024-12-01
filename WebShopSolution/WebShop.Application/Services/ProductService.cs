using WebShop.Application.InterfacesServices;
using WebShop.Application.Notifications;
using WebShop.Domain.Entities;
using WebShop.Infrastructure.Persistence.UnitOfWork;

namespace WebShop.Application.Services;

public class ProductService : GenericService<ProductEntity, int>, IProductService
{
    private readonly ProductSubject _productSubject;

    public ProductService(IUnitOfWork unitOfWork, ProductSubject productSubject)
        : base(unitOfWork)
    {
        _productSubject = productSubject ?? throw new ArgumentNullException(nameof(productSubject));
    }

    public override async Task<ProductEntity> AddAsync(ProductEntity entity)
    {
        var product = await base.AddAsync(entity);
        _productSubject.Notify(product);
        return product;
    }

    public async Task<IEnumerable<ProductEntity>> GetProductsByNameAsync(string name)
    {
        return await _unitOfWork.Products.GetProductsByNameAsync(name);
    }
}