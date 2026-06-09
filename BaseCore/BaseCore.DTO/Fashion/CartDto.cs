// ============================================================================
// CartDto.cs — DTO mô tả GIỎ HÀNG của user (response body cho API /cart)
// ============================================================================
// Mục đích: RESPONSE DTO trả về cho FE để render trang giỏ hàng. Gộp toàn bộ
// thông tin cần thiết (sản phẩm, biến thể, ảnh, đơn giá, số lượng) vào MỘT
// payload duy nhất → FE không phải gọi nhiều API rời rạc cho từng item.
//
// Khác Entity Cart / CartItem ở chỗ nào?
// - Entity Cart chỉ lưu reference (VariantId, ProductId) → muốn hiện tên sản
//   phẩm, ảnh, màu, size phải JOIN nhiều bảng.
// - DTO này đã được FLATTEN: tên sản phẩm, ColorName, SizeName, PrimaryImageUrl
//   được "kéo phẳng" vào CartItemDto → FE chỉ cần lặp 1 vòng for render.
// - Subtotal được tính sẵn ở server (sum LineTotal) để FE không phải tính lại,
//   tránh sai lệch khi giá thay đổi giữa lúc lấy data.
// ============================================================================
using System;
using System.Collections.Generic;

namespace BaseCore.DTO.Fashion
{
    // ════════════════════════════════════════════════════════════
    // DTO GIỎ HÀNG
    // ════════════════════════════════════════════════════════════
    public class CartDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // Mốc thời gian cập nhật cuối — dùng để hiện "Cập nhật lúc X" hoặc làm
        // cache key. Nếu giỏ hàng quá cũ (>30 ngày) FE có thể prompt user xóa giỏ.
        public DateTime UpdatedAt { get; set; }

        // Danh sách item trong giỏ. Khởi tạo `= new()` để tránh NullReferenceException
        // khi FE serialize/deserialize lúc giỏ rỗng.
        public List<CartItemDto> Items { get; set; } = new();

        // Tổng tiền hàng (CHƯA bao gồm phí ship + giảm giá). Server tính sẵn để
        // client không phải nhân/cộng, tránh sai lệch số liệu khi giá thay đổi.
        public decimal Subtotal { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // DTO MỤC GIỎ HÀNG
    // ════════════════════════════════════════════════════════════
    public class CartItemDto
    {
        public int Id { get; set; }

        // ID của biến thể cụ thể (combo màu + size + SKU). Một sản phẩm có thể có
        // nhiều variant → chính variant này mới là thứ cộng/trừ tồn kho thực.
        public int VariantId { get; set; }

        // ID sản phẩm cha — dùng để client tạo link "Quay lại trang sản phẩm".
        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";
        public string SKU { get; set; } = "";

        // Màu/Size có thể null nếu sản phẩm không phân loại (vd: phụ kiện một mẫu).
        public string? ColorName { get; set; }
        public string? SizeName { get; set; }

        // URL ảnh đại diện của variant (theo màu). Server đã chọn sẵn ảnh primary
        // để FE chỉ việc <img src={PrimaryImageUrl} />. Nullable vì có thể chưa có ảnh.
        public string? PrimaryImageUrl { get; set; }

        // Giá tại thời điểm THÊM vào giỏ (snapshot). Nếu sau đó admin đổi giá sản
        // phẩm, item trong giỏ vẫn giữ giá cũ cho đến khi user refresh giỏ.
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        // PROPERTY TÍNH TOÁN (computed) — không lưu DB, chỉ tính khi serialize JSON.
        // Đây là tổng tiền cho 1 dòng = đơn giá × số lượng. Đặt ở DTO (không phải
        // Entity) vì đây là logic trình bày, không phải dữ liệu gốc.
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
