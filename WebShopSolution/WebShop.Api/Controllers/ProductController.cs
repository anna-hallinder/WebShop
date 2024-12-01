using Microsoft.AspNetCore.Mvc;
using WebShop.Application.InterfacesServices;
using WebShop.Domain.Entities;
using WebShop.Infrastructure.Persistence.UnitOfWork;

namespace WebShop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _productService = productService;
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductEntity product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest();
                }

                var result = await _productService.AddAsync(product);
                return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the product.");
                return StatusCode(500, "An error occurred while adding the product.");
            }
        }


        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return Ok(products);
        }

        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {id} not found.");
                return NotFound();
            }

            return Ok(product);
        }


        // PUT: api/Product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductEntity updatedProduct)
        {
            if (id != updatedProduct.Id)
            {
                return BadRequest("Product ID mismatch.");
            }

            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            await _unitOfWork.Products.UpdateAsync(updatedProduct);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Product with ID {id} updated successfully.");
            return NoContent();
        }

        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Product with ID {id} deleted successfully.");
            return NoContent();
        }

        // GET: api/Product/search?name=value
        [HttpGet("search")]
        public async Task<IActionResult> SearchProductsByName([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Search query cannot be null or empty.");
            }

            var products = await _unitOfWork.Products.GetProductsByNameAsync(name);
            return Ok(products);
        }
    }
}