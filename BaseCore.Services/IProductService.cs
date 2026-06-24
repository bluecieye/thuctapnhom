

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // INTERFACE SERVICE SẢN PHẨM
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // PRODUCT SERVICE — INTERFACE
    // ════════════════════════════════════════════════════════════
    public interface IProductService
    {

        

        

        

        

        

        

        

        
        Task<(List<Product> Items, int TotalCount, decimal PriceMin, decimal PriceMax)> SearchAsync(
            string? keyword, int? categoryId, List<int>? categoryIds,
            string? gender, string? season,
            decimal? minPrice, decimal? maxPrice,
            int? sizeId, int? colorId, bool inStockOnly,
            bool newOnly,
            string? sortBy,
            int page, int pageSize);

        Task<Product?> GetByIdAsync(int id);

        Task<List<Product>> GetNewArrivalsAsync(int limit);

        Task<List<Product>> GetBestSellersAsync(int limit);

        

        
        Task<Product> CreateAsync(Product product);

        
        Task UpdateAsync(Product product);

        Task DeleteAsync(int id);
    }
}
