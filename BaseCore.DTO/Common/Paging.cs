// ============================================================================
// Paging.cs — DTO mô tả tham số PHÂN TRANG cho mọi API danh sách (list/search)
// ============================================================================
// Mục đích: Đây là REQUEST DTO, được client gửi lên server dưới dạng query string
// hoặc body khi gọi các API trả về danh sách dài (sản phẩm, đơn hàng, user...).
// Server dùng 2 trường này để tính toán số lượng bản ghi cần SKIP và TAKE trong
// câu LINQ truy vấn database, tránh trả về toàn bộ data gây quá tải.
//
// Vì sao không dùng Entity?
// - Đây không phải dữ liệu lưu xuống DB, chỉ là "tham số" trao đổi giữa client và
//   server → tách thành DTO riêng cho gọn, không phụ thuộc EF Core.
// - Đặt trong namespace Common vì được TÁI SỬ DỤNG ở nhiều module (Product, Order,
//   User...), không thuộc nghiệp vụ cụ thể nào.
// ============================================================================
namespace BaseCore.DTO.Common
{
    // ════════════════════════════════════════════════════════════
    // PHÂN TRANG
    // ════════════════════════════════════════════════════════════
    public class Paging
    {
        // Trang hiện tại người dùng đang xem (đếm từ 1).
        // Lưu ý: nếu client gửi 0 hoặc số âm, server thường tự normalize thành 1
        // để tránh lỗi tính toán offset = (CurrentPage - 1) * PageSize.
        public int CurrentPage { get; set; }

        // Số bản ghi mỗi trang. Thường giới hạn trong khoảng 10–100 để tránh
        // client cố tình gửi PageSize quá lớn nhằm "rút hết" data hoặc gây DoS.
        public int PageSize { get; set; }
    }
}
