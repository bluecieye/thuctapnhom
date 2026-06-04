

using System.Threading.Tasks;
using BaseCore.Entities;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // INTERFACE SERVICE GIỎ HÀNG
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // CART SERVICE — INTERFACE
    // ════════════════════════════════════════════════════════════
    public interface ICartService
    {

        

        
        Task<Cart> GetCartAsync(int userId);

        

        

        
        
        Task<Cart> AddItemAsync(int userId, int variantId, int quantity);

        

        
        Task<Cart> UpdateItemAsync(int userId, int cartItemId, int quantity);

        
        
        Task<Cart> RemoveItemAsync(int userId, int cartItemId);

        
        
        Task ClearAsync(int userId);
    }
}
