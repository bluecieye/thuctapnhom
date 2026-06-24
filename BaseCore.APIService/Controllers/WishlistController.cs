

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER DANH SÁCH YÊU THÍCH
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/wishlist")]

    [Authorize]
    public class WishlistController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly IWishlistRepositoryEF _repo;
        public WishlistController(IWishlistRepositoryEF repo) { _repo = repo; }

        private int CurrentUserId
            => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetMine() => Ok(await _repo.GetByUserAsync(CurrentUserId));

        // ════════════════════════════════════════════════════════════
        // TẠO MỚI (POST)
        // ════════════════════════════════════════════════════════════
        [HttpPost("{productId:int}")]
        public async Task<IActionResult> Add(int productId)
        {
            
            if (await _repo.ExistsAsync(CurrentUserId, productId))
                return Ok(new { message = "Đã có trong wishlist." });
            
            await _repo.AddAsync(new Wishlist { UserId = CurrentUserId, ProductId = productId });
            return Ok(new { message = "Đã thêm vào wishlist." });
        }

        

        // ════════════════════════════════════════════════════════════
        // XÓA (DELETE)
        // ════════════════════════════════════════════════════════════
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Remove(int productId)
        {
            await _repo.RemoveAsync(CurrentUserId, productId);
            return NoContent();
        }
    }
}
