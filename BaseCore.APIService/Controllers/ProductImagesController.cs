

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using BaseCore.Entities;

using BaseCore.Repository;

using Microsoft.EntityFrameworkCore;

using System.Text.RegularExpressions;

namespace BaseCore.APIService.Controllers
{
    // ════════════════════════════════════════════════════════════
    // CONTROLLER ẢNH SẢN PHẨM
    // ════════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/product-images")]

    public class ProductImagesController : ControllerBase
    {
        // ════════════════════════════════════════════════════════════
        // BIẾN & HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        private readonly MySqlDbContext _context;

        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public ProductImagesController(MySqlDbContext context, IConfiguration config, IWebHostEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".avif", ".gif" };

        private string GetMediaRoot()
        {
            var root = _config["Media:Root"] ?? "..\\Media";
            if (!Path.IsPathRooted(root))
                root = Path.GetFullPath(Path.Combine(_env.ContentRootPath, root));
            return root;
        }

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // LẤY DANH SÁCH / CHI TIẾT (GET)
        // ════════════════════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "Admin,Marketing,Warehouse")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? productId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            
            var query = _context.ProductImages
                .Include(i => i.Product)
                .AsQueryable();
            
            if (productId.HasValue)
                query = query.Where(i => i.ProductId == productId.Value);
            
