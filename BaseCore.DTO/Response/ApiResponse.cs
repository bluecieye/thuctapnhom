// ============================================================================
// ApiResponse.cs — DTO chung cho API trả về DANH SÁCH dạng GRID (có phân trang)
// ============================================================================
// Mục đích: RESPONSE DTO chuyên dùng cho các trang quản trị dùng grid (DataTable,
// ag-Grid, ant-table...). Server gói tổng số bản ghi + dữ liệu trang hiện tại
// vào MỘT envelope chuẩn → FE chỉ cần parse 1 cấu trúc duy nhất, không phải
// custom mỗi endpoint.
//
// Vì sao không dùng Entity?
// - Đây không phải dữ liệu domain mà là "container" gói dữ liệu phản hồi.
// - Tách ra giúp các API list/grid có shape thống nhất, FE dùng chung component
//   pagination cho mọi entity (User, Product, Order, ...).
//
// Lưu ý đặt tên file: file tên ApiResponse.cs nhưng class bên trong là GridResponse.
// Đây là di sản code; có thể đổi tên file thành GridResponse.cs trong refactor sau.
// ============================================================================
namespace BaseCore.DTO.Response
{
    // ════════════════════════════════════════════════════════════
    // RESPONSE API
    // ════════════════════════════════════════════════════════════
    public class GridResponse
    {
        // Tổng số bản ghi (KHÔNG phải số bản ghi trang hiện tại). Dùng để
        // pagination component tính ra "Trang X / Y" và disable nút Next/Prev.
        public int TotalRecords { get; set; }

        // Dữ liệu thực — kiểu object để chứa BẤT KỲ loại list nào (List<UserDto>,
        // List<ProductDto>, ...). Đánh đổi: mất type-safety nhưng được tính linh
        // hoạt — 1 class GridResponse dùng cho mọi grid khắp hệ thống.
        // FE thường cast về Array khi consume.
        public object Records { get; set; }
    }
}
