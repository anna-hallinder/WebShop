using WebShop.Domain.Entities;
using WebShop.Infrastructure.Persistence.Repositories;
using WebShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WebShopTests.UnitTests.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly ProductRepository _productRepository;
        private readonly WebShopDb _context;
        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<WebShopDb>()
                .UseInMemoryDatabase(databaseName: "WebShopTest")
                .Options;
            _context = new WebShopDb(options);
            _productRepository = new ProductRepository(_context);
        }


        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            // Arrange
            var product = new ProductEntity()
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100.00m
            };

            // Act
            await _productRepository.AddAsync(product);
            await _context.SaveChangesAsync();
            var addedProduct = await _context.Products.FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(addedProduct);
            Assert.Equal(product.Name, addedProduct.Name);
            Assert.Equal(product.Description, addedProduct.Description);
            Assert.Equal(product.Price, addedProduct.Price);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct()
        {
            // Arrange
            var product = new ProductEntity()
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100.00m
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var addedProduct = await _productRepository.GetByIdAsync(product.Id);

            // Assert
            Assert.NotNull(addedProduct);
            Assert.Equal(product.Name, addedProduct.Name);
            Assert.Equal(product.Description, addedProduct.Description);
            Assert.Equal(product.Price, addedProduct.Price);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var product1 = new ProductEntity()
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100.00m
            };
            var product2 = new ProductEntity()
            {
                Name = "Another Product",
                Description = "Another Description",
                Price = 200.00m
            };
            await _context.Products.AddAsync(product1);
            await _context.Products.AddAsync(product2);
            await _context.SaveChangesAsync();

            // Act
            var products = await _productRepository.GetAllAsync();

            // Assert
            Assert.Contains(product1, products);
            Assert.Contains(product2, products);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            // Arrange
            var product = new ProductEntity()
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100.00m
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            product.Name = "Updated Product";
            await _productRepository.UpdateAsync(product);
            await _context.SaveChangesAsync();
            var updatedProduct = await _context.Products.FindAsync(product.Id);

            // Assert
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product", updatedProduct.Name);
            Assert.Equal(product.Description, updatedProduct.Description);
            Assert.Equal(product.Price, updatedProduct.Price);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProductById()
        {
            // Arrange
            var product = new ProductEntity()
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100.00m
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _productRepository.DeleteAsync(product.Id);
            var deletedProduct = await _context.Products.FindAsync(product.Id);

            // Assert
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task GetProductsByNameAsync_ShouldReturnProducts()
        {
            // Arrange
            var product1 = new ProductEntity()
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100.00m
            };
            var product2 = new ProductEntity()
            {
                Name = "Another Product",
                Description = "Another Description",
                Price = 200.00m
            };
            await _context.Products.AddAsync(product1);
            await _context.Products.AddAsync(product2);
            await _context.SaveChangesAsync();

            // Act
            var products = await _productRepository.GetProductsByNameAsync("Test");

            // Assert
            Assert.Contains(product1, products);
            Assert.DoesNotContain(product2, products);
        }
    }
}