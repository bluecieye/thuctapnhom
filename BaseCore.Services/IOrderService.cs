

using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCore.Entities;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // DTO YÊU CẦU ĐẶT HÀNG
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // CREATE ORDER REQUEST — DTO
    // ════════════════════════════════════════════════════════════
    public class CreateOrderRequest
    {
        // Null = khách vãng lai (guest). Có giá trị = đơn của thành viên.
        public int? UserId { get; set; }

        // Địa chỉ trong sổ địa chỉ (chỉ dùng cho thành viên). Khách vãng lai bỏ trống
        // và truyền địa chỉ trực tiếp qua khối Guest* bên dưới.
        public int AddressId { get; set; }

        public int ShippingCarrierId { get; set; }

        public string? CouponCode { get; set; }

        public string? UserEmail { get; set; }

        public string PaymentMethod { get; set; } = "COD";

        public string? Note { get; set; }

        // Email nhận xác nhận đơn (bắt buộc với khách vãng lai; thành viên có thể
        // dùng email tài khoản nếu để trống).
        public string? Email { get; set; }

        // ── Địa chỉ giao hàng nhập trực tiếp (guest checkout) ──
        public string? GuestName { get; set; }
        public string? GuestPhone { get; set; }
        public string? GuestStreet { get; set; }
        public string? GuestWard { get; set; }
        public int? GuestProvinceId { get; set; }


        public List<CreateOrderItem>? Items { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // DTO DÒNG SẢN PHẨM TRONG ĐƠN
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // CREATE ORDER ITEM — DTO
    // ════════════════════════════════════════════════════════════
    public class CreateOrderItem
    {
        public int VariantId { get; set; }  
        public int Quantity { get; set; }   
    }

    // ════════════════════════════════════════════════════════════
    // INTERFACE SERVICE ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════

    // ════════════════════════════════════════════════════════════
    // ORDER SERVICE — INTERFACE
    // ════════════════════════════════════════════════════════════
    public interface IOrderService
    {

        

        

        

        
        Task<Order> PlaceOrderAsync(CreateOrderRequest request);

        Task<List<Order>> GetByUserAsync(int userId);

        Task<Order?> GetByIdAsync(int id);

        // Tra cứu đơn theo mã đơn + thông tin liên hệ (email HOẶC số điện thoại).
        // Dùng cho khách vãng lai tra cứu mà không cần đăng nhập. Trả null nếu
        // không khớp (tránh lộ đơn của người khác chỉ bằng mã).
        Task<Order?> TrackAsync(string orderCode, string contact);

        

        

        
        Task<Order?> ChangeStatusAsync(int orderId, string newStatus, string? note = null, string? trackingNumber = null);

        Task<bool> CancelAsync(int orderId);
    }
}
