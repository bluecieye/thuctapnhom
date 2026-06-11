// ============================================================================
// ProductDto.cs — DTO biểu diễn SẢN PHẨM + variant + ảnh (response chi tiết)
// ============================================================================
// Mục đích: RESPONSE DTO chính của module sản phẩm. Phục vụ cả 2 màn hình:
// - List view: trang shop, search, danh mục (FE chỉ dùng Name, BasePrice, ảnh
//   đầu tiên, AverageRating, ReviewCount)
// - Detail view: trang chi tiết sản phẩm (dùng FULL các trường + Variants để
//   render dropdown chọn màu/size).
//
// Khác Entity Product ở chỗ nào?
// - Entity Product chỉ giữ BasePrice và CategoryId; phải JOIN qua Category để
//   lấy CategoryName, JOIN Reviews để tính AverageRating.
// - DTO này đã PRE-COMPUTE:
//     + CategoryName (lấy sẵn từ JOIN, FE không cần gọi API category riêng)
//     + TotalStock = SUM(Variants.Stock) — tổng tồn kho toàn bộ variant
//     + AverageRating + ReviewCount — agregate từ bảng Review
// - Loại bỏ các navigation property circular (Variant → Product → Variants → ...)
//   để JSON serialize không bị lỗi infinite loop.
// ============================================================================
using System.Collections.Generic;

namespace BaseCore.DTO.Fashion
{
    // ════════════════════════════════════════════════════════════
    // DTO SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        // Giá gốc/giá hiển thị mặc định. Lưu ý: giá thực bán có thể khác theo
        // variant (ProductVariantDto.Price) — vd: variant size XXL đắt hơn 10%.
        public decimal BasePrice { get; set; }

        public int CategoryId { get; set; }

        // Tên danh mục — lấy sẵn từ JOIN với bảng Category để FE không phải gọi
        // thêm API. Nullable phòng trường hợp sản phẩm chưa gán category.
        public string? CategoryName { get; set; }

        // Danh sách biến thể (combo màu × size). Render dropdown ở trang detail.
        public List<ProductVariantDto> Variants { get; set; } = new();

        // Danh sách ảnh (có thể nhiều ảnh cho cùng 1 màu). Sort theo DisplayOrder
        // ở phía server trước khi trả về.
        public List<ProductImageDto> Images { get; set; } = new();

        // Tổng tồn kho khả dụng (đã trừ ReservedStock của tất cả variant).
        // Dùng để hiện badge "Sắp hết hàng" hoặc disable nút "Thêm vào giỏ".
        public int TotalStock { get; set; }

        // Điểm đánh giá trung bình (1.0–5.0). Server tính sẵn từ bảng Review.
        // Dùng double thay vì decimal vì độ chính xác không cần quá cao + nhẹ hơn.
        public double AverageRating { get; set; }

        // Số lượng review — dùng để hiện "(N đánh giá)" cạnh sao.
        public int ReviewCount { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // DTO BIẾN THỂ SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = "";

        // Giá riêng của variant này (override BasePrice của Product).
        public decimal Price { get; set; }

        // Tồn kho VẬT LÝ trong kho.
        public int Stock { get; set; }

        // Tồn kho ĐÃ GIỮ CHỖ — số lượng đã có trong giỏ hàng/đơn pending của
        // các user khác. Trừ ra để tránh oversell (bán quá số lượng thực có).
        public int ReservedStock { get; set; }

        // PROPERTY TÍNH TOÁN — tồn kho thực sự khả dụng để bán mới.
        // Đặt ở DTO (không Entity) vì ReservedStock có thể tính theo nhiều cách
        // khác nhau ở từng tầng service.
        public int AvailableStock => Stock - ReservedStock;

        // Thông tin màu sắc. ColorHex là mã màu hex (vd: "#FF0000") để FE render
        // swatch màu trực quan thay vì chỉ hiện tên text.
        public int ColorId { get; set; }
        public string? ColorName { get; set; }
        public string? ColorHex { get; set; }

        public int SizeId { get; set; }
        public string? SizeName { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // DTO HÌNH ẢNH SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    public class ProductImageDto
    {
        public int Id { get; set; }

        // Tên file ảnh (không phải URL đầy đủ) — FE tự ghép với base CDN URL.
        // Cách này giúp đổi domain CDN mà không phải migrate DB.
        public string FileName { get; set; } = "";

        // Ảnh gắn với màu nào (vì cùng sản phẩm, mỗi màu có set ảnh riêng).
        public int ColorId { get; set; }

        // Ảnh đại diện — mỗi (Product, ColorId) chỉ có duy nhất 1 ảnh IsPrimary
        // = true. Dùng làm thumbnail trên list view + ảnh đầu tiên ở detail.
        public bool IsPrimary { get; set; }

        // Thứ tự hiển thị trong gallery (asc: 0 → N).
        public int DisplayOrder { get; set; }
    }
}
