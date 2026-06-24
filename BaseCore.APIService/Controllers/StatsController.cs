

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseCore.Common;
using BaseCore.Repository;

namespace BaseCore.APIService.Controllers
{
    [ApiController]
    [Route("api/stats")]
    [Authorize(Roles = "Admin,WarehouseStaff,Marketing")]
    public class StatsController : ControllerBase
    {
        private readonly MySqlDbContext _db;
        public StatsController(MySqlDbContext db) { _db = db; }

        // ════════════════════════════════════════════════════════════
        // TỔNG QUAN KINH DOANH
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var products   = await _db.Products.CountAsync();
            var variants   = await _db.ProductVariants.CountAsync();
            var categories = await _db.Categories.CountAsync();
            var orders     = await _db.Orders.CountAsync();
            var users      = await _db.Users.CountAsync();
            var promotions = await _db.Coupons.CountAsync();

            // Doanh thu từ đơn không bị huỷ
            var totalRevenue = await _db.Orders
                .Where(o => o.Status != "Cancelled")
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var completedCount = await _db.Orders.CountAsync(o => o.Status != "Cancelled");
            var avgOrderValue  = completedCount > 0 ? totalRevenue / completedCount : 0m;

            var lowStockVariants = await _db.ProductVariants
                .CountAsync(v => v.Stock - v.ReservedStock <= Constants.LOW_STOCK_THRESHOLD
                              && v.Stock - v.ReservedStock >= 0);

            var ordersByStatusRaw = await _db.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToListAsync();

            var ordersByStatus = ordersByStatusRaw
                .ToDictionary(x => x.status ?? "Unknown", x => x.count);

            return Ok(new
            {
                products,
                variants,
                categories,
                orders,
                users,
                promotions,
                totalRevenue,
                avgOrderValue,
                lowStockVariants,
                ordersByStatus,
            });
        }

        // ════════════════════════════════════════════════════════════
        // BIẾN THỂ BÁN CHẠY NHẤT (THEO MÀUSẮC/SIZE)
        // ════════════════════════════════════════════════════════════
        [HttpGet("variant-top-sales")]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> GetVariantTopSales([FromQuery] int limit = 10)
        {
            var topSales = await _db.OrderDetails
                .Where(d => d.Order != null && d.Order.Status != "Cancelled")
                .GroupBy(d => d.VariantId)
                .Select(g => new
                {
                    VariantId    = g.Key,
                    TotalSold    = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => (decimal?)x.Quantity * x.UnitPrice) ?? 0m,
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(Math.Clamp(limit, 1, 50))
                .ToListAsync();

            var variantIds = topSales.Select(x => x.VariantId).ToList();
            var variants   = await _db.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.Color)
                .Include(v => v.Size)
                .Where(v => variantIds.Contains(v.Id))
                .ToDictionaryAsync(v => v.Id);

            return Ok(topSales.Select(x =>
            {
                variants.TryGetValue(x.VariantId, out var v);
                return new
                {
                    variantId   = x.VariantId,
                    sku         = v?.SKU         ?? "-",
                    productName = v?.Product?.Name ?? "-",
                    colorName   = v?.Color?.Name   ?? "-",
                    sizeName    = v?.Size?.Name    ?? "-",
                    totalSold   = x.TotalSold,
                    revenue     = x.TotalRevenue,
                };
            }));
        }
    }
}
