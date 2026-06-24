

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY MỤC GIỎ HÀNG
    // ════════════════════════════════════════════════════════════
    public class CartItem
    {
        
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int CartId { get; set; }

        public Cart? Cart { get; set; }

        public int VariantId { get; set; }

        public ProductVariant? Variant { get; set; }
    }
}
