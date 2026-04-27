using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BaseCore.MyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _secretKey;

        public AuthController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _secretKey = configuration["Jwt:SecretKey"];
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Username and password are required" });

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand(
                "SELECT * FROM Users WHERE UserName = @username AND IsActive = 1", conn);
            cmd.Parameters.AddWithValue("@username", request.Username);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return Unauthorized(new { message = "Invalid username or password" });

            var storedPassword = reader["Password"].ToString();
            var salt = reader["Salt"] as byte[];
            Console.WriteLine($"🔵 Salt length: {salt?.Length}");
            Console.WriteLine($"🔵 Password hash: {storedPassword}");
            var userType = (int)reader["UserType"];
            var userId = reader["Id"].ToString();
            var name = reader["Name"].ToString();
            var email = reader["Email"].ToString();

            // Verify password
           // Thay đoạn verify password cũ bằng này
            bool isValid = false;
            if (salt != null && salt.Length > 0)
            {
                var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: request.Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
                isValid = (hashed == storedPassword);
            }
            else
            {
                isValid = (storedPassword == request.Password);
            }

            if (!isValid)
                return Unauthorized(new { message = "Invalid username or password" });

            // Generate JWT
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, userType == 1 ? "Admin" : "User")
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            return Ok(new
            {
                token,
                userId,
                username = request.Username,
                name,
                email,
                role = userType == 1 ? "Admin" : "User",
                expiresIn = 28800
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}