// ============================================================================
// JsonRestApiResponse.cs — Bộ ENVELOPE chuẩn cho mọi API response của hệ thống
// ============================================================================
// Mục đích: Định nghĩa cấu trúc response THỐNG NHẤT cho toàn bộ REST API. File
// chứa 5 thành phần liên kết với nhau:
// 1. JsonRestApiResponse: envelope kiểu cũ (IsRequestSuccess + Payload + Error)
// 2. ErrorResponseMessage: chi tiết lỗi gắn vào JsonRestApiResponse
// 3. IRestApiResponse + ResponseCreator: pattern Strategy để tạo response
// 4. ResponseFactory: facade tiện gọi nhanh (GetResponse / GetResponseOk)
// 5. ResponseMessage: envelope kiểu mới có thêm StatusCode dạng số
//
// Vì sao không dùng Entity?
// - Đây không phải dữ liệu domain mà là "lớp vỏ" gói data + meta gửi xuống FE.
// - Không liên quan database, chỉ là contract giữa client và server.
//
// Vì sao có 2 envelope (JsonRestApiResponse và ResponseMessage)?
// - JsonRestApiResponse: legacy — dùng boolean IsRequestSuccess + ErrorResponseMessage.
// - ResponseMessage: phiên bản mới — dùng StatusCode số (xem ConstantResponseCode).
//   Khi refactor toàn hệ thống, các endpoint cũ vẫn dùng JsonRestApiResponse để
//   không vỡ contract với client cũ; endpoint mới dùng ResponseMessage.
// ============================================================================
namespace BaseCore.DTO.Response
{
    // ════════════════════════════════════════════════════════════
    // JSON REST API RESPONSE
    // ════════════════════════════════════════════════════════════
    public class JsonRestApiResponse
    {
        // ════════════════════════════════════════════════════════════
        // PROPERTIES
        // ════════════════════════════════════════════════════════════
        // Cờ thành công đơn giản — FE chỉ cần check `if (res.IsRequestSuccess)`.
        public bool IsRequestSuccess { get; set; }

        // Payload dữ liệu thực — object để chấp nhận BẤT KỲ kiểu nào (DTO, List,
        // primitive). Đánh đổi type-safety lấy tính linh hoạt — 1 envelope dùng
        // cho mọi endpoint.
        public object Payload { get; set; }

        // Chi tiết lỗi (code + message). null khi thành công.
        public ErrorResponseMessage Error { get; set; }

        // ════════════════════════════════════════════════════════════
        // METHODS
        // ════════════════════════════════════════════════════════════
        // Constructor cho trường hợp THÀNH CÔNG — không có error.
        // Payload optional vì có endpoint chỉ cần trả "OK" không kèm data.
        public JsonRestApiResponse(bool isRequestSuccess, object payload = null)
        {
            IsRequestSuccess = isRequestSuccess;
            Payload = payload;
            Error = null;
        }

        // Constructor cho trường hợp LỖI — bắt buộc có errorCode (dùng làm i18n
        // key, xem ConstantResponseMessage). errorMessage là text bổ sung.
        // Vẫn cho phép gửi kèm payload trong lỗi (vd: trả lại danh sách field
        // validate sai) để FE highlight các trường input lỗi.
        public JsonRestApiResponse(bool isRequestSuccess, string errorCode, string errorMessage = "", object payload = null)
        {
            IsRequestSuccess = isRequestSuccess;
            Payload = payload;
            Error = new ErrorResponseMessage(errorCode, errorMessage);
        }
    }

    // ════════════════════════════════════════════════════════════
    // THÔNG BÁO LỖI RESPONSE
    // ════════════════════════════════════════════════════════════
    public class ErrorResponseMessage
    {
        // ════════════════════════════════════════════════════════════
        // PROPERTIES
        // ════════════════════════════════════════════════════════════
        // Mã lỗi dạng i18n key (vd: "RECORD_NOT_FOUND") — FE dùng để dịch.
        public string Code { get; set; }

        // Message text bổ sung — FE có thể hiển thị fallback nếu chưa có
        // bản dịch cho Code.
        public string Message { get; set; }

