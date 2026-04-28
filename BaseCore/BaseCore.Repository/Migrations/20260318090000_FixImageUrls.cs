using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseCore.Repository.Migrations
{
    public partial class FixImageUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=900&q=80' WHERE Id = 1");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1542272604-787c3835535d?auto=format&fit=crop&w=900&q=80' WHERE Id = 4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1594938298603-c8148c4b4545?auto=format&fit=crop&w=900&q=80' WHERE Id = 1");
            migrationBuilder.Sql("UPDATE Products SET ImageUrl = 'https://images.unsplash.com/photo-1523381210434-271e8be8a52f?auto=format&fit=crop&w=900&q=80' WHERE Id = 4");
        }
    }
}
