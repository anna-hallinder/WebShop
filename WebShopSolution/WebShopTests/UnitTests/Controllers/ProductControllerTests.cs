using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebShop.Api.Controllers;
using WebShop.Application.InterfacesServices;
using WebShop.Domain.Entities;
using WebShop.Infrastructure.Persistence.UnitOfWork;

namespace WebShopTests.UnitTests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductController>>();

            _controller = new ProductController(_unitOfWorkMock.Object, _loggerMock.Object, _productServiceMock.Object);
        }

        // AddProduct
        // 1. Lyckat Tillägg. Testa att metoden returnerar CreatedAtActionResult när en produkt läggs till framgångsrikt.
        // 2. Ogiltiga Data. Null-produkt: Testa att metoden returnerar BadRequest om produkten är null.
        // 3. Felhantering. Testa att metoden loggar och returnerar 500 Internal Server Error om ett undantag inträffar i tjänstelagret.

        [Fact]
        public async Task AddProduct_ReturnsCreatedAtActionResult_WhenProductIsAdded()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };
            _productServiceMock
                .Setup(service => service.AddAsync(product))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.AddProduct(product);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<ProductEntity>(createdResult.Value);
            Assert.Equal(product.Id, returnValue.Id);
            Assert.Equal(product.Name, returnValue.Name);
            Assert.Equal(nameof(_controller.GetProductById), createdResult.ActionName);
        }

        [Fact]
        public async Task AddProduct_ReturnsBadRequest_WhenProductIsNull()
        {
            // Arrange
            ProductEntity product = null;

            // Act
            var result = await _controller.AddProduct(product);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddProduct_Returns500_WhenServiceThrowsException()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };
            var exceptionMessage = "Service error";
            var exception = new Exception(exceptionMessage);

            _productServiceMock
                .Setup(service => service.AddAsync(product))
                .ThrowsAsync(exception);

            // Act
            var result = await _controller.AddProduct(product);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while adding the product.", statusCodeResult.Value);

            // Verify logging
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while adding the product.")),
                    exception,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        // GetAllProducts
        // 1.  Lyckad hämtning av produkter. Testa att metoden returnerar en 200 OK-status med rätt lista av produkter.
        // 2.  När inga produkter finns. Testa att metoden returnerar en tom lista när inga produkter finns i databasen.

        [Fact]
        public async Task GetAllProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<ProductEntity>
            {
                new ProductEntity { Id = 1, Name = "Product 1" },
                new ProductEntity { Id = 2, Name = "Product 2" }
            };
            _unitOfWorkMock
                .Setup(uow => uow.Products.GetAllAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProductEntity>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Equal(products, returnValue);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkResult_WithEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var products = new List<ProductEntity>();
            _unitOfWorkMock
                .Setup(uow => uow.Products.GetAllAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProductEntity>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        // GetProductById
        // 1. Lyckad hämtning. Testa att metoden returnerar en 200 OK-status med rätt produkt.
        // 2. Produkt saknas. Testa att metoden returnerar en NotFound-status om produkten inte finns i databasen.
        // 3. Loggning. Testa att metoden loggar en varning om produkten inte finns i databasen.

        [Fact]
        public async Task GetProductById_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };
            _unitOfWorkMock
                .Setup(uow => uow.Products.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductById(product.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProductEntity>(okResult.Value);
            Assert.Equal(product, returnValue);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;
            _unitOfWorkMock
                .Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync((ProductEntity)null);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProductById_LogsWarning_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;
            _unitOfWorkMock
                .Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync((ProductEntity)null);

            // Act
            await _controller.GetProductById(productId);

            // Assert
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Product with ID {productId} not found.")),
                    null,
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        // UpdateProduct
        // 1. Lyckad uppdatering. Testa att metoden returnerar NoContent när en produkt uppdateras.
        // 2. Ogiltig produkt. Testa att metoden returnerar BadRequest om produkt-ID inte matchar.
        // 3. Produkt saknas. Testa att metoden returnerar NotFound om produkten inte finns i databasen.
        // 4. Felhantering. Testa att metoden kastar ett undantag om uppdateringen misslyckas.

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent_WhenProductIsUpdated()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };
            _unitOfWorkMock
                .Setup(uow => uow.Products.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.UpdateProduct(product.Id, product);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify update was called
            _unitOfWorkMock.Verify(uow => uow.Products.UpdateAsync(product), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsBadRequest_WhenProductIdDoesNotMatch()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };

            // Act
            var result = await _controller.UpdateProduct(2, product);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };
            _unitOfWorkMock
                .Setup(uow => uow.Products.GetByIdAsync(product.Id))
                .ReturnsAsync((ProductEntity)null);

            // Act
            var result = await _controller.UpdateProduct(product.Id, product);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_ThrowsException_WhenUpdateFails()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "Test Product" };

            _unitOfWorkMock
                .Setup(uow => uow.Products.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            _unitOfWorkMock
                .Setup(uow => uow.Products.UpdateAsync(product))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            Exception exception = await Assert.ThrowsAsync<Exception>(() => _controller.UpdateProduct(product.Id, product));

            // Assert
            Assert.Equal("Database error", exception.Message);
        }


        // DeleteProduct
        // 1. Lyckad borttagning. Testa att metoden returnerar NoContent när en produkt tas bort.
        // 2. Produkt saknas. Testa att metoden returnerar NotFound om produkten inte finns i databasen.
        // 3. Felhantering. Testa att metoden kastar ett undantag om borttagningen misslyckas.

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenProductIsDeleted()
        {
            // Arrange
            var productId = 1;
            var product = new ProductEntity { Id = productId, Name = "Test Product" };
            _unitOfWorkMock
                .Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _unitOfWorkMock.Verify(uow => uow.Products.DeleteAsync(productId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;
            _unitOfWorkMock
                .Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync((ProductEntity)null);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _unitOfWorkMock.Verify(uow => uow.Products.DeleteAsync(It.IsAny<int>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteProduct_ThrowsException_WhenDeleteFails()
        {
            // Arrange
            var productId = 1;
            var product = new ProductEntity { Id = productId, Name = "Test Product" };

            _unitOfWorkMock
                .Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _unitOfWorkMock
                .Setup(uow => uow.Products.DeleteAsync(productId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            Exception exception = await Assert.ThrowsAsync<Exception>(() => _controller.DeleteProduct(productId));

            // Assert
            Assert.Equal("Database error", exception.Message);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        }


        // SearchProductsByName
        // 1. Lyckad sökning. Testa att metoden returnerar en 200 OK-status med matchande produkter.
        // 2. Inga matchningar. Testa att metoden returnerar en 200 OK-status med en tom lista om inga matchningar finns.
        // 3. Ogiltig sökterm. Testa att metoden returnerar BadRequest om söktermen är null eller tom.

        [Fact]
        public async Task SearchProductsByName_ReturnsOkResult_WithMatchingProducts()
        {
            // Arrange
            var searchTerm = "Test";
            var matchingProducts = new List<ProductEntity>
            {
                new ProductEntity { Id = 1, Name = "Test Product 1" },
                new ProductEntity { Id = 2, Name = "Test Product 2" }
            };

            _unitOfWorkMock
                .Setup(uow => uow.Products.GetProductsByNameAsync(searchTerm))
                .ReturnsAsync(matchingProducts);

            // Act
            var result = await _controller.SearchProductsByName(searchTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProductEntity>>(okResult.Value);
            Assert.Equal(matchingProducts.Count, returnValue.Count);
            Assert.Equal(matchingProducts, returnValue);
        }

        [Fact]
        public async Task SearchProductsByName_ReturnsOkResult_WithEmptyList_WhenNoMatchesExist()
        {
            // Arrange
            var searchTerm = "NonExistent";
            var emptyList = new List<ProductEntity>();

            _unitOfWorkMock
                .Setup(uow => uow.Products.GetProductsByNameAsync(searchTerm))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.SearchProductsByName(searchTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProductEntity>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task SearchProductsByName_ReturnsBadRequest_WhenSearchTermIsNullOrEmpty()
        {
            // Arrange
            string searchTerm = null;

            // Act
            var result = await _controller.SearchProductsByName(searchTerm);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Search query cannot be null or empty.", badRequestResult.Value);
        }
    }
}