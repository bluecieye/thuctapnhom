

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using BaseCore.Repository.EFCore;

using BaseCore.Services;

namespace BaseCore.APIService.Controllers
{

    // ════════════════════════════════════════════════════════════
    // DTO YÊU CẦU ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════
    public class UpdateStatusDto
    {
        
        public string Status { get; set; } = "";
        
        public string? Note { get; set; }
    }

    

    public class ApplyCouponDto
    {
        public string Code { get; set; } = "";
        
        public decimal Subtotal { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // CONTROLLER ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/orders")]

    [Authorize]
    public class OrdersController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly IOrderService _orderService;

        private readonly IOrderRepositoryEF _orderRepo;

        private readonly ICouponService _couponService;

        public OrdersController(IOrderService orderService, IOrderRepositoryEF orderRepo, ICouponService couponService)
        {
            _orderService = orderService;
            _orderRepo = orderRepo;
            _couponService = couponService;
        }

        private int CurrentUserId
            => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;



        // ════════════════════════════════════════════════════════════
        // ĐẶT HÀNG
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        public async Task<IActionResult> Place([FromBody] CreateOrderRequest request)
        {
            
            request.UserId = CurrentUserId;
            try
            {
                var order = await _orderService.PlaceOrderAsync(request);
                
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        // ════════════════════════════════════════════════════════════
        // LỊCH SỬ ĐƠN HÀNG
        // ════════════════════════════════════════════════════════════
        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
            => Ok(await _orderService.GetByUserAsync(CurrentUserId));

        

        

        

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (order.UserId != CurrentUserId
                && !User.IsInRole("Admin")
                && !User.IsInRole("WarehouseStaff"))
                return Forbid();
            return Ok(order);
        }



        // ════════════════════════════════════════════════════════════
        // HỦY ĐƠN
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            if (order.UserId != CurrentUserId && !User.IsInRole("Admin"))
                return Forbid();
            try
            {
                await _orderService.CancelAsync(id);
                return Ok(new { message = "Đã huỷ đơn hàng." });
            }
            catch (System.InvalidOperationException ex)
            {
                
                return BadRequest(new { message = ex.Message });
            }
        }

        

        

        

        

        // ════════════════════════════════════════════════════════════
        // CHUYỂN TRẠNG THÁI
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin,WarehouseStaff")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                var order = await _orderService.ChangeStatusAsync(id, dto.Status, dto.Note);
                return order == null ? NotFound() : Ok(order);
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // [GET] DANH SÁCH ADMIN
        // ════════════════════════════════════════════════════════════
        [HttpGet("admin")]
        [Authorize(Roles = "Admin,WarehouseStaff")]
        public async Task<IActionResult> SearchAdmin(
            [FromQuery] string? keyword,
            [FromQuery] string? status,
            [FromQuery] decimal? minAmount,
            [FromQuery] decimal? maxAmount,
            [FromQuery] System.DateTime? dateFrom,
            [FromQuery] System.DateTime? dateTo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            
            var (orders, total) = await _orderRepo.SearchAdminAsync(
                keyword, status, minAmount, maxAmount, dateFrom, dateTo, page, pageSize);
            return Ok(new
            {
                items = orders, totalCount = total, page, pageSize,
                
                totalPages = (int)System.Math.Ceiling((double)total / pageSize)
            });
        }

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // ÁP DỤNG MÃ GIẢM GIÁ
        // ════════════════════════════════════════════════════════════
        [HttpPost("apply-coupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto dto)
        {
            var result = await _couponService.ApplyAsync(dto.Code, dto.Subtotal);
            
            return Ok(new
            {
                isValid = result.IsValid,
                discountAmount = result.DiscountAmount,
                message = result.Message,
                coupon = result.Coupon
            });
        }
    }
}
