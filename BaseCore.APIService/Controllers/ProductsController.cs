

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BaseCore.Entities;

using BaseCore.Services;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/products")]

    public class ProductsController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly IProductService _service;

        public ProductsController(IProductService service) { _service = service; }

        

        

        

        

        

        

        

        

        
        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? keyword,
            [FromQuery] int? categoryId,
            [FromQuery] List<int>? categoryIds,
            [FromQuery] string? gender,
            [FromQuery] string? season,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? sizeId,
            [FromQuery] int? colorId,
            [FromQuery] bool inStockOnly = false,
            [FromQuery] bool newOnly = false,
            [FromQuery] string? sortBy = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {

            
            
            var (items, total, priceMin, priceMax) = await _service.SearchAsync(
                keyword, categoryId, categoryIds, gender, season,
                minPrice, maxPrice, sizeId, colorId, inStockOnly, newOnly, sortBy, page, pageSize);
            return Ok(new
            {
                items, totalCount = total, priceMin, priceMax, page, pageSize,
                
                totalPages = (int)Math.Ceiling((double)total / pageSize)
            });
        }

        

        
        
        // ════════════════════════════════════════════════════════════
        // CHI TIẾT SẢN PHẨM (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        

        

        // ════════════════════════════════════════════════════════════
        // DANH SÁCH NỔI BẬT (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet("new-arrivals")]
        public async Task<IActionResult> NewArrivals([FromQuery] int limit = 12)
            => Ok(await _service.GetNewArrivalsAsync(Math.Clamp(limit, 1, 30)));

        

        
        
        [HttpGet("best-sellers")]
        public async Task<IActionResult> BestSellers([FromQuery] int limit = 30)
            => Ok(await _service.GetBestSellersAsync(Math.Clamp(limit, 1, 30)));

        

        

        // ════════════════════════════════════════════════════════════
        // TẠO MỚI (POST)
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            var saved = await _service.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = saved.Id }, saved);
        }

        

        
        
        // ════════════════════════════════════════════════════════════
        // CẬP NHẬT (PUT)
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            if (id != product.Id) return BadRequest();
            await _service.UpdateAsync(product);
            return NoContent();
        }

        

        

        // ════════════════════════════════════════════════════════════
        // XÓA (DELETE)
        // ════════════════════════════════════════════════════════════
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
