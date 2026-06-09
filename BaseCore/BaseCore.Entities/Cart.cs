

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY GIỎ HÀNG
    // ════════════════════════════════════════════════════════════
    public class Cart
    {
        
        public int Id { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }

        public User? User { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
