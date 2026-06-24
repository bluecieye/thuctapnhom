// ============================================================================
// CouponDto.cs — DTO biểu diễn MÃ GIẢM GIÁ (coupon) + kết quả áp mã
// ============================================================================
// Mục đích: Chứa 2 DTO:
// - CouponDto: vừa là REQUEST (admin tạo/sửa coupon), vừa là RESPONSE (FE hiện
//   danh sách coupon khả dụng cho user).
// - ApplyCouponResultDto: RESPONSE riêng cho API "kiểm tra mã" — trả về kết quả
//   validate (hợp lệ/không) + số tiền giảm thực tế để FE hiện ngay trên checkout.
//
// Khác Entity Coupon ở chỗ nào?
// - Entity Coupon có navigation property tới Orders (List các đơn dùng mã) →
//   tránh leak ra ngoài + tránh circular reference khi serialize JSON.
// - DTO không expose các field nội bộ như CreatedBy, UpdatedBy, IsDeleted.
// ============================================================================
using System;

namespace BaseCore.DTO.Fashion
{
    // ════════════════════════════════════════════════════════════
    // DTO MÃ GIẢM GIÁ
    // ════════════════════════════════════════════════════════════
    public class CouponDto
    {
        public int Id { get; set; }

        // Mã code người dùng nhập trên ô coupon (vd: "SUMMER2025", "FREESHIP").
        // Thường unique và UPPERCASE; server so sánh case-insensitive.
        public string Code { get; set; } = "";

        // Loại giảm giá: "Percent" (giảm theo %) hoặc "Fixed" (giảm số tiền cố định).
        // Có thể đổi sang enum nhưng giữ string cho dễ extend (vd: thêm "FreeShip"
        // sau này mà không phải migrate enum).
        public string DiscountType { get; set; } = "Percent";

        // Giá trị giảm — ý nghĩa phụ thuộc DiscountType:
        // - Percent: 10 nghĩa là giảm 10% (không phải 0.1)
        // - Fixed:   50000 nghĩa là giảm 50.000đ
        public decimal DiscountValue { get; set; }

        // Số tiền tối thiểu của đơn hàng để mã có hiệu lực. Vd: 200.000đ trở lên
        // mới được dùng mã. Để 0 nghĩa là không giới hạn.
        public decimal MinOrderAmount { get; set; }

        // Số tiền giảm TỐI ĐA (chỉ áp dụng cho Percent). Vd: giảm 50% nhưng max
        // 100.000đ — đơn 1 triệu vẫn chỉ giảm 100k chứ không phải 500k.
        // Nullable: null = không giới hạn trần.
        public decimal? MaxDiscount { get; set; }

        // Khoảng thời gian hiệu lực. Server check DateTime.Now ∈ [StartDate, EndDate]
        // khi user apply mã. Lưu ý timezone: thường lưu UTC, convert khi hiển thị.
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Tổng số lượt được dùng (toàn hệ thống, không phải per-user).
        // 0 hoặc -1 thường được hiểu là không giới hạn (tùy convention dự án).
        public int UsageLimit { get; set; }

        // Số lượt đã dùng (server tự tăng mỗi khi có đơn dùng mã thành công).
        // Khi UsedCount >= UsageLimit → mã hết hiệu lực.
        public int UsedCount { get; set; }

        // Cờ bật/tắt thủ công của admin — kể cả còn hạn + còn lượt, set false
        // sẽ vô hiệu hóa ngay (dùng để gỡ mã lỗi/lạm dụng).
        public bool IsActive { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // DTO KẾT QUẢ ÁP DỤNG MÃ GIẢM GIÁ
    // ════════════════════════════════════════════════════════════
    public class ApplyCouponResultDto
    {
        // Kết quả validate: true nếu mã hợp lệ và đủ điều kiện áp dụng cho giỏ.
        public bool IsValid { get; set; }

        // Số tiền được giảm thực tế (đã tính cả MaxDiscount, MinOrderAmount).
        // Nếu IsValid = false thì thường là 0.
        public decimal DiscountAmount { get; set; }

        // Thông báo lý do mã không hợp lệ (vd: "Mã đã hết hạn", "Đơn chưa đủ
        // 200.000đ"). Nullable: null khi IsValid = true.
        public string? Message { get; set; }

        // Trả kèm chi tiết coupon để FE hiện thông tin (loại giảm, % giảm, ...).
        // Nullable: null nếu mã không tồn tại hoặc không hợp lệ.
        public CouponDto? Coupon { get; set; }
    }
}
