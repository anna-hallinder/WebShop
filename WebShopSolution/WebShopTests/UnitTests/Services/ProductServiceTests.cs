using WebShop.Domain.Entities;
using WebShop.Application.Notifications;
using WebShop.Infrastructure.Persistence.UnitOfWork;
using Moq;
using WebShop.Application.Services;
using WebShop.Domain.InterfacesRepositories;

namespace WebShopTests.UnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly ProductService _productService;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ProductSubject> _productSubjectMock;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productSubjectMock = new Mock<ProductSubject>();

            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);

            _productService = new ProductService(
                _unitOfWorkMock.Object,
                _productSubjectMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddProductAndNotify()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };

            _productRepositoryMock
                .Setup(repo => repo.AddAsync(product))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.Repository<ProductEntity, int>())
                .Returns(_productRepositoryMock.Object);

            _unitOfWorkMock
                .Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            _productSubjectMock
                .Setup(ps => ps.Notify(product))
                .Verifiable();

            // Act
            var addedProduct = await _productService.AddAsync(product);

            // Assert
            Assert.Equal(product, addedProduct);
            _productRepositoryMock.Verify(repo => repo.AddAsync(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
            _productSubjectMock.Verify(ps => ps.Notify(product), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };

            _unitOfWorkMock
                .Setup(u => u.Repository<ProductEntity, int>())
                .Returns(_productRepositoryMock.Object);

            _productRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<ProductEntity>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => _productService.AddAsync(product));

            // Assert
            Assert.Equal("Database error", exception.Message);

            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<ProductEntity>
            {
                new ProductEntity { Id = 1, Name = "Product 1" },
                new ProductEntity { Id = 2, Name = "Product 2" }
            };

            _unitOfWorkMock
                .Setup(u => u.Repository<ProductEntity, int>())
                .Returns(_productRepositoryMock.Object);

            _productRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            Assert.Equal(products, result);
            _productRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoProductsExist()
        {
            // Arrange
            _unitOfWorkMock
                .Setup(u => u.Repository<ProductEntity, int>())
                .Returns(_productRepositoryMock.Object);

            _productRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<ProductEntity>());

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            Assert.Empty(result);
            _productRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };

            _productRepositoryMock
                .Setup(repo => repo.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            _unitOfWorkMock
                .Setup(u => u.Repository<ProductEntity, int>())
                .Returns(_productRepositoryMock.Object);

            // Act
            var result = await _productService.GetByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Name, result.Name);

            _productRepositoryMock.Verify(repo => repo.GetByIdAsync(product.Id), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Updated Product" };

            _productRepositoryMock
                .Setup(repo => repo.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            _unitOfWorkMock
                .Setup(u => u.Repository<ProductEntity, int>())
                .Returns(_productRepositoryMock.Object);

            _productRepositoryMock
                .Setup(repo => repo.UpdateAsync(product))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _productService.UpdateAsync(product);

            // Assert
            Assert.True(result);
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Non-existent Product" };

            _productRepositoryMock
                .Setup(repo => repo.GetByIdAsync(product.Id))
                .ReturnsAsync((ProductEntity)null);

            _unitOfWorkMock
                .Setup(u => u.Repository<ProductEntity, int>())
                .Returns(_productRepositoryMock.Object);

            // Act
            var result = await _productService.UpdateAsync(product);

            // Assert
            Assert.False(result);
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<ProductEntity>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProduct()
        {
            // Arrange
            var productId = 1;
            var product = new ProductEntity { Id = productId };

            _productRepositoryMock
                .Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _unitOfWorkMock
                .Setup(u => u.Repository<ProductEntity, int>())
                .Returns(_productRepositoryMock.Object);

            _productRepositoryMock
                .Setup(repo => repo.DeleteAsync(productId))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(u => u.CompleteAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _productService.DeleteAsync(productId);

            // Assert
            Assert.True(result);
            _productRepositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }


        [Fact]
        public async Task GetProductsByNameAsync_ShouldReturnMatchingProducts()
        {
            // Arrange
            var searchTerm = "Test";
            var matchingProducts = new List<ProductEntity>
            {
                new ProductEntity { Id = 1, Name = "Test Product 1" },
                new ProductEntity { Id = 2, Name = "Test Product 2" }
            };
            _productRepositoryMock
                .Setup(repo => repo.GetProductsByNameAsync(searchTerm))
                .ReturnsAsync(matchingProducts);

            // Act
            var result = await _productService.GetProductsByNameAsync(searchTerm);

            // Assert
            Assert.Equal(matchingProducts, result);
            _productRepositoryMock.Verify(repo => repo.GetProductsByNameAsync(searchTerm), Times.Once);
        }
    }
}