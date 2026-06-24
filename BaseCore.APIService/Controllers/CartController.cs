

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using BaseCore.Services;

namespace BaseCore.APIService.Controllers
{

    // ════════════════════════════════════════════════════════════
    // DTO YÊU CẦU GIỎ HÀNG
    // ════════════════════════════════════════════════════════════
    public class AddCartItemDto
    {
        
        public int VariantId { get; set; }
        
        public int Quantity { get; set; } = 1;
    }

    

    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // CONTROLLER GIỎ HÀNG
    // ════════════════════════════════════════════════════════════
    [ApiController]

    [Route("api/cart")]

    [Authorize]
    public class CartController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly ICartService _service;

        public CartController(ICartService service) { _service = service; }

        private int CurrentUserId
            => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;



        // ════════════════════════════════════════════════════════════
        // [GET] DANH SÁCH & CHI TIẾT
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _service.GetCartAsync(CurrentUserId));



        // ════════════════════════════════════════════════════════════
        // [POST] TẠO MỚI
        // ════════════════════════════════════════════════════════════
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
        {
            try
            {
                var cart = await _service.AddItemAsync(CurrentUserId, dto.VariantId, dto.Quantity);
                return Ok(cart);
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        

        

        

        
        // ════════════════════════════════════════════════════════════
        // [PUT] CẬP NHẬT
        // ════════════════════════════════════════════════════════════
        [HttpPut("items/{itemId:int}")]
        public async Task<IActionResult> UpdateItem(int itemId, [FromBody] UpdateCartItemDto dto)
        {
            try
            {
                var cart = await _service.UpdateItemAsync(CurrentUserId, itemId, dto.Quantity);
                return Ok(cart);
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        

        
        
        // ════════════════════════════════════════════════════════════
        // [DELETE] XÓA
        // ════════════════════════════════════════════════════════════
        [HttpDelete("items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(int itemId)
            => Ok(await _service.RemoveItemAsync(CurrentUserId, itemId));

        

        
        
        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            await _service.ClearAsync(CurrentUserId);
            return NoContent();
        }
    }
}
