

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY ĐƠN VỊ VẬN CHUYỂN
    // ════════════════════════════════════════════════════════════
    public class ShippingCarrier
    {
        
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string LogoFileName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<ShippingRate> Rates { get; set; } = new List<ShippingRate>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
