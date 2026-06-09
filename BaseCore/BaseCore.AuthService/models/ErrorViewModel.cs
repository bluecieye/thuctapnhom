

namespace BaseCore.AuthService.Models
{
    // ════════════════════════════════════════════════════════════
    // MODEL HIỂN THỊ LỖI
    // ════════════════════════════════════════════════════════════
    public class ErrorViewModel
    {

        
        public string? RequestId { get; set; }

        
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
