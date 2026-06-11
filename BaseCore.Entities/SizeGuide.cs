

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY BẢNG SIZE
    // ════════════════════════════════════════════════════════════
    public class SizeGuide
    {
        
        public int Id { get; set; }

        public decimal Chest { get; set; }

        public decimal Waist { get; set; }

        public decimal Length { get; set; }

        public decimal Shoulder { get; set; }

        public int SizeId { get; set; }
        
        public Size? Size { get; set; }
    }
}
