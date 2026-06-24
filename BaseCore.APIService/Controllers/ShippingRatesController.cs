

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;
using BaseCore.Repository;

using BaseCore.Services;

namespace BaseCore.APIService.Controllers
{

    // ════════════════════════════════════════════════════════════
    // REQUEST TÍNH CƯỚC
    // ════════════════════════════════════════════════════════════
    public class QuoteRequest
    {
        public int? CarrierId { get; set; }
        public int ProvinceId { get; set; }
        public string? ProvinceCode { get; set; }
        public decimal OrderTotal { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // CONTROLLER CƯỚC VẬN CHUYỂN
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/shipping-rates")]
    public class ShippingRatesController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _context;
        private readonly IShippingService _shippingService;
        public ShippingRatesController(MySqlDbContext context, IShippingService shippingService)
        {
            _context = context;
            _shippingService = shippingService;
        }

        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int? carrierId, [FromQuery] int? provinceId)
        {
            var query = _context.ShippingRates.AsQueryable();
            
            if (carrierId.HasValue) query = query.Where(r => r.CarrierId == carrierId);
            if (provinceId.HasValue) query = query.Where(r => r.ProvinceId == provinceId);
            var items = await query
                
                .OrderBy(r => r.CarrierId).ThenBy(r => r.ProvinceId)

                .Select(r => new
                {
                    r.Id, r.CarrierId, r.ProvinceId, r.Fee, r.EstimatedDays,
                    Carrier = new { r.Carrier!.Id, r.Carrier.Name, r.Carrier.Code },
                    Province = new { r.Province!.Id, r.Province.Name, r.Province.Code, r.Province.Region }
                })
                .ToListAsync();
            return Ok(items);
        }

        
        
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.ShippingRates
                .Where(x => x.Id == id)
                
                .Select(r => new
                {
                    r.Id, r.CarrierId, r.ProvinceId, r.Fee, r.EstimatedDays,
                    Carrier = new { r.Carrier!.Id, r.Carrier.Name, r.Carrier.Code },
                    Province = new { r.Province!.Id, r.Province.Name, r.Province.Code, r.Province.Region }
                })
                .FirstOrDefaultAsync();
            return item == null ? NotFound() : Ok(item);
        }

        
        
        // ════════════════════════════════════════════════════════════
        // TẠO MỚI (POST)
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ShippingRate rate)
        {
            
            var dup = await _context.ShippingRates
                .AnyAsync(x => x.CarrierId == rate.CarrierId && x.ProvinceId == rate.ProvinceId);
            if (dup) return BadRequest(new { message = "Đã có cước cho carrier+tỉnh này." });
            _context.ShippingRates.Add(rate);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = rate.Id }, rate);
        }

        
        
        // ════════════════════════════════════════════════════════════
        // CẬP NHẬT (PUT)
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingRate rate)
        {
            if (id != rate.Id) return BadRequest();
            var existing = await _context.ShippingRates.FindAsync(id);
            if (existing == null) return NotFound();
            existing.CarrierId = rate.CarrierId;
            existing.ProvinceId = rate.ProvinceId;
            existing.Fee = rate.Fee;
            existing.EstimatedDays = rate.EstimatedDays;
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
            var r = await _context.ShippingRates.FindAsync(id);
            if (r == null) return NotFound();
            _context.ShippingRates.Remove(r);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        

        

        // ════════════════════════════════════════════════════════════
        // QUOTE CƯỚC
        // ════════════════════════════════════════════════════════════
        [HttpPost("quote")]
        public async Task<IActionResult> Quote([FromBody] QuoteRequest req)
        {
            if (req.ProvinceId == 0 && !string.IsNullOrEmpty(req.ProvinceCode))
            {
                var p = await _context.Provinces.FirstOrDefaultAsync(x => x.Code == req.ProvinceCode);
                if (p == null) return BadRequest(new { message = "Tỉnh/thành không hợp lệ." });
                req.ProvinceId = p.Id;
            }
            try
            {
                if (req.CarrierId.HasValue)
                {
                    var q = await _shippingService.QuoteAsync(req.CarrierId.Value, req.ProvinceId, req.OrderTotal);

                    return Ok(new[] { q });
                }
                var all = await _shippingService.QuoteAllCarriersAsync(req.ProvinceId, req.OrderTotal);
                return Ok(all);
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
