using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BaseCore.MyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly string _connectionString;

        public ProductsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = new List<object>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT * FROM Products", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new
                {
                    id = reader["Id"],
                    name = reader["Name"].ToString(),
                    price = reader["Price"],
                    stock = reader["Stock"],
                    imageUrl = reader["ImageUrl"].ToString(),
                    description = reader["Description"].ToString(),
                    categoryId = reader["CategoryId"]
                });
            }
            return Ok(products);
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("SELECT * FROM Products WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return NotFound(new { message = "Product not found" });

            return Ok(new
            {
                id = reader["Id"],
                name = reader["Name"].ToString(),
                price = reader["Price"],
                stock = reader["Stock"],
                imageUrl = reader["ImageUrl"].ToString(),
                description = reader["Description"].ToString(),
                categoryId = reader["CategoryId"]
            });
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductRequest request)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                INSERT INTO Products (Name, Price, Stock, ImageUrl, Description, CategoryId)
                VALUES (@name, @price, @stock, @imageUrl, @description, @categoryId)", conn);
            cmd.Parameters.AddWithValue("@name", request.Name);
            cmd.Parameters.AddWithValue("@price", request.Price);
            cmd.Parameters.AddWithValue("@stock", request.Stock);
            cmd.Parameters.AddWithValue("@imageUrl", request.ImageUrl ?? "");
            cmd.Parameters.AddWithValue("@description", request.Description ?? "");
            cmd.Parameters.AddWithValue("@categoryId", request.CategoryId);
            await cmd.ExecuteNonQueryAsync();
            return Ok(new { message = "Product created successfully" });
        }

        // PUT: api/products/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductRequest request)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"
                UPDATE Products SET 
                Name = @name, Price = @price, Stock = @stock,
                ImageUrl = @imageUrl, Description = @description, CategoryId = @categoryId
                WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", request.Name);
            cmd.Parameters.AddWithValue("@price", request.Price);
            cmd.Parameters.AddWithValue("@stock", request.Stock);
            cmd.Parameters.AddWithValue("@imageUrl", request.ImageUrl ?? "");
            cmd.Parameters.AddWithValue("@description", request.Description ?? "");
            cmd.Parameters.AddWithValue("@categoryId", request.CategoryId);
            await cmd.ExecuteNonQueryAsync();
            return Ok(new { message = "Product updated successfully" });
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand("DELETE FROM Products WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
            return Ok(new { message = "Product deleted successfully" });
        }
    }

    public class ProductRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }
}