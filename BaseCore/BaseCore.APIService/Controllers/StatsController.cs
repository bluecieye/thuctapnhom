

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseCore.Repository;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER THỐNG KÊ
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/stats")]

    [Authorize(Roles = "Admin,WarehouseStaff,Marketing")]
    public class StatsController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _db;
        public StatsController(MySqlDbContext db) { _db = db; }

        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════


        
        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            
            var products   = await _db.Products.CountAsync();
            var variants   = await _db.ProductVariants.CountAsync();
            var categories = await _db.Categories.CountAsync();
            var orders     = await _db.Orders.CountAsync();
            // Đếm TẤT CẢ user (Admin/Staff/Marketing/Customer) — khớp label "Người dùng" trên card.
            var users      = await _db.Users.CountAsync();
            var promotions = await _db.Coupons.CountAsync();
            var reviews    = await _db.Reviews.CountAsync();
            var images     = await _db.ProductImages.CountAsync();
            var sizeguides = await _db.SizeGuides.CountAsync();

            var lowStockVariants = await _db.ProductVariants
                .CountAsync(v => v.Stock - v.ReservedStock <= 5 && v.Stock - v.ReservedStock >= 0);

            var ordersByStatusRaw = await _db.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToListAsync();

            var ordersByStatus = ordersByStatusRaw.ToDictionary(x => x.status ?? "Unknown", x => x.count);

            return Ok(new
            {
                products,
                variants,
                categories,
                orders,
                users,
                promotions,
                reviews,
                images,
                sizeguides,
                lowStockVariants,
                ordersByStatus,
            });
        }
    }
}
