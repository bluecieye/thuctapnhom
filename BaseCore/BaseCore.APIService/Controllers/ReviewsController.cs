

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using BaseCore.Services;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER ĐÁNH GIÁ
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly IReviewService _service;
        public ReviewsController(IReviewService service) { _service = service; }



        private int CurrentUserId
            => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;



        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet("admin")]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> GetAllAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            
            var items = await _service.GetAllAdminAsync(page, pageSize);
            
            var total = await _service.CountAllAsync();

            return Ok(new { items, total, page, pageSize, totalPages = (int)Math.Ceiling((double)total / pageSize) });
        }

        

        [HttpGet("product/{productId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            
            var (avg, count) = await _service.GetSummaryAsync(productId);
            
            var reviews = await _service.GetByProductAsync(productId);
            
            return Ok(new { average = avg, count, reviews });
        }

        

        // ════════════════════════════════════════════════════════════
        // TẠO MỚI (POST)
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            
            request.UserId = CurrentUserId;
            try
            {
                
                var review = await _service.CreateAsync(request);
                return Ok(review);
            }
            catch (System.InvalidOperationException ex)
            {
                
                return BadRequest(new { message = ex.Message });
            }
        }

        

        // ════════════════════════════════════════════════════════════
        // XÓA (DELETE)
        // ════════════════════════════════════════════════════════════
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            
            return NoContent();
        }
    }
}
