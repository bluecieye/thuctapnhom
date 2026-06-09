// ============================================================================
// SortColumn.cs — DTO mô tả tiêu chí SẮP XẾP cho API danh sách
// ============================================================================
// Mục đích: REQUEST DTO đi kèm Paging, cho phép client yêu cầu server sắp xếp
// kết quả theo cột nào, chiều tăng hay giảm. Ví dụ: trên trang sản phẩm, người
// dùng chọn "Giá: thấp → cao" → FE gửi { ColumnName: "Price", IsAsc: true }.
//
// Vì sao tách DTO riêng (không nhét vào Paging)?
// - Một số API chỉ cần phân trang KHÔNG cần sort → giữ Paging tối giản.
// - Có API cần MULTI-SORT (sort theo nhiều cột) → dùng List<SortColumn> dễ mở rộng.
// - Logic ánh xạ tên cột → expression LINQ thường do helper riêng xử lý, tách DTO
//   khỏi Paging giúp tách bạch trách nhiệm.
//
// Lưu ý bảo mật: server PHẢI whitelist các ColumnName được phép sort để tránh
// client gửi tên cột nhạy cảm (vd: "PasswordHash") hoặc gây SQL injection.
// ============================================================================
namespace BaseCore.DTO.Common
{
    // ════════════════════════════════════════════════════════════
    // SẮP XẾP CỘT
    // ════════════════════════════════════════════════════════════
    public class SortColumn
    {
        // Tên cột để sắp xếp. Thường khớp tên property của Entity (vd: "Price",
        // "CreatedAt", "Name"). Server map sang câu OrderBy LINQ tương ứng.
        public string ColumnName { get; set; }

        // true = tăng dần (ASC, A→Z, nhỏ→lớn, cũ→mới)
        // false = giảm dần (DESC, Z→A, lớn→nhỏ, mới→cũ)
        // Dùng bool đơn giản, gọn hơn enum vì chỉ có 2 giá trị.
        public bool IsAsc { get; set; }
    }
}
