// ============================================================================
// UserDto.cs — DTO biểu diễn THÔNG TIN NGƯỜI DÙNG (response, không có password)
// ============================================================================
// Mục đích: RESPONSE DTO trả về thông tin user cho:
// - FE customer: trang AccountPage hiện hồ sơ
// - FE admin: trang quản lý user (danh sách, chi tiết)
// - Sau login: trả về kèm JWT token (token nằm ở response khác, không trong DTO)
//
// Khác Entity User ở chỗ QUAN TRỌNG NHẤT:
// - Entity User có PasswordHash, Salt, SecurityStamp, RefreshToken, ResetToken...
//   → TUYỆT ĐỐI không được leak ra ngoài. DTO này CỐ TÌNH bỏ các field đó.
// - Entity có navigation property tới Addresses, Orders, Reviews — bỏ để tránh
//   payload JSON nặng + circular reference. Khi cần địa chỉ/đơn hàng FE gọi
//   API riêng.
// - Đây là minh chứng kinh điển vì sao phải tách DTO khỏi Entity:
//   nếu trả thẳng Entity User → rò rỉ PasswordHash = thảm họa bảo mật.
// ============================================================================
using System;

namespace BaseCore.DTO.Fashion
{
    // ════════════════════════════════════════════════════════════
    // DTO NGƯỜI DÙNG
    // ════════════════════════════════════════════════════════════
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";

        // Vai trò: "Admin" | "Customer" | "WarehouseStaff" | "Marketing".
        // FE dùng để hiện/ẩn menu (vd: chỉ Admin thấy menu /admin/users).
        // Lưu ý bảo mật: KHÔNG được tin tưởng giá trị này phía client để
        // phân quyền — server phải tự check JWT claim trong mọi endpoint.
        public string Role { get; set; } = "Customer";

        // Cờ kích hoạt tài khoản. Admin có thể set false để khóa user mà
        // không xóa hẳn (giữ lịch sử đơn hàng). User bị khóa → login sẽ
        // bị reject ngay từ AuthService.
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
