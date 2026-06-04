

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BaseCore.Repository.EFCore;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER TỒN KHO
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/inventory")]


    [Authorize(Roles = "Admin,WarehouseStaff")]
    public class InventoryController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly IProductVariantRepositoryEF _repo;
        public InventoryController(IProductVariantRepositoryEF repo) { _repo = repo; }



        // ════════════════════════════════════════════════════════════
        // [GET] DANH SÁCH & CHI TIẾT
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var items = await _repo.GetAllWithDetailsAsync(keyword, page, pageSize);
            var total = await _repo.CountAllAsync(keyword);
            return Ok(new { items, total, page, pageSize, totalPages = (int)Math.Ceiling((double)total / pageSize) });
        }

        

        

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 5)
            => Ok(await _repo.GetLowStockAsync(threshold));
    }
}
