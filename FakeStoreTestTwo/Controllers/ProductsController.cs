using FakeStoreTwo.Models;
using FakeStoreTwo.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeStoreTestTwo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var results = await _service.GetAll();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);
            if(result == null)
            {
                return NotFound(new { Message = $"The requested item was not found." });
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _service.DeleteProduct(id);
                if (result == null || result == false)
                {
                    return NotFound(new { Message = $"Cannot find the product with the ID: {id}" });
                }
                return Ok(result);
            }
            catch(Exception e)
            {
                throw new Exception($"Unhandled exception. Exception: {e.Message}");
            }
            
        }

        [HttpGet("products/{category}")]
        public async Task<ActionResult> GetByCategory(string category)
        {
            try
            {
                var result = await _service.GetByCategory(category);
                if(result == null || result.Count() == 0)
                {
                    return NotFound(new { Message = $"Cannot find products with the category : {category}" });
                }
                return Ok(result);
            }
            catch(Exception e)
            {
                throw new Exception($"Unhandled exception: Exception : {e.Message}");
            }
        }

        [HttpGet("products/categories")]
        public async Task<ActionResult> GetAllCategories()
        {
            var result = await _service.GetCategories();
            return Ok(result);
        }

        [HttpPost("multiple")]
        public async Task<ActionResult> PostMultiple([FromBody] List<ProductModel> products)
        {
            try
            {
                var result = await _service.PostMultipleProducts(products);
                if (result == null || result.Count() == 0)
                {
                    return BadRequest(new { Message = "Unable to post the products" });
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new Exception($"Unhandled exception: Exception : {e.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostProduct([FromBody] ProductModel product)
        {
            try
            {
                var result = await _service.PostProduct(product);
                if (result == null)
                {
                    return BadRequest(new { Message = "Unable to post the product" });
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new Exception($"Unhandled exception: Exception : {e.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutProduct(int id, [FromBody] ProductModel product)
        {
            try
            {
                var result = await _service.PutProduct(id, product);
                if(result == null || result == false)
                {
                    return NotFound(new { Message = $"Unable to put the product with ID: {id}" });
                }
                return Ok(result);
            }
            catch(Exception e)
            {
                throw new Exception($"Unhandled exception. Exception : {e.Message}");
            }
        }

        [HttpGet("pagination")]
        public async Task<ActionResult> PaginateProducts([FromQuery]int? page, [FromQuery]int? pageSize, [FromQuery] string? sortOrder, [FromQuery] string? sortField)
        {
            var result = await _service.PaginateProducts(page, pageSize, sortOrder, sortField);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult> Search([FromQuery] string? searchField, [FromQuery] string? searchParameter)
        {
            var result = await _service.SearchProducts(searchField, searchParameter);
            return Ok(result);
        }

        [HttpGet("avg-rate-by-category")]
        public async Task<ActionResult> GetAvgRateByCategory()
        {
            var result = await _service.GetAvgRateByCategory();
            return Ok(result);
        }


        [HttpGet("avg-count-by-category")]
        public async Task<ActionResult> GetCountRateByCategory()
        {
            var result = await _service.GetAvgCountByCategory();
            return Ok(result);
        }
    }
}
