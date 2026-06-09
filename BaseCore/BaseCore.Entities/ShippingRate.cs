

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY CƯỚC VẬN CHUYỂN
    // ════════════════════════════════════════════════════════════
    public class ShippingRate
    {
        
        public int Id { get; set; }

        public int CarrierId { get; set; }
        
        public ShippingCarrier? Carrier { get; set; }

        public int ProvinceId { get; set; }
        
        public Province? Province { get; set; }

        
        public decimal Fee { get; set; }

        public int EstimatedDays { get; set; } = 3;
    }
}
