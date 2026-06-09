// ============================================================================
// SearchResult.cs — DTO chứa META-DATA của kết quả phân trang trả về client
// ============================================================================
// Mục đích: Đây là RESPONSE DTO (phần meta), thường là base class hoặc property
// đi kèm danh sách dữ liệu thực. Server tính sẵn tổng số bản ghi + tổng số trang
// rồi gửi xuống để client biết cách dựng pagination UI (vd: hiện "Trang 3/12",
// nút Next/Prev, dropdown chọn trang).
//
// Tách riêng khỏi Paging vì:
// - Paging là INPUT (client → server: tôi muốn trang mấy, bao nhiêu bản ghi)
// - SearchResult là OUTPUT (server → client: kết quả phân trang là vậy)
// Hai vai trò ngược chiều nhau nên tách 2 class cho rõ ràng, dễ đọc code.
// ============================================================================
namespace BaseCore.DTO.Common
{
    // ════════════════════════════════════════════════════════════
    // KẾT QUẢ TÌM KIẾM
    // ════════════════════════════════════════════════════════════
    public class SearchResult
    {
        // Tổng số bản ghi khớp điều kiện tìm kiếm (KHÔNG phải số bản ghi trang hiện tại).
        // Dùng để client hiện "Tìm thấy 1.234 sản phẩm" hoặc tính TotalPage phía FE
        // nếu server chưa tính sẵn.
        public int TotalRecords { get; set; }

        // Tổng số trang = ceil(TotalRecords / PageSize). Server tính sẵn để client
        // không phải nhân chia, tránh sai lệch khi PageSize thay đổi giữa các request.
        public int TotalPage { get; set; }
    }
}
