

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using BaseCore.Entities;
using BaseCore.Repository;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER ĐƠN VỊ VẬN CHUYỂN
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/shipping-carriers")]
    public class ShippingCarriersController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _context;
        public ShippingCarriersController(MySqlDbContext context) { _context = context; }



        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? activeOnly = null)
        {
            
            var query = _context.ShippingCarriers.AsQueryable();
            
            if (activeOnly == true) query = query.Where(c => c.IsActive);
            
            return Ok(await query.OrderBy(c => c.Id).ToListAsync());
        }

        
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _context.ShippingCarriers.FindAsync(id);
            return c == null ? NotFound() : Ok(c);
        }

        
        
        // ════════════════════════════════════════════════════════════
        // TẠO MỚI (POST)
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ShippingCarrier carrier)
        {
            
            _context.ShippingCarriers.Add(carrier);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetById), new { id = carrier.Id }, carrier);
        }

        
        
        // ════════════════════════════════════════════════════════════
        // CẬP NHẬT (PUT)
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingCarrier carrier)
        {
            
            if (id != carrier.Id) return BadRequest();
            
            var existing = await _context.ShippingCarriers.FindAsync(id);
            if (existing == null) return NotFound();
            
            existing.Name = carrier.Name;
            existing.Code = carrier.Code;
            existing.LogoFileName = carrier.LogoFileName;
            existing.IsActive = carrier.IsActive;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        

        // ════════════════════════════════════════════════════════════
        // XÓA (DELETE)
        // ════════════════════════════════════════════════════════════
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.ShippingCarriers.FindAsync(id);
            if (c == null) return NotFound();
            _context.ShippingCarriers.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
