using Microsoft.EntityFrameworkCore;
using Moq;
using WebShop.Domain.Entities;
using WebShop.Infrastructure.Persistence;
using WebShop.Infrastructure.Persistence.Repositories;


namespace WebShopTests.UnitTests.UnitOfWork
{
    public class UnitOfWorkTests
    {
        private readonly WebShop.Infrastructure.Persistence.UnitOfWork.UnitOfWork _unitOfWork;
        private readonly WebShopDb _context;

        public UnitOfWorkTests()
        {
            var options = new DbContextOptionsBuilder<WebShopDb>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new WebShopDb(options);
            _unitOfWork = new WebShop.Infrastructure.Persistence.UnitOfWork.UnitOfWork(_context);
        }

        [Fact]
        public void Repository_ShouldReturnGenericRepository()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<WebShopDb>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new WebShopDb(options);
            var unitOfWork = new WebShop.Infrastructure.Persistence.UnitOfWork.UnitOfWork(context);

            // Act
            var repository = unitOfWork.Repository<ProductEntity, int>();

            // Assert
            Assert.NotNull(repository);
            Assert.IsType<GenericRepository<ProductEntity, int>>(repository);
        }

        [Fact]
        public async Task CompleteAsync_ShouldReturnNumberOfAffectedRows()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };
            _context.Products.Add(product);

            // Act
            var result = await _unitOfWork.CompleteAsync();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Dispose_ShouldDisposeContext()
        {
            // Arrange
            var contextMock = new Mock<WebShopDb>();
            var unitOfWork = new WebShop.Infrastructure.Persistence.UnitOfWork.UnitOfWork(contextMock.Object);

            // Act
            unitOfWork.Dispose();

            // Assert
            contextMock.Verify(c => c.Dispose(), Times.Once);
        }
    }
}