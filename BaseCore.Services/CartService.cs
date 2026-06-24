

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;
using BaseCore.Repository;
using BaseCore.Repository.EFCore;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // SERVICE GIỎ HÀNG
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // CART SERVICE — IMPLEMENTATION
    // ════════════════════════════════════════════════════════════
    public class CartService : ICartService
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN THÀNH VIÊN
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // FIELDS
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _context;

        private readonly ICartRepositoryEF _repo;

        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CONSTRUCTOR
        // ════════════════════════════════════════════════════════════
        public CartService(MySqlDbContext context, ICartRepositoryEF repo)
        {
            _context = context;
            _repo = repo;
        }

        // ════════════════════════════════════════════════════════════
        // LẤY GIỎ HÀNG
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // GET CART
        // ════════════════════════════════════════════════════════════
        public async Task<Cart> GetCartAsync(int userId)
        {
            await _repo.GetOrCreateByUserAsync(userId);
            return (await _repo.GetWithItemsAsync(userId))!;
        }

        // ════════════════════════════════════════════════════════════
        // THÊM SẢN PHẨM VÀO GIỎ
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // ADD ITEM
        // ════════════════════════════════════════════════════════════
        public async Task<Cart> AddItemAsync(int userId, int variantId, int quantity)
        {
            
            if (quantity <= 0) throw new InvalidOperationException("Số lượng phải lớn hơn 0.");

            var variant = await _context.ProductVariants.FindAsync(variantId)
                ?? throw new InvalidOperationException("Biến thể sản phẩm không tồn tại.");

            
            if (variant.Stock - variant.ReservedStock < quantity)
                throw new InvalidOperationException("Không đủ tồn kho.");

            var cart = await _repo.GetOrCreateByUserAsync(userId);

            var existing = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cart.Id && i.VariantId == variantId);

            if (existing != null)
            {
                
                existing.Quantity += quantity;
            }
            else
            {
                
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
                    VariantId = variantId,
                    Quantity = quantity
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (await _repo.GetWithItemsAsync(userId))!;
        }

        // ════════════════════════════════════════════════════════════
        // CẬP NHẬT SỐ LƯỢNG
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // UPDATE ITEM
        // ════════════════════════════════════════════════════════════
        public async Task<Cart> UpdateItemAsync(int userId, int cartItemId, int quantity)
        {
            if (quantity <= 0) throw new InvalidOperationException("Số lượng phải lớn hơn 0.");

            var cart = await _repo.GetOrCreateByUserAsync(userId);

            
            var item = await _context.CartItems
                .Include(i => i.Variant)
                .FirstOrDefaultAsync(i => i.Id == cartItemId && i.CartId == cart.Id)
                ?? throw new InvalidOperationException("Mục giỏ hàng không tồn tại.");

            if (item.Variant.Stock - item.Variant.ReservedStock < quantity)
                throw new InvalidOperationException("Không đủ tồn kho.");

            item.Quantity = quantity;
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (await _repo.GetWithItemsAsync(userId))!;
        }

        // ════════════════════════════════════════════════════════════
        // XOÁ SẢN PHẨM KHỎI GIỎ
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // REMOVE ITEM
        // ════════════════════════════════════════════════════════════
        public async Task<Cart> RemoveItemAsync(int userId, int cartItemId)
        {
            var cart = await _repo.GetOrCreateByUserAsync(userId);
            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.Id == cartItemId && i.CartId == cart.Id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return (await _repo.GetWithItemsAsync(userId))!;
        }

        // ════════════════════════════════════════════════════════════
        // XOÁ TOÀN BỘ GIỎ HÀNG
        // ════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════
        // CLEAR CART
        // ════════════════════════════════════════════════════════════
        public Task ClearAsync(int userId) => _repo.ClearAsync(userId);
    }
}
