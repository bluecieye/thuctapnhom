

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using BaseCore.Entities;

using BaseCore.Services;

namespace BaseCore.APIService.Controllers
{

    // ════════════════════════════════════════════════════════════
    // CONTROLLER ĐỊA CHỈ
    // ════════════════════════════════════════════════════════════
    [ApiController]



    [Route("api/addresses")]

    [Authorize]
    public class AddressesController : ControllerBase
    {

        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly IAddressService _service;

        public AddressesController(IAddressService service) { _service = service; }



        private int CurrentUserId
            => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;



        // ════════════════════════════════════════════════════════════
        // [GET] DANH SÁCH & CHI TIẾT
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetMine() => Ok(await _service.GetByUserAsync(CurrentUserId));



        // ════════════════════════════════════════════════════════════
        // [POST] TẠO MỚI
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Address address)
        {
            
            address.UserId = CurrentUserId;
            return Ok(await _service.CreateAsync(address));
        }

        

        

        // ════════════════════════════════════════════════════════════
        // [PUT] CẬP NHẬT
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Address address)
        {
            
            if (id != address.Id) return BadRequest();
            
            address.UserId = CurrentUserId;
            await _service.UpdateAsync(address);
            return NoContent();
        }

        

        // ════════════════════════════════════════════════════════════
        // [PUT] HÀNH ĐỘNG ĐẶC BIỆT
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}/default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            await _service.SetDefaultAsync(CurrentUserId, id);
            return NoContent();
        }

        

        

        // ════════════════════════════════════════════════════════════
        // [DELETE] XÓA
        // ════════════════════════════════════════════════════════════
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {

            var addr = await _service.GetByIdAsync(id);
            if (addr == null) return NotFound();
            
            if (addr.UserId != CurrentUserId) return Forbid();
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
