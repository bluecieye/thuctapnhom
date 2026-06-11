

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════
    public class Order
    {
        
        public int Id { get; set; }

        
        public string Status { get; set; } = "Pending"; 

        public decimal TotalAmount { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal DiscountAmount { get; set; } = 0;

        public string PaymentMethod { get; set; } = "COD"; 

        
        public string ShippingAddress { get; set; } = string.Empty;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? EstimatedDelivery { get; set; }

        public int UserId { get; set; }
        
        public User? User { get; set; }

        public int? CouponId { get; set; }
        
        public Coupon? Coupon { get; set; }

        public int? ShippingCarrierId { get; set; }
        
        public ShippingCarrier? ShippingCarrier { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
