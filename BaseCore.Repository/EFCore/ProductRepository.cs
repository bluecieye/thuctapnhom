

using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;
using BaseCore.Common.Helpers;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public interface IProductRepositoryEF : IRepository<Product>
    {

        

        Task<(List<Product> Items, int TotalCount, decimal PriceMin, decimal PriceMax)> SearchAsync(
            string? keyword,
            int? categoryId,
            List<int>? categoryIds,
            string? gender,
            string? season,
            decimal? minPrice,
            decimal? maxPrice,
            int? sizeId,
            int? colorId,
            bool inStockOnly,
            bool newOnly,
            string? sortBy,
            int page,
            int pageSize);

        Task<Product?> GetByIdWithDetailsAsync(int id);

        Task<List<Product>> GetNewArrivalsAsync(int limit);

        Task<List<Product>> GetBestSellersAsync(int limit);
    }

public static class ProductWindow
    {
        public const int NewArrivalscount = 30;
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public class ProductRepositoryEF : Repository<Product>, IProductRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO & HÀM PHỤ TRỢ
        // ════════════════════════════════════════════════════════════
        public ProductRepositoryEF(MySqlDbContext context) : base(context) { }


        // ════════════════════════════════════════════════════════════
        // TÌM KIẾM
        // ════════════════════════════════════════════════════════════

        public async Task<(List<Product> Items, int TotalCount, decimal PriceMin, decimal PriceMax)> SearchAsync(
            string? keyword, int? categoryId, List<int>? categoryIds,
            string? gender, string? season,
            decimal? minPrice, decimal? maxPrice,
            int? sizeId, int? colorId,
            bool inStockOnly,
            bool newOnly,
            string? sortBy,
            int page, int pageSize)
        {

            
            var query = _dbSet
                .Include(p => p.Category)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(p =>
                    EF.Functions.Collate(p.Name, SearchHelper.Collation).Contains(kw) ||
                    EF.Functions.Collate(p.Description, SearchHelper.Collation).Contains(kw));
            }

            if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId);

            
            if (categoryIds != null && categoryIds.Count > 0)
                query = query.Where(p => categoryIds.Contains(p.CategoryId));

            
            
            if (!string.IsNullOrWhiteSpace(gender)) query = query.Where(p => p.Category!.Gender == gender);
            if (!string.IsNullOrWhiteSpace(season)) query = query.Where(p => p.Category!.Season == season);

            
            
            if (sizeId.HasValue)     query = query.Where(p => p.Variants.Any(v => v.SizeId == sizeId));
            if (colorId.HasValue)    query = query.Where(p => p.Variants.Any(v => v.ColorId == colorId));

            
            
            if (inStockOnly)         query = query.Where(p => p.Variants.Any(v => v.Stock - v.ReservedStock > 0));

            var priceMin = await query.MinAsync(p => (decimal?)(p.BasePrice * (1 - p.DiscountPercent / 100.0m))) ?? 0m;
            var priceMax = await query.MaxAsync(p => (decimal?)(p.BasePrice * (1 - p.DiscountPercent / 100.0m))) ?? 0m;

            if (minPrice.HasValue)
                query = query.Where(p =>
                    p.Variants.Any(v => v.Price * (1 - p.DiscountPercent / 100.0m) >= minPrice));
            if (maxPrice.HasValue)
                query = query.Where(p =>
                    p.Variants.Any(v => v.Price * (1 - p.DiscountPercent / 100.0m) <= maxPrice));

            var total = await query.CountAsync();

            query = sortBy switch
            {
                "priceAsc"  => query.OrderBy(p => p.Variants.Min(v => (decimal?)v.Price) ?? p.BasePrice),
                "priceDesc" => query.OrderByDescending(p => p.Variants.Max(v => (decimal?)v.Price) ?? p.BasePrice),

                "idDesc"    => query.OrderByDescending(p => p.Id),
                _           => query.OrderByDescending(p => p.CreatedAt), 
            };

            var items = await query
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();
            return (items, total, priceMin, priceMax);
        }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        public Task<Product?> GetByIdWithDetailsAsync(int id)
            => _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images).ThenInclude(i => i.Color)
                .Include(p => p.Variants).ThenInclude(v => v.Color)
                .Include(p => p.Variants).ThenInclude(v => v.Size)
                .FirstOrDefaultAsync(p => p.Id == id);



        public Task<List<Product>> GetNewArrivalsAsync(int limit)
        {
            return _dbSet
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .OrderByDescending(p => p.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
        
        // ════════════════════════════════════════════════════════════
        // SẢN PHẨM BÁN CHẠY
        // ════════════════════════════════════════════════════════════

        public async Task<List<Product>> GetBestSellersAsync(int limit)
        {
            
            var top = await _context.OrderDetails
                .GroupBy(d => d.Variant!.ProductId)                              
                .Select(g => new { ProductId = g.Key, Sold = g.Sum(x => x.Quantity) }) 
                .OrderByDescending(x => x.Sold)                                   
                .Take(limit)                                                      
                .ToListAsync();

            var ids = top.Select(t => t.ProductId).ToList();

            
            var products = await _dbSet
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            
            return products
                .OrderBy(p => ids.IndexOf(p.Id))
                .ToList();
        }
    }
}
