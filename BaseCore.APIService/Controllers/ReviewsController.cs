

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using System.IO;
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
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public ReviewsController(IReviewService service, IConfiguration config, IWebHostEnvironment env)
        {
            _service = service;
            _config = config;
            _env = env;
        }

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".avif", ".gif" };



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
        // UPLOAD ẢNH ĐÁNH GIÁ (POST) — trả về URL ảnh đã lưu
        // ════════════════════════════════════════════════════════════
        [HttpPost("upload")]
        [Authorize]
        [RequestSizeLimit(8 * 1024 * 1024)] // tối đa 8MB
        public async Task<IActionResult> Upload([FromForm] IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Chưa chọn ảnh." });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (Array.IndexOf(AllowedExtensions, ext) < 0)
                return BadRequest(new { message = "Định dạng ảnh không hỗ trợ." });

            var folder = Path.Combine(GetMediaRoot(), "reviews");
            Directory.CreateDirectory(folder);

            // Tên file ngẫu nhiên để tránh trùng & lộ tên gốc.
            var fileName = $"rv-{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // URL public phục vụ qua static files: /images/reviews/<file>
            return Ok(new { url = $"/images/reviews/{fileName}" });
        }

        private string GetMediaRoot()
        {
            var root = _config["Media:Root"] ?? "..\\Media";
            if (!Path.IsPathRooted(root))
                root = Path.GetFullPath(Path.Combine(_env.ContentRootPath, root));
            return root;
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
