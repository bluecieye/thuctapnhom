using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseCore.Entities;
using BaseCore.Repository.EFCore;

namespace BaseCore.APIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerRepositoryEF _manufacturerRepository;

        public ManufacturersController(IManufacturerRepositoryEF manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var manufacturers = await _manufacturerRepository.GetAllAsync();
            return Ok(manufacturers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
            if (manufacturer == null)
                return NotFound(new { message = "Manufacturer not found" });

            return Ok(manufacturer);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ManufacturerDto dto)
        {
            var existing = await _manufacturerRepository.GetByNameAsync(dto.Name);
            if (existing != null)
                return BadRequest(new { message = "Manufacturer name already exists" });

            var manufacturer = new Manufacturer
            {
                Name = dto.Name,
                Country = dto.Country,
                Description = dto.Description,
                Website = dto.Website
            };

            await _manufacturerRepository.AddAsync(manufacturer);
            return CreatedAtAction(nameof(GetById), new { id = manufacturer.Id }, manufacturer);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] ManufacturerDto dto)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
            if (manufacturer == null)
                return NotFound(new { message = "Manufacturer not found" });

            manufacturer.Name = dto.Name ?? manufacturer.Name;
            manufacturer.Country = dto.Country ?? manufacturer.Country;
            manufacturer.Description = dto.Description ?? manufacturer.Description;
            manufacturer.Website = dto.Website ?? manufacturer.Website;

            await _manufacturerRepository.UpdateAsync(manufacturer);
            return Ok(manufacturer);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
            if (manufacturer == null)
                return NotFound(new { message = "Manufacturer not found" });

            await _manufacturerRepository.DeleteAsync(manufacturer);
            return Ok(new { message = "Manufacturer deleted successfully" });
        }
    }

    public class ManufacturerDto
    {
        public string Name { get; set; } = "";
        public string? Country { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
    }
}