        // ════════════════════════════════════════════════════════════
        // METHODS
        // ════════════════════════════════════════════════════════════
        public ErrorResponseMessage(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    // ════════════════════════════════════════════════════════════
    // INTERFACE REST API RESPONSE
    // ════════════════════════════════════════════════════════════
    // Pattern Strategy: định nghĩa cách tạo ResponseMessage. Tách interface ra
    // để dễ mock trong unit test + dễ mở rộng (vd: ResponseCreatorWithLogger).
    public interface IRestApiResponse
    {
        ResponseMessage Create(bool isSuccess, int statusCode, string statusMessage, object payload);
    }

    // ════════════════════════════════════════════════════════════
    // BỘ TẠO RESPONSE
    // ════════════════════════════════════════════════════════════
    public class ResponseCreator : IRestApiResponse
    {
        // Tạo object ResponseMessage chuẩn hóa.
        // IMPORTANT: IsSuccess KHÔNG đọc từ tham số `isSuccess` truyền vào mà tự
        // suy ra từ statusCode == 0 (ConstantResponseCode.SUCCESS). Đây là điểm
        // dễ gây nhầm lẫn — dù caller truyền isSuccess=true mà statusCode != 0
        // thì IsSuccess vẫn ra false. Hành vi này giúp đảm bảo nhất quán nhưng
        // làm tham số `isSuccess` trở nên dư thừa.
        public ResponseMessage Create(bool isSuccess, int statusCode, string statusMessage, object payload = null)
        {
            ResponseMessage message = new ResponseMessage
            {
                IsSuccess = statusCode == ConstantResponseCode.SUCCESS,
                StatusCode = statusCode,
                Message = statusMessage,
                Payload = payload
            };
            return message;
        }
    }

    // ════════════════════════════════════════════════════════════
    // FACTORY RESPONSE
    // ════════════════════════════════════════════════════════════
    // Facade tiện lợi — controller chỉ cần gọi 1 dòng để tạo response chuẩn,
    // không phải tự new ResponseCreator mỗi lần.
    public class ResponseFactory
    {
        // Tạo response LỖI/CẢNH BÁO với statusCode + message tùy ý.
        // Lưu ý: cố định isSuccess=false trong call → mọi response qua hàm này
        // đều coi là không thành công ở góc nhìn caller, nhưng IsSuccess thực tế
        // vẫn được ResponseCreator tự suy ra từ statusCode.
        public static ResponseMessage GetResponse(int statusCode, string message, object payload = null)
        {
            var objResponse = new ResponseCreator();
            return objResponse.Create(false, statusCode, message, payload);
        }

        // Tạo response THÀNH CÔNG — statusCode mặc định = 0, message rỗng.
        // Hàm này là "happy path" tiện gọi: return ResponseFactory.GetResponseOk(data);
        public static ResponseMessage GetResponseOk(object payload = null)
        {
            var objResponse = new ResponseCreator();
            return objResponse.Create(true, ConstantResponseCode.SUCCESS, "", payload);
        }
    }

    // ════════════════════════════════════════════════════════════
    // THÔNG BÁO RESPONSE
    // ════════════════════════════════════════════════════════════
    // Envelope kiểu MỚI — dùng StatusCode dạng số thay vì chỉ boolean.
    // Cho phép phân biệt nhiều trạng thái (SUCCESS/ERROR/WARNING/INFO) thay vì
    // chỉ có pass/fail. Khuyến nghị dùng cho mọi endpoint mới.
    public class ResponseMessage
    {
        // Tổng kết pass/fail nhanh — derived từ StatusCode == SUCCESS.
        public bool IsSuccess { get; set; }

        // Mã trạng thái dạng số — xem ConstantResponseCode (0/1/2/3).
        // Lưu ý: không phải HTTP status code; đây là code nghiệp vụ.
        public int StatusCode { get; set; }

        // Message text (có thể là i18n key hoặc text fallback).
        public string Message { get; set; }

        // Payload dữ liệu thực — kiểu object linh hoạt như JsonRestApiResponse.
        public object Payload { get; set; }
    }

}
