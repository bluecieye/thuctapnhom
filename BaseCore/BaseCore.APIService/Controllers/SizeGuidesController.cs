

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BaseCore.Entities;
using BaseCore.Repository;
using Microsoft.EntityFrameworkCore;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER HƯỚNG DẪN SIZE
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/size-guides")]
    public class SizeGuidesController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _context;
        public SizeGuidesController(MySqlDbContext context) { _context = context; }

        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════


        
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.SizeGuides
                
                .Include(g => g.Size)
                
                .OrderBy(g => g.SizeId)
                .ToListAsync();
            return Ok(items);
        }

        
        // ════════════════════════════════════════════════════════════
        // TẠO MỚI (POST)
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> Create([FromBody] SizeGuide guide)
        {
            
            var exists = await _context.SizeGuides.AnyAsync(g => g.SizeId == guide.SizeId);
            if (exists)
                return BadRequest(new { message = "Size này đã có bảng đo." });
            _context.SizeGuides.Add(guide);
            await _context.SaveChangesAsync();
            
            var saved = await _context.SizeGuides
                .Include(g => g.Size)
                .FirstAsync(g => g.Id == guide.Id);
            return Ok(saved);
        }

        

        // ════════════════════════════════════════════════════════════
        // CẬP NHẬT (PUT)
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Marketing,Warehouse")]
        public async Task<IActionResult> Update(int id, [FromBody] SizeGuide guide)
        {
            var existing = await _context.SizeGuides.FindAsync(id);
            if (existing == null) return NotFound();
            
            existing.SizeId = guide.SizeId;
            existing.Chest = guide.Chest;
            existing.Waist = guide.Waist;
            existing.Shoulder = guide.Shoulder;
            existing.Length = guide.Length;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        
        
        // ════════════════════════════════════════════════════════════
        // XÓA (DELETE)
        // ════════════════════════════════════════════════════════════
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Marketing,Warehouse")]
        public async Task<IActionResult> Delete(int id)
        {
            var guide = await _context.SizeGuides.FindAsync(id);
            if (guide == null) return NotFound();
            _context.SizeGuides.Remove(guide);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