            var total = await query.CountAsync();
            var items = await query

                
                .OrderByDescending(i => i.Id)
                
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();
            return Ok(new { items, total, page, pageSize, totalPages = (int)Math.Ceiling((double)total / pageSize) });
        }

        

        
        
        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var images = await _context.ProductImages
                .Where(i => i.ProductId == productId)
                .OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id)
                .ToListAsync();
            return Ok(images);
        }

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // TẠO MỚI (POST)
        // ════════════════════════════════════════════════════════════
        [HttpPost]
        [Authorize(Roles = "Admin,Marketing,Warehouse")]
        public async Task<IActionResult> Create([FromBody] ProductImage image)
        {
            if (image.IsPrimary)
            {
                
                var others = await _context.ProductImages
                    .Where(i => i.ProductId == image.ProductId && i.IsPrimary)
                    .ToListAsync();
                
                others.ForEach(i => i.IsPrimary = false);
            }
            
            _context.ProductImages.Add(image);
            
            await _context.SaveChangesAsync();
            return Ok(image);
        }

        

        

        

        

        

        
        
        // ════════════════════════════════════════════════════════════
        // TẢI LÊN FILE ẢNH
        // ════════════════════════════════════════════════════════════
        [HttpPost("upload")]
        [Authorize(Roles = "Admin,Marketing,Warehouse")]
        [RequestSizeLimit(20_000_000)]  
        public async Task<IActionResult> Upload(
            [FromForm] IFormFile? file,
            [FromForm] int productId,
            [FromForm] int colorId,
            [FromForm] bool isPrimary = false,
            [FromForm] int displayOrder = 0)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Chưa chọn file ảnh." });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new { message = $"Định dạng không hỗ trợ. Cho phép: {string.Join(", ", AllowedExtensions)}" });

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return BadRequest(new { message = "Sản phẩm không tồn tại." });
            if (string.IsNullOrWhiteSpace(product.Slug))
                return BadRequest(new { message = "Sản phẩm chưa có slug. Hãy cập nhật slug trước khi upload ảnh." });

            var savedFileName = await SaveUploadedFileAsync(file, product.Slug, ext);

            if (isPrimary)
            {
                var others = await _context.ProductImages
                    .Where(i => i.ProductId == productId && i.IsPrimary)
                    .ToListAsync();
                others.ForEach(i => i.IsPrimary = false);
            }

            var image = new ProductImage
            {
                ProductId = productId,
                ColorId = colorId,
                IsPrimary = isPrimary,
                DisplayOrder = displayOrder,
                FileName = savedFileName,
            };
            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
            return Ok(image);
        }

        

        
        
        [HttpPut("{id:int}/upload")]
        [Authorize(Roles = "Admin,Marketing,Warehouse")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> ReplaceFile(
            int id,
            [FromForm] IFormFile? file,
            [FromForm] int colorId,
            [FromForm] bool isPrimary = false,
            [FromForm] int displayOrder = 0)
        {
            var existing = await _context.ProductImages.FindAsync(id);
            if (existing == null) return NotFound();
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Chưa chọn file ảnh." });

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new { message = $"Định dạng không hỗ trợ. Cho phép: {string.Join(", ", AllowedExtensions)}" });

            var product = await _context.Products.FindAsync(existing.ProductId);
            if (product == null || string.IsNullOrWhiteSpace(product.Slug))
                return BadRequest(new { message = "Sản phẩm không tồn tại hoặc chưa có slug." });

            var savedFileName = await SaveUploadedFileAsync(file, product.Slug, ext);

            if (isPrimary && !existing.IsPrimary)
            {
                var others = await _context.ProductImages
                    .Where(i => i.ProductId == existing.ProductId && i.IsPrimary && i.Id != id)
                    .ToListAsync();
                others.ForEach(i => i.IsPrimary = false);
            }

            existing.FileName = savedFileName;
            existing.ColorId = colorId;
            existing.IsPrimary = isPrimary;
            existing.DisplayOrder = displayOrder;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // ════════════════════════════════════════════════════════════
        // HÀM PHỤ TRỢ
        // ════════════════════════════════════════════════════════════
        private async Task<string> SaveUploadedFileAsync(IFormFile file, string slug, string ext)
        {
            var folder = Path.Combine(GetMediaRoot(), "products", slug);
            Directory.CreateDirectory(folder);

            
            var rawName = Path.GetFileNameWithoutExtension(file.FileName) ?? string.Empty;
            var safeBase = Regex.Replace(rawName, "[^a-zA-Z0-9_-]+", "-").Trim('-').ToLowerInvariant();
            if (string.IsNullOrEmpty(safeBase)) safeBase = "image";

            var fileName = safeBase + ext;
            var fullPath = Path.Combine(folder, fileName);
            int counter = 1;
            while (System.IO.File.Exists(fullPath))
            {
                fileName = $"{safeBase}-{counter}{ext}";
                fullPath = Path.Combine(folder, fileName);
                counter++;
            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }

        

        

        
        // ════════════════════════════════════════════════════════════
        // CẬP NHẬT (PUT)
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Marketing,Warehouse")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductImage image)
        {
            
            var existing = await _context.ProductImages.FindAsync(id);
            if (existing == null) return NotFound();
            
            if (image.IsPrimary && !existing.IsPrimary)
            {
                var others = await _context.ProductImages
                    
                    .Where(i => i.ProductId == existing.ProductId && i.IsPrimary && i.Id != id)
                    .ToListAsync();
                others.ForEach(i => i.IsPrimary = false);
            }
            
            existing.FileName = image.FileName;
            existing.ColorId = image.ColorId;
            existing.IsPrimary = image.IsPrimary;
            existing.DisplayOrder = image.DisplayOrder;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        

        

        // ════════════════════════════════════════════════════════════
        // XÓA (DELETE)
        // ════════════════════════════════════════════════════════════
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Marketing,Warehouse")]
        public async Task<IActionResult> Delete(int id)
        {
            var image = await _context.ProductImages.FindAsync(id);
            if (image == null) return NotFound();
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        

        

        // ════════════════════════════════════════════════════════════
        // ĐẶT ẢNH ĐẠI DIỆN
        // ════════════════════════════════════════════════════════════
        [HttpPut("{id:int}/set-primary")]
        [Authorize(Roles = "Admin,Marketing,Warehouse")]
        public async Task<IActionResult> SetPrimary(int id)
        {
            var image = await _context.ProductImages.FindAsync(id);
            if (image == null) return NotFound();
            
            var all = await _context.ProductImages
                .Where(i => i.ProductId == image.ProductId)
                .ToListAsync();
            
            all.ForEach(i => i.IsPrimary = i.Id == id);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
