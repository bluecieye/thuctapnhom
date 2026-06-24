

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public class Product
    {
        
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        
        public string Slug { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        
        public decimal BasePrice { get; set; }

        public int DiscountPercent { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CategoryId { get; set; }
        
        public Category? Category { get; set; }

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
