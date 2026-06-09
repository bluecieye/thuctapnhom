

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY HÌNH ẢNH SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public class ProductImage
    {
        
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        
        public bool IsPrimary { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;

        public int ProductId { get; set; }
        
        public Product? Product { get; set; }

        public int ColorId { get; set; }
        
        public Color? Color { get; set; }
    }
}
