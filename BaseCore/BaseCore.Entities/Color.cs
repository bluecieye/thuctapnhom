

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY MÀU SẮC
    // ════════════════════════════════════════════════════════════
    public class Color
    {
        
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string HexCode { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
