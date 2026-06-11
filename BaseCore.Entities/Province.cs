

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY TỈNH/THÀNH
    // ════════════════════════════════════════════════════════════
    public class Province
    {
        
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty; 

        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        public ICollection<ShippingRate> ShippingRates { get; set; } = new List<ShippingRate>();
    }
}
