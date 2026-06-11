

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY KÍCH CỠ
    // ════════════════════════════════════════════════════════════
    public class Size
    {
        
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        
        public ICollection<SizeGuide> SizeGuides { get; set; } = new List<SizeGuide>();
    }
}
