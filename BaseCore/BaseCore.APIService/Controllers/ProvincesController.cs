

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using BaseCore.Repository;

namespace BaseCore.APIService.Controllers
{

    // ════════════════════════════════════════════════════════════
    // CONTROLLER TỈNH/THÀNH
    // ════════════════════════════════════════════════════════════
    [ApiController]

    [Route("api/provinces")]
    public class ProvincesController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _context;

        public ProvincesController(MySqlDbContext context) { _context = context; }



        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            
            var items = await _context.Provinces

                
                .OrderBy(p => p.Id <= 5 ? 0 : 1) 
                
                .ThenBy(p => p.Name)
                
                .ToListAsync();
            
            return Ok(items);
        }

        

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            
            var p = await _context.Provinces.FindAsync(id);
            
            return p == null ? NotFound() : Ok(p);
        }
    }
}
