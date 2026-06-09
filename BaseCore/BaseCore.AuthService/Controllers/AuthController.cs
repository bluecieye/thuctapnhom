

using Microsoft.AspNetCore.Mvc;
using BaseCore.Common;              
using BaseCore.Entities;            
using BaseCore.Services.Authen;     
using System.Threading.Tasks;

namespace BaseCore.AuthService.Controllers
{

    // ════════════════════════════════════════════════════════════
    // MODEL YÊU CẦU & PHẢN HỒI
    // ════════════════════════════════════════════════════════════
    public class LoginRequest
    {
        public string Username { get; set; } = "";  
        public string Password { get; set; } = "";
    }

    

    public class LoginResponse
    {
        public string Token { get; set; } = "";     
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";      
        public int ExpiresIn { get; set; }          
    }

    

    public class RegisterRequest
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Phone { get; set; }          
    }

    // ════════════════════════════════════════════════════════════
    // CONTROLLER XÁC THỰC
    // ════════════════════════════════════════════════════════════
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // KHAI BÁO & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly IUserService _userService;

        private readonly IConfiguration _configuration;

        private const int TokenExpirationMinutes = 480;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        

        // ════════════════════════════════════════════════════════════
        // [POST] ĐĂNG NHẬP
        // ════════════════════════════════════════════════════════════
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Username và password là bắt buộc." });

            

            var user = await _userService.Authenticate(request.Username, request.Password);

            
            if (user == null)
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu." });

            var secret = _configuration["Jwt:SecretKey"]
                ?? "YourSecretKeyForAuthenticationShouldBeLongEnough";

            
            var token = TokenHelper.GenerateToken(secret, TokenExpirationMinutes,
                user.Id.ToString(), user.Username, user.Role);

            return Ok(new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresIn = TokenExpirationMinutes * 60   
            });
        }

        
        
        // ════════════════════════════════════════════════════════════
        // [POST] ĐĂNG KÝ
        // ════════════════════════════════════════════════════════════
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Email))
                return BadRequest(new { message = "Username và email là bắt buộc." });

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
                return BadRequest(new { message = "Mật khẩu phải ít nhất 6 ký tự." });

            try
            {

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Phone = request.Phone ?? "",
                    Role = "Customer"
                };

                var created = await _userService.Create(user, request.Password);

                return Ok(new { message = "Đăng ký thành công.", userId = created.Id });
            }
            catch (System.Exception ex)
            {

                return BadRequest(new { message = "Đăng ký thất bại: " + ex.Message });
            }
        }
    }
}
