

using Microsoft.AspNetCore.Mvc;

using BaseCore.Repository;

using Microsoft.EntityFrameworkCore;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER LOOKUP (DỮ LIỆU TRA CỨU)
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/lookups")]

    public class LookupsController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _context;
        public LookupsController(MySqlDbContext context) { _context = context; }



        // ════════════════════════════════════════════════════════════
        // [GET] DANH SÁCH & CHI TIẾT
        // ════════════════════════════════════════════════════════════
        [HttpGet("colors")]
        public async Task<IActionResult> Colors() => Ok(await _context.Colors.OrderBy(c => c.Name).ToListAsync());

        

        
        
        [HttpGet("sizes")]
        public async Task<IActionResult> Sizes() => Ok(await _context.Sizes.OrderBy(s => s.Id).ToListAsync());

        

        
        
        [HttpGet("size-guide")]
        public async Task<IActionResult> SizeGuide()
        {
            
            var guides = await _context.SizeGuides
                
                .Include(g => g.Size)

                .OrderBy(g => g.Size!.Id)
                .ToListAsync();
            return Ok(guides);
        }

        

        
        
        [HttpGet("banners")]
        public async Task<IActionResult> Banners()
        {
            var banners = await _context.Banners
                
                .Where(b => b.IsActive)
                
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();
            return Ok(banners);
        }
    }
}
