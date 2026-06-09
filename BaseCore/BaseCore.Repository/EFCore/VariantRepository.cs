

using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;
using BaseCore.Common.Helpers;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY BIẾN THỂ SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public interface IProductVariantRepositoryEF : IRepository<ProductVariant>
    {

        Task<ProductVariant?> GetBySkuAsync(string sku);

        Task<List<ProductVariant>> GetByProductIdAsync(int productId);

        Task<List<ProductVariant>> GetAllWithDetailsAsync(string? keyword, int page, int pageSize);

        Task<int> CountAllAsync(string? keyword);

        Task<List<ProductVariant>> GetLowStockAsync(int threshold);

        Task<bool> ReserveAsync(int variantId, int quantity);
        Task<bool> ReleaseAsync(int variantId, int quantity);
        Task<bool> CommitAsync(int variantId, int quantity);
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY BIẾN THỂ SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public class ProductVariantRepositoryEF : Repository<ProductVariant>, IProductVariantRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public ProductVariantRepositoryEF(MySqlDbContext context) : base(context) { }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        public Task<ProductVariant?> GetBySkuAsync(string sku)
            => _dbSet.Include(v => v.Product).FirstOrDefaultAsync(v => v.SKU == sku);

        

        
        public async Task<List<ProductVariant>> GetByProductIdAsync(int productId)
        {
            var list = await _dbSet
                .AsNoTracking()
                .Include(v => v.Color)
                .Include(v => v.Size)
                .Where(v => v.ProductId == productId)
                .OrderBy(v => v.Color!.Name).ThenBy(v => v.Size!.Name)
                .ToListAsync();
            BreakNavCycles(list);
            return list;
        }

        

        
        
        public async Task<List<ProductVariant>> GetAllWithDetailsAsync(string? keyword, int page, int pageSize)
        {
            var q = _dbSet
                .AsNoTracking()
                .Include(v => v.Product)
                .Include(v => v.Color)
                .Include(v => v.Size)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                q = q.Where(v =>
                    EF.Functions.Collate(v.SKU, SearchHelper.Collation).Contains(kw) ||
                    EF.Functions.Collate(v.Product.Name, SearchHelper.Collation).Contains(kw));
            }

            var list = await q.OrderByDescending(v => v.Id)
                              .Skip((page - 1) * pageSize).Take(pageSize)
                              .ToListAsync();

            BreakNavCycles(list);
            return list;
        }

        // ════════════════════════════════════════════════════════════
        // HÀM PHỤ TRỢ
        // ════════════════════════════════════════════════════════════
        // Cắt back-reference do EF fixup sinh ra (Color.Variants, Size.Variants, Product.Variants/Images)
        // để JSON serializer khỏi đệ quy vượt MaxDepth=32. Phải chạy SAU ToListAsync vì AsNoTracking
        // vẫn để EF làm relationship fixup trong cùng query.
        private static void BreakNavCycles(List<ProductVariant> list)
        {
            foreach (var v in list)
            {
                if (v.Color != null) v.Color.Variants = new List<ProductVariant>();
                if (v.Size != null) { v.Size.Variants = new List<ProductVariant>(); v.Size.SizeGuides = new List<SizeGuide>(); }
                if (v.Product != null)
                {
                    v.Product.Variants = new List<ProductVariant>();
                    v.Product.Images = new List<ProductImage>();
                }
            }
        }

        // ════════════════════════════════════════════════════════════
        // TỔNG HỢP
        // ════════════════════════════════════════════════════════════

        public Task<int> CountAllAsync(string? keyword)
        {
            var q = _dbSet.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {

                var kw = keyword.Trim();
                q = q.Where(v =>
                    EF.Functions.Collate(v.SKU, SearchHelper.Collation).Contains(kw) ||
                    EF.Functions.Collate(v.Product.Name, SearchHelper.Collation).Contains(kw));
            }
            return q.CountAsync();
        }

        // ════════════════════════════════════════════════════════════
        // TRUY VẤN TỒN KHO
        // ════════════════════════════════════════════════════════════

        public async Task<List<ProductVariant>> GetLowStockAsync(int threshold)
        {
            var list = await _dbSet
                .AsNoTracking()
                .Include(v => v.Product)
                .Include(v => v.Color)
                .Include(v => v.Size)
                .Where(v => v.Stock - v.ReservedStock <= threshold)
                .OrderBy(v => v.Stock - v.ReservedStock)
                .ToListAsync();
            BreakNavCycles(list);
            return list;
        }

        // ════════════════════════════════════════════════════════════
        // THAY ĐỔI TỒN KHO (RESERVE / RELEASE / COMMIT)
        // ════════════════════════════════════════════════════════════

        public async Task<bool> ReserveAsync(int variantId, int quantity)
        {
            var v = await _dbSet.FindAsync(variantId);
            if (v == null) return false;
            if (v.Stock - v.ReservedStock < quantity) return false;
            v.ReservedStock += quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        

        
        public async Task<bool> ReleaseAsync(int variantId, int quantity)
        {
            var v = await _dbSet.FindAsync(variantId);
            if (v == null) return false;
            v.ReservedStock = Math.Max(0, v.ReservedStock - quantity);
            await _context.SaveChangesAsync();
            return true;
        }

        

        

        
        public async Task<bool> CommitAsync(int variantId, int quantity)
        {
            var v = await _dbSet.FindAsync(variantId);
            if (v == null) return false;
            v.Stock = Math.Max(0, v.Stock - quantity);
            v.ReservedStock = Math.Max(0, v.ReservedStock - quantity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
