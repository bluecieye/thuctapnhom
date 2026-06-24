// ============================================================================
// ConstantResponseMessage.cs — Tập hợp HẰNG SỐ message code dùng cho API response
// ============================================================================
// Mục đích: Đây KHÔNG phải DTO theo nghĩa truyền dữ liệu, mà là 2 class static
// chứa hằng số:
// - ConstantResponseMessage: các CODE message (kiểu i18n key) trả về cho FE.
//   FE bắt đúng key này và dịch sang ngôn ngữ phù hợp (vd: "RECORD_ADDED" →
//   tiếng Việt: "Thêm bản ghi thành công"). Comment gốc ghi chú là dành cho
//   AngularJS translate — nay vẫn tương thích với i18next/react-intl.
// - ConstantResponseCode: status code dạng số dùng để FE switch-case xử lý.
//
// Vì sao tách ra constant?
// - Tránh hard-code chuỗi rải rác trong codebase → typo sẽ không bị compiler bắt.
// - Một chỗ đổi message key → đổi toàn hệ thống.
// - FE và BE có thể share danh sách key qua TypeScript enum/d.ts để đồng bộ.
// ============================================================================
namespace BaseCore.DTO.Response
{
    // ════════════════════════════════════════════════════════════
    // HẰNG SỐ THÔNG BÁO RESPONSE
    // ════════════════════════════════════════════════════════════
    //These message will be used to match the language used in angularjs translate.
    public static class ConstantResponseMessage
    {
        // ----- Nhóm CODE i18n key (FE dịch sang ngôn ngữ tương ứng) -----
        // Đây là CODE để FE map sang i18n, KHÔNG phải text hiển thị trực tiếp.
        public static readonly string SUCCESS = "SUCCESS";
        public static readonly string RECORD_ADDED = "RECORD_ADDED";
        public static readonly string RECORD_ADDED_FAIL = "RECORD_ADDED_FAIL";
        public static readonly string RECORD_UPDATED = "RECORD_UPDATED";
        public static readonly string RECORD_UPDATED_FAIL = "RECORD_UPDATED_FAIL";
        public static readonly string RECORD_CANCELLED = "RECORD_CANCELLED";
        public static readonly string RECORD_REMOVED = "RECORD_REMOVED";
        public static readonly string RECORD_REMOVED_FAIL = "RECORD_REMOVED_FAIL";
        public static readonly string RECORD_COPY_TO = "RECORD_COPY_TO";
        public static readonly string RECORD_COPY_TO_FAIL = "RECORD_COPY_TO_FAIL";
        public static readonly string RECORD_APPROVED = "RECORD_APPROVED";
        public static readonly string RECORD_REJECTED = "RECORD_REJECTED";
        public static readonly string RECORD_NOT_ALLOW_REMOVE = "RECORD_NOT_ALLOW_REMOVE";
        public static readonly string MSG_ACTIVE_DEACTIVE_FAIL = "MSG_ACTIVE_DEACTIVE_FAIL";

        public static readonly string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public static readonly string DUPLICATED_RECORD = "DUPLICATED_RECORD";
        public static readonly string INVALID_INPUT = "INVALID_INPUT";
        public static readonly string RECORD_NOT_FOUND = "MSG_RECORD_NOT_FOUND";
        public static readonly string RECORD_ALREADY_EXISTS = "RECORD_ALREADY_EXISTS";
        public static readonly string NOT_PERMISSION = "NOT_PERMISSION";
        // Lưu ý: EXIST_ACTION_CODE chứa TEXT TIẾNG VIỆT chứ không phải i18n key —
        // đây là lệch chuẩn so với các hằng còn lại. Khi gặp, FE sẽ hiển thị
        // nguyên văn. Nên refactor về dạng key (vd: "EXIST_ACTION_CODE") khi có thời gian.
        public static readonly string EXIST_ACTION_CODE = "Mã điều khiển không được trùng! ";


        // ----- Nhóm MESSAGE đã sẵn text tiếng Anh (fallback nếu FE không dịch) -----
        // Các key có tiền tố MSG_ là TEXT thực tế, dùng khi FE chưa cấu hình i18n
        // hoặc khi BE gửi trực tiếp message gốc.
        public static readonly string MSG_INVALID_INPUT = "There are some field not input correctly!";
        public static readonly string MSG_RECORD_ADDED = "Record added successfully!";
        public static readonly string MSG_RECORD_ADDED_FAIL = "Record added fail!";
        public static readonly string MSG_RECORD_UPDATED = "Record updated successfully!";
        public static readonly string MSG_RECORD_UPDATED_FAIL = "Record updated fail!";
        public static readonly string MSG_DUPLICATED_RECORD = "Ops, Record exists!";
        public static readonly string MSG_RECORD_NOT_ALLOW_REMOVE = "Record unable to delete, please try another record.";
        public static readonly string MSG_RECORD_REMOVED = "Record removed successfully.";
        public static readonly string MSG_RECORD_REMOVED_FAIL = "There is a error when delete, Please try again or confirm with Admin.";

        #region Group

        // Code dành riêng cho validate tham số nhóm (Group entity).
        public static readonly string GROUP_VALID_PARAMS = "GROUP_VALID_PARAMS";

        #endregion
    }

    // ════════════════════════════════════════════════════════════
    // HẰNG SỐ MÃ RESPONSE
    // ════════════════════════════════════════════════════════════
    //TODO
    // Status code dạng số dùng cho ResponseMessage.StatusCode.
    // Tách enum-like class này để FE/BE switch-case theo cùng convention.
    // Quy ước: 0 = thành công, !=0 = có vấn đề (mức độ khác nhau).
    public static class ConstantResponseCode
    {
        public const int SUCCESS = 0;   // Thao tác thành công hoàn toàn
        public const int ERROR = 1;     // Lỗi nghiệp vụ hoặc lỗi hệ thống
        public const int WARNING = 2;   // Cảnh báo — thao tác xong nhưng có lưu ý
        public const int INFO = 3;      // Thông báo thông tin (không phải lỗi)
    }
}
