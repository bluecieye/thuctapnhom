

namespace BaseCore.Entities
{
    // ════════════════════════════════════════════════════════════
    // ENTITY ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════
    public class Order
    {

        public int Id { get; set; }

        // Mã đơn hàng dạng chuỗi thân thiện (vd "MOON-A1B2C3"), DUY NHẤT.
        // Dùng để khách (kể cả khách vãng lai) tra cứu đơn mà không cần biết Id nội bộ.
        public string OrderCode { get; set; } = string.Empty;


        public string Status { get; set; } = "Pending";

        public decimal TotalAmount { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal DiscountAmount { get; set; } = 0;

        public string PaymentMethod { get; set; } = "COD"; 

        
        public string ShippingAddress { get; set; } = string.Empty;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? EstimatedDelivery { get; set; }

        public string? TrackingNumber { get; set; }

        // Nullable: đơn của KHÁCH VÃNG LAI (guest checkout) không có tài khoản
        // nên UserId = null. Đơn của thành viên thì gắn UserId.
        public int? UserId { get; set; }

        public User? User { get; set; }

        // ── Thông tin liên hệ người nhận (snapshot tại thời điểm đặt) ──
        // Email luôn được lưu để gửi xác nhận đơn (cả thành viên lẫn khách).
        public string? CustomerEmail { get; set; }

        // Họ tên & SĐT người nhận — với khách vãng lai đây là nguồn duy nhất
        // (không có Address trong sổ địa chỉ). Cũng dùng để xác minh khi tra cứu đơn.
        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public int? CouponId { get; set; }
        
        public Coupon? Coupon { get; set; }

        public int? ShippingCarrierId { get; set; }
        
        public ShippingCarrier? ShippingCarrier { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
