
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BaseCore.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BaseCore.Services
{
    // ════════════════════════════════════════════════════════════
    // CẤU HÌNH EMAIL (đọc từ section "Email" trong appsettings.json)
    // ════════════════════════════════════════════════════════════
    // Để service không phụ thuộc trực tiếp vào Microsoft.Extensions.Configuration,
    // ta bind cấu hình ở tầng API (Program.cs) rồi đăng ký POCO này dạng singleton.
    public class EmailSettings
    {
        public bool Enabled { get; set; } = false;     // false = chỉ log ra console, KHÔNG gửi thật
        public string Host { get; set; } = "";          // vd: smtp.gmail.com
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string User { get; set; } = "";          // tài khoản SMTP
        public string Password { get; set; } = "";      // App Password (KHÔNG dùng mật khẩu thường)
        public string FromEmail { get; set; } = "";      // email người gửi
        public string FromName { get; set; } = "MOON Fashion";
    }

    // ════════════════════════════════════════════════════════════
    // INTERFACE DỊCH VỤ EMAIL
    // ════════════════════════════════════════════════════════════
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(Order order);
        Task SendNewsletterWelcomeAsync(string toEmail);
        Task SendOrderStatusUpdateAsync(Order order);
    }

    // ════════════════════════════════════════════════════════════
    // DỊCH VỤ EMAIL — IMPLEMENTATION (SMTP)
    // ════════════════════════════════════════════════════════════
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(EmailSettings settings)
        {
            _settings = settings;
        }

        public async Task SendOrderConfirmationAsync(Order order)
        {
            if (string.IsNullOrWhiteSpace(order.CustomerEmail))
                return;

            var subject = $"[MOON] Xác nhận đơn hàng {order.OrderCode}";
            var body = BuildOrderHtml(order);

            // Khi chưa cấu hình SMTP (Enabled=false) → chỉ log ra console để dev
            // vẫn thấy nội dung email mà không cần tài khoản SMTP thật.
            if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.Host))
            {
                Console.WriteLine("──────────────────────────────────────────────");
                Console.WriteLine($"📧 [EMAIL - DEV MODE] Tới: {order.CustomerEmail}");
                Console.WriteLine($"   Tiêu đề: {subject}");
                Console.WriteLine($"   Mã đơn: {order.OrderCode} · Tổng: {order.TotalAmount:#,##0} đ");
                Console.WriteLine("   (Bật Email:Enabled=true + cấu hình SMTP để gửi thật)");
                Console.WriteLine("──────────────────────────────────────────────");
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(order.CustomerEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.User, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendNewsletterWelcomeAsync(string toEmail)
        {
            if (string.IsNullOrWhiteSpace(toEmail)) return;

            var subject = "[MOON] Chào mừng bạn! Ưu đãi 10% dành riêng cho bạn";
            var body = $@"
<div style='font-family:Arial,Helvetica,sans-serif;max-width:560px;margin:0 auto;color:#222'>
  <h2 style='font-weight:300;letter-spacing:2px'>MOON</h2>
  <p>Cảm ơn bạn đã đăng ký nhận tin từ <b>MOON Fashion</b>!</p>
  <div style='background:#f8fafc;border:1px solid #e5e7eb;border-radius:12px;padding:20px;margin:16px 0;text-align:center'>
    <div style='font-size:13px;color:#888;margin-bottom:8px'>Mã giảm giá của bạn</div>
    <div style='font-size:28px;font-weight:700;letter-spacing:4px;color:#111'>WELCOME10</div>
    <div style='font-size:13px;color:#16a34a;margin-top:8px'>Giảm 10% — không giới hạn giá trị đơn hàng</div>
  </div>
  <p style='font-size:14px;color:#555'>Nhập mã <b>WELCOME10</b> khi thanh toán để nhận ưu đãi. Mã áp dụng cho tất cả sản phẩm.</p>
  <div style='background:#fff7ed;border:1px solid #fed7aa;border-radius:8px;padding:14px;margin:16px 0;font-size:13px;color:#92400e'>
    <b>⚠ Lưu ý quan trọng:</b> Mã chỉ có hiệu lực khi bạn <b>đăng nhập</b> bằng chính email <b>{toEmail}</b> đã đăng ký. Vui lòng tạo tài khoản hoặc đăng nhập trước khi thanh toán.
  </div>
  <p style='margin-top:24px;font-size:12px;color:#999'>Email tự động từ hệ thống MOON Fashion. Vui lòng không trả lời email này.</p>
</div>";

            if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.Host))
            {
                Console.WriteLine($"📧 [EMAIL - DEV MODE] Newsletter welcome → {toEmail}");
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.User, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendOrderStatusUpdateAsync(Order order)
        {
            if (string.IsNullOrWhiteSpace(order.CustomerEmail)) return;

            var statusVi = order.Status switch
            {
                "Processing" => "Đang xử lý",
                "Shipping"   => "Đang giao hàng",
                "Delivered"  => "Đã giao thành công",
                "Cancelled"  => "Đã huỷ",
                _            => null
            };
            if (statusVi == null) return;

            var subject = $"[MOON] Đơn hàng {order.OrderCode} — {statusVi}";

            var trackingHtml = !string.IsNullOrWhiteSpace(order.TrackingNumber)
                ? $@"<div style='background:#f0f9ff;border:1px solid #bae6fd;border-radius:8px;padding:12px;margin:12px 0'>
                       <div style='font-size:12px;color:#0369a1'>Mã vận đơn</div>
                       <div style='font-size:20px;font-weight:700;letter-spacing:2px;color:#0c4a6e'>{order.TrackingNumber}</div>
                     </div>"
                : "";

            var noteHtml = !string.IsNullOrWhiteSpace(order.Note)
                ? $"<p style='font-size:14px;color:#555'><b>Ghi chú:</b> {WebUtility.HtmlEncode(order.Note)}</p>"
                : "";

            var body = $@"
<div style='font-family:Arial,Helvetica,sans-serif;max-width:560px;margin:0 auto;color:#222'>
  <h2 style='font-weight:300;letter-spacing:2px'>MOON</h2>
  <p>Xin chào, đơn hàng <strong>{order.OrderCode}</strong> của bạn vừa được cập nhật:</p>
  <div style='background:#f8fafc;border:1px solid #e5e7eb;border-radius:12px;padding:20px;margin:16px 0;text-align:center'>
    <div style='font-size:13px;color:#888;margin-bottom:8px'>Trạng thái</div>
    <div style='font-size:22px;font-weight:700;color:#111'>{statusVi}</div>
  </div>
  {trackingHtml}
  {noteHtml}
  <p style='margin-top:24px;font-size:12px;color:#999'>Email tự động từ hệ thống MOON Fashion. Vui lòng không trả lời email này.</p>
</div>";

            if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.Host))
            {
                Console.WriteLine($"📧 [EMAIL - DEV MODE] Status update ({order.Status}) → {order.CustomerEmail}");
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(order.CustomerEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.User, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        // ── Dựng nội dung HTML đơn giản, gọn cho email client ──
        private static string BuildOrderHtml(Order order)
        {
            string Money(decimal v) => v.ToString("#,##0", CultureInfo.InvariantCulture) + " đ";

            var rows = new StringBuilder();
            foreach (var d in order.OrderDetails)
            {
                var name = d.Variant?.Product?.Name ?? $"Sản phẩm #{d.VariantId}";
                var color = d.Variant?.Color?.Name;
                var size = d.Variant?.Size?.Name;
                var meta = string.Join(" · ", new[] { color, size == null ? null : $"Size {size}" }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
                rows.Append($@"
                    <tr>
                      <td style='padding:8px 0;border-bottom:1px solid #eee'>
                        <div style='font-weight:600;color:#111'>{WebUtility.HtmlEncode(name)}</div>
                        <div style='font-size:12px;color:#888'>{WebUtility.HtmlEncode(meta)}</div>
                      </td>
                      <td style='padding:8px 0;border-bottom:1px solid #eee;text-align:center;color:#555'>{d.Quantity}</td>
                      <td style='padding:8px 0;border-bottom:1px solid #eee;text-align:right;color:#111'>{Money(d.UnitPrice * d.Quantity)}</td>
                    </tr>");
            }

            var discountRow = order.DiscountAmount > 0
                ? $"<tr><td colspan='2' style='padding:4px 0;color:#16a34a'>Giảm giá</td><td style='padding:4px 0;text-align:right;color:#16a34a'>-{Money(order.DiscountAmount)}</td></tr>"
                : "";

            return $@"
<div style='font-family:Arial,Helvetica,sans-serif;max-width:560px;margin:0 auto;color:#222'>
  <h2 style='font-weight:300;letter-spacing:2px'>MOON</h2>
  <p>Cảm ơn bạn đã đặt hàng! Đơn của bạn đã được ghi nhận.</p>
  <div style='background:#f8fafc;border:1px solid #e5e7eb;border-radius:12px;padding:16px;margin:16px 0'>
    <div style='font-size:13px;color:#888'>Mã đơn hàng</div>
    <div style='font-size:22px;font-weight:700;letter-spacing:1px'>{order.OrderCode}</div>
    <div style='font-size:13px;color:#555;margin-top:6px'>Dùng mã này để tra cứu đơn tại mục <b>Tra cứu đơn hàng</b>.</div>
  </div>

  <table style='width:100%;border-collapse:collapse;font-size:14px'>
    <thead>
      <tr style='color:#888;font-size:12px;text-transform:uppercase'>
        <th style='text-align:left;padding-bottom:6px'>Sản phẩm</th>
        <th style='text-align:center;padding-bottom:6px'>SL</th>
        <th style='text-align:right;padding-bottom:6px'>Thành tiền</th>
      </tr>
    </thead>
    <tbody>{rows}</tbody>
    <tfoot>
      <tr><td colspan='2' style='padding:8px 0 4px;color:#555'>Phí vận chuyển</td><td style='padding:8px 0 4px;text-align:right'>{(order.ShippingFee == 0 ? "Miễn phí" : Money(order.ShippingFee))}</td></tr>
      {discountRow}
      <tr><td colspan='2' style='padding:8px 0;font-weight:700;font-size:16px'>Tổng cộng</td><td style='padding:8px 0;text-align:right;font-weight:700;font-size:16px'>{Money(order.TotalAmount)}</td></tr>
    </tfoot>
  </table>

  <div style='margin-top:16px;font-size:14px;color:#555'>
    <div><b>Người nhận:</b> {WebUtility.HtmlEncode(order.CustomerName ?? "")} · {WebUtility.HtmlEncode(order.CustomerPhone ?? "")}</div>
    <div><b>Địa chỉ:</b> {WebUtility.HtmlEncode(order.ShippingAddress)}</div>
    <div><b>Thanh toán:</b> {WebUtility.HtmlEncode(order.PaymentMethod)}</div>
  </div>

  <p style='margin-top:24px;font-size:12px;color:#999'>Email tự động từ hệ thống MOON Fashion. Vui lòng không trả lời email này.</p>
</div>";
        }
    }
}
