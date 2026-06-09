// ============================================================================
// ReviewDto.cs — DTO biểu diễn ĐÁNH GIÁ sản phẩm + tổng hợp review
// ============================================================================
// Mục đích: Chứa 2 DTO:
// - ReviewDto: vừa REQUEST (user submit review từ trang đã mua), vừa RESPONSE
//   (trang chi tiết sản phẩm hiện list review).
// - ReviewSummaryDto: RESPONSE thuần — gói gọn meta-data (điểm trung bình, số
//   lượt) + list review. Trả từ API GET /products/{id}/reviews trong 1 lần.
//
// Khác Entity Review ở chỗ nào?
// - Entity Review chỉ lưu UserId; phải JOIN bảng User để lấy Username. DTO đã
//   flatten Username sẵn để FE hiện "Bình luận bởi alice" mà không cần API user.
// - Loại bỏ các field nhạy cảm như IP người review, IsHidden (admin moderation).
// - SizeAccuracy được giữ riêng dạng số 1-5 (vd: 1=quá nhỏ, 3=vừa, 5=quá lớn)
//   để FE render thanh "Mức độ vừa vặn" trên trang chi tiết.
// ============================================================================
using System;
using System.Collections.Generic;

namespace BaseCore.DTO.Fashion
{
    // ════════════════════════════════════════════════════════════
    // DTO ĐÁNH GIÁ
    // ════════════════════════════════════════════════════════════
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }

        // Tên hiển thị của người review — lấy sẵn từ JOIN bảng User.
        // Có thể là username đầy đủ hoặc đã ẩn bớt (vd: "al***ce") tùy chính
        // sách bảo mật của dự án.
        public string Username { get; set; } = "";

        // Số sao đánh giá (1-5). Khi nhận request từ client, controller PHẢI
        // validate giá trị trong khoảng này để tránh dữ liệu bẩn.
        public int Rating { get; set; }

        public string Comment { get; set; } = "";

        // URL ảnh user đính kèm review (ảnh thực tế khi mặc, tránh ảnh studio).
        // Nullable vì review có thể không có ảnh.
        public string? ImageUrl { get; set; }

        // Mức độ vừa vặn (1-5): 1=Rất nhỏ, 2=Hơi nhỏ, 3=Vừa, 4=Hơi rộng, 5=Rất rộng.
        // Dùng riêng vì sản phẩm thời trang đặc thù — kích cỡ thường lệch chuẩn.
        public int SizeAccuracy { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    // ════════════════════════════════════════════════════════════
    // DTO TỔNG HỢP ĐÁNH GIÁ
    // ════════════════════════════════════════════════════════════
    public class ReviewSummaryDto
    {
        // Điểm trung bình tổng hợp (1.0–5.0). Server tính sẵn = AVG(Rating).
        public double AverageRating { get; set; }

        // Tổng số review — dùng hiển thị "Dựa trên N đánh giá".
        public int Count { get; set; }

        // List review chi tiết, có thể đã được phân trang ở phía controller.
        public List<ReviewDto> Reviews { get; set; } = new();
    }
}
