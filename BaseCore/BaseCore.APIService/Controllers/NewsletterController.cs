using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;
using BaseCore.Repository;
using BaseCore.Services;

namespace BaseCore.APIService.Controllers
{
    [ApiController]
    [Route("api/newsletter")]
    public class NewsletterController : ControllerBase
    {
        private readonly MySqlDbContext _context;
        private readonly IEmailService _emailService;

        public NewsletterController(MySqlDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("subscribe")]
        [AllowAnonymous]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Vui lòng nhập email." });

            var email = dto.Email.Trim().ToLower();

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                return BadRequest(new { message = "Email không hợp lệ." });

            var already = await _context.NewsletterSubscribers.AnyAsync(s => s.Email == email);
            if (already)
                return Ok(new { message = "Email này đã được đăng ký trước đó.", alreadySubscribed = true });

            _context.NewsletterSubscribers.Add(new NewsletterSubscriber
            {
                Email = email,
                SubscribedAt = DateTime.UtcNow,
            });

            var welcome = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == "WELCOME10");
            if (welcome != null) welcome.UsageLimit += 1;

            await _context.SaveChangesAsync();

            // Gửi email chào mừng — lỗi không làm hỏng đăng ký.
            try
            {
                await _emailService.SendNewsletterWelcomeAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ERROR] Newsletter welcome thất bại cho {email}: {ex.Message}");
            }

            return Ok(new { message = "Đăng ký thành công! Mã giảm giá 10% đã được gửi tới email của bạn.", alreadySubscribed = false });
        }
    }

    public class SubscribeDto
    {
        public string Email { get; set; } = "";
    }
}
