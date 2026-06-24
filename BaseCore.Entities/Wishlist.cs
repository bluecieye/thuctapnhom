

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY WISHLIST
    // ════════════════════════════════════════════════════════════
    public class Wishlist
    {
        
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        
        public User? User { get; set; }

        public int ProductId { get; set; }
        
        public Product? Product { get; set; }

    }
}
