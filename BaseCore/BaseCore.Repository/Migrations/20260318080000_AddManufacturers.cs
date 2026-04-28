using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace BaseCore.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddManufacturers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "Name", "Country", "Description", "Website" },
                values: new object[,]
                {
                    { 1,  "FormAsh",     "Vietnam",     "Contemporary fashion label known for clean, structured pieces.", null },
                    { 2,  "Urban Chill", "USA",         "Streetwear brand focused on comfortable, everyday essentials.", null },
                    { 3,  "StreetStep",  "South Korea", "Minimalist footwear label built for urban environments.", null },
                    { 4,  "BlueAura",    "Japan",       "Premium denim and outerwear manufacturer.", null },
                    { 5,  "LoomLyfe",    "Vietnam",     "Handcrafted accessories made from sustainable natural materials.", null },
                    { 6,  "Denim House", "USA",         "Dedicated denim label with a focus on fit and durability.", null },
                    { 7,  "Rainform",    "UK",          "Technical outerwear brand built for city rain.", null },
                    { 8,  "CalmThread",  "Vietnam",     "Basics brand focused on premium fabric and minimal design.", null },
                    { 9,  "Pace",        "USA",         "Footwear brand for active urban lifestyles.", null },
                    { 10, "BuckleCo",    "Italy",       "Leather accessories crafted with Italian hardware.", null }
                });

            migrationBuilder.AddColumn<int>(
                name: "ManufacturerId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ManufacturerId",
                table: "Products",
                column: "ManufacturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Manufacturers_ManufacturerId",
                table: "Products",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Link existing products to their manufacturers
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 1  WHERE Id = 1");
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 2  WHERE Id = 2");
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 3  WHERE Id = 3");
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 4  WHERE Id = 4");
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 5  WHERE Id = 5");
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 6  WHERE Id = 6");
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 7  WHERE Id = 7");
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 8  WHERE Id = 8");
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 9  WHERE Id = 9");
            migrationBuilder.Sql("UPDATE Products SET ManufacturerId = 10 WHERE Id = 10");

            // Fix image URLs for any database that still has the old broken Unsplash IDs
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1594938298603-c8148c4b4545?auto=format&fit=crop&w=900&q=80' WHERE Id = 1");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1576566588028-4147f3842f27?auto=format&fit=crop&w=900&q=80' WHERE Id = 2");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=900&q=80' WHERE Id = 3");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1523381210434-271e8be8a52f?auto=format&fit=crop&w=900&q=80' WHERE Id = 4");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1548036328-c9fa89d128fa?auto=format&fit=crop&w=900&q=80' WHERE Id = 5");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1541099649105-f69ad21f3246?auto=format&fit=crop&w=900&q=80' WHERE Id = 6");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1539533018447-63fcce2678e3?auto=format&fit=crop&w=900&q=80' WHERE Id = 7");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?auto=format&fit=crop&w=900&q=80' WHERE Id = 8");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1491553895911-0055eca6402d?auto=format&fit=crop&w=900&q=80' WHERE Id = 9");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?auto=format&fit=crop&w=900&q=80' WHERE Id = 10");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Manufacturers_ManufacturerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ManufacturerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
