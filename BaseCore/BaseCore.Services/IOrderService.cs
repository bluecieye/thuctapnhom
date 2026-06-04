

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
        
        public int UserId { get; set; }

        public int AddressId { get; set; }

        public int ShippingCarrierId { get; set; }

        public string? CouponCode { get; set; }

        public string PaymentMethod { get; set; } = "COD";

        public string? Note { get; set; }

        
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

        

        

        
        Task<Order?> ChangeStatusAsync(int orderId, string newStatus, string? note = null);

        Task<bool> CancelAsync(int orderId);
    }
}
