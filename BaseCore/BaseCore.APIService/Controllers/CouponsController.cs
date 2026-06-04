

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BaseCore.Entities;

using BaseCore.Services;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER MÃ GIẢM GIÁ
    // ════════════════════════════════════════════════════════════
    [ApiController]

    [Route("api/coupons")]

    public class CouponsController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly ICouponService _service;

        public CouponsController(ICouponService service) { _service = service; }



        // ════════════════════════════════════════════════════════════
        // [GET] DANH SÁCH & CHI TIẾT
        // ════════════════════════════════════════════════════════════
        [HttpGet("active")]
        public async Task<IActionResult> Active() => Ok(await _service.GetActiveAsync());

        // GET /api/coupons — admin lấy TẤT CẢ coupon (kể cả hết hạn / inactive) để quản lý.
        [HttpGet]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        

        
        
        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var c = await _service.GetByCodeAsync(code);
            return c == null ? NotFound() : Ok(c);
        }



        // ════════════════════════════════════════════════════════════
        // CRUD ADMIN — [POST] TẠO MỚI
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> Create([FromBody] Coupon coupon)
        {
            
            if (await _service.GetByCodeAsync(coupon.Code) != null)
                return BadRequest(new { message = "Mã coupon đã tồn tại." });
            return Ok(await _service.CreateAsync(coupon));
        }

        

        
        
        // ════════════════════════════════════════════════════════════
        // CRUD ADMIN — [PUT] CẬP NHẬT
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> Update(int id, [FromBody] Coupon coupon)
        {
            if (id != coupon.Id) return BadRequest();
            await _service.UpdateAsync(coupon);
            return NoContent();
        }

        

        
        // ════════════════════════════════════════════════════════════
        // CRUD ADMIN — [DELETE] XÓA
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
