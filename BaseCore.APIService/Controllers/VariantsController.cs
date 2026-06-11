

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseCore.Entities;

using BaseCore.Repository.EFCore;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // DTO REQUEST
    // ════════════════════════════════════════════════════════════
    public class StockAdjustDto
    {
        public int Delta { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // CONTROLLER BIẾN THỂ
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/variants")]
    public class VariantsController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly IProductVariantRepositoryEF _repo;
        public VariantsController(IProductVariantRepositoryEF repo) { _repo = repo; }

        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var v = await _repo.GetByIdAsync(id);
            return v == null ? NotFound() : Ok(v);
        }

        

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var items = await _repo.GetByProductIdAsync(productId);
            return Ok(items);
        }

        

        [HttpGet("sku/{sku}")]
        public async Task<IActionResult> GetBySku(string sku)
        {
            var v = await _repo.GetBySkuAsync(sku);
            return v == null ? NotFound() : Ok(v);
        }

        
        
        // ════════════════════════════════════════════════════════════
        // TẠO MỚI (POST)
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize(Roles = "Admin,WarehouseStaff")]
        public async Task<IActionResult> Create([FromBody] ProductVariant variant)
        {
            
            if (await _repo.GetBySkuAsync(variant.SKU) != null)
                return BadRequest(new { message = "SKU đã tồn tại." });
            var saved = await _repo.AddAsync(variant);
            return CreatedAtAction(nameof(GetById), new { id = saved.Id }, saved);
        }

        
        
        // ════════════════════════════════════════════════════════════
        // CẬP NHẬT (PUT)
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,WarehouseStaff")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductVariant variant)
        {
            if (id != variant.Id) return BadRequest();
            
            await _repo.UpdateAsync(variant);
            return NoContent();
        }

        

        
        // ════════════════════════════════════════════════════════════
        // ĐIỀU CHỈNH TỒN KHO
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}/stock")]
        [Authorize(Roles = "Admin,WarehouseStaff")]
        public async Task<IActionResult> AdjustStock(int id, [FromBody] StockAdjustDto dto)
        {
            var v = await _repo.GetByIdAsync(id);
            if (v == null) return NotFound();
            v.Stock = Math.Max(0, v.Stock + dto.Delta);
            await _repo.UpdateAsync(v);
            return Ok(v);
        }

        
        
        // ════════════════════════════════════════════════════════════
        // XÓA (DELETE)
        // ════════════════════════════════════════════════════════════
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
