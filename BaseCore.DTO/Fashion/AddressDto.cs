// ============================================================================
// AddressDto.cs — DTO biểu diễn ĐỊA CHỈ GIAO HÀNG của user (cả request + response)
// ============================================================================
// Mục đích: Dùng cho cả 2 chiều:
// - REQUEST: client gửi lên khi thêm/sửa địa chỉ trong sổ địa chỉ
// - RESPONSE: server trả danh sách địa chỉ để FE render trong AccountPage/Checkout
//
// Khác Entity Address ở chỗ nào?
// - Entity Address gắn navigation property User, có thể chứa các field hệ thống
//   (CreatedAt, UpdatedAt, RowVersion...) mà client KHÔNG cần biết.
// - DTO chỉ giữ các field hiển thị/nhập liệu thuần, tránh leak thông tin nội bộ
//   và giảm payload JSON khi truyền qua mạng.
// - DTO không có circular reference (User → Addresses → User → ...) gây lỗi
//   khi serialize JSON.
// ============================================================================
namespace BaseCore.DTO.Fashion
{
    // ════════════════════════════════════════════════════════════
    // DTO ĐỊA CHỈ
    // ════════════════════════════════════════════════════════════
    public class AddressDto
    {
        // ID địa chỉ. Khi TẠO MỚI client gửi 0 hoặc bỏ trống; khi CẬP NHẬT phải
        // gửi đúng ID đã có để server biết update bản ghi nào.
        public int Id { get; set; }

        // ID user sở hữu địa chỉ. Server KHÔNG nên tin tưởng giá trị client gửi
        // mà nên lấy từ JWT claim → tránh user A chỉnh sửa địa chỉ của user B.
        public int UserId { get; set; }

        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Street { get; set; } = "";
        public string Ward { get; set; } = "";
        public string City { get; set; } = "";

        // Cờ đánh dấu địa chỉ mặc định. Một user chỉ được phép có DUY NHẤT 1 địa
        // chỉ IsDefault = true → khi set true cho địa chỉ này, service phải tự
        // động set false cho các địa chỉ khác trong cùng UserId (transaction).
        public bool IsDefault { get; set; }
    }
}
