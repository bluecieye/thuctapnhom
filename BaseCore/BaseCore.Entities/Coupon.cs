

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY MÃ GIẢM GIÁ
    // ════════════════════════════════════════════════════════════
    public class Coupon
    {
        
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string DiscountType { get; set; } = "Percent";

        public decimal DiscountValue { get; set; }

        public decimal MinOrderAmount { get; set; }

        
        public decimal? MaxDiscount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int UsageLimit { get; set; } = 0;

        public int UsedCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
