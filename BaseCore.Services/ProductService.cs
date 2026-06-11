

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaseCore.Common.Helpers;
using BaseCore.Entities;
using BaseCore.Repository;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // SERVICE SẢN PHẨM
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // PRODUCT SERVICE — IMPLEMENTATION
    // ════════════════════════════════════════════════════════════
    public class ProductService : IProductService
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN THÀNH VIÊN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // FIELDS
        // ════════════════════════════════════════════════════════════
        private readonly IProductRepositoryEF _repo;

        private readonly MySqlDbContext _ctx;

        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ════════════════════════════════════════════════════════════
        public ProductService(IProductRepositoryEF repo, MySqlDbContext ctx)
        {
            _repo = repo;
            _ctx = ctx;
        }

        

        
        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // QUERY METHODS
        // ════════════════════════════════════════════════════════════
        public Task<(List<Product> Items, int TotalCount, decimal PriceMin, decimal PriceMax)> SearchAsync(
            string? keyword, int? categoryId, List<int>? categoryIds,
            string? gender, string? season,
            decimal? minPrice, decimal? maxPrice,
            int? sizeId, int? colorId, bool inStockOnly,
            bool newOnly,
            string? sortBy,
            int page, int pageSize)
            => _repo.SearchAsync(keyword, categoryId, categoryIds, gender, season,
                minPrice, maxPrice, sizeId, colorId, inStockOnly, newOnly, sortBy, page, pageSize);

        public Task<Product?> GetByIdAsync(int id) => _repo.GetByIdWithDetailsAsync(id);

        public Task<List<Product>> GetNewArrivalsAsync(int limit) => _repo.GetNewArrivalsAsync(limit);

        public Task<List<Product>> GetBestSellersAsync(int limit) => _repo.GetBestSellersAsync(limit);

        

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // TẠO SẢN PHẨM
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CREATE PRODUCT
        // ════════════════════════════════════════════════════════════
        public async Task<Product> CreateAsync(Product product)
        {
            product.CreatedAt = System.DateTime.UtcNow;
            product.Slug = await EnsureUniqueSlugAsync(
                string.IsNullOrWhiteSpace(product.Slug) ? SlugHelper.Generate(product.Name) : SlugHelper.Generate(product.Slug),
                excludeId: null);
            return await _repo.AddAsync(product);
        }

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // CẬP NHẬT / XOÁ SẢN PHẨM
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // UPDATE / DELETE PRODUCT
        // ════════════════════════════════════════════════════════════
        public async Task UpdateAsync(Product product)
        {
            if (!string.IsNullOrWhiteSpace(product.Slug))
                product.Slug = await EnsureUniqueSlugAsync(SlugHelper.Generate(product.Slug), excludeId: product.Id);
            await _repo.UpdateAsync(product);
        }

        public Task DeleteAsync(int id) => _repo.DeleteByIdAsync(id);

        

        

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // HÀM PHỤ TRỢ NỘI BỘ
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // SLUG HELPERS
        // ════════════════════════════════════════════════════════════
        private async Task<string> EnsureUniqueSlugAsync(string baseSlug, int? excludeId)
        {
            if (string.IsNullOrWhiteSpace(baseSlug)) baseSlug = "san-pham";
            var slug = baseSlug;
            var i = 2;
            
            while (await _ctx.Products.AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId)))
            {
                slug = $"{baseSlug}-{i++}";  
            }
            return slug;
        }
    }
}
