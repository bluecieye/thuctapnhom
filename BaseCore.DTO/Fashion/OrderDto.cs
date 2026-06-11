// ============================================================================
// OrderDto.cs — DTO biểu diễn ĐƠN HÀNG (response chi tiết + lịch sử đơn)
// ============================================================================
// Mục đích: Chủ yếu là RESPONSE DTO trả về cho:
// - FE customer: trang "Đơn hàng của tôi", trang chi tiết đơn
// - FE admin: trang quản lý đơn hàng (duyệt, đổi trạng thái)
// Có thể dùng làm REQUEST khi admin tạo/sửa đơn thủ công, nhưng thường thì
// CreateOrderRequestDto sẽ là DTO riêng tối giản hơn.
//
// Khác Entity Order ở chỗ nào?
// - Entity Order chỉ lưu OrderItem.VariantId; muốn hiện tên sản phẩm/màu/size
//   phải JOIN bảng. DTO này đã flatten sẵn để FE render ngay.
// - DTO bổ sung CouponCode (string) bên cạnh CouponId — FE chỉ cần hiện mã,
//   không cần truy vấn thêm.
// - Loại bỏ các navigation property gây vòng lặp khi serialize JSON.
// ============================================================================
using System;
using System.Collections.Generic;

namespace BaseCore.DTO.Fashion
{
    // ════════════════════════════════════════════════════════════
    // DTO ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // Trạng thái đơn: "Pending" | "Confirmed" | "Shipping" | "Delivered" |
        // "Cancelled" | "Returned". Giữ kiểu string để dễ thêm trạng thái mới
        // mà không phải đổi enum + migrate DB.
        public string Status { get; set; } = "Pending";

        // Phương thức thanh toán: "COD" (nhận tiền tận nơi) | "VNPay" | "Momo" | ...
        public string PaymentMethod { get; set; } = "COD";

        // Địa chỉ giao hàng SNAPSHOT — server lưu nguyên chuỗi text tại thời điểm
        // đặt hàng. Nếu sau đó user xóa AddressDto thì đơn cũ vẫn giữ địa chỉ gốc
        // (không bị mất dữ liệu lịch sử).
        public string ShippingAddress { get; set; } = "";

        // Ghi chú của khách (vd: "Giao giờ hành chính", "Gọi trước khi tới").
        public string? Note { get; set; }

        // Tổng tiền cuối cùng phải thanh toán = Subtotal + ShippingFee - DiscountAmount.
        // Server tính sẵn để client không tự cộng trừ (tránh lệch số liệu).
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        // Ngày dự kiến giao — service tự tính dựa trên ShippingFee/khu vực.
        // Nullable: null khi đơn vừa tạo chưa có lịch ship.
        public DateTime? EstimatedDelivery { get; set; }

        // Coupon đã áp dụng (nếu có). Cả CouponId và CouponCode đều nullable.
        // Lưu CouponCode kèm vì admin có thể xóa coupon sau đó, đơn cũ vẫn hiển
        // thị được mã đã dùng (snapshot).
        public int? CouponId { get; set; }
        public string? CouponCode { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }

    // ════════════════════════════════════════════════════════════
    // DTO CHI TIẾT ĐƠN HÀNG
    // ════════════════════════════════════════════════════════════
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int VariantId { get; set; }

        // Snapshot tên sản phẩm + SKU + màu + size tại thời điểm đặt. Nếu admin
        // đổi tên/màu sản phẩm sau đó, đơn cũ vẫn hiện thông tin gốc → đảm bảo
        // tính chính xác của lịch sử đơn hàng.
        public string ProductName { get; set; } = "";
        public string SKU { get; set; } = "";
        public string? ColorName { get; set; }
        public string? SizeName { get; set; }

        public int Quantity { get; set; }

        // Giá tại thời điểm đặt — KHÔNG đổi kể cả sau này admin đổi giá variant.
        // Đây là nguyên tắc kế toán cơ bản: dữ liệu giao dịch phải bất biến.
        public decimal UnitPrice { get; set; }

        // PROPERTY TÍNH TOÁN (computed) — không lưu DB, chỉ tính lúc serialize.
        // Tiền của 1 dòng = đơn giá × số lượng. Đặt ở DTO vì là logic trình bày.
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
