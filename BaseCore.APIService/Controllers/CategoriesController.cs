

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BaseCore.Entities;

using BaseCore.Services;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER DANH MỤC
    // ════════════════════════════════════════════════════════════
    [ApiController]

    [Route("api/categories")]

    public class CategoriesController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service) { _service = service; }



        // ════════════════════════════════════════════════════════════
        // [GET] DANH SÁCH & CHI TIẾT
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

        

        
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _service.GetByIdAsync(id);
            
            return c == null ? NotFound() : Ok(c);
        }



        // ════════════════════════════════════════════════════════════
        // [POST] TẠO MỚI
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            var saved = await _service.CreateAsync(category);
            
            return CreatedAtAction(nameof(GetById), new { id = saved.Id }, saved);
        }

        

        
        
        // ════════════════════════════════════════════════════════════
        // [PUT] CẬP NHẬT
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Marketing")]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            
            if (id != category.Id) return BadRequest();
            await _service.UpdateAsync(category);
            return NoContent();
        }

        

        
        // ════════════════════════════════════════════════════════════
        // [DELETE] XÓA
        // ════════════════════════════════════════════════════════════
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
