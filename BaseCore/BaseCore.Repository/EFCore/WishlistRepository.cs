

using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY WISHLIST
    // ════════════════════════════════════════════════════════════
    public interface IWishlistRepositoryEF : IRepository<Wishlist>
    {

        Task<List<Wishlist>> GetByUserAsync(int userId);

        Task<bool> ExistsAsync(int userId, int productId);

        Task RemoveAsync(int userId, int productId);
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY WISHLIST
    // ════════════════════════════════════════════════════════════
    public class WishlistRepositoryEF : Repository<Wishlist>, IWishlistRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public WishlistRepositoryEF(MySqlDbContext context) : base(context) { }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        public Task<List<Wishlist>> GetByUserAsync(int userId)
            => _dbSet
                .AsNoTracking()
                .Include(w => w.Product).ThenInclude(p => p.Category)
                .Include(w => w.Product).ThenInclude(p => p.Images)
                .Include(w => w.Product).ThenInclude(p => p.Variants)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

        

        
        public Task<bool> ExistsAsync(int userId, int productId)
            => _dbSet.AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC THAY ĐỔI
        // ════════════════════════════════════════════════════════════

        public async Task RemoveAsync(int userId, int productId)
        {
            var item = await _dbSet.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            if (item != null)
            {
                _dbSet.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
