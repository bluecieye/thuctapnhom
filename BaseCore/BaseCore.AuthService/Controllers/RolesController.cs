

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseCore.AuthService.Controllers
{

    // ════════════════════════════════════════════════════════════
    // MODEL VAI TRÒ
    // ════════════════════════════════════════════════════════════
    public class RoleDto
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public string[] Permissions { get; set; } = System.Array.Empty<string>();
    }

    // ════════════════════════════════════════════════════════════
    // CONTROLLER VAI TRÒ & QUYỀN
    // ════════════════════════════════════════════════════════════
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {

        // ════════════════════════════════════════════════════════════
        // DANH MỤC VAI TRÒ TĨNH
        // ════════════════════════════════════════════════════════════
        private static readonly RoleDto[] _roles = new[]
        {
            
            new RoleDto {
                Name = "Customer",
                Description = "Khách hàng — duyệt, đặt hàng, đánh giá.",
                Permissions = new[] {
                    "products.read","categories.read",
                    "orders.create","orders.read.own","orders.cancel.own",  
                    "cart.manage","wishlist.manage","reviews.create","addresses.manage"
                }
            },

            new RoleDto {
                Name = "WarehouseStaff",
                Description = "Nhân viên kho — kiểm kho, đóng gói, cập nhật vận đơn.",
                Permissions = new[] {
                    "products.read","variants.read","variants.adjust-stock",
                    "orders.read","orders.update-status","inventory.read"
                }
            },

            new RoleDto {
                Name = "Marketing",
                Description = "Marketing — sản phẩm, khuyến mãi, banner.",
                Permissions = new[] {
                    "products.write","categories.write",
                    "coupons.write","banners.write","reviews.moderate"
                }
            },

            new RoleDto {
                Name = "Admin",
                Description = "Quản trị viên — toàn quyền.",
                Permissions = new[] { "*" }
            }
        };

        

        // ════════════════════════════════════════════════════════════
        // [GET] DANH SÁCH VAI TRÒ & QUYỀN
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public IActionResult GetAll() => Ok(_roles);

        
        
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {

            var dto = System.Array.Find(_roles, r => r.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));

            return dto == null ? NotFound() : Ok(dto);
        }

        
        
        [HttpGet("{name}/permissions")]
        public IActionResult GetPermissions(string name)
        {
            var dto = System.Array.Find(_roles, r => r.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));

            return dto == null ? NotFound() : Ok(new { role = dto.Name, permissions = dto.Permissions });
        }
    }
}
