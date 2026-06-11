

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseCore.Entities;
using BaseCore.Services.Authen;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCore.AuthService.Controllers
{

    // ════════════════════════════════════════════════════════════
    // MODEL YÊU CẦU & PHẢN HỒI
    // ════════════════════════════════════════════════════════════
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    
    
    public class CreateUserRequest
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Phone { get; set; }
        public string Role { get; set; } = "Customer";   
    }

    

    public class UpdateUserRequest
    {
        public string? Password { get; set; }   
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }      
    }

    // ════════════════════════════════════════════════════════════
    // CONTROLLER NGƯỜI DÙNG
    // ════════════════════════════════════════════════════════════
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // KHAI BÁO & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly IUserService _userService;

        public UserController(IUserService userService) { _userService = userService; }

        // ════════════════════════════════════════════════════════════
        // HÀM HỖ TRỢ
        // ════════════════════════════════════════════════════════════
        private static UserResponse Map(User u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        };

        

        // ════════════════════════════════════════════════════════════
        // [GET] DANH SÁCH & CHI TIẾT NGƯỜI DÙNG
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,                 
            [FromQuery] int page = 1,                    
            [FromQuery] int pageSize = 10,               
            [FromQuery] string? role = null,             
            [FromQuery] bool? isActive = null)           
        {

            var (users, total) = await _userService.Search(keyword, page, pageSize, role, isActive);

            return Ok(new
            {
                data = users.Select(Map),                                   
                totalCount = total, page, pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize)    
            });
        }

        

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetById(id);
            return user == null ? NotFound() : Ok(Map(user));
        }

        
        
        // ════════════════════════════════════════════════════════════
        // [POST/PUT/DELETE] TẠO - CẬP NHẬT - XOÁ NGƯỜI DÙNG
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Email)
                || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Username, email, password là bắt buộc." });

            try
            {
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Phone = request.Phone ?? "",
                    Role = request.Role     
                };
                var created = await _userService.Create(user, request.Password);

                
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, Map(created));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Tạo user thất bại: " + ex.Message });
            }
        }

        
        
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
        {
            
            var user = await _userService.GetById(id);
            if (user == null) return NotFound();

            
            if (request.Email != null) user.Email = request.Email;
            if (request.Phone != null) user.Phone = request.Phone;
            if (request.Role != null) user.Role = request.Role;
            if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;

            await _userService.Update(user, request.Password);
            return Ok(Map(user));
        }

        
        
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetById(id);
            if (user == null) return NotFound();

            await _userService.Delete(id);
            return NoContent();   
        }
    }
}
