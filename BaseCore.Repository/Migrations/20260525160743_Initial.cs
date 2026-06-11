using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BaseCore.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Season = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HexCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Percent"),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: false),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingCarriers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LogoFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingCarriers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Customer"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarrierId = table.Column<int>(type: "int", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRates_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingRates_ShippingCarriers_CarrierId",
                        column: x => x.CarrierId,
                        principalTable: "ShippingCarriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SizeGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chest = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Waist = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Length = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Shoulder = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SizeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SizeGuides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SizeGuides_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Ward = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "COD"),
                    ShippingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedDelivery = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CouponId = table.Column<int>(type: "int", nullable: true),
                    ShippingCarrierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_ShippingCarriers_ShippingCarrierId",
                        column: x => x.ShippingCarrierId,
                        principalTable: "ShippingCarriers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ReservedStock = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    SizeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SizeAccuracy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    VariantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    VariantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_ProductVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Gender", "Name", "Season" },
                values: new object[,]
                {
                    { 1, "Male", "Áo khoác Nam", "Fall-Winter" },
                    { 2, "Male", "Áo Nam", "All" },
                    { 3, "Male", "Quần dài Nam", "All" },
                    { 4, "Male", "Quần short Nam", "Summer" },
                    { 5, "Female", "Áo Nữ", "All" },
                    { 6, "Female", "Quần dài Nữ", "All" },
                    { 7, "Female", "Quần short Nữ", "Summer" },
                    { 8, "Female", "Áo khoác Nữ", "Fall-Winter" },
                    { 9, "Female", "Váy Nữ", "All" },
                    { 10, "Female", "Chân váy Nữ", "All" }
                });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "Id", "Code", "HexCode", "Name" },
                values: new object[,]
                {
                    { 1, "DEN", "#000000", "Đen" },
                    { 2, "TRG", "#FFFFFF", "Trắng" },
                    { 3, "TKM", "#FFFDD0", "Trắng kem" },
                    { 4, "XNH", "#D3D3D3", "Xám nhạt" },
                    { 5, "XAM", "#808080", "Xám" },
                    { 6, "XDM", "#404040", "Xám đậm" },
                    { 7, "TCH", "#36454F", "Than chì" },
                    { 8, "KEM", "#F5F5DC", "Kem" },
                    { 9, "BE", "#D8C9A8", "Be" },
                    { 10, "NUD", "#E3BC9A", "Nude" },
                    { 11, "CML", "#C19A6B", "Camel" },
                    { 12, "KHK", "#C3B091", "Khaki" },
                    { 13, "NNH", "#A0826D", "Nâu nhạt" },
                    { 14, "NAU", "#8B4513", "Nâu" },
                    { 15, "NDM", "#5D3A1A", "Nâu đậm" },
                    { 16, "SCL", "#7B3F00", "Socola" },
                    { 17, "DTU", "#FF0000", "Đỏ tươi" },
                    { 18, "DO", "#E74C3C", "Đỏ" },
                    { 19, "DGA", "#B22222", "Đỏ gạch" },
                    { 20, "DDO", "#800020", "Đỏ đô" },
                    { 21, "HNH", "#FFB6C1", "Hồng nhạt" },
                    { 22, "HPH", "#F4C2C2", "Hồng phấn" },
                    { 23, "HNG", "#FF69B4", "Hồng" },
                    { 24, "HCS", "#E91E63", "Hồng cánh sen" },
                    { 25, "CDA", "#FFCBA4", "Cam đào" },
                    { 26, "CAM", "#FF7F50", "Cam" },
                    { 27, "VNH", "#FFFACD", "Vàng nhạt" },
                    { 28, "VAN", "#FFD700", "Vàng" },
                    { 29, "VMT", "#D2A007", "Vàng mù tạt" },
                    { 30, "XMT", "#98FF98", "Xanh mint" },
                    { 31, "XLA", "#2ECC71", "Xanh lá" },
                    { 32, "XOL", "#808000", "Xanh olive" },
                    { 33, "XRE", "#556B2F", "Xanh rêu" },
                    { 34, "XDT", "#87CEEB", "Xanh da trời" },
                    { 35, "XDN", "#1560BD", "Xanh denim" },
                    { 36, "XDG", "#0066CC", "Xanh dương" },
                    { 37, "XNV", "#1B2A4E", "Xanh navy" },
                    { 38, "XNC", "#0ABAB5", "Xanh ngọc" },
                    { 39, "TLV", "#E6E6FA", "Tím lavender" },
                    { 40, "TIM", "#9B59B6", "Tím" }
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "DiscountType", "DiscountValue", "EndDate", "IsActive", "MaxDiscount", "MinOrderAmount", "StartDate", "UsageLimit", "UsedCount" },
                values: new object[] { 1, "AOPHONG", "Percent", 10.00m, new DateTime(2026, 5, 23, 2, 43, 0, 0, DateTimeKind.Utc), true, 100000.00m, 1000000.00m, new DateTime(2026, 5, 18, 2, 42, 0, 0, DateTimeKind.Utc), 5, 1 });

            migrationBuilder.InsertData(
                table: "Provinces",
                columns: new[] { "Id", "Code", "Name", "Region" },
                values: new object[,]
                {
                    { 1, "HN", "Hà Nội", "Bắc" },
                    { 2, "HCM", "Hồ Chí Minh", "Nam" },
                    { 3, "HP", "Hải Phòng", "Bắc" },
                    { 4, "DN", "Đà Nẵng", "Trung" },
                    { 5, "CT", "Cần Thơ", "Nam" },
                    { 6, "HUE", "Huế", "Trung" },
                    { 7, "LCH", "Lai Châu", "Bắc" },
                    { 8, "DB", "Điện Biên", "Bắc" },
                    { 9, "SL", "Sơn La", "Bắc" },
                    { 10, "LS", "Lạng Sơn", "Bắc" },
                    { 11, "CB", "Cao Bằng", "Bắc" },
                    { 12, "TQ", "Tuyên Quang", "Bắc" },
                    { 13, "LC", "Lào Cai", "Bắc" },
                    { 14, "TN", "Thái Nguyên", "Bắc" },
                    { 15, "PT", "Phú Thọ", "Bắc" },
                    { 16, "BN", "Bắc Ninh", "Bắc" },
                    { 17, "HY", "Hưng Yên", "Bắc" },
                    { 18, "NB", "Ninh Bình", "Bắc" },
                    { 19, "QN", "Quảng Ninh", "Bắc" },
                    { 20, "TH", "Thanh Hóa", "Bắc" },
                    { 21, "NA", "Nghệ An", "Trung" },
                    { 22, "HT", "Hà Tĩnh", "Trung" },
                    { 23, "QT", "Quảng Trị", "Trung" },
                    { 24, "QNG", "Quảng Ngãi", "Trung" },
                    { 25, "GL", "Gia Lai", "Trung" },
                    { 26, "KH", "Khánh Hòa", "Trung" },
                    { 27, "LD", "Lâm Đồng", "Trung" },
                    { 28, "DL", "Đắk Lắk", "Trung" },
                    { 29, "DNA", "Đồng Nai", "Nam" },
                    { 30, "TNI", "Tây Ninh", "Nam" },
                    { 31, "VL", "Vĩnh Long", "Nam" },
                    { 32, "DT", "Đồng Tháp", "Nam" },
                    { 33, "CM", "Cà Mau", "Nam" },
                    { 34, "AG", "An Giang", "Nam" }
                });

            migrationBuilder.InsertData(
                table: "ShippingCarriers",
                columns: new[] { "Id", "Code", "IsActive", "LogoFileName", "Name" },
                values: new object[,]
                {
                    { 1, "GHN", true, "ghn.png", "Giao Hàng Nhanh" },
                    { 2, "GHTK", true, "ghtk.png", "Giao Hàng Tiết Kiệm" },
                    { 3, "VTP", true, "vtp.png", "Viettel Post" }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "Id", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "XS", 10 },
                    { 2, "S", 20 },
                    { 3, "M", 30 },
                    { 4, "L", 40 },
                    { 5, "XL", 50 },
                    { 6, "2XL", 60 },
                    { 7, "3XL", 70 },
                    { 8, "4XL", 80 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "PasswordHash", "Phone", "Role", "Salt", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 14, 16, 38, 50, 351, DateTimeKind.Utc), "admin@basecore.com", true, "0OZgMfYMryrBVtxJSbwo99nZnU7nMbomE3lJE6D90Fk=", "", "Admin", "S2lUlFCjGhgFF3e3273/ig==", "admin" },
                    { 2, new DateTime(2026, 5, 18, 2, 56, 45, 923, DateTimeKind.Utc), "gaga@gmail.com", true, "DZwp/omksPF5eut49Dqqu6Q+IJDu9JBtADlFrUGy98w=", "", "WarehouseStaff", "Pc+b7h/O6GwB4uU8f7jffw==", "hoang" },
                    { 3, new DateTime(2026, 5, 18, 2, 57, 55, 145, DateTimeKind.Utc), "123@1.com", true, "LK71lL5grGQ60TojQGRpG5U8nLk+44+05XLKYW9hqBs=", "", "Marketing", "46r/zfx9TSZYtwLb8oHw2g==", "nhat" },
                    { 4, new DateTime(2026, 5, 18, 2, 59, 20, 502, DateTimeKind.Utc), "user@gmail.com", true, "kpn72D1LswdgaQAZ36E1rDMtviDrd++VKN3Gw8hjbdQ=", "", "Customer", "5VUKC78BYlplNthDspI0Ig==", "user" }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 18, 3, 55, 21, 154, DateTimeKind.Utc), 1 },
                    { 2, new DateTime(2026, 5, 18, 3, 54, 32, 242, DateTimeKind.Utc), 4 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CouponId", "CreatedAt", "DiscountAmount", "EstimatedDelivery", "Note", "PaymentMethod", "ShippingAddress", "ShippingCarrierId", "ShippingFee", "Status", "TotalAmount", "UserId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 5, 18, 2, 42, 6, 37, DateTimeKind.Utc), 0.00m, new DateTime(2026, 5, 20, 2, 42, 6, 38, DateTimeKind.Utc), "aa", "COD", "a, b, c, d, e, Tuyên Quang", 3, 28000.00m, "Delivered", 418000.00m, 1 },
                    { 2, 1, new DateTime(2026, 5, 18, 2, 44, 5, 506, DateTimeKind.Utc), 100000.00m, new DateTime(2026, 5, 20, 2, 44, 5, 506, DateTimeKind.Utc), "ngon\n", "COD", "a, b, c, d, e, Tuyên Quang", 2, 0.00m, "Cancelled", 1080000.00m, 1 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BasePrice", "CategoryId", "CreatedAt", "Description", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, 290000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 500, DateTimeKind.Utc), "Áo ba lỗ nam cotton rib co giãn, form ôm vừa, thoáng mát, phù hợp tập gym hoặc mặc hàng ngày.", "Áo Ba Lỗ Nam", "ao-ba-lo-nam" },
                    { 2, 290000.00m, 5, new DateTime(2026, 5, 14, 16, 44, 8, 503, DateTimeKind.Utc), "Áo ba lỗ nữ cotton rib mềm mại, form ôm vừa, phù hợp mặc trong hoặc layer.", "Áo Ba Lỗ Nữ", "ao-ba-lo-nu" },
                    { 3, 690000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 503, DateTimeKind.Utc), "Áo gi lê nam vải tweed cao cấp, form slim, phù hợp mặc công sở hoặc dạo phố.", "Áo Gi Lê Nam", "ao-gi-le-nam" },
                    { 4, 650000.00m, 5, new DateTime(2026, 5, 14, 16, 44, 8, 503, DateTimeKind.Utc), "Áo gi lê nữ thiết kế hiện đại, không tay, layer cùng áo sơ mi hoặc áo thun.", "Áo Gi Lê Nữ", "ao-gi-le-nu" },
                    { 5, 890000.00m, 8, new DateTime(2026, 5, 14, 16, 44, 8, 506, DateTimeKind.Utc), "Blazer denim nữ oversized, vải denim mềm 11oz, phong cách casual-chic.", "Áo Khoác Blazer Denim Nữ", "ao-khoac-blazer-denim-nu" },
                    { 6, 790000.00m, 1, new DateTime(2026, 5, 14, 16, 44, 8, 506, DateTimeKind.Utc), "Áo khoác bomber nam chất liệu polyester nhẹ, bo gấu rib co giãn, phong cách street.", "Áo Khoác Bomber Nam", "ao-khoac-bomber-nam" },
                    { 7, 750000.00m, 8, new DateTime(2026, 5, 14, 16, 44, 8, 506, DateTimeKind.Utc), "Áo khoác cotton nữ mỏng nhẹ, không lót, phù hợp mùa xuân hè, dáng rộng thoải mái.", "Áo Khoác Cotton Nữ", "ao-khoac-cotton-nu" },
                    { 8, 790000.00m, 8, new DateTime(2026, 5, 14, 16, 44, 8, 506, DateTimeKind.Utc), "Áo khoác jean nữ dáng crop, vải denim 12oz wash nhẹ, phom casual everyday.", "Áo Khoác Denim Nữ", "ao-khoac-denim-nu" },
                    { 9, 390000.00m, 5, new DateTime(2026, 5, 15, 8, 38, 41, 532, DateTimeKind.Utc), "Áo kiểu cổ đổ nữ vải cotton mềm, form suông thanh lịch, phù hợp đi làm và dạo phố.", "Áo Cổ Đổ Nữ", "ao-co-do-nu" },
                    { 10, 890000.00m, 1, new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), "Áo khoác linen nữ nhẹ thoáng, dáng rộng, cài cúc 3 hạt, thích hợp mùa hè.", "Áo Khoác Linen Nam", "ao-khoac-linen-nam" },
                    { 11, 690000.00m, 8, new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), "Áo khoác thể thao nữ chất liệu polyester co giãn 4 chiều, thấm hút tốt.", "Áo Khoác Thể Thao Nữ", "ao-khoac-the-thao-nu" },
                    { 12, 1290000.00m, 1, new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), "Áo khoác tuxedo nam vải polyester bóng, ve áo lapel satin, phong cách dự tiệc.", "Áo Khoác Tuxedo Nam", "ao-khoac-tuxedo-nam" },
                    { 13, 350000.00m, 5, new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), "Áo kiểu nữ cổ tròn viền ren điểm nhún, vải chiffon nhẹ bay, thanh lịch.", "Áo Kiểu Cổ Điểm Nhún Nữ", "ao-kieu-co-diem-nhun-nu" },
                    { 14, 490000.00m, 5, new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), "Áo blouse nữ pin tucked cổ đứng, vải cotton poplin, form suông chỉn chu.", "Áo Pin Tucked Blouse Nữ", "ao-pin-tucked-blouse-nu" },
                    { 15, 420000.00m, 5, new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), "Áo sơ mi chiffon trong suốt thanh lịch, tay dài, cổ V nhẹ nhàng, phù hợp đi làm.", "Áo Sơ Mi Chiffon Nữ", "ao-so-mi-chiffon-nu" },
                    { 16, 490000.00m, 5, new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), "Áo sơ mi nữ cổ ve đôi thanh lịch, vải lụa matte mềm rủ, phù hợp công sở.", "Áo Sơ Mi Cổ 2 Ve Nữ", "ao-so-mi-co-2-ve-nu" },
                    { 17, 450000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), "Áo sơ mi form rộng mặc khoác, vải cotton nhẹ, phong cách unisex casual.", "Áo Sơ Mi Khoác Unisex", "ao-so-mi-khoac" },
                    { 18, 550000.00m, 5, new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), "Áo sơ mi linen nữ thoáng mát, tay dài gập xắn được, phù hợp mùa hè và du lịch.", "Áo Sơ Mi Linen Nữ", "ao-so-mi-linen-nu" },
                    { 19, 380000.00m, 5, new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), "Áo sơ mi nữ thắt nơ buộc eo, cổ V nhẹ, vải voile mỏng nhẹ bay bướm.", "Áo Sơ Mi Nữ Buộc Dây", "ao-so-mi-nu-buoc-day" },
                    { 20, 290000.00m, 5, new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), "Áo thun nữ cotton 200gsm, cổ tròn, form boxy nhẹ nhàng, co giãn 2 chiều.", "Áo Thun Nữ", "ao-thun-nu" },
                    { 21, 1590000.00m, 8, new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), "Trench coat nữ double-breasted, dây thắt eo, vải gabardine cotton, dáng dài thanh lịch.", "Áo Trench Coat Nữ", "ao-trench-coat-nu" },
                    { 22, 490000.00m, 10, new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), "Chân váy đắp chéo bất đối xứng, vải lụa matte rủ đẹp, tôn dáng.", "Chân Váy Đắp Chéo Nữ", "chan-vay-dap-cheo-nu" },
                    { 23, 390000.00m, 10, new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), "Chân váy jersey co giãn 2 chiều, cạp chun thoải mái, thích hợp casual đến đi làm.", "Chân Váy Jersey Nữ", "chan-vay-jersey-nu" },
                    { 24, 490000.00m, 10, new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), "Chân váy maxi dài sát gót, vải thun co giãn, dáng ôm thon gọn tôn dáng.", "Chân Váy Maxi Nữ", "chan-vay-maxi-nu" },
                    { 25, 450000.00m, 10, new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), "Chân váy xếp ly rủ nhẹ nhàng, lưng thun, vải chiffon mềm bay, mang nét nữ tính.", "Chân Váy Xếp Rủ Nữ", "chan-vay-xep-ru-nu" },
                    { 26, 590000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), "Áo hoodie không tay (vest hoodie) nỉ bông, mũ 2 lớp, túi kangaroo, phong cách streetwear.", "Hoodie Không Tay Nam", "hoodie-khong-tay-nam" },
                    { 27, 590000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), "Hoodie nỉ bông dày 380gsm, mũ 2 lớp, túi kangaroo, form oversize thoải mái.", "Hoodie Nỉ Nam", "hoodie-ni-nam" },
                    { 28, 650000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), "Hoodie zip-up nỉ dày, khóa kéo YKK chắc chắn, tay rib, phù hợp lớp ngoài mùa lạnh.", "Hoodie Khóa Kéo Nam", "hoodie-khoa-keo-nam" },
                    { 29, 550000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), "Áo jumper len mỏng cổ tròn, form suông nhẹ, layer tiện dụng mùa thu đông.", "Jumper Nam", "jumper-nam" },
                    { 30, 490000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), "Áo polo nam cotton piqué 220gsm, cổ bẻ rib 3 cúc, form regular fit lịch sự.", "Polo Nam", "polo-nam" },
                    { 31, 450000.00m, 6, new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), "Quần dài nữ cạp chun co giãn, vải thun lạnh mịn, form suông thoải mái.", "Quần Dài Cạp Chun Nữ", "quan-dai-cap-chun-nu" },
                    { 32, 490000.00m, 6, new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), "Quần dài nam dây rút điều chỉnh eo, vải thun gió nhẹ, 2 túi sườn tiện dụng.", "Quần Dài Dây Rút Nữ", "quan-dai-day-rut-nu" },
                    { 33, 590000.00m, 6, new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), "Quần satin nữ bóng mượt, cạp chun ẩn, form suông thanh lịch, thích hợp đi làm và dạ tiệc.", "Quần Dài Satin Nữ", "quan-dai-satin-nu" },
                    { 34, 690000.00m, 3, new DateTime(2026, 5, 14, 16, 44, 8, 523, DateTimeKind.Utc), "Quần jean nam denim 12oz, phom slim tapered, wash nhẹ tự nhiên, 5 túi chuẩn.", "Quần Jean Nam", "quan-jean-nam" },
                    { 35, 650000.00m, 6, new DateTime(2026, 5, 14, 16, 44, 8, 523, DateTimeKind.Utc), "Quần jean nữ lưng cao, denim co giãn 2 chiều, phom skinny tôn dáng.", "Quần Jean Nữ", "quan-jean-nu" },
                    { 36, 690000.00m, 6, new DateTime(2026, 5, 14, 16, 44, 8, 526, DateTimeKind.Utc), "Quần jean nữ ripped knee, lưng cao, dáng skinny năng động phong cách.", "Quần Jean Nữ Rách Gối", "quan-jean-nu-rach-goi" },
                    { 37, 490000.00m, 3, new DateTime(2026, 5, 14, 16, 44, 8, 526, DateTimeKind.Utc), "Quần jogger nam cotton french terry, gấu bo rib, túi zip tiện dụng, mặc gym hoặc casual.", "Quần Jogger Nam", "quan-jogger-nam" },
                    { 38, 590000.00m, 6, new DateTime(2026, 5, 14, 16, 44, 8, 526, DateTimeKind.Utc), "Quần ống suông vải muslin cao cấp, nhẹ bay tự nhiên, cạp chun ẩn, phong cách boho thanh lịch.", "Quần Ống Sương Muslin Nữ", "quan-ong-suong-muslin-nu" },
                    { 39, 450000.00m, 4, new DateTime(2026, 5, 14, 16, 44, 8, 526, DateTimeKind.Utc), "Quần short jean nam denim 10oz, gấu xước nhẹ, phom regular, túi hộp tiện dụng.", "Quần Short Jean Nam", "quan-short-jean-nam" },
                    { 40, 390000.00m, 4, new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), "Quần short nỉ bông dày, cạp thun lưng, túi zip, mặc tại nhà hoặc gym thoải mái.", "Quần Short Nỉ Nam", "quan-short-ni-nam" },
                    { 41, 490000.00m, 4, new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), "Quần short nam 4 túi hộp kiểu cargo, vải kaki dày dặn, phong cách outdoor.", "Quần Short Túi Hộp Nam", "quan-short-tui-hop-nam" },
                    { 42, 690000.00m, 3, new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), "Quần tây nam vải polyester blend, form slim straight, li phẳng, thích hợp đi làm và sự kiện.", "Quần Tây Nam", "quan-tay-nam" },
                    { 43, 690000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), "Sơ mi chống nhăn vải cotton pha polyester, giữ phẳng cả ngày, phù hợp đi làm.", "Sơ Mi Chống Nhăn Nam", "so-mi-chong-nhan-nam" },
                    { 44, 590000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), "Sơ mi cotton 100% thoáng mát, form regular, cổ button-down, phù hợp mặc quanh năm.", "Sơ Mi Cotton Nam", "so-mi-cotton-nam" },
                    { 45, 590000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), "Áo sơ mi jean denim nhẹ 8oz, cổ đứng button-down, style western casual.", "Sơ Mi Jean Nam", "so-mi-jean-nam" },
                    { 46, 590000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), "Sweater len mịn cổ tròn, đan dệt mịn, form rộng ấm áp, phù hợp thu đông.", "Áo Sweater Nam", "sweater-nam" },
                    { 47, 290000.00m, 2, new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), "T-shirt cotton 220gsm cổ tròn, form regular clean, item cơ bản không thể thiếu.", "T-Shirt Nam", "tshirt-nam" },
                    { 48, 490000.00m, 9, new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), "Váy 2 dây xếp nếp ngực vải lụa matte mịn, dáng suông dài qua gối, nữ tính thanh lịch.", "Váy 2 Dây Xếp Nếp Nữ", "vay-2-day-xep-nep-nu" },
                    { 49, 490000.00m, 9, new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), "Váy chiffon nữ bay nhẹ, tầng xếp nếp nhẹ nhàng, phù hợp dạo phố và sự kiện.", "Váy Chiffon Nữ", "vay-chiffon-nu" },
                    { 50, 390000.00m, 9, new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), "Váy cotton dây rút cổ, tay phồng điểm nhún, vải co giãn nhẹ mặc thoải mái mùa hè.", "Váy Dây Rút Cotton Nữ", "vay-day-rut-cotton-nu" },
                    { 51, 490000.00m, 9, new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), "Váy midi dáng bút chì ôm nhẹ, vải thun gân mịn, dài qua đầu gối tôn dáng.", "Váy Midi Nữ", "vay-midi-nu" },
                    { 52, 590000.00m, 9, new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), "Váy satin bóng mượt, cổ V thanh lịch, dáng suông thân thiện với mọi vóc dáng.", "Váy Satin Nữ", "vay-satin-nu" },
                    { 53, 590000.00m, 9, new DateTime(2026, 5, 14, 16, 44, 8, 536, DateTimeKind.Utc), "Váy sơ mi dáng shirt dress, cổ bẻ, hàng cúc ngực, thắt lưng kèm, phong cách preppy.", "Váy Sơ Mi Nữ", "vay-so-mi-nu" },
                    { 54, 650000.00m, 9, new DateTime(2026, 5, 14, 16, 44, 8, 536, DateTimeKind.Utc), "Váy sơ mi kèm thắt lưng vải, dáng midi thắt eo tôn dáng, vải cotton nhẹ mịn.", "Váy Sơ Mi Có Thắt Lưng Nữ", "vay-so-mi-co-that-lung-nu" },
                    { 55, 450000.00m, 9, new DateTime(2026, 5, 14, 16, 44, 8, 536, DateTimeKind.Utc), "Váy tunic dáng suông dài qua hông, vải linen thoáng mát, phù hợp đi biển và casual.", "Váy Tunic Nữ", "vay-tunic-nu" }
                });

            migrationBuilder.InsertData(
                table: "ShippingRates",
                columns: new[] { "Id", "CarrierId", "EstimatedDays", "Fee", "ProvinceId" },
                values: new object[,]
                {
                    { 1, 1, 1, 15000.00m, 1 },
                    { 2, 1, 2, 25000.00m, 2 },
                    { 3, 1, 1, 20000.00m, 3 },
                    { 4, 1, 3, 35000.00m, 4 },
                    { 5, 1, 4, 40000.00m, 5 },
                    { 6, 1, 3, 35000.00m, 6 },
                    { 7, 1, 2, 25000.00m, 7 },
                    { 8, 1, 2, 25000.00m, 8 },
                    { 9, 1, 2, 25000.00m, 9 },
                    { 10, 1, 2, 25000.00m, 10 },
                    { 11, 1, 2, 25000.00m, 11 },
                    { 12, 1, 2, 25000.00m, 12 },
                    { 13, 1, 2, 25000.00m, 13 },
                    { 14, 1, 2, 25000.00m, 14 },
                    { 15, 1, 2, 25000.00m, 15 },
                    { 16, 1, 2, 25000.00m, 16 },
                    { 17, 1, 2, 25000.00m, 17 },
                    { 18, 1, 2, 25000.00m, 18 },
                    { 19, 1, 2, 25000.00m, 19 },
                    { 20, 1, 2, 25000.00m, 20 },
                    { 21, 1, 3, 35000.00m, 21 },
                    { 22, 1, 3, 35000.00m, 22 },
                    { 23, 1, 3, 35000.00m, 23 },
                    { 24, 1, 3, 35000.00m, 24 },
                    { 25, 1, 3, 35000.00m, 25 },
                    { 26, 1, 3, 35000.00m, 26 },
                    { 27, 1, 3, 35000.00m, 27 },
                    { 28, 1, 3, 35000.00m, 28 },
                    { 29, 1, 4, 40000.00m, 29 },
                    { 30, 1, 4, 40000.00m, 30 },
                    { 31, 1, 4, 40000.00m, 31 },
                    { 32, 1, 4, 40000.00m, 32 },
                    { 33, 1, 4, 40000.00m, 33 },
                    { 34, 1, 4, 40000.00m, 34 },
                    { 35, 2, 1, 13000.00m, 1 },
                    { 36, 2, 2, 23000.00m, 2 },
                    { 37, 2, 1, 18000.00m, 3 },
                    { 38, 2, 3, 33000.00m, 4 },
                    { 39, 2, 4, 38000.00m, 5 },
                    { 40, 2, 3, 33000.00m, 6 },
                    { 41, 2, 2, 23000.00m, 7 },
                    { 42, 2, 2, 23000.00m, 8 },
                    { 43, 2, 2, 23000.00m, 9 },
                    { 44, 2, 2, 23000.00m, 10 },
                    { 45, 2, 2, 23000.00m, 11 },
                    { 46, 2, 2, 23000.00m, 12 },
                    { 47, 2, 2, 23000.00m, 13 },
                    { 48, 2, 2, 23000.00m, 14 },
                    { 49, 2, 2, 23000.00m, 15 },
                    { 50, 2, 2, 23000.00m, 16 },
                    { 51, 2, 2, 23000.00m, 17 },
                    { 52, 2, 2, 23000.00m, 18 },
                    { 53, 2, 2, 23000.00m, 19 },
                    { 54, 2, 2, 23000.00m, 20 },
                    { 55, 2, 3, 33000.00m, 21 },
                    { 56, 2, 3, 33000.00m, 22 },
                    { 57, 2, 3, 33000.00m, 23 },
                    { 58, 2, 3, 33000.00m, 24 },
                    { 59, 2, 3, 33000.00m, 25 },
                    { 60, 2, 3, 33000.00m, 26 },
                    { 61, 2, 3, 33000.00m, 27 },
                    { 62, 2, 3, 33000.00m, 28 },
                    { 63, 2, 4, 38000.00m, 29 },
                    { 64, 2, 4, 38000.00m, 30 },
                    { 65, 2, 4, 38000.00m, 31 },
                    { 66, 2, 4, 38000.00m, 32 },
                    { 67, 2, 4, 38000.00m, 33 },
                    { 68, 2, 4, 38000.00m, 34 },
                    { 69, 3, 1, 18000.00m, 1 },
                    { 70, 3, 2, 28000.00m, 2 },
                    { 71, 3, 1, 23000.00m, 3 },
                    { 72, 3, 3, 38000.00m, 4 },
                    { 73, 3, 4, 43000.00m, 5 },
                    { 74, 3, 3, 38000.00m, 6 },
                    { 75, 3, 2, 28000.00m, 7 },
                    { 76, 3, 2, 28000.00m, 8 },
                    { 77, 3, 2, 28000.00m, 9 },
                    { 78, 3, 2, 28000.00m, 10 },
                    { 79, 3, 2, 28000.00m, 11 },
                    { 80, 3, 2, 28000.00m, 12 },
                    { 81, 3, 2, 28000.00m, 13 },
                    { 82, 3, 2, 28000.00m, 14 },
                    { 83, 3, 2, 28000.00m, 15 },
                    { 84, 3, 2, 28000.00m, 16 },
                    { 85, 3, 2, 28000.00m, 17 },
                    { 86, 3, 2, 28000.00m, 18 },
                    { 87, 3, 2, 28000.00m, 19 },
                    { 88, 3, 2, 28000.00m, 20 },
                    { 89, 3, 3, 38000.00m, 21 },
                    { 90, 3, 3, 38000.00m, 22 },
                    { 91, 3, 3, 38000.00m, 23 },
                    { 92, 3, 3, 38000.00m, 24 },
                    { 93, 3, 3, 38000.00m, 25 },
                    { 94, 3, 3, 38000.00m, 26 },
                    { 95, 3, 3, 38000.00m, 27 },
                    { 96, 3, 3, 38000.00m, 28 },
                    { 97, 3, 4, 43000.00m, 29 },
                    { 98, 3, 4, 43000.00m, 30 },
                    { 99, 3, 4, 43000.00m, 31 },
                    { 100, 3, 4, 43000.00m, 32 },
                    { 101, 3, 4, 43000.00m, 33 },
                    { 102, 3, 4, 43000.00m, 34 }
                });

            migrationBuilder.InsertData(
                table: "SizeGuides",
                columns: new[] { "Id", "Chest", "Length", "Shoulder", "SizeId", "Waist" },
                values: new object[,]
                {
                    { 1, 76.00m, 63.00m, 37.00m, 1, 60.00m },
                    { 2, 82.00m, 65.00m, 39.00m, 2, 64.00m },
                    { 3, 88.00m, 67.00m, 41.00m, 3, 68.00m },
                    { 4, 94.00m, 69.00m, 43.00m, 4, 74.00m },
                    { 5, 100.00m, 71.00m, 45.00m, 5, 80.00m },
                    { 6, 106.00m, 73.00m, 47.00m, 6, 86.00m },
                    { 7, 112.00m, 75.00m, 49.00m, 7, 92.00m },
                    { 8, 118.00m, 77.00m, 51.00m, 8, 98.00m }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "ColorId", "DisplayOrder", "FileName", "IsPrimary", "ProductId" },
                values: new object[,]
                {
                    { 1, 1, 0, "ao3lo-main.avif", true, 1 },
                    { 2, 1, 1, "ao3lo-den.avif", false, 1 },
                    { 3, 8, 2, "ao3lo-kem.avif", false, 1 },
                    { 4, 1, 0, "ao3lonu-main.avif", true, 2 },
                    { 5, 1, 1, "ao3lonu-den.avif", false, 2 },
                    { 7, 2, 3, "ao3lonu-trang.avif", false, 2 },
                    { 8, 1, 0, "aogile-main.avif", true, 3 },
                    { 9, 1, 1, "aogile.avif", false, 3 },
                    { 10, 1, 0, "aogilenu-main.avif", true, 4 },
                    { 11, 1, 1, "aogilenu-den.avif", false, 4 },
                    { 12, 11, 2, "aogilenu-bedam.avif", false, 4 },
                    { 13, 8, 3, "aogilenu-kem.avif", false, 4 },
                    { 14, 35, 0, "aoblazerdenimnu-main.avif", true, 5 },
                    { 15, 35, 1, "aoblazerdenimnu.avif", false, 5 },
                    { 16, 1, 0, "aobomber-main.avif", true, 6 },
                    { 17, 1, 1, "aobomber.avif", false, 6 },
                    { 18, 14, 0, "aokhoaccottonnu-main.avif", true, 7 },
                    { 19, 14, 1, "aokhoaccottonnu.avif", false, 7 },
                    { 20, 35, 0, "aokhoacdenimnu-main.avif", true, 8 },
                    { 21, 35, 1, "aokhoacdenimnu.avif", false, 8 },
                    { 22, 14, 0, "aocodo-main.avif", true, 9 },
                    { 23, 14, 1, "aocodo-nau.avif", false, 9 },
                    { 24, 2, 2, "aocodo-trang.avif", false, 9 },
                    { 25, 8, 0, "aokhoaclinen-main.avif", true, 10 },
                    { 26, 8, 1, "aokhoaclinen-kem.avif", false, 10 },
                    { 27, 12, 2, "aokhoaclinen-kemdam.avif", false, 10 },
                    { 28, 2, 0, "aokhoacthethaonu-main.avif", true, 11 },
                    { 29, 2, 1, "aokhoacthethaonu.avif", false, 11 },
                    { 30, 1, 0, "aokhoactuxedo-main.avif", true, 12 },
                    { 31, 1, 1, "aokhoactuxedo.avif", false, 12 },
                    { 32, 2, 0, "aokieucodiemnhun-main.avif", true, 13 },
                    { 33, 3, 1, "aokieucodiemnhun.avif", false, 13 },
                    { 34, 26, 0, "aopintuckedblouse-main.avif", true, 14 },
                    { 35, 26, 1, "aopintuckedblouse.avif", false, 14 },
                    { 36, 33, 0, "aosomichiffon-main.avif", true, 15 },
                    { 37, 9, 0, "aosomico2venu-main.avif", true, 16 },
                    { 38, 9, 1, "aosomico2venu-be.avif", false, 16 },
                    { 39, 40, 2, "aosomico2venu-tim.avif", false, 16 },
                    { 40, 1, 0, "aosomikhoac-main.avif", true, 17 },
                    { 41, 1, 1, "aosomikhoac.avif", false, 17 },
                    { 42, 2, 0, "aosomilinennu-main.avif", true, 18 },
                    { 43, 9, 1, "aosomilinennu-be.avif", false, 18 },
                    { 44, 2, 2, "aosomilinennu-trang.avif", false, 18 },
                    { 45, 36, 3, "aosomilinennu-xanhduong.avif", false, 18 },
                    { 46, 25, 0, "aosominubuocday-main.avif", true, 19 },
                    { 47, 25, 1, "aosominubuocday.avif", false, 19 },
                    { 48, 8, 0, "aothunnu-main.avif", true, 20 },
                    { 49, 8, 1, "aothunnu-kem.avif", false, 20 },
                    { 50, 28, 2, "aothunnu-vang.avif", false, 20 },
                    { 51, 9, 0, "aotrenchcoatnu-main.avif", true, 21 },
                    { 52, 9, 1, "aotrenchcoatnu.avif", false, 21 },
                    { 53, 1, 0, "chanvaydapcheo-main.avif", true, 22 },
                    { 54, 1, 1, "chanvaydapcheo.avif", false, 22 },
                    { 55, 14, 0, "chanvayjersey-main.avif", true, 23 },
                    { 56, 14, 1, "chanvayjersey-nau.avif", false, 23 },
                    { 57, 32, 2, "chanvayjersey-xanholive.avif", false, 23 },
                    { 58, 1, 0, "chanvaymaxi-main.avif", true, 24 },
                    { 59, 1, 1, "chanvaymaxi.avif", false, 24 },
                    { 60, 19, 0, "chanvayxepru-main.avif", true, 25 },
                    { 61, 19, 1, "chanvayxepru.avif", false, 25 },
                    { 62, 1, 0, "hoddiekhongtay-main.avif", true, 26 },
                    { 63, 1, 1, "hoddiekhongtay-den.avif", false, 26 },
                    { 64, 5, 2, "hoddiekhongtay-xam.avif", false, 26 },
                    { 65, 1, 0, "hoodie-main.avif", true, 27 },
                    { 66, 1, 1, "hoodie-den.avif", false, 27 },
                    { 67, 23, 2, "hoodie-hong.avif", false, 27 },
                    { 68, 14, 3, "hoodie-nau.avif", false, 27 },
                    { 69, 5, 0, "hoodiekhoakeo-main.avif", true, 28 },
                    { 70, 5, 1, "hoodiekhoakeo.avif", false, 28 },
                    { 71, 1, 0, "jumper-main.avif", true, 29 },
                    { 72, 1, 1, "jumper.avif", false, 29 },
                    { 73, 35, 0, "polo-main.avif", true, 30 },
                    { 74, 1, 1, "polo-den.avif", false, 30 },
                    { 75, 2, 2, "polo-trang.avif", false, 30 },
                    { 76, 35, 3, "polo-xanhbien.avif", false, 30 },
                    { 77, 1, 0, "quandaicapchun-main.avif", true, 31 },
                    { 78, 1, 1, "quandaicapchun-den.avif", false, 31 },
                    { 79, 14, 2, "quandaicapchun-nau.avif", false, 31 },
                    { 80, 2, 0, "quandaidayrut-main.avif", true, 32 },
                    { 81, 2, 1, "quandaidayrut.avif", false, 32 },
                    { 82, 1, 0, "quandaisatin-main.avif", true, 33 },
                    { 83, 1, 1, "quandaisatin-den.avif", false, 33 },
                    { 84, 8, 2, "quandaisatin-kem.avif", false, 33 },
                    { 85, 1, 0, "quanjean-main.avif", true, 34 },
                    { 86, 1, 1, "quanjean-den.avif", false, 34 },
                    { 87, 35, 2, "quanjean-xanhbien.avif", false, 34 },
                    { 88, 36, 0, "quanjeannu-main.avif", true, 35 },
                    { 89, 1, 1, "quanjeannu-den.avif", false, 35 },
                    { 90, 36, 2, "quanjeannu-xanhduong.avif", false, 35 },
                    { 91, 36, 0, "quanjeannurachgoi-main.avif", true, 36 },
                    { 92, 36, 1, "quanjeannurachgoi-xanhduong.avif", false, 36 },
                    { 93, 37, 2, "quanjeannurachgoi-xanhduongdam.avif", false, 36 },
                    { 94, 1, 0, "quanjogger-main.avif", true, 37 },
                    { 95, 1, 1, "quanjogger-den.avif", false, 37 },
                    { 96, 5, 2, "quanjogger-xam.avif", false, 37 },
                    { 97, 1, 0, "quanongsuongmuslin-main.avif", true, 38 },
                    { 98, 1, 1, "quanongsuongmuslin-nau.avif", false, 38 },
                    { 99, 33, 2, "quanongsuongmuslin-xanhkaki.avif", false, 38 },
                    { 100, 35, 0, "quanshortjean-main.avif", true, 39 },
                    { 101, 1, 1, "quanshortjean-den.avif", false, 39 },
                    { 102, 35, 2, "quanshortjean-xanh.avif", false, 39 },
                    { 103, 5, 0, "quanshortni-main.avif", true, 40 },
                    { 104, 1, 1, "quanshortni-den.avif", false, 40 },
                    { 105, 5, 2, "quanshortni-xam.avif", false, 40 },
                    { 106, 12, 0, "quanshorttuihop-main.avif", true, 41 },
                    { 107, 12, 1, "quanshorttuihop.avif", false, 41 },
                    { 108, 12, 0, "quantay-main.avif", true, 42 },
                    { 109, 12, 1, "quantay.avif", false, 42 },
                    { 110, 2, 0, "somichongnhan-main.avif", true, 43 },
                    { 111, 2, 1, "somichongnhan-trang.avif", false, 43 },
                    { 112, 1, 2, "somichongnhan-den.avif", false, 43 },
                    { 113, 35, 3, "somichongnhan-xanhbien.avif", false, 43 },
                    { 114, 35, 0, "somicotton-main.avif", true, 44 },
                    { 115, 18, 1, "somicotton-do.avif", false, 44 },
                    { 116, 35, 2, "somicotton-xanhbien.avif", false, 44 },
                    { 117, 35, 0, "somijean-main.avif", true, 45 },
                    { 118, 35, 1, "somijean.avif", false, 45 },
                    { 119, 23, 0, "sweater-main.avif", true, 46 },
                    { 120, 23, 1, "sweater-hong.avif", false, 46 },
                    { 121, 2, 2, "sweater-trang.avif", false, 46 },
                    { 122, 5, 3, "sweater-xam.avif", false, 46 },
                    { 123, 8, 0, "tshirt-main.avif", true, 47 },
                    { 124, 1, 1, "tshirt-den.avif", false, 47 },
                    { 125, 8, 2, "tshirt-kem.avif", false, 47 },
                    { 126, 14, 0, "vay2dayxepnep-main.avif", true, 48 },
                    { 127, 14, 1, "vay2dayxepnep-nau.avif", false, 48 },
                    { 128, 28, 2, "vay2dayxepnep-vang.avif", false, 48 },
                    { 129, 36, 3, "vay2dayxepnep-xanhduong.avif", false, 48 },
                    { 130, 1, 0, "vaychiffon-main.avif", true, 49 },
                    { 131, 1, 1, "vaychiffon-den.avif", false, 49 },
                    { 132, 14, 2, "vaychiffon-nau.avif", false, 49 },
                    { 133, 2, 0, "vaydayrutcotton-main.jpg", true, 50 },
                    { 134, 2, 1, "vaydayrutcotton.jpg", false, 50 },
                    { 135, 3, 0, "vaymidi-main.avif", true, 51 },
                    { 136, 3, 1, "vaymidi.avif", false, 51 },
                    { 137, 40, 0, "vaysatin-main.avif", true, 52 },
                    { 138, 40, 1, "vaysatin.avif", false, 52 },
                    { 139, 36, 0, "vaysomi-main.avif", true, 53 },
                    { 140, 2, 1, "vaysomi-trang.avif", false, 53 },
                    { 141, 36, 2, "vaysomi-xanhduong.avif", false, 53 },
                    { 142, 2, 0, "vaysomicothatlung-main.avif", true, 54 },
                    { 143, 2, 1, "vaysomicothatlung-trang.avif", false, 54 },
                    { 144, 36, 2, "vaysomicothatlung-xanhduong.avif", false, 54 },
                    { 145, 24, 0, "vaytunic-main.avif", true, 55 },
                    { 146, 24, 1, "vaytunic.avif", false, 55 },
                    { 203, 9, 0, "ao3lonu-be.avif", false, 2 }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "ColorId", "Price", "ProductId", "ReservedStock", "SKU", "SizeId", "Stock" },
                values: new object[,]
                {
                    { 1, 1, 290000.00m, 1, 0, "ABLNAM-DEN-S", 2, 15 },
                    { 2, 1, 290000.00m, 1, 0, "ABLNAM-DEN-M", 3, 20 },
                    { 3, 1, 290000.00m, 1, 0, "ABLNAM-DEN-L", 4, 15 },
                    { 4, 1, 290000.00m, 1, 0, "ABLNAM-DEN-XL", 5, 10 },
                    { 5, 8, 290000.00m, 1, 0, "ABLNAM-KEM-S", 2, 12 },
                    { 6, 8, 290000.00m, 1, 0, "ABLNAM-KEM-M", 3, 15 },
                    { 7, 8, 290000.00m, 1, 0, "ABLNAM-KEM-L", 4, 12 },
                    { 8, 8, 290000.00m, 1, 0, "ABLNAM-KEM-XL", 5, 8 },
                    { 9, 1, 290000.00m, 2, 0, "ABLNU-DEN-S", 2, 15 },
                    { 10, 1, 290000.00m, 2, 0, "ABLNU-DEN-M", 3, 20 },
                    { 11, 1, 290000.00m, 2, 0, "ABLNU-DEN-L", 4, 15 },
                    { 12, 9, 290000.00m, 2, 0, "ABLNU-BE-S", 2, 12 },
                    { 13, 9, 290000.00m, 2, 0, "ABLNU-BE-M", 3, 15 },
                    { 14, 9, 290000.00m, 2, 0, "ABLNU-BE-L", 4, 10 },
                    { 15, 2, 290000.00m, 2, 0, "ABLNU-TRG-S", 2, 12 },
                    { 16, 2, 290000.00m, 2, 0, "ABLNU-TRG-M", 3, 15 },
                    { 17, 2, 290000.00m, 2, 0, "ABLNU-TRG-L", 4, 10 },
                    { 18, 1, 690000.00m, 3, 0, "AGLN-DEN-S", 2, 8 },
                    { 19, 1, 690000.00m, 3, 0, "AGLN-DEN-M", 3, 12 },
                    { 20, 1, 690000.00m, 3, 0, "AGLN-DEN-L", 4, 10 },
                    { 21, 1, 650000.00m, 4, 0, "AGLNU-DEN-S", 2, 10 },
                    { 22, 1, 650000.00m, 4, 0, "AGLNU-DEN-M", 3, 12 },
                    { 23, 1, 650000.00m, 4, 0, "AGLNU-DEN-L", 4, 8 },
                    { 24, 11, 650000.00m, 4, 0, "AGLNU-CAM-S", 2, 8 },
                    { 25, 11, 650000.00m, 4, 0, "AGLNU-CAM-M", 3, 10 },
                    { 26, 11, 650000.00m, 4, 0, "AGLNU-CAM-L", 4, 7 },
                    { 27, 8, 650000.00m, 4, 0, "AGLNU-KEM-S", 2, 8 },
                    { 28, 8, 650000.00m, 4, 0, "AGLNU-KEM-M", 3, 10 },
                    { 29, 8, 650000.00m, 4, 0, "AGLNU-KEM-L", 4, 7 },
                    { 30, 35, 890000.00m, 5, 0, "ABDN-XDB-S", 2, 8 },
                    { 31, 35, 890000.00m, 5, 0, "ABDN-XDB-M", 3, 12 },
                    { 32, 35, 890000.00m, 5, 0, "ABDN-XDB-L", 4, 9 },
                    { 33, 1, 790000.00m, 6, 0, "AKBN-DEN-S", 2, 10 },
                    { 34, 1, 790000.00m, 6, 0, "AKBN-DEN-M", 3, 15 },
                    { 35, 1, 790000.00m, 6, 0, "AKBN-DEN-L", 4, 12 },
                    { 36, 1, 790000.00m, 6, 0, "AKBN-DEN-XL", 5, 8 },
                    { 37, 8, 750000.00m, 7, 0, "AKCTNU-KEM-S", 2, 10 },
                    { 38, 8, 750000.00m, 7, 0, "AKCTNU-KEM-M", 3, 14 },
                    { 39, 8, 750000.00m, 7, 0, "AKCTNU-KEM-L", 4, 10 },
                    { 40, 35, 790000.00m, 8, 0, "AKDNU-XDB-S", 2, 10 },
                    { 41, 35, 790000.00m, 8, 0, "AKDNU-XDB-M", 3, 14 },
                    { 42, 35, 790000.00m, 8, 0, "AKDNU-XDB-L", 4, 11 },
                    { 43, 14, 390000.00m, 9, 0, "ACDNU-NAU-S", 2, 11 },
                    { 44, 14, 390000.00m, 9, 0, "ACDNU-NAU-M", 3, 15 },
                    { 45, 14, 390000.00m, 9, 0, "ACDNU-NAU-L", 4, 10 },
                    { 46, 2, 390000.00m, 9, 0, "ACDNU-TRG-S", 2, 12 },
                    { 47, 2, 390000.00m, 9, 0, "ACDNU-TRG-M", 3, 15 },
                    { 48, 2, 390000.00m, 9, 0, "ACDNU-TRG-L", 4, 10 },
                    { 49, 8, 890000.00m, 10, 0, "AKLN-KEM-S", 2, 10 },
                    { 50, 8, 890000.00m, 10, 0, "AKLN-KEM-M", 3, 14 },
                    { 51, 8, 890000.00m, 10, 0, "AKLN-KEM-L", 4, 10 },
                    { 52, 12, 890000.00m, 10, 0, "AKLN-KHK-S", 2, 8 },
                    { 53, 12, 890000.00m, 10, 0, "AKLN-KHK-M", 3, 10 },
                    { 54, 12, 890000.00m, 10, 0, "AKLN-KHK-L", 4, 7 },
                    { 55, 1, 690000.00m, 11, 0, "AKTTNU-DEN-S", 2, 12 },
                    { 56, 1, 690000.00m, 11, 0, "AKTTNU-DEN-M", 3, 15 },
                    { 57, 1, 690000.00m, 11, 0, "AKTTNU-DEN-L", 4, 12 },
                    { 58, 1, 1290000.00m, 12, 0, "AKTXN-DEN-S", 2, 8 },
                    { 59, 1, 1290000.00m, 12, 0, "AKTXN-DEN-M", 3, 10 },
                    { 60, 1, 1290000.00m, 12, 0, "AKTXN-DEN-L", 4, 8 },
                    { 61, 2, 350000.00m, 13, 0, "AKCDN-TRG-S", 2, 15 },
                    { 62, 2, 350000.00m, 13, 0, "AKCDN-TRG-M", 3, 20 },
                    { 63, 2, 350000.00m, 13, 0, "AKCDN-TRG-L", 4, 15 },
                    { 64, 2, 490000.00m, 14, 0, "APTBNU-TRG-S", 2, 12 },
                    { 65, 2, 490000.00m, 14, 0, "APTBNU-TRG-M", 3, 15 },
                    { 66, 2, 490000.00m, 14, 0, "APTBNU-TRG-L", 4, 12 },
                    { 67, 33, 420000.00m, 15, 0, "ASMCFN-XRE-S", 2, 12 },
                    { 68, 33, 420000.00m, 15, 0, "ASMCFN-XRE-M", 3, 15 },
                    { 69, 33, 420000.00m, 15, 0, "ASMCFN-XRE-L", 4, 12 },
                    { 70, 9, 490000.00m, 16, 0, "ASM2VNU-BE-S", 2, 12 },
                    { 71, 9, 490000.00m, 16, 0, "ASM2VNU-BE-M", 3, 15 },
                    { 72, 9, 490000.00m, 16, 0, "ASM2VNU-BE-L", 4, 10 },
                    { 73, 40, 490000.00m, 16, 0, "ASM2VNU-TIM-S", 2, 10 },
                    { 74, 40, 490000.00m, 16, 0, "ASM2VNU-TIM-M", 3, 12 },
                    { 75, 40, 490000.00m, 16, 0, "ASM2VNU-TIM-L", 4, 9 },
                    { 76, 2, 450000.00m, 17, 0, "ASMK-TRG-S", 2, 12 },
                    { 77, 2, 450000.00m, 17, 0, "ASMK-TRG-M", 3, 15 },
                    { 78, 2, 450000.00m, 17, 0, "ASMK-TRG-L", 4, 12 },
                    { 79, 2, 450000.00m, 17, 0, "ASMK-TRG-XL", 5, 8 },
                    { 80, 9, 550000.00m, 18, 0, "ASMLNN-BE-S", 2, 12 },
                    { 81, 9, 550000.00m, 18, 0, "ASMLNN-BE-M", 3, 15 },
                    { 82, 9, 550000.00m, 18, 0, "ASMLNN-BE-L", 4, 10 },
                    { 83, 2, 550000.00m, 18, 0, "ASMLNN-TRG-S", 2, 10 },
                    { 84, 2, 550000.00m, 18, 0, "ASMLNN-TRG-M", 3, 14 },
                    { 85, 2, 550000.00m, 18, 0, "ASMLNN-TRG-L", 4, 10 },
                    { 86, 36, 550000.00m, 18, 0, "ASMLNN-XND-S", 2, 8 },
                    { 87, 36, 550000.00m, 18, 0, "ASMLNN-XND-M", 3, 12 },
                    { 88, 36, 550000.00m, 18, 0, "ASMLNN-XND-L", 4, 9 },
                    { 89, 2, 380000.00m, 19, 0, "ASMNBD-TRG-S", 2, 15 },
                    { 90, 2, 380000.00m, 19, 0, "ASMNBD-TRG-M", 3, 18 },
                    { 91, 2, 380000.00m, 19, 0, "ASMNBD-TRG-L", 4, 12 },
                    { 92, 8, 290000.00m, 20, 0, "ATNU-KEM-S", 2, 20 },
                    { 93, 8, 290000.00m, 20, 0, "ATNU-KEM-M", 3, 25 },
                    { 94, 8, 290000.00m, 20, 0, "ATNU-KEM-L", 4, 18 },
                    { 95, 28, 290000.00m, 20, 0, "ATNU-VNG-S", 2, 15 },
                    { 96, 28, 290000.00m, 20, 0, "ATNU-VNG-M", 3, 18 },
                    { 97, 28, 290000.00m, 20, 0, "ATNU-VNG-L", 4, 12 },
                    { 98, 9, 1590000.00m, 21, 0, "ATCNU-BE-S", 2, 6 },
                    { 99, 9, 1590000.00m, 21, 0, "ATCNU-BE-M", 3, 8 },
                    { 100, 9, 1590000.00m, 21, 0, "ATCNU-BE-L", 4, 6 },
                    { 101, 1, 490000.00m, 22, 0, "CVDCNU-DEN-S", 2, 12 },
                    { 102, 1, 490000.00m, 22, 0, "CVDCNU-DEN-M", 3, 15 },
                    { 103, 1, 490000.00m, 22, 0, "CVDCNU-DEN-L", 4, 10 },
                    { 104, 14, 390000.00m, 23, 0, "CVJNU-NAU-S", 2, 12 },
                    { 105, 14, 390000.00m, 23, 0, "CVJNU-NAU-M", 3, 15 },
                    { 106, 14, 390000.00m, 23, 0, "CVJNU-NAU-L", 4, 10 },
                    { 107, 32, 390000.00m, 23, 0, "CVJNU-OLV-S", 2, 10 },
                    { 108, 32, 390000.00m, 23, 0, "CVJNU-OLV-M", 3, 12 },
                    { 109, 32, 390000.00m, 23, 0, "CVJNU-OLV-L", 4, 8 },
                    { 110, 1, 490000.00m, 24, 0, "CVMXNU-DEN-S", 2, 12 },
                    { 111, 1, 490000.00m, 24, 0, "CVMXNU-DEN-M", 3, 15 },
                    { 112, 1, 490000.00m, 24, 0, "CVMXNU-DEN-L", 4, 10 },
                    { 113, 8, 450000.00m, 25, 0, "CVXRNU-KEM-S", 2, 12 },
                    { 114, 8, 450000.00m, 25, 0, "CVXRNU-KEM-M", 3, 15 },
                    { 115, 8, 450000.00m, 25, 0, "CVXRNU-KEM-L", 4, 10 },
                    { 116, 1, 590000.00m, 26, 0, "HDKTN-DEN-S", 2, 12 },
                    { 117, 1, 590000.00m, 26, 0, "HDKTN-DEN-M", 3, 15 },
                    { 118, 1, 590000.00m, 26, 0, "HDKTN-DEN-L", 4, 12 },
                    { 119, 5, 590000.00m, 26, 0, "HDKTN-XAM-S", 2, 10 },
                    { 120, 5, 590000.00m, 26, 0, "HDKTN-XAM-M", 3, 14 },
                    { 121, 5, 590000.00m, 26, 0, "HDKTN-XAM-L", 4, 10 },
                    { 122, 1, 590000.00m, 27, 0, "HDNN-DEN-S", 2, 12 },
                    { 123, 1, 590000.00m, 27, 0, "HDNN-DEN-M", 3, 18 },
                    { 124, 1, 590000.00m, 27, 0, "HDNN-DEN-L", 4, 14 },
                    { 125, 23, 590000.00m, 27, 0, "HDNN-HONG-S", 2, 10 },
                    { 126, 23, 590000.00m, 27, 0, "HDNN-HONG-M", 3, 12 },
                    { 127, 23, 590000.00m, 27, 0, "HDNN-HONG-L", 4, 9 },
                    { 128, 14, 590000.00m, 27, 0, "HDNN-NAU-S", 2, 8 },
                    { 129, 14, 590000.00m, 27, 0, "HDNN-NAU-M", 3, 10 },
                    { 130, 14, 590000.00m, 27, 0, "HDNN-NAU-L", 4, 8 },
                    { 131, 5, 650000.00m, 28, 0, "HDKKN-XAM-S", 2, 12 },
                    { 132, 5, 650000.00m, 28, 0, "HDKKN-XAM-M", 3, 15 },
                    { 133, 5, 650000.00m, 28, 0, "HDKKN-XAM-L", 4, 12 },
                    { 134, 5, 650000.00m, 28, 0, "HDKKN-XAM-XL", 5, 8 },
                    { 135, 8, 550000.00m, 29, 0, "JMPNU-KEM-S", 2, 10 },
                    { 136, 8, 550000.00m, 29, 0, "JMPNU-KEM-M", 3, 14 },
                    { 137, 8, 550000.00m, 29, 0, "JMPNU-KEM-L", 4, 10 },
                    { 138, 1, 490000.00m, 30, 0, "POLON-DEN-S", 2, 15 },
                    { 139, 1, 490000.00m, 30, 0, "POLON-DEN-M", 3, 20 },
                    { 140, 1, 490000.00m, 30, 0, "POLON-DEN-L", 4, 18 },
                    { 141, 2, 490000.00m, 30, 0, "POLON-TRG-S", 2, 12 },
                    { 142, 2, 490000.00m, 30, 0, "POLON-TRG-M", 3, 18 },
                    { 143, 2, 490000.00m, 30, 0, "POLON-TRG-L", 4, 15 },
                    { 144, 35, 490000.00m, 30, 0, "POLON-XDB-S", 2, 10 },
                    { 145, 35, 490000.00m, 30, 0, "POLON-XDB-M", 3, 14 },
                    { 146, 35, 490000.00m, 30, 0, "POLON-XDB-L", 4, 12 },
                    { 147, 1, 450000.00m, 31, 0, "QDCCNU-DEN-S", 2, 15 },
                    { 148, 1, 450000.00m, 31, 0, "QDCCNU-DEN-M", 3, 18 },
                    { 149, 1, 450000.00m, 31, 0, "QDCCNU-DEN-L", 4, 14 },
                    { 150, 14, 450000.00m, 31, 0, "QDCCNU-NAU-S", 2, 10 },
                    { 151, 14, 450000.00m, 31, 0, "QDCCNU-NAU-M", 3, 14 },
                    { 152, 14, 450000.00m, 31, 0, "QDCCNU-NAU-L", 4, 10 },
                    { 153, 5, 490000.00m, 32, 0, "QDDRN-XAM-S", 2, 12 },
                    { 154, 5, 490000.00m, 32, 0, "QDDRN-XAM-M", 3, 15 },
                    { 155, 5, 490000.00m, 32, 0, "QDDRN-XAM-L", 4, 12 },
                    { 156, 5, 490000.00m, 32, 0, "QDDRN-XAM-XL", 5, 8 },
                    { 157, 1, 590000.00m, 33, 0, "QDSTNU-DEN-S", 2, 12 },
                    { 158, 1, 590000.00m, 33, 0, "QDSTNU-DEN-M", 3, 15 },
                    { 159, 1, 590000.00m, 33, 0, "QDSTNU-DEN-L", 4, 10 },
                    { 160, 8, 590000.00m, 33, 0, "QDSTNU-KEM-S", 2, 10 },
                    { 161, 8, 590000.00m, 33, 0, "QDSTNU-KEM-M", 3, 12 },
                    { 162, 8, 590000.00m, 33, 0, "QDSTNU-KEM-L", 4, 9 },
                    { 163, 1, 690000.00m, 34, 0, "QJNN-DEN-S", 2, 12 },
                    { 164, 1, 690000.00m, 34, 0, "QJNN-DEN-M", 3, 18 },
                    { 165, 1, 690000.00m, 34, 0, "QJNN-DEN-L", 4, 15 },
                    { 166, 35, 690000.00m, 34, 0, "QJNN-XDB-S", 2, 10 },
                    { 167, 35, 690000.00m, 34, 0, "QJNN-XDB-M", 3, 15 },
                    { 168, 35, 690000.00m, 34, 0, "QJNN-XDB-L", 4, 12 },
                    { 169, 1, 650000.00m, 35, 0, "QJNNU-DEN-S", 2, 15 },
                    { 170, 1, 650000.00m, 35, 0, "QJNNU-DEN-M", 3, 18 },
                    { 171, 1, 650000.00m, 35, 0, "QJNNU-DEN-L", 4, 14 },
                    { 172, 36, 650000.00m, 35, 0, "QJNNU-XND-S", 2, 12 },
                    { 173, 36, 650000.00m, 35, 0, "QJNNU-XND-M", 3, 15 },
                    { 174, 36, 650000.00m, 35, 0, "QJNNU-XND-L", 4, 12 },
                    { 175, 36, 690000.00m, 36, 0, "QJNRG-XND-S", 2, 12 },
                    { 176, 36, 690000.00m, 36, 0, "QJNRG-XND-M", 3, 15 },
                    { 177, 36, 690000.00m, 36, 0, "QJNRG-XND-L", 4, 10 },
                    { 178, 37, 690000.00m, 36, 0, "QJNRG-XNV-S", 2, 10 },
                    { 179, 37, 690000.00m, 36, 0, "QJNRG-XNV-M", 3, 12 },
                    { 180, 37, 690000.00m, 36, 0, "QJNRG-XNV-L", 4, 9 },
                    { 181, 1, 490000.00m, 37, 0, "QJGN-DEN-S", 2, 15 },
                    { 182, 1, 490000.00m, 37, 0, "QJGN-DEN-M", 3, 20 },
                    { 183, 1, 490000.00m, 37, 0, "QJGN-DEN-L", 4, 16 },
                    { 184, 5, 490000.00m, 37, 0, "QJGN-XAM-S", 2, 12 },
                    { 185, 5, 490000.00m, 37, 0, "QJGN-XAM-M", 3, 15 },
                    { 186, 5, 490000.00m, 37, 0, "QJGN-XAM-L", 4, 12 },
                    { 187, 14, 590000.00m, 38, 0, "QOSMN-NAU-S", 2, 10 },
                    { 188, 14, 590000.00m, 38, 0, "QOSMN-NAU-M", 3, 12 },
                    { 189, 14, 590000.00m, 38, 0, "QOSMN-NAU-L", 4, 9 },
                    { 190, 33, 590000.00m, 38, 0, "QOSMN-XKK-S", 2, 8 },
                    { 191, 33, 590000.00m, 38, 0, "QOSMN-XKK-M", 3, 10 },
                    { 192, 33, 590000.00m, 38, 0, "QOSMN-XKK-L", 4, 8 },
                    { 193, 1, 450000.00m, 39, 0, "QSJN-DEN-S", 2, 15 },
                    { 194, 1, 450000.00m, 39, 0, "QSJN-DEN-M", 3, 20 },
                    { 195, 1, 450000.00m, 39, 0, "QSJN-DEN-L", 4, 16 },
                    { 196, 31, 450000.00m, 39, 0, "QSJN-XNL-S", 2, 12 },
                    { 197, 31, 450000.00m, 39, 0, "QSJN-XNL-M", 3, 15 },
                    { 198, 31, 450000.00m, 39, 0, "QSJN-XNL-L", 4, 12 },
                    { 199, 1, 390000.00m, 40, 0, "QSNN-DEN-S", 2, 18 },
                    { 200, 1, 390000.00m, 40, 0, "QSNN-DEN-M", 3, 22 },
                    { 201, 1, 390000.00m, 40, 0, "QSNN-DEN-L", 4, 18 },
                    { 202, 5, 390000.00m, 40, 0, "QSNN-XAM-S", 2, 14 },
                    { 203, 5, 390000.00m, 40, 0, "QSNN-XAM-M", 3, 18 },
                    { 204, 5, 390000.00m, 40, 0, "QSNN-XAM-L", 4, 14 },
                    { 205, 12, 490000.00m, 41, 0, "QSTHN-KHK-S", 2, 12 },
                    { 206, 12, 490000.00m, 41, 0, "QSTHN-KHK-M", 3, 16 },
                    { 207, 12, 490000.00m, 41, 0, "QSTHN-KHK-L", 4, 12 },
                    { 208, 12, 490000.00m, 41, 0, "QSTHN-KHK-XL", 5, 8 },
                    { 209, 1, 690000.00m, 42, 0, "QTN-DEN-S", 2, 10 },
                    { 210, 1, 690000.00m, 42, 0, "QTN-DEN-M", 3, 14 },
                    { 211, 1, 690000.00m, 42, 0, "QTN-DEN-L", 4, 12 },
                    { 212, 1, 690000.00m, 42, 0, "QTN-DEN-XL", 5, 8 },
                    { 213, 2, 690000.00m, 43, 0, "SMCNN-TRG-S", 2, 12 },
                    { 214, 2, 690000.00m, 43, 0, "SMCNN-TRG-M", 3, 16 },
                    { 215, 2, 690000.00m, 43, 0, "SMCNN-TRG-L", 4, 12 },
                    { 216, 1, 690000.00m, 43, 0, "SMCNN-DEN-S", 2, 10 },
                    { 217, 1, 690000.00m, 43, 0, "SMCNN-DEN-M", 3, 14 },
                    { 218, 1, 690000.00m, 43, 0, "SMCNN-DEN-L", 4, 10 },
                    { 219, 35, 690000.00m, 43, 0, "SMCNN-XDB-S", 2, 8 },
                    { 220, 35, 690000.00m, 43, 0, "SMCNN-XDB-M", 3, 12 },
                    { 221, 35, 690000.00m, 43, 0, "SMCNN-XDB-L", 4, 9 },
                    { 222, 18, 590000.00m, 44, 0, "SMCTN-DO-S", 2, 10 },
                    { 223, 18, 590000.00m, 44, 0, "SMCTN-DO-M", 3, 14 },
                    { 224, 18, 590000.00m, 44, 0, "SMCTN-DO-L", 4, 10 },
                    { 225, 35, 590000.00m, 44, 0, "SMCTN-XDB-S", 2, 10 },
                    { 226, 35, 590000.00m, 44, 0, "SMCTN-XDB-M", 3, 14 },
                    { 227, 35, 590000.00m, 44, 0, "SMCTN-XDB-L", 4, 10 },
                    { 228, 35, 590000.00m, 45, 0, "SMJN-XDB-S", 2, 10 },
                    { 229, 35, 590000.00m, 45, 0, "SMJN-XDB-M", 3, 14 },
                    { 230, 35, 590000.00m, 45, 0, "SMJN-XDB-L", 4, 10 },
                    { 231, 23, 590000.00m, 46, 0, "SWTNU-HONG-S", 2, 10 },
                    { 232, 23, 590000.00m, 46, 0, "SWTNU-HONG-M", 3, 14 },
                    { 233, 23, 590000.00m, 46, 0, "SWTNU-HONG-L", 4, 10 },
                    { 234, 2, 590000.00m, 46, 0, "SWTNU-TRG-S", 2, 10 },
                    { 235, 2, 590000.00m, 46, 0, "SWTNU-TRG-M", 3, 12 },
                    { 236, 2, 590000.00m, 46, 0, "SWTNU-TRG-L", 4, 9 },
                    { 237, 5, 590000.00m, 46, 0, "SWTNU-XAM-S", 2, 10 },
                    { 238, 5, 590000.00m, 46, 0, "SWTNU-XAM-M", 3, 12 },
                    { 239, 5, 590000.00m, 46, 0, "SWTNU-XAM-L", 4, 9 },
                    { 240, 1, 290000.00m, 47, 0, "TSN-DEN-S", 2, 20 },
                    { 241, 1, 290000.00m, 47, 0, "TSN-DEN-M", 3, 25 },
                    { 242, 1, 290000.00m, 47, 0, "TSN-DEN-L", 4, 20 },
                    { 243, 8, 290000.00m, 47, 0, "TSN-KEM-S", 2, 15 },
                    { 244, 8, 290000.00m, 47, 0, "TSN-KEM-M", 3, 20 },
                    { 245, 8, 290000.00m, 47, 0, "TSN-KEM-L", 4, 15 },
                    { 246, 14, 490000.00m, 48, 0, "V2DXN-NAU-S", 2, 12 },
                    { 247, 14, 490000.00m, 48, 0, "V2DXN-NAU-M", 3, 15 },
                    { 248, 14, 490000.00m, 48, 0, "V2DXN-NAU-L", 4, 10 },
                    { 249, 28, 490000.00m, 48, 0, "V2DXN-VNG-S", 2, 10 },
                    { 250, 28, 490000.00m, 48, 0, "V2DXN-VNG-M", 3, 12 },
                    { 251, 28, 490000.00m, 48, 0, "V2DXN-VNG-L", 4, 9 },
                    { 252, 36, 490000.00m, 48, 0, "V2DXN-XND-S", 2, 8 },
                    { 253, 36, 490000.00m, 48, 0, "V2DXN-XND-M", 3, 12 },
                    { 254, 36, 490000.00m, 48, 0, "V2DXN-XND-L", 4, 9 },
                    { 255, 1, 490000.00m, 49, 0, "VCFNU-DEN-S", 2, 12 },
                    { 256, 1, 490000.00m, 49, 0, "VCFNU-DEN-M", 3, 15 },
                    { 257, 1, 490000.00m, 49, 0, "VCFNU-DEN-L", 4, 10 },
                    { 258, 14, 490000.00m, 49, 0, "VCFNU-NAU-S", 2, 10 },
                    { 259, 14, 490000.00m, 49, 0, "VCFNU-NAU-M", 3, 12 },
                    { 260, 14, 490000.00m, 49, 0, "VCFNU-NAU-L", 4, 9 },
                    { 261, 2, 390000.00m, 50, 0, "VDRCNU-TRG-S", 2, 15 },
                    { 262, 2, 390000.00m, 50, 0, "VDRCNU-TRG-M", 3, 18 },
                    { 263, 2, 390000.00m, 50, 0, "VDRCNU-TRG-L", 4, 14 },
                    { 264, 2, 490000.00m, 51, 0, "VMDN-TRANG-S", 2, 12 },
                    { 265, 2, 490000.00m, 51, 0, "VMDN-TRANG-M", 3, 15 },
                    { 266, 2, 490000.00m, 51, 0, "VMDN-TRANG-L", 4, 10 },
                    { 267, 40, 500000.00m, 52, 0, "VSTN-TIM-S", 2, 10 },
                    { 268, 40, 550000.00m, 52, 0, "VSTN-TIM-M", 3, 14 },
                    { 269, 40, 590000.00m, 52, 0, "VSTN-TIM-L", 4, 10 },
                    { 270, 2, 500000.00m, 53, 0, "VSMNU-TRG-S", 2, 12 },
                    { 271, 2, 550000.00m, 53, 0, "VSMNU-TRG-M", 3, 15 },
                    { 272, 2, 590000.00m, 53, 0, "VSMNU-TRG-L", 4, 10 },
                    { 273, 36, 500000.00m, 53, 0, "VSMNU-XND-S", 2, 8 },
                    { 274, 36, 550000.00m, 53, 0, "VSMNU-XND-M", 3, 12 },
                    { 275, 36, 590000.00m, 53, 0, "VSMNU-XND-L", 4, 9 },
                    { 276, 2, 650000.00m, 54, 0, "VSMTLNU-TRG-S", 2, 10 },
                    { 277, 2, 650000.00m, 54, 0, "VSMTLNU-TRG-M", 3, 14 },
                    { 278, 2, 650000.00m, 54, 0, "VSMTLNU-TRG-L", 4, 10 },
                    { 279, 36, 650000.00m, 54, 0, "VSMTLNU-XND-S", 2, 8 },
                    { 280, 36, 650000.00m, 54, 0, "VSMTLNU-XND-M", 3, 10 },
                    { 281, 36, 650000.00m, 54, 0, "VSMTLNU-XND-L", 4, 8 },
                    { 282, 8, 350000.00m, 55, 0, "VTNNU-KEM-S", 2, 12 },
                    { 283, 8, 400000.00m, 55, 0, "VTNNU-KEM-M", 3, 15 },
                    { 284, 8, 450000.00m, 55, 0, "VTNNU-KEM-L", 4, 10 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "ImageUrl", "ProductId", "Rating", "SizeAccuracy", "UserId" },
                values: new object[] { 1, "", new DateTime(2026, 5, 18, 3, 53, 39, 324, DateTimeKind.Utc), null, 53, 5, 3, 1 });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "Id", "CartId", "Quantity", "VariantId" },
                values: new object[] { 3, 1, 1, 48 });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "Id", "OrderId", "Quantity", "UnitPrice", "VariantId" },
                values: new object[,]
                {
                    { 1, 1, 1, 390000.00m, 43 },
                    { 2, 2, 2, 590000.00m, 275 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ProvinceId",
                table: "Addresses",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_VariantId",
                table: "CartItems",
                columns: new[] { "CartId", "VariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_VariantId",
                table: "CartItems",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_VariantId",
                table: "OrderDetails",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CouponId",
                table: "Orders",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingCarrierId",
                table: "Orders",
                column: "ShippingCarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ColorId",
                table: "ProductImages",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId_ColorId",
                table: "ProductImages",
                columns: new[] { "ProductId", "ColorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ColorId",
                table: "ProductVariants",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_SizeId",
                table: "ProductVariants",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_SKU",
                table: "ProductVariants",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_Name",
                table: "Provinces",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingCarriers_Code",
                table: "ShippingCarriers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_CarrierId_ProvinceId",
                table: "ShippingRates",
                columns: new[] { "CarrierId", "ProvinceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_ProvinceId",
                table: "ShippingRates",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_SizeGuides_SizeId",
                table: "SizeGuides",
                column: "SizeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_ProductId",
                table: "Wishlists",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId_ProductId",
                table: "Wishlists",
                columns: new[] { "UserId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ShippingRates");

            migrationBuilder.DropTable(
                name: "SizeGuides");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "ShippingCarriers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
