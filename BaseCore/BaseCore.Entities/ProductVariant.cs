                                

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY BIẾN THỂ SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public class ProductVariant
    {
        
        public int Id { get; set; }

        public string SKU { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        
        
        public int ReservedStock { get; set; }

        public int ProductId { get; set; }
        
        public Product? Product { get; set; }

        public int ColorId { get; set; }
        
        public Color? Color { get; set; }

        public int SizeId { get; set; }
        
        public Size? Size { get; set; }

        
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
