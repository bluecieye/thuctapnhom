

using Microsoft.EntityFrameworkCore;

using BaseCore.Entities;

namespace BaseCore.Repository.EFCore
{

    // ════════════════════════════════════════════════════════════
    // INTERFACE REPOSITORY GIỎ HÀNG
    // ════════════════════════════════════════════════════════════
    public interface ICartRepositoryEF : IRepository<Cart>
    {

        Task<Cart> GetOrCreateByUserAsync(int userId);

        Task<Cart?> GetWithItemsAsync(int userId);

        Task ClearAsync(int userId);
    }

    // ════════════════════════════════════════════════════════════
    // REPOSITORY GIỎ HÀNG
    // ════════════════════════════════════════════════════════════
    public class CartRepositoryEF : Repository<Cart>, ICartRepositoryEF
    {
        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public CartRepositoryEF(MySqlDbContext context) : base(context) { }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN & THAY ĐỔI
        // ════════════════════════════════════════════════════════════

        public async Task<Cart> GetOrCreateByUserAsync(int userId)
        {
            var cart = await _dbSet.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null) return cart;

            cart = new Cart { UserId = userId, UpdatedAt = DateTime.UtcNow };
            _dbSet.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC TRUY VẤN
        // ════════════════════════════════════════════════════════════

        public Task<Cart?> GetWithItemsAsync(int userId)
            => _dbSet
                .Include(c => c.CartItems).ThenInclude(i => i.Variant).ThenInclude(v => v.Product).ThenInclude(p => p.Images)
                .Include(c => c.CartItems).ThenInclude(i => i.Variant).ThenInclude(v => v.Color)
                .Include(c => c.CartItems).ThenInclude(i => i.Variant).ThenInclude(v => v.Size)
                .FirstOrDefaultAsync(c => c.UserId == userId);

        // ════════════════════════════════════════════════════════════
        // PHƯƠNG THỨC THAY ĐỔI
        // ════════════════════════════════════════════════════════════

        public async Task ClearAsync(int userId)
        {
            var cart = await _dbSet.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return;
            _context.CartItems.RemoveRange(cart.CartItems);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
