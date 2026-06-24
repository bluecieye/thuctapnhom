

using Microsoft.EntityFrameworkCore;

using BaseCore.Entities;

namespace BaseCore.Repository
{

    // ════════════════════════════════════════════════════════════
    // MYSQL DB CONTEXT
    // ════════════════════════════════════════════════════════════
    public class MySqlDbContext : DbContext
    {

        // ════════════════════════════════════════════════════════════
        // HÀM KHỞI TẠO
        // ════════════════════════════════════════════════════════════
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options) { }

        // ════════════════════════════════════════════════════════════
        // DBSET
        // ════════════════════════════════════════════════════════════

        public DbSet<User> Users => Set<User>();
        public DbSet<Address> Addresses => Set<Address>();

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Color> Colors => Set<Color>();
        public DbSet<Size> Sizes => Set<Size>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<SizeGuide> SizeGuides => Set<SizeGuide>();

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Coupon> Coupons => Set<Coupon>();

        public DbSet<Province> Provinces => Set<Province>();
        public DbSet<ShippingCarrier> ShippingCarriers => Set<ShippingCarrier>();
        public DbSet<ShippingRate> ShippingRates => Set<ShippingRate>();

        public DbSet<Banner> Banners => Set<Banner>();

        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();

        // ════════════════════════════════════════════════════════════
        // OVERRIDE (CẤU HÌNH MODEL)
        // ════════════════════════════════════════════════════════════

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // ════════════════════════════════════════════════════════════
            // NGƯỜI DÙNG
            // ════════════════════════════════════════════════════════════
            mb.Entity<User>(e =>
            {
                e.HasKey(x => x.Id);                                          
                e.Property(x => x.Username).HasMaxLength(50).IsRequired();    
                e.Property(x => x.Email).HasMaxLength(100).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(20);
                e.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
                e.Property(x => x.Salt).HasMaxLength(255);
                
                e.Property(x => x.Role).HasMaxLength(20).HasDefaultValue("Customer");
                
                e.HasIndex(x => x.Username).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();

                
                e.HasOne(x => x.Cart).WithOne(c => c.User)
                 .HasForeignKey<Cart>(c => c.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            
            
            mb.Entity<Address>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
                e.Property(x => x.Phone).HasMaxLength(20).IsRequired();
                e.Property(x => x.Street).HasMaxLength(255).IsRequired();
                e.Property(x => x.Ward).HasMaxLength(100).IsRequired();

                e.HasOne(x => x.User).WithMany(u => u.Addresses)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Province).WithMany(p => p.Addresses)
                 .HasForeignKey(x => x.ProvinceId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ════════════════════════════════════════════════════════════
            // DANH MỤC SẢN PHẨM (CATEGORY / COLOR / SIZE / PRODUCT / VARIANT / IMAGE / SIZE GUIDE)
            // ════════════════════════════════════════════════════════════
            mb.Entity<Category>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Gender).HasMaxLength(20);
                e.Property(x => x.Season).HasMaxLength(20);
            });

            mb.Entity<Color>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(50).IsRequired();
                e.Property(x => x.HexCode).HasMaxLength(10);  
            });

            mb.Entity<Size>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(20).IsRequired();
            });

            
            
            mb.Entity<Product>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Slug).HasMaxLength(220).IsRequired();
                
                e.HasIndex(x => x.Slug).IsUnique();
                e.Property(x => x.Description).HasMaxLength(2000);

                e.Property(x => x.BasePrice).HasPrecision(18, 2);

                e.HasOne(x => x.Category).WithMany(c => c.Products)
                 .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            
            
            mb.Entity<ProductVariant>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.SKU).HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.SKU).IsUnique();             
                e.Property(x => x.Price).HasPrecision(18, 2);

                e.HasOne(x => x.Product).WithMany(p => p.Variants)
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
                
                e.HasOne(x => x.Color).WithMany(c => c.Variants)
                 .HasForeignKey(x => x.ColorId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Size).WithMany(s => s.Variants)
                 .HasForeignKey(x => x.SizeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            
            
            mb.Entity<ProductImage>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.FileName).HasMaxLength(255).IsRequired();
                e.HasOne(x => x.Product).WithMany(p => p.Images)
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Color).WithMany(c => c.ProductImages)
                 .HasForeignKey(x => x.ColorId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.ProductId, x.ColorId });
            });

            
            
            mb.Entity<SizeGuide>(e =>
            {
                e.HasKey(x => x.Id);
                
                e.Property(x => x.Chest).HasPrecision(5, 2);
                e.Property(x => x.Waist).HasPrecision(5, 2);
                e.Property(x => x.Length).HasPrecision(5, 2);
                e.Property(x => x.Shoulder).HasPrecision(5, 2);
                e.HasOne(x => x.Size).WithMany(s => s.SizeGuides)
                 .HasForeignKey(x => x.SizeId)
                 .OnDelete(DeleteBehavior.Cascade);
                
                e.HasIndex(x => x.SizeId).IsUnique();
            });

            // ════════════════════════════════════════════════════════════
            // ĐƠN HÀNG
            // ════════════════════════════════════════════════════════════
            mb.Entity<Order>(e =>
            {
                e.HasKey(x => x.Id);
                
                e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Pending");
                e.Property(x => x.PaymentMethod).HasMaxLength(20).HasDefaultValue("COD");
                e.Property(x => x.TotalAmount).HasPrecision(18, 2);
                e.Property(x => x.ShippingFee).HasPrecision(18, 2);
                e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
                e.Property(x => x.ShippingAddress).HasMaxLength(500);
                e.Property(x => x.Note).HasMaxLength(500);

                
                e.HasOne(x => x.User).WithMany(u => u.Orders)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Coupon).WithMany(c => c.Orders)
                 .HasForeignKey(x => x.CouponId)
                 .OnDelete(DeleteBehavior.SetNull);
                
                e.HasOne(x => x.ShippingCarrier).WithMany(c => c.Orders)
                 .HasForeignKey(x => x.ShippingCarrierId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            
            
            mb.Entity<OrderDetail>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.UnitPrice).HasPrecision(18, 2);
                e.HasOne(x => x.Order).WithMany(o => o.OrderDetails)
                 .HasForeignKey(x => x.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Variant).WithMany(v => v.OrderDetails)
                 .HasForeignKey(x => x.VariantId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ════════════════════════════════════════════════════════════
            // GIỎ HÀNG
            // ════════════════════════════════════════════════════════════
            mb.Entity<Cart>(e =>
            {
                e.HasKey(x => x.Id);
            });

            
            
            mb.Entity<CartItem>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Cart).WithMany(c => c.CartItems)
                 .HasForeignKey(x => x.CartId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Variant).WithMany(v => v.CartItems)
                 .HasForeignKey(x => x.VariantId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.CartId, x.VariantId }).IsUnique();
            });

            // ════════════════════════════════════════════════════════════
            // MÃ GIẢM GIÁ
            // ════════════════════════════════════════════════════════════
            mb.Entity<Coupon>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();
                
                e.Property(x => x.DiscountType).HasMaxLength(20).HasDefaultValue("Percent");
                e.Property(x => x.DiscountValue).HasPrecision(18, 2);
                e.Property(x => x.MinOrderAmount).HasPrecision(18, 2);
                e.Property(x => x.MaxDiscount).HasPrecision(18, 2);
            });

            // ════════════════════════════════════════════════════════════
            // BANNER
            // ════════════════════════════════════════════════════════════
            mb.Entity<Banner>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.FileName).HasMaxLength(255).IsRequired();
                e.Property(x => x.LinkUrl).HasMaxLength(500);
            });

            // ════════════════════════════════════════════════════════════
            // WISHLIST & ĐÁNH GIÁ
            // ════════════════════════════════════════════════════════════
            mb.Entity<Wishlist>(e =>
            {
                e.HasKey(x => x.Id);
                
                e.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();
                e.HasOne(x => x.User).WithMany(u => u.Wishlists)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Product).WithMany(p => p.Wishlists)
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            
            
            mb.Entity<Review>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Comment).HasMaxLength(2000);
                e.Property(x => x.ImageUrl).HasMaxLength(500);
                
                e.HasOne(x => x.User).WithMany(u => u.Reviews)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
                
                e.HasOne(x => x.Product).WithMany(p => p.Reviews)
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ════════════════════════════════════════════════════════════
            // VẬN CHUYỂN (PROVINCE / CARRIER / RATE)
            // ════════════════════════════════════════════════════════════
            mb.Entity<Province>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Code).HasMaxLength(10);
                e.Property(x => x.Region).HasMaxLength(20);   
                e.HasIndex(x => x.Name).IsUnique();
            });

            mb.Entity<ShippingCarrier>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.Property(x => x.Code).HasMaxLength(20).IsRequired();
                e.Property(x => x.LogoFileName).HasMaxLength(255);
                e.HasIndex(x => x.Code).IsUnique();
            });

            
            
            mb.Entity<ShippingRate>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Fee).HasPrecision(18, 2);
                e.HasOne(x => x.Carrier).WithMany(c => c.Rates)
                 .HasForeignKey(x => x.CarrierId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Province).WithMany(p => p.ShippingRates)
                 .HasForeignKey(x => x.ProvinceId)
                 .OnDelete(DeleteBehavior.Cascade);
                
                e.HasIndex(x => new { x.CarrierId, x.ProvinceId }).IsUnique();
            });

            mb.Entity<NewsletterSubscriber>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Email).HasMaxLength(150).IsRequired();
                e.HasIndex(x => x.Email).IsUnique();
            });

            SeedData(mb);
        }

        // ════════════════════════════════════════════════════════════
        // DỮ LIỆU MẪU
        // ════════════════════════════════════════════════════════════

        private static void SeedData(ModelBuilder mb)
        {
// AUTO-GENERATED HasData blocks from sẽ.sql

            // Province: 34 rows — Code khớp với numeric code của provinces.open-api.vn/api/v2
            mb.Entity<Province>().HasData(
                new Province { Id = 1,  Name = "Hà Nội",       Code = "1",  Region = "Bắc"   },
                new Province { Id = 2,  Name = "Hồ Chí Minh",  Code = "79", Region = "Nam"   },
                new Province { Id = 3,  Name = "Hải Phòng",    Code = "31", Region = "Bắc"   },
                new Province { Id = 4,  Name = "Đà Nẵng",      Code = "48", Region = "Trung" },
                new Province { Id = 5,  Name = "Cần Thơ",      Code = "92", Region = "Nam"   },
                new Province { Id = 6,  Name = "Huế",          Code = "46", Region = "Trung" },
                new Province { Id = 7,  Name = "Lai Châu",     Code = "12", Region = "Bắc"   },
                new Province { Id = 8,  Name = "Điện Biên",    Code = "11", Region = "Bắc"   },
                new Province { Id = 9,  Name = "Sơn La",       Code = "14", Region = "Bắc"   },
                new Province { Id = 10, Name = "Lạng Sơn",     Code = "20", Region = "Bắc"   },
                new Province { Id = 11, Name = "Cao Bằng",     Code = "4",  Region = "Bắc"   },
                new Province { Id = 12, Name = "Tuyên Quang",  Code = "8",  Region = "Bắc"   },
                new Province { Id = 13, Name = "Lào Cai",      Code = "15", Region = "Bắc"   },
                new Province { Id = 14, Name = "Thái Nguyên",  Code = "19", Region = "Bắc"   },
                new Province { Id = 15, Name = "Phú Thọ",      Code = "25", Region = "Bắc"   },
                new Province { Id = 16, Name = "Bắc Ninh",     Code = "24", Region = "Bắc"   },
                new Province { Id = 17, Name = "Hưng Yên",     Code = "33", Region = "Bắc"   },
                new Province { Id = 18, Name = "Ninh Bình",    Code = "37", Region = "Bắc"   },
                new Province { Id = 19, Name = "Quảng Ninh",   Code = "22", Region = "Bắc"   },
                new Province { Id = 20, Name = "Thanh Hóa",    Code = "38", Region = "Bắc"   },
                new Province { Id = 21, Name = "Nghệ An",      Code = "40", Region = "Trung" },
                new Province { Id = 22, Name = "Hà Tĩnh",      Code = "42", Region = "Trung" },
                new Province { Id = 23, Name = "Quảng Trị",    Code = "44", Region = "Trung" },
                new Province { Id = 24, Name = "Quảng Ngãi",   Code = "51", Region = "Trung" },
                new Province { Id = 25, Name = "Gia Lai",       Code = "52", Region = "Trung" },
                new Province { Id = 26, Name = "Khánh Hòa",    Code = "56", Region = "Trung" },
                new Province { Id = 27, Name = "Lâm Đồng",     Code = "68", Region = "Trung" },
                new Province { Id = 28, Name = "Đắk Lắk",      Code = "66", Region = "Trung" },
                new Province { Id = 29, Name = "Đồng Nai",     Code = "75", Region = "Nam"   },
                new Province { Id = 30, Name = "Tây Ninh",     Code = "80", Region = "Nam"   },
                new Province { Id = 31, Name = "Vĩnh Long",    Code = "86", Region = "Nam"   },
                new Province { Id = 32, Name = "Đồng Tháp",    Code = "82", Region = "Nam"   },
                new Province { Id = 33, Name = "Cà Mau",       Code = "96", Region = "Nam"   },
                new Province { Id = 34, Name = "An Giang",     Code = "91", Region = "Nam"   }
            );

            // Category: 10 rows
            mb.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Áo khoác Nam", Gender = "Male", Season = "Fall-Winter" },
                new Category { Id = 2, Name = "Áo Nam", Gender = "Male", Season = "All" },
                new Category { Id = 3, Name = "Quần dài Nam", Gender = "Male", Season = "All" },
                new Category { Id = 4, Name = "Quần short Nam", Gender = "Male", Season = "Summer" },
                new Category { Id = 5, Name = "Áo Nữ", Gender = "Female", Season = "All" },
                new Category { Id = 6, Name = "Quần dài Nữ", Gender = "Female", Season = "All" },
                new Category { Id = 7, Name = "Quần short Nữ", Gender = "Female", Season = "Summer" },
                new Category { Id = 8, Name = "Áo khoác Nữ", Gender = "Female", Season = "Fall-Winter" },
                new Category { Id = 9, Name = "Váy Nữ", Gender = "Female", Season = "All" },
                new Category { Id = 10, Name = "Chân váy Nữ", Gender = "Female", Season = "All" }
            );

            // Color: 40 rows
            mb.Entity<Color>().HasData(
                new Color { Id = 1, Name = "Đen", HexCode = "#000000", Code = "DEN" },
                new Color { Id = 2, Name = "Trắng", HexCode = "#FFFFFF", Code = "TRG" },
                new Color { Id = 3, Name = "Trắng kem", HexCode = "#FFFDD0", Code = "TKM" },
                new Color { Id = 4, Name = "Xám nhạt", HexCode = "#D3D3D3", Code = "XNH" },
                new Color { Id = 5, Name = "Xám", HexCode = "#808080", Code = "XAM" },
                new Color { Id = 6, Name = "Xám đậm", HexCode = "#404040", Code = "XDM" },
                new Color { Id = 7, Name = "Than chì", HexCode = "#36454F", Code = "TCH" },
                new Color { Id = 8, Name = "Kem", HexCode = "#F5F5DC", Code = "KEM" },
                new Color { Id = 9, Name = "Be", HexCode = "#D8C9A8", Code = "BE" },
                new Color { Id = 10, Name = "Nude", HexCode = "#E3BC9A", Code = "NUD" },
                new Color { Id = 11, Name = "Camel", HexCode = "#C19A6B", Code = "CML" },
                new Color { Id = 12, Name = "Khaki", HexCode = "#C3B091", Code = "KHK" },
                new Color { Id = 13, Name = "Nâu nhạt", HexCode = "#A0826D", Code = "NNH" },
                new Color { Id = 14, Name = "Nâu", HexCode = "#8B4513", Code = "NAU" },
                new Color { Id = 15, Name = "Nâu đậm", HexCode = "#5D3A1A", Code = "NDM" },
                new Color { Id = 16, Name = "Socola", HexCode = "#7B3F00", Code = "SCL" },
                new Color { Id = 17, Name = "Đỏ tươi", HexCode = "#FF0000", Code = "DTU" },
                new Color { Id = 18, Name = "Đỏ", HexCode = "#E74C3C", Code = "DO" },
                new Color { Id = 19, Name = "Đỏ gạch", HexCode = "#B22222", Code = "DGA" },
                new Color { Id = 20, Name = "Đỏ đô", HexCode = "#800020", Code = "DDO" },
                new Color { Id = 21, Name = "Hồng nhạt", HexCode = "#FFB6C1", Code = "HNH" },
                new Color { Id = 22, Name = "Hồng phấn", HexCode = "#F4C2C2", Code = "HPH" },
                new Color { Id = 23, Name = "Hồng", HexCode = "#FF69B4", Code = "HNG" },
                new Color { Id = 24, Name = "Hồng cánh sen", HexCode = "#E91E63", Code = "HCS" },
                new Color { Id = 25, Name = "Cam đào", HexCode = "#FFCBA4", Code = "CDA" },
                new Color { Id = 26, Name = "Cam", HexCode = "#FF7F50", Code = "CAM" },
                new Color { Id = 27, Name = "Vàng nhạt", HexCode = "#FFFACD", Code = "VNH" },
                new Color { Id = 28, Name = "Vàng", HexCode = "#FFD700", Code = "VAN" },
                new Color { Id = 29, Name = "Vàng mù tạt", HexCode = "#D2A007", Code = "VMT" },
                new Color { Id = 30, Name = "Xanh mint", HexCode = "#98FF98", Code = "XMT" },
                new Color { Id = 31, Name = "Xanh lá", HexCode = "#2ECC71", Code = "XLA" },
                new Color { Id = 32, Name = "Xanh olive", HexCode = "#808000", Code = "XOL" },
                new Color { Id = 33, Name = "Xanh rêu", HexCode = "#556B2F", Code = "XRE" },
                new Color { Id = 34, Name = "Xanh da trời", HexCode = "#87CEEB", Code = "XDT" },
                new Color { Id = 35, Name = "Xanh denim", HexCode = "#1560BD", Code = "XDN" },
                new Color { Id = 36, Name = "Xanh dương", HexCode = "#0066CC", Code = "XDG" },
                new Color { Id = 37, Name = "Xanh navy", HexCode = "#1B2A4E", Code = "XNV" },
                new Color { Id = 38, Name = "Xanh ngọc", HexCode = "#0ABAB5", Code = "XNC" },
                new Color { Id = 39, Name = "Tím lavender", HexCode = "#E6E6FA", Code = "TLV" },
                new Color { Id = 40, Name = "Tím", HexCode = "#9B59B6", Code = "TIM" }
            );

            // Size: 8 rows
            mb.Entity<Size>().HasData(
                new Size { Id = 1, Name = "XS", SortOrder = 10 },
                new Size { Id = 2, Name = "S", SortOrder = 20 },
                new Size { Id = 3, Name = "M", SortOrder = 30 },
                new Size { Id = 4, Name = "L", SortOrder = 40 },
                new Size { Id = 5, Name = "XL", SortOrder = 50 },
                new Size { Id = 6, Name = "2XL", SortOrder = 60 },
                new Size { Id = 7, Name = "3XL", SortOrder = 70 },
                new Size { Id = 8, Name = "4XL", SortOrder = 80 }
            );

            // SizeGuide: 8 rows
            mb.Entity<SizeGuide>().HasData(
                new SizeGuide { Id = 1, Chest = 76.00m, Waist = 60.00m, Length = 63.00m, Shoulder = 37.00m, SizeId = 1 },
                new SizeGuide { Id = 2, Chest = 82.00m, Waist = 64.00m, Length = 65.00m, Shoulder = 39.00m, SizeId = 2 },
                new SizeGuide { Id = 3, Chest = 88.00m, Waist = 68.00m, Length = 67.00m, Shoulder = 41.00m, SizeId = 3 },
                new SizeGuide { Id = 4, Chest = 94.00m, Waist = 74.00m, Length = 69.00m, Shoulder = 43.00m, SizeId = 4 },
                new SizeGuide { Id = 5, Chest = 100.00m, Waist = 80.00m, Length = 71.00m, Shoulder = 45.00m, SizeId = 5 },
                new SizeGuide { Id = 6, Chest = 106.00m, Waist = 86.00m, Length = 73.00m, Shoulder = 47.00m, SizeId = 6 },
                new SizeGuide { Id = 7, Chest = 112.00m, Waist = 92.00m, Length = 75.00m, Shoulder = 49.00m, SizeId = 7 },
                new SizeGuide { Id = 8, Chest = 118.00m, Waist = 98.00m, Length = 77.00m, Shoulder = 51.00m, SizeId = 8 }
            );

            // ShippingCarrier: 3 rows
            mb.Entity<ShippingCarrier>().HasData(
                new ShippingCarrier { Id = 1, Name = "Giao Hàng Nhanh", Code = "GHN", LogoFileName = "ghn.png", IsActive = true },
                new ShippingCarrier { Id = 2, Name = "Giao Hàng Tiết Kiệm", Code = "GHTK", LogoFileName = "ghtk.png", IsActive = true },
                new ShippingCarrier { Id = 3, Name = "Viettel Post", Code = "VTP", LogoFileName = "vtp.png", IsActive = true }
            );

            // ShippingRate: 102 rows
            mb.Entity<ShippingRate>().HasData(
                new ShippingRate { Id = 1, CarrierId = 1, ProvinceId = 1, Fee = 15000.00m, EstimatedDays = 1 },
                new ShippingRate { Id = 2, CarrierId = 1, ProvinceId = 2, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 3, CarrierId = 1, ProvinceId = 3, Fee = 20000.00m, EstimatedDays = 1 },
                new ShippingRate { Id = 4, CarrierId = 1, ProvinceId = 4, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 5, CarrierId = 1, ProvinceId = 5, Fee = 40000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 6, CarrierId = 1, ProvinceId = 6, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 7, CarrierId = 1, ProvinceId = 7, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 8, CarrierId = 1, ProvinceId = 8, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 9, CarrierId = 1, ProvinceId = 9, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 10, CarrierId = 1, ProvinceId = 10, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 11, CarrierId = 1, ProvinceId = 11, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 12, CarrierId = 1, ProvinceId = 12, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 13, CarrierId = 1, ProvinceId = 13, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 14, CarrierId = 1, ProvinceId = 14, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 15, CarrierId = 1, ProvinceId = 15, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 16, CarrierId = 1, ProvinceId = 16, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 17, CarrierId = 1, ProvinceId = 17, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 18, CarrierId = 1, ProvinceId = 18, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 19, CarrierId = 1, ProvinceId = 19, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 20, CarrierId = 1, ProvinceId = 20, Fee = 25000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 21, CarrierId = 1, ProvinceId = 21, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 22, CarrierId = 1, ProvinceId = 22, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 23, CarrierId = 1, ProvinceId = 23, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 24, CarrierId = 1, ProvinceId = 24, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 25, CarrierId = 1, ProvinceId = 25, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 26, CarrierId = 1, ProvinceId = 26, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 27, CarrierId = 1, ProvinceId = 27, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 28, CarrierId = 1, ProvinceId = 28, Fee = 35000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 29, CarrierId = 1, ProvinceId = 29, Fee = 40000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 30, CarrierId = 1, ProvinceId = 30, Fee = 40000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 31, CarrierId = 1, ProvinceId = 31, Fee = 40000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 32, CarrierId = 1, ProvinceId = 32, Fee = 40000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 33, CarrierId = 1, ProvinceId = 33, Fee = 40000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 34, CarrierId = 1, ProvinceId = 34, Fee = 40000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 35, CarrierId = 2, ProvinceId = 1, Fee = 13000.00m, EstimatedDays = 1 },
                new ShippingRate { Id = 36, CarrierId = 2, ProvinceId = 2, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 37, CarrierId = 2, ProvinceId = 3, Fee = 18000.00m, EstimatedDays = 1 },
                new ShippingRate { Id = 38, CarrierId = 2, ProvinceId = 4, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 39, CarrierId = 2, ProvinceId = 5, Fee = 38000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 40, CarrierId = 2, ProvinceId = 6, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 41, CarrierId = 2, ProvinceId = 7, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 42, CarrierId = 2, ProvinceId = 8, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 43, CarrierId = 2, ProvinceId = 9, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 44, CarrierId = 2, ProvinceId = 10, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 45, CarrierId = 2, ProvinceId = 11, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 46, CarrierId = 2, ProvinceId = 12, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 47, CarrierId = 2, ProvinceId = 13, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 48, CarrierId = 2, ProvinceId = 14, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 49, CarrierId = 2, ProvinceId = 15, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 50, CarrierId = 2, ProvinceId = 16, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 51, CarrierId = 2, ProvinceId = 17, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 52, CarrierId = 2, ProvinceId = 18, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 53, CarrierId = 2, ProvinceId = 19, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 54, CarrierId = 2, ProvinceId = 20, Fee = 23000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 55, CarrierId = 2, ProvinceId = 21, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 56, CarrierId = 2, ProvinceId = 22, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 57, CarrierId = 2, ProvinceId = 23, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 58, CarrierId = 2, ProvinceId = 24, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 59, CarrierId = 2, ProvinceId = 25, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 60, CarrierId = 2, ProvinceId = 26, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 61, CarrierId = 2, ProvinceId = 27, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 62, CarrierId = 2, ProvinceId = 28, Fee = 33000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 63, CarrierId = 2, ProvinceId = 29, Fee = 38000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 64, CarrierId = 2, ProvinceId = 30, Fee = 38000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 65, CarrierId = 2, ProvinceId = 31, Fee = 38000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 66, CarrierId = 2, ProvinceId = 32, Fee = 38000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 67, CarrierId = 2, ProvinceId = 33, Fee = 38000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 68, CarrierId = 2, ProvinceId = 34, Fee = 38000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 69, CarrierId = 3, ProvinceId = 1, Fee = 18000.00m, EstimatedDays = 1 },
                new ShippingRate { Id = 70, CarrierId = 3, ProvinceId = 2, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 71, CarrierId = 3, ProvinceId = 3, Fee = 23000.00m, EstimatedDays = 1 },
                new ShippingRate { Id = 72, CarrierId = 3, ProvinceId = 4, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 73, CarrierId = 3, ProvinceId = 5, Fee = 43000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 74, CarrierId = 3, ProvinceId = 6, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 75, CarrierId = 3, ProvinceId = 7, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 76, CarrierId = 3, ProvinceId = 8, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 77, CarrierId = 3, ProvinceId = 9, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 78, CarrierId = 3, ProvinceId = 10, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 79, CarrierId = 3, ProvinceId = 11, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 80, CarrierId = 3, ProvinceId = 12, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 81, CarrierId = 3, ProvinceId = 13, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 82, CarrierId = 3, ProvinceId = 14, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 83, CarrierId = 3, ProvinceId = 15, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 84, CarrierId = 3, ProvinceId = 16, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 85, CarrierId = 3, ProvinceId = 17, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 86, CarrierId = 3, ProvinceId = 18, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 87, CarrierId = 3, ProvinceId = 19, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 88, CarrierId = 3, ProvinceId = 20, Fee = 28000.00m, EstimatedDays = 2 },
                new ShippingRate { Id = 89, CarrierId = 3, ProvinceId = 21, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 90, CarrierId = 3, ProvinceId = 22, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 91, CarrierId = 3, ProvinceId = 23, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 92, CarrierId = 3, ProvinceId = 24, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 93, CarrierId = 3, ProvinceId = 25, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 94, CarrierId = 3, ProvinceId = 26, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 95, CarrierId = 3, ProvinceId = 27, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 96, CarrierId = 3, ProvinceId = 28, Fee = 38000.00m, EstimatedDays = 3 },
                new ShippingRate { Id = 97, CarrierId = 3, ProvinceId = 29, Fee = 43000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 98, CarrierId = 3, ProvinceId = 30, Fee = 43000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 99, CarrierId = 3, ProvinceId = 31, Fee = 43000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 100, CarrierId = 3, ProvinceId = 32, Fee = 43000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 101, CarrierId = 3, ProvinceId = 33, Fee = 43000.00m, EstimatedDays = 4 },
                new ShippingRate { Id = 102, CarrierId = 3, ProvinceId = 34, Fee = 43000.00m, EstimatedDays = 4 }
            );

            // Coupon: 2 rows
            mb.Entity<Coupon>().HasData(
                new Coupon { Id = 1, Code = "AOPHONG",   DiscountType = "Percent", DiscountValue = 10.00m, MinOrderAmount = 1000000.00m, MaxDiscount = 100000.00m, StartDate = new DateTime(2026, 5, 18, 2, 42, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2026, 5, 23, 2, 43, 0, 0, DateTimeKind.Utc), UsageLimit = 5,  UsedCount = 1, IsActive = true,  IsNewsletterCoupon = false },
                new Coupon { Id = 2, Code = "WELCOME10", DiscountType = "Percent", DiscountValue = 10.00m, MinOrderAmount = 0m,          MaxDiscount = null,       StartDate = new DateTime(2026, 1, 1,  0, 0,  0, 0, DateTimeKind.Utc), EndDate = new DateTime(2030, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), UsageLimit = 0,  UsedCount = 0, IsActive = true,  IsNewsletterCoupon = true  }
            );

            // Banner: 0 rows — skipped

            // User: 4 rows
            mb.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Email = "admin@basecore.com", Phone = "", PasswordHash = "0OZgMfYMryrBVtxJSbwo99nZnU7nMbomE3lJE6D90Fk=", Salt = "S2lUlFCjGhgFF3e3273/ig==", Role = "Admin", IsActive = true, CreatedAt = new DateTime(2026, 5, 14, 16, 38, 50, 351, DateTimeKind.Utc) },
                new User { Id = 2, Username = "hoang", Email = "gaga@gmail.com", Phone = "", PasswordHash = "DZwp/omksPF5eut49Dqqu6Q+IJDu9JBtADlFrUGy98w=", Salt = "Pc+b7h/O6GwB4uU8f7jffw==", Role = "WarehouseStaff", IsActive = true, CreatedAt = new DateTime(2026, 5, 18, 2, 56, 45, 923, DateTimeKind.Utc) },
                new User { Id = 3, Username = "nhat", Email = "123@1.com", Phone = "", PasswordHash = "LK71lL5grGQ60TojQGRpG5U8nLk+44+05XLKYW9hqBs=", Salt = "46r/zfx9TSZYtwLb8oHw2g==", Role = "Marketing", IsActive = true, CreatedAt = new DateTime(2026, 5, 18, 2, 57, 55, 145, DateTimeKind.Utc) },
                new User { Id = 4, Username = "user", Email = "user@gmail.com", Phone = "", PasswordHash = "kpn72D1LswdgaQAZ36E1rDMtviDrd++VKN3Gw8hjbdQ=", Salt = "5VUKC78BYlplNthDspI0Ig==", Role = "Customer", IsActive = true, CreatedAt = new DateTime(2026, 5, 18, 2, 59, 20, 502, DateTimeKind.Utc) }
            );

            // Product: 55 rows
            mb.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Áo Ba Lỗ Nam", Slug = "ao-ba-lo-nam", Description = "Áo ba lỗ nam cotton rib co giãn, form ôm vừa, thoáng mát, phù hợp tập gym hoặc mặc hàng ngày.", BasePrice = 290000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 500, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 2, Name = "Áo Ba Lỗ Nữ", Slug = "ao-ba-lo-nu", Description = "Áo ba lỗ nữ cotton rib mềm mại, form ôm vừa, phù hợp mặc trong hoặc layer.", BasePrice = 290000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 503, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 3, Name = "Áo Gi Lê Nam", Slug = "ao-gi-le-nam", Description = "Áo gi lê nam vải tweed cao cấp, form slim, phù hợp mặc công sở hoặc dạo phố.", BasePrice = 690000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 503, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 4, Name = "Áo Gi Lê Nữ", Slug = "ao-gi-le-nu", Description = "Áo gi lê nữ thiết kế hiện đại, không tay, layer cùng áo sơ mi hoặc áo thun.", BasePrice = 650000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 503, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 5, Name = "Áo Khoác Blazer Denim Nữ", Slug = "ao-khoac-blazer-denim-nu", Description = "Blazer denim nữ oversized, vải denim mềm 11oz, phong cách casual-chic.", BasePrice = 890000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 506, DateTimeKind.Utc), CategoryId = 8 },
                new Product { Id = 6, Name = "Áo Khoác Bomber Nam", Slug = "ao-khoac-bomber-nam", Description = "Áo khoác bomber nam chất liệu polyester nhẹ, bo gấu rib co giãn, phong cách street.", BasePrice = 790000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 506, DateTimeKind.Utc), CategoryId = 1 },
                new Product { Id = 7, Name = "Áo Khoác Cotton Nữ", Slug = "ao-khoac-cotton-nu", Description = "Áo khoác cotton nữ mỏng nhẹ, không lót, phù hợp mùa xuân hè, dáng rộng thoải mái.", BasePrice = 750000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 506, DateTimeKind.Utc), CategoryId = 8 },
                new Product { Id = 8, Name = "Áo Khoác Denim Nữ", Slug = "ao-khoac-denim-nu", Description = "Áo khoác jean nữ dáng crop, vải denim 12oz wash nhẹ, phom casual everyday.", BasePrice = 790000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 506, DateTimeKind.Utc), CategoryId = 8 },
                new Product { Id = 9, Name = "Áo Cổ Đổ Nữ", Slug = "ao-co-do-nu", Description = "Áo kiểu cổ đổ nữ vải cotton mềm, form suông thanh lịch, phù hợp đi làm và dạo phố.", BasePrice = 390000.00m, CreatedAt = new DateTime(2026, 5, 15, 8, 38, 41, 532, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 10, Name = "Áo Khoác Linen Nam", Slug = "ao-khoac-linen-nam", Description = "Áo khoác linen nữ nhẹ thoáng, dáng rộng, cài cúc 3 hạt, thích hợp mùa hè.", BasePrice = 890000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), CategoryId = 1 },
                new Product { Id = 11, Name = "Áo Khoác Thể Thao Nữ", Slug = "ao-khoac-the-thao-nu", Description = "Áo khoác thể thao nữ chất liệu polyester co giãn 4 chiều, thấm hút tốt.", BasePrice = 690000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), CategoryId = 8 },
                new Product { Id = 12, Name = "Áo Khoác Tuxedo Nam", Slug = "ao-khoac-tuxedo-nam", Description = "Áo khoác tuxedo nam vải polyester bóng, ve áo lapel satin, phong cách dự tiệc.", BasePrice = 1290000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), CategoryId = 1 },
                new Product { Id = 13, Name = "Áo Kiểu Cổ Điểm Nhún Nữ", Slug = "ao-kieu-co-diem-nhun-nu", Description = "Áo kiểu nữ cổ tròn viền ren điểm nhún, vải chiffon nhẹ bay, thanh lịch.", BasePrice = 350000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 14, Name = "Áo Pin Tucked Blouse Nữ", Slug = "ao-pin-tucked-blouse-nu", Description = "Áo blouse nữ pin tucked cổ đứng, vải cotton poplin, form suông chỉn chu.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 15, Name = "Áo Sơ Mi Chiffon Nữ", Slug = "ao-so-mi-chiffon-nu", Description = "Áo sơ mi chiffon trong suốt thanh lịch, tay dài, cổ V nhẹ nhàng, phù hợp đi làm.", BasePrice = 420000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 16, Name = "Áo Sơ Mi Cổ 2 Ve Nữ", Slug = "ao-so-mi-co-2-ve-nu", Description = "Áo sơ mi nữ cổ ve đôi thanh lịch, vải lụa matte mềm rủ, phù hợp công sở.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 510, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 17, Name = "Áo Sơ Mi Khoác Unisex", Slug = "ao-so-mi-khoac", Description = "Áo sơ mi form rộng mặc khoác, vải cotton nhẹ, phong cách unisex casual.", BasePrice = 450000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 18, Name = "Áo Sơ Mi Linen Nữ", Slug = "ao-so-mi-linen-nu", Description = "Áo sơ mi linen nữ thoáng mát, tay dài gập xắn được, phù hợp mùa hè và du lịch.", BasePrice = 550000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 19, Name = "Áo Sơ Mi Nữ Buộc Dây", Slug = "ao-so-mi-nu-buoc-day", Description = "Áo sơ mi nữ thắt nơ buộc eo, cổ V nhẹ, vải voile mỏng nhẹ bay bướm.", BasePrice = 380000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 20, Name = "Áo Thun Nữ", Slug = "ao-thun-nu", Description = "Áo thun nữ cotton 200gsm, cổ tròn, form boxy nhẹ nhàng, co giãn 2 chiều.", BasePrice = 290000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), CategoryId = 5 },
                new Product { Id = 21, Name = "Áo Trench Coat Nữ", Slug = "ao-trench-coat-nu", Description = "Trench coat nữ double-breasted, dây thắt eo, vải gabardine cotton, dáng dài thanh lịch.", BasePrice = 1590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), CategoryId = 8 },
                new Product { Id = 22, Name = "Chân Váy Đắp Chéo Nữ", Slug = "chan-vay-dap-cheo-nu", Description = "Chân váy đắp chéo bất đối xứng, vải lụa matte rủ đẹp, tôn dáng.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 513, DateTimeKind.Utc), CategoryId = 10 },
                new Product { Id = 23, Name = "Chân Váy Jersey Nữ", Slug = "chan-vay-jersey-nu", Description = "Chân váy jersey co giãn 2 chiều, cạp chun thoải mái, thích hợp casual đến đi làm.", BasePrice = 390000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), CategoryId = 10 },
                new Product { Id = 24, Name = "Chân Váy Maxi Nữ", Slug = "chan-vay-maxi-nu", Description = "Chân váy maxi dài sát gót, vải thun co giãn, dáng ôm thon gọn tôn dáng.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), CategoryId = 10 },
                new Product { Id = 25, Name = "Chân Váy Xếp Rủ Nữ", Slug = "chan-vay-xep-ru-nu", Description = "Chân váy xếp ly rủ nhẹ nhàng, lưng thun, vải chiffon mềm bay, mang nét nữ tính.", BasePrice = 450000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), CategoryId = 10 },
                new Product { Id = 26, Name = "Hoodie Không Tay Nam", Slug = "hoodie-khong-tay-nam", Description = "Áo hoodie không tay (vest hoodie) nỉ bông, mũ 2 lớp, túi kangaroo, phong cách streetwear.", BasePrice = 590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 27, Name = "Hoodie Nỉ Nam", Slug = "hoodie-ni-nam", Description = "Hoodie nỉ bông dày 380gsm, mũ 2 lớp, túi kangaroo, form oversize thoải mái.", BasePrice = 590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 516, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 28, Name = "Hoodie Khóa Kéo Nam", Slug = "hoodie-khoa-keo-nam", Description = "Hoodie zip-up nỉ dày, khóa kéo YKK chắc chắn, tay rib, phù hợp lớp ngoài mùa lạnh.", BasePrice = 650000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 29, Name = "Jumper Nam", Slug = "jumper-nam", Description = "Áo jumper len mỏng cổ tròn, form suông nhẹ, layer tiện dụng mùa thu đông.", BasePrice = 550000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 30, Name = "Polo Nam", Slug = "polo-nam", Description = "Áo polo nam cotton piqué 220gsm, cổ bẻ rib 3 cúc, form regular fit lịch sự.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 31, Name = "Quần Dài Cạp Chun Nữ", Slug = "quan-dai-cap-chun-nu", Description = "Quần dài nữ cạp chun co giãn, vải thun lạnh mịn, form suông thoải mái.", BasePrice = 450000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), CategoryId = 6 },
                new Product { Id = 32, Name = "Quần Dài Dây Rút Nữ", Slug = "quan-dai-day-rut-nu", Description = "Quần dài nam dây rút điều chỉnh eo, vải thun gió nhẹ, 2 túi sườn tiện dụng.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), CategoryId = 6 },
                new Product { Id = 33, Name = "Quần Dài Satin Nữ", Slug = "quan-dai-satin-nu", Description = "Quần satin nữ bóng mượt, cạp chun ẩn, form suông thanh lịch, thích hợp đi làm và dạ tiệc.", BasePrice = 590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 520, DateTimeKind.Utc), CategoryId = 6 },
                new Product { Id = 34, Name = "Quần Jean Nam", Slug = "quan-jean-nam", Description = "Quần jean nam denim 12oz, phom slim tapered, wash nhẹ tự nhiên, 5 túi chuẩn.", BasePrice = 690000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 523, DateTimeKind.Utc), CategoryId = 3 },
                new Product { Id = 35, Name = "Quần Jean Nữ", Slug = "quan-jean-nu", Description = "Quần jean nữ lưng cao, denim co giãn 2 chiều, phom skinny tôn dáng.", BasePrice = 650000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 523, DateTimeKind.Utc), CategoryId = 6 },
                new Product { Id = 36, Name = "Quần Jean Nữ Rách Gối", Slug = "quan-jean-nu-rach-goi", Description = "Quần jean nữ ripped knee, lưng cao, dáng skinny năng động phong cách.", BasePrice = 690000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 526, DateTimeKind.Utc), CategoryId = 6 },
                new Product { Id = 37, Name = "Quần Jogger Nam", Slug = "quan-jogger-nam", Description = "Quần jogger nam cotton french terry, gấu bo rib, túi zip tiện dụng, mặc gym hoặc casual.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 526, DateTimeKind.Utc), CategoryId = 3 },
                new Product { Id = 38, Name = "Quần Ống Sương Muslin Nữ", Slug = "quan-ong-suong-muslin-nu", Description = "Quần ống suông vải muslin cao cấp, nhẹ bay tự nhiên, cạp chun ẩn, phong cách boho thanh lịch.", BasePrice = 590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 526, DateTimeKind.Utc), CategoryId = 6 },
                new Product { Id = 39, Name = "Quần Short Jean Nam", Slug = "quan-short-jean-nam", Description = "Quần short jean nam denim 10oz, gấu xước nhẹ, phom regular, túi hộp tiện dụng.", BasePrice = 450000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 526, DateTimeKind.Utc), CategoryId = 4 },
                new Product { Id = 40, Name = "Quần Short Nỉ Nam", Slug = "quan-short-ni-nam", Description = "Quần short nỉ bông dày, cạp thun lưng, túi zip, mặc tại nhà hoặc gym thoải mái.", BasePrice = 390000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), CategoryId = 4 },
                new Product { Id = 41, Name = "Quần Short Túi Hộp Nam", Slug = "quan-short-tui-hop-nam", Description = "Quần short nam 4 túi hộp kiểu cargo, vải kaki dày dặn, phong cách outdoor.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), CategoryId = 4 },
                new Product { Id = 42, Name = "Quần Tây Nam", Slug = "quan-tay-nam", Description = "Quần tây nam vải polyester blend, form slim straight, li phẳng, thích hợp đi làm và sự kiện.", BasePrice = 690000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), CategoryId = 3 },
                new Product { Id = 43, Name = "Sơ Mi Chống Nhăn Nam", Slug = "so-mi-chong-nhan-nam", Description = "Sơ mi chống nhăn vải cotton pha polyester, giữ phẳng cả ngày, phù hợp đi làm.", BasePrice = 690000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 44, Name = "Sơ Mi Cotton Nam", Slug = "so-mi-cotton-nam", Description = "Sơ mi cotton 100% thoáng mát, form regular, cổ button-down, phù hợp mặc quanh năm.", BasePrice = 590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 45, Name = "Sơ Mi Jean Nam", Slug = "so-mi-jean-nam", Description = "Áo sơ mi jean denim nhẹ 8oz, cổ đứng button-down, style western casual.", BasePrice = 590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 46, Name = "Áo Sweater Nam", Slug = "sweater-nam", Description = "Sweater len mịn cổ tròn, đan dệt mịn, form rộng ấm áp, phù hợp thu đông.", BasePrice = 590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 47, Name = "T-Shirt Nam", Slug = "tshirt-nam", Description = "T-shirt cotton 220gsm cổ tròn, form regular clean, item cơ bản không thể thiếu.", BasePrice = 290000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 530, DateTimeKind.Utc), CategoryId = 2 },
                new Product { Id = 48, Name = "Váy 2 Dây Xếp Nếp Nữ", Slug = "vay-2-day-xep-nep-nu", Description = "Váy 2 dây xếp nếp ngực vải lụa matte mịn, dáng suông dài qua gối, nữ tính thanh lịch.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), CategoryId = 9 },
                new Product { Id = 49, Name = "Váy Chiffon Nữ", Slug = "vay-chiffon-nu", Description = "Váy chiffon nữ bay nhẹ, tầng xếp nếp nhẹ nhàng, phù hợp dạo phố và sự kiện.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), CategoryId = 9 },
                new Product { Id = 50, Name = "Váy Dây Rút Cotton Nữ", Slug = "vay-day-rut-cotton-nu", Description = "Váy cotton dây rút cổ, tay phồng điểm nhún, vải co giãn nhẹ mặc thoải mái mùa hè.", BasePrice = 390000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), CategoryId = 9 },
                new Product { Id = 51, Name = "Váy Midi Nữ", Slug = "vay-midi-nu", Description = "Váy midi dáng bút chì ôm nhẹ, vải thun gân mịn, dài qua đầu gối tôn dáng.", BasePrice = 490000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), CategoryId = 9 },
                new Product { Id = 52, Name = "Váy Satin Nữ", Slug = "vay-satin-nu", Description = "Váy satin bóng mượt, cổ V thanh lịch, dáng suông thân thiện với mọi vóc dáng.", BasePrice = 590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 533, DateTimeKind.Utc), CategoryId = 9 },
                new Product { Id = 53, Name = "Váy Sơ Mi Nữ", Slug = "vay-so-mi-nu", Description = "Váy sơ mi dáng shirt dress, cổ bẻ, hàng cúc ngực, thắt lưng kèm, phong cách preppy.", BasePrice = 590000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 536, DateTimeKind.Utc), CategoryId = 9 },
                new Product { Id = 54, Name = "Váy Sơ Mi Có Thắt Lưng Nữ", Slug = "vay-so-mi-co-that-lung-nu", Description = "Váy sơ mi kèm thắt lưng vải, dáng midi thắt eo tôn dáng, vải cotton nhẹ mịn.", BasePrice = 650000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 536, DateTimeKind.Utc), CategoryId = 9 },
                new Product { Id = 55, Name = "Váy Tunic Nữ", Slug = "vay-tunic-nu", Description = "Váy tunic dáng suông dài qua hông, vải linen thoáng mát, phù hợp đi biển và casual.", BasePrice = 450000.00m, CreatedAt = new DateTime(2026, 5, 14, 16, 44, 8, 536, DateTimeKind.Utc), CategoryId = 9 }
            );

            // ProductVariant: 284 rows
            mb.Entity<ProductVariant>().HasData(
                new ProductVariant { Id = 1, SKU = "ABLNAM-DEN-S", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 1, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 2, SKU = "ABLNAM-DEN-M", Price = 290000.00m, Stock = 20, ReservedStock = 0, ProductId = 1, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 3, SKU = "ABLNAM-DEN-L", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 1, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 4, SKU = "ABLNAM-DEN-XL", Price = 290000.00m, Stock = 10, ReservedStock = 0, ProductId = 1, ColorId = 1, SizeId = 5 },
                new ProductVariant { Id = 5, SKU = "ABLNAM-KEM-S", Price = 290000.00m, Stock = 12, ReservedStock = 0, ProductId = 1, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 6, SKU = "ABLNAM-KEM-M", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 1, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 7, SKU = "ABLNAM-KEM-L", Price = 290000.00m, Stock = 12, ReservedStock = 0, ProductId = 1, ColorId = 8, SizeId = 4 },
                new ProductVariant { Id = 8, SKU = "ABLNAM-KEM-XL", Price = 290000.00m, Stock = 8, ReservedStock = 0, ProductId = 1, ColorId = 8, SizeId = 5 },
                new ProductVariant { Id = 9, SKU = "ABLNU-DEN-S", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 2, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 10, SKU = "ABLNU-DEN-M", Price = 290000.00m, Stock = 20, ReservedStock = 0, ProductId = 2, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 11, SKU = "ABLNU-DEN-L", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 2, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 12, SKU = "ABLNU-BE-S", Price = 290000.00m, Stock = 12, ReservedStock = 0, ProductId = 2, ColorId = 9, SizeId = 2 },
                new ProductVariant { Id = 13, SKU = "ABLNU-BE-M", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 2, ColorId = 9, SizeId = 3 },
                new ProductVariant { Id = 14, SKU = "ABLNU-BE-L", Price = 290000.00m, Stock = 10, ReservedStock = 0, ProductId = 2, ColorId = 9, SizeId = 4 },
                new ProductVariant { Id = 15, SKU = "ABLNU-TRG-S", Price = 290000.00m, Stock = 12, ReservedStock = 0, ProductId = 2, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 16, SKU = "ABLNU-TRG-M", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 2, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 17, SKU = "ABLNU-TRG-L", Price = 290000.00m, Stock = 10, ReservedStock = 0, ProductId = 2, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 18, SKU = "AGLN-DEN-S", Price = 690000.00m, Stock = 8, ReservedStock = 0, ProductId = 3, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 19, SKU = "AGLN-DEN-M", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 3, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 20, SKU = "AGLN-DEN-L", Price = 690000.00m, Stock = 10, ReservedStock = 0, ProductId = 3, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 21, SKU = "AGLNU-DEN-S", Price = 650000.00m, Stock = 10, ReservedStock = 0, ProductId = 4, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 22, SKU = "AGLNU-DEN-M", Price = 650000.00m, Stock = 12, ReservedStock = 0, ProductId = 4, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 23, SKU = "AGLNU-DEN-L", Price = 650000.00m, Stock = 8, ReservedStock = 0, ProductId = 4, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 24, SKU = "AGLNU-CAM-S", Price = 650000.00m, Stock = 8, ReservedStock = 0, ProductId = 4, ColorId = 11, SizeId = 2 },
                new ProductVariant { Id = 25, SKU = "AGLNU-CAM-M", Price = 650000.00m, Stock = 10, ReservedStock = 0, ProductId = 4, ColorId = 11, SizeId = 3 },
                new ProductVariant { Id = 26, SKU = "AGLNU-CAM-L", Price = 650000.00m, Stock = 7, ReservedStock = 0, ProductId = 4, ColorId = 11, SizeId = 4 },
                new ProductVariant { Id = 27, SKU = "AGLNU-KEM-S", Price = 650000.00m, Stock = 8, ReservedStock = 0, ProductId = 4, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 28, SKU = "AGLNU-KEM-M", Price = 650000.00m, Stock = 10, ReservedStock = 0, ProductId = 4, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 29, SKU = "AGLNU-KEM-L", Price = 650000.00m, Stock = 7, ReservedStock = 0, ProductId = 4, ColorId = 8, SizeId = 4 },
                new ProductVariant { Id = 30, SKU = "ABDN-XDB-S", Price = 890000.00m, Stock = 8, ReservedStock = 0, ProductId = 5, ColorId = 35, SizeId = 2 },
                new ProductVariant { Id = 31, SKU = "ABDN-XDB-M", Price = 890000.00m, Stock = 12, ReservedStock = 0, ProductId = 5, ColorId = 35, SizeId = 3 },
                new ProductVariant { Id = 32, SKU = "ABDN-XDB-L", Price = 890000.00m, Stock = 9, ReservedStock = 0, ProductId = 5, ColorId = 35, SizeId = 4 },
                new ProductVariant { Id = 33, SKU = "AKBN-DEN-S", Price = 790000.00m, Stock = 10, ReservedStock = 0, ProductId = 6, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 34, SKU = "AKBN-DEN-M", Price = 790000.00m, Stock = 15, ReservedStock = 0, ProductId = 6, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 35, SKU = "AKBN-DEN-L", Price = 790000.00m, Stock = 12, ReservedStock = 0, ProductId = 6, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 36, SKU = "AKBN-DEN-XL", Price = 790000.00m, Stock = 8, ReservedStock = 0, ProductId = 6, ColorId = 1, SizeId = 5 },
                new ProductVariant { Id = 37, SKU = "AKCTNU-KEM-S", Price = 750000.00m, Stock = 10, ReservedStock = 0, ProductId = 7, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 38, SKU = "AKCTNU-KEM-M", Price = 750000.00m, Stock = 14, ReservedStock = 0, ProductId = 7, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 39, SKU = "AKCTNU-KEM-L", Price = 750000.00m, Stock = 10, ReservedStock = 0, ProductId = 7, ColorId = 8, SizeId = 4 },
                new ProductVariant { Id = 40, SKU = "AKDNU-XDB-S", Price = 790000.00m, Stock = 10, ReservedStock = 0, ProductId = 8, ColorId = 35, SizeId = 2 },
                new ProductVariant { Id = 41, SKU = "AKDNU-XDB-M", Price = 790000.00m, Stock = 14, ReservedStock = 0, ProductId = 8, ColorId = 35, SizeId = 3 },
                new ProductVariant { Id = 42, SKU = "AKDNU-XDB-L", Price = 790000.00m, Stock = 11, ReservedStock = 0, ProductId = 8, ColorId = 35, SizeId = 4 },
                new ProductVariant { Id = 43, SKU = "ACDNU-NAU-S", Price = 390000.00m, Stock = 11, ReservedStock = 0, ProductId = 9, ColorId = 14, SizeId = 2 },
                new ProductVariant { Id = 44, SKU = "ACDNU-NAU-M", Price = 390000.00m, Stock = 15, ReservedStock = 0, ProductId = 9, ColorId = 14, SizeId = 3 },
                new ProductVariant { Id = 45, SKU = "ACDNU-NAU-L", Price = 390000.00m, Stock = 10, ReservedStock = 0, ProductId = 9, ColorId = 14, SizeId = 4 },
                new ProductVariant { Id = 46, SKU = "ACDNU-TRG-S", Price = 390000.00m, Stock = 12, ReservedStock = 0, ProductId = 9, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 47, SKU = "ACDNU-TRG-M", Price = 390000.00m, Stock = 15, ReservedStock = 0, ProductId = 9, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 48, SKU = "ACDNU-TRG-L", Price = 390000.00m, Stock = 10, ReservedStock = 0, ProductId = 9, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 49, SKU = "AKLN-KEM-S", Price = 890000.00m, Stock = 10, ReservedStock = 0, ProductId = 10, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 50, SKU = "AKLN-KEM-M", Price = 890000.00m, Stock = 14, ReservedStock = 0, ProductId = 10, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 51, SKU = "AKLN-KEM-L", Price = 890000.00m, Stock = 10, ReservedStock = 0, ProductId = 10, ColorId = 8, SizeId = 4 },
                new ProductVariant { Id = 52, SKU = "AKLN-KHK-S", Price = 890000.00m, Stock = 8, ReservedStock = 0, ProductId = 10, ColorId = 12, SizeId = 2 },
                new ProductVariant { Id = 53, SKU = "AKLN-KHK-M", Price = 890000.00m, Stock = 10, ReservedStock = 0, ProductId = 10, ColorId = 12, SizeId = 3 },
                new ProductVariant { Id = 54, SKU = "AKLN-KHK-L", Price = 890000.00m, Stock = 7, ReservedStock = 0, ProductId = 10, ColorId = 12, SizeId = 4 },
                new ProductVariant { Id = 55, SKU = "AKTTNU-DEN-S", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 11, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 56, SKU = "AKTTNU-DEN-M", Price = 690000.00m, Stock = 15, ReservedStock = 0, ProductId = 11, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 57, SKU = "AKTTNU-DEN-L", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 11, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 58, SKU = "AKTXN-DEN-S", Price = 1290000.00m, Stock = 8, ReservedStock = 0, ProductId = 12, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 59, SKU = "AKTXN-DEN-M", Price = 1290000.00m, Stock = 10, ReservedStock = 0, ProductId = 12, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 60, SKU = "AKTXN-DEN-L", Price = 1290000.00m, Stock = 8, ReservedStock = 0, ProductId = 12, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 61, SKU = "AKCDN-TRG-S", Price = 350000.00m, Stock = 15, ReservedStock = 0, ProductId = 13, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 62, SKU = "AKCDN-TRG-M", Price = 350000.00m, Stock = 20, ReservedStock = 0, ProductId = 13, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 63, SKU = "AKCDN-TRG-L", Price = 350000.00m, Stock = 15, ReservedStock = 0, ProductId = 13, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 64, SKU = "APTBNU-TRG-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 14, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 65, SKU = "APTBNU-TRG-M", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 14, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 66, SKU = "APTBNU-TRG-L", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 14, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 67, SKU = "ASMCFN-XRE-S", Price = 420000.00m, Stock = 12, ReservedStock = 0, ProductId = 15, ColorId = 33, SizeId = 2 },
                new ProductVariant { Id = 68, SKU = "ASMCFN-XRE-M", Price = 420000.00m, Stock = 15, ReservedStock = 0, ProductId = 15, ColorId = 33, SizeId = 3 },
                new ProductVariant { Id = 69, SKU = "ASMCFN-XRE-L", Price = 420000.00m, Stock = 12, ReservedStock = 0, ProductId = 15, ColorId = 33, SizeId = 4 },
                new ProductVariant { Id = 70, SKU = "ASM2VNU-BE-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 16, ColorId = 9, SizeId = 2 },
                new ProductVariant { Id = 71, SKU = "ASM2VNU-BE-M", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 16, ColorId = 9, SizeId = 3 },
                new ProductVariant { Id = 72, SKU = "ASM2VNU-BE-L", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 16, ColorId = 9, SizeId = 4 },
                new ProductVariant { Id = 73, SKU = "ASM2VNU-TIM-S", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 16, ColorId = 40, SizeId = 2 },
                new ProductVariant { Id = 74, SKU = "ASM2VNU-TIM-M", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 16, ColorId = 40, SizeId = 3 },
                new ProductVariant { Id = 75, SKU = "ASM2VNU-TIM-L", Price = 490000.00m, Stock = 9, ReservedStock = 0, ProductId = 16, ColorId = 40, SizeId = 4 },
                new ProductVariant { Id = 76, SKU = "ASMK-TRG-S", Price = 450000.00m, Stock = 12, ReservedStock = 0, ProductId = 17, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 77, SKU = "ASMK-TRG-M", Price = 450000.00m, Stock = 15, ReservedStock = 0, ProductId = 17, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 78, SKU = "ASMK-TRG-L", Price = 450000.00m, Stock = 12, ReservedStock = 0, ProductId = 17, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 79, SKU = "ASMK-TRG-XL", Price = 450000.00m, Stock = 8, ReservedStock = 0, ProductId = 17, ColorId = 2, SizeId = 5 },
                new ProductVariant { Id = 80, SKU = "ASMLNN-BE-S", Price = 550000.00m, Stock = 12, ReservedStock = 0, ProductId = 18, ColorId = 9, SizeId = 2 },
                new ProductVariant { Id = 81, SKU = "ASMLNN-BE-M", Price = 550000.00m, Stock = 15, ReservedStock = 0, ProductId = 18, ColorId = 9, SizeId = 3 },
                new ProductVariant { Id = 82, SKU = "ASMLNN-BE-L", Price = 550000.00m, Stock = 10, ReservedStock = 0, ProductId = 18, ColorId = 9, SizeId = 4 },
                new ProductVariant { Id = 83, SKU = "ASMLNN-TRG-S", Price = 550000.00m, Stock = 10, ReservedStock = 0, ProductId = 18, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 84, SKU = "ASMLNN-TRG-M", Price = 550000.00m, Stock = 14, ReservedStock = 0, ProductId = 18, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 85, SKU = "ASMLNN-TRG-L", Price = 550000.00m, Stock = 10, ReservedStock = 0, ProductId = 18, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 86, SKU = "ASMLNN-XND-S", Price = 550000.00m, Stock = 8, ReservedStock = 0, ProductId = 18, ColorId = 36, SizeId = 2 },
                new ProductVariant { Id = 87, SKU = "ASMLNN-XND-M", Price = 550000.00m, Stock = 12, ReservedStock = 0, ProductId = 18, ColorId = 36, SizeId = 3 },
                new ProductVariant { Id = 88, SKU = "ASMLNN-XND-L", Price = 550000.00m, Stock = 9, ReservedStock = 0, ProductId = 18, ColorId = 36, SizeId = 4 },
                new ProductVariant { Id = 89, SKU = "ASMNBD-TRG-S", Price = 380000.00m, Stock = 15, ReservedStock = 0, ProductId = 19, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 90, SKU = "ASMNBD-TRG-M", Price = 380000.00m, Stock = 18, ReservedStock = 0, ProductId = 19, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 91, SKU = "ASMNBD-TRG-L", Price = 380000.00m, Stock = 12, ReservedStock = 0, ProductId = 19, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 92, SKU = "ATNU-KEM-S", Price = 290000.00m, Stock = 20, ReservedStock = 0, ProductId = 20, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 93, SKU = "ATNU-KEM-M", Price = 290000.00m, Stock = 25, ReservedStock = 0, ProductId = 20, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 94, SKU = "ATNU-KEM-L", Price = 290000.00m, Stock = 18, ReservedStock = 0, ProductId = 20, ColorId = 8, SizeId = 4 },
                new ProductVariant { Id = 95, SKU = "ATNU-VNG-S", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 20, ColorId = 28, SizeId = 2 },
                new ProductVariant { Id = 96, SKU = "ATNU-VNG-M", Price = 290000.00m, Stock = 18, ReservedStock = 0, ProductId = 20, ColorId = 28, SizeId = 3 },
                new ProductVariant { Id = 97, SKU = "ATNU-VNG-L", Price = 290000.00m, Stock = 12, ReservedStock = 0, ProductId = 20, ColorId = 28, SizeId = 4 },
                new ProductVariant { Id = 98, SKU = "ATCNU-BE-S", Price = 1590000.00m, Stock = 6, ReservedStock = 0, ProductId = 21, ColorId = 9, SizeId = 2 },
                new ProductVariant { Id = 99, SKU = "ATCNU-BE-M", Price = 1590000.00m, Stock = 8, ReservedStock = 0, ProductId = 21, ColorId = 9, SizeId = 3 },
                new ProductVariant { Id = 100, SKU = "ATCNU-BE-L", Price = 1590000.00m, Stock = 6, ReservedStock = 0, ProductId = 21, ColorId = 9, SizeId = 4 },
                new ProductVariant { Id = 101, SKU = "CVDCNU-DEN-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 22, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 102, SKU = "CVDCNU-DEN-M", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 22, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 103, SKU = "CVDCNU-DEN-L", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 22, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 104, SKU = "CVJNU-NAU-S", Price = 390000.00m, Stock = 12, ReservedStock = 0, ProductId = 23, ColorId = 14, SizeId = 2 },
                new ProductVariant { Id = 105, SKU = "CVJNU-NAU-M", Price = 390000.00m, Stock = 15, ReservedStock = 0, ProductId = 23, ColorId = 14, SizeId = 3 },
                new ProductVariant { Id = 106, SKU = "CVJNU-NAU-L", Price = 390000.00m, Stock = 10, ReservedStock = 0, ProductId = 23, ColorId = 14, SizeId = 4 },
                new ProductVariant { Id = 107, SKU = "CVJNU-OLV-S", Price = 390000.00m, Stock = 10, ReservedStock = 0, ProductId = 23, ColorId = 32, SizeId = 2 },
                new ProductVariant { Id = 108, SKU = "CVJNU-OLV-M", Price = 390000.00m, Stock = 12, ReservedStock = 0, ProductId = 23, ColorId = 32, SizeId = 3 },
                new ProductVariant { Id = 109, SKU = "CVJNU-OLV-L", Price = 390000.00m, Stock = 8, ReservedStock = 0, ProductId = 23, ColorId = 32, SizeId = 4 },
                new ProductVariant { Id = 110, SKU = "CVMXNU-DEN-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 24, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 111, SKU = "CVMXNU-DEN-M", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 24, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 112, SKU = "CVMXNU-DEN-L", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 24, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 113, SKU = "CVXRNU-KEM-S", Price = 450000.00m, Stock = 12, ReservedStock = 0, ProductId = 25, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 114, SKU = "CVXRNU-KEM-M", Price = 450000.00m, Stock = 15, ReservedStock = 0, ProductId = 25, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 115, SKU = "CVXRNU-KEM-L", Price = 450000.00m, Stock = 10, ReservedStock = 0, ProductId = 25, ColorId = 8, SizeId = 4 },
                new ProductVariant { Id = 116, SKU = "HDKTN-DEN-S", Price = 590000.00m, Stock = 12, ReservedStock = 0, ProductId = 26, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 117, SKU = "HDKTN-DEN-M", Price = 590000.00m, Stock = 15, ReservedStock = 0, ProductId = 26, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 118, SKU = "HDKTN-DEN-L", Price = 590000.00m, Stock = 12, ReservedStock = 0, ProductId = 26, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 119, SKU = "HDKTN-XAM-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 26, ColorId = 5, SizeId = 2 },
                new ProductVariant { Id = 120, SKU = "HDKTN-XAM-M", Price = 590000.00m, Stock = 14, ReservedStock = 0, ProductId = 26, ColorId = 5, SizeId = 3 },
                new ProductVariant { Id = 121, SKU = "HDKTN-XAM-L", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 26, ColorId = 5, SizeId = 4 },
                new ProductVariant { Id = 122, SKU = "HDNN-DEN-S", Price = 590000.00m, Stock = 12, ReservedStock = 0, ProductId = 27, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 123, SKU = "HDNN-DEN-M", Price = 590000.00m, Stock = 18, ReservedStock = 0, ProductId = 27, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 124, SKU = "HDNN-DEN-L", Price = 590000.00m, Stock = 14, ReservedStock = 0, ProductId = 27, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 125, SKU = "HDNN-HONG-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 27, ColorId = 23, SizeId = 2 },
                new ProductVariant { Id = 126, SKU = "HDNN-HONG-M", Price = 590000.00m, Stock = 12, ReservedStock = 0, ProductId = 27, ColorId = 23, SizeId = 3 },
                new ProductVariant { Id = 127, SKU = "HDNN-HONG-L", Price = 590000.00m, Stock = 9, ReservedStock = 0, ProductId = 27, ColorId = 23, SizeId = 4 },
                new ProductVariant { Id = 128, SKU = "HDNN-NAU-S", Price = 590000.00m, Stock = 8, ReservedStock = 0, ProductId = 27, ColorId = 14, SizeId = 2 },
                new ProductVariant { Id = 129, SKU = "HDNN-NAU-M", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 27, ColorId = 14, SizeId = 3 },
                new ProductVariant { Id = 130, SKU = "HDNN-NAU-L", Price = 590000.00m, Stock = 8, ReservedStock = 0, ProductId = 27, ColorId = 14, SizeId = 4 },
                new ProductVariant { Id = 131, SKU = "HDKKN-XAM-S", Price = 650000.00m, Stock = 12, ReservedStock = 0, ProductId = 28, ColorId = 5, SizeId = 2 },
                new ProductVariant { Id = 132, SKU = "HDKKN-XAM-M", Price = 650000.00m, Stock = 15, ReservedStock = 0, ProductId = 28, ColorId = 5, SizeId = 3 },
                new ProductVariant { Id = 133, SKU = "HDKKN-XAM-L", Price = 650000.00m, Stock = 12, ReservedStock = 0, ProductId = 28, ColorId = 5, SizeId = 4 },
                new ProductVariant { Id = 134, SKU = "HDKKN-XAM-XL", Price = 650000.00m, Stock = 8, ReservedStock = 0, ProductId = 28, ColorId = 5, SizeId = 5 },
                new ProductVariant { Id = 135, SKU = "JMPNU-KEM-S", Price = 550000.00m, Stock = 10, ReservedStock = 0, ProductId = 29, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 136, SKU = "JMPNU-KEM-M", Price = 550000.00m, Stock = 14, ReservedStock = 0, ProductId = 29, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 137, SKU = "JMPNU-KEM-L", Price = 550000.00m, Stock = 10, ReservedStock = 0, ProductId = 29, ColorId = 8, SizeId = 4 },
                new ProductVariant { Id = 138, SKU = "POLON-DEN-S", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 30, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 139, SKU = "POLON-DEN-M", Price = 490000.00m, Stock = 20, ReservedStock = 0, ProductId = 30, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 140, SKU = "POLON-DEN-L", Price = 490000.00m, Stock = 18, ReservedStock = 0, ProductId = 30, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 141, SKU = "POLON-TRG-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 30, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 142, SKU = "POLON-TRG-M", Price = 490000.00m, Stock = 18, ReservedStock = 0, ProductId = 30, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 143, SKU = "POLON-TRG-L", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 30, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 144, SKU = "POLON-XDB-S", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 30, ColorId = 35, SizeId = 2 },
                new ProductVariant { Id = 145, SKU = "POLON-XDB-M", Price = 490000.00m, Stock = 14, ReservedStock = 0, ProductId = 30, ColorId = 35, SizeId = 3 },
                new ProductVariant { Id = 146, SKU = "POLON-XDB-L", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 30, ColorId = 35, SizeId = 4 },
                new ProductVariant { Id = 147, SKU = "QDCCNU-DEN-S", Price = 450000.00m, Stock = 15, ReservedStock = 0, ProductId = 31, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 148, SKU = "QDCCNU-DEN-M", Price = 450000.00m, Stock = 18, ReservedStock = 0, ProductId = 31, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 149, SKU = "QDCCNU-DEN-L", Price = 450000.00m, Stock = 14, ReservedStock = 0, ProductId = 31, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 150, SKU = "QDCCNU-NAU-S", Price = 450000.00m, Stock = 10, ReservedStock = 0, ProductId = 31, ColorId = 14, SizeId = 2 },
                new ProductVariant { Id = 151, SKU = "QDCCNU-NAU-M", Price = 450000.00m, Stock = 14, ReservedStock = 0, ProductId = 31, ColorId = 14, SizeId = 3 },
                new ProductVariant { Id = 152, SKU = "QDCCNU-NAU-L", Price = 450000.00m, Stock = 10, ReservedStock = 0, ProductId = 31, ColorId = 14, SizeId = 4 },
                new ProductVariant { Id = 153, SKU = "QDDRN-XAM-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 32, ColorId = 5, SizeId = 2 },
                new ProductVariant { Id = 154, SKU = "QDDRN-XAM-M", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 32, ColorId = 5, SizeId = 3 },
                new ProductVariant { Id = 155, SKU = "QDDRN-XAM-L", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 32, ColorId = 5, SizeId = 4 },
                new ProductVariant { Id = 156, SKU = "QDDRN-XAM-XL", Price = 490000.00m, Stock = 8, ReservedStock = 0, ProductId = 32, ColorId = 5, SizeId = 5 },
                new ProductVariant { Id = 157, SKU = "QDSTNU-DEN-S", Price = 590000.00m, Stock = 12, ReservedStock = 0, ProductId = 33, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 158, SKU = "QDSTNU-DEN-M", Price = 590000.00m, Stock = 15, ReservedStock = 0, ProductId = 33, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 159, SKU = "QDSTNU-DEN-L", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 33, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 160, SKU = "QDSTNU-KEM-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 33, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 161, SKU = "QDSTNU-KEM-M", Price = 590000.00m, Stock = 12, ReservedStock = 0, ProductId = 33, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 162, SKU = "QDSTNU-KEM-L", Price = 590000.00m, Stock = 9, ReservedStock = 0, ProductId = 33, ColorId = 8, SizeId = 4 },
                new ProductVariant { Id = 163, SKU = "QJNN-DEN-S", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 34, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 164, SKU = "QJNN-DEN-M", Price = 690000.00m, Stock = 18, ReservedStock = 0, ProductId = 34, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 165, SKU = "QJNN-DEN-L", Price = 690000.00m, Stock = 15, ReservedStock = 0, ProductId = 34, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 166, SKU = "QJNN-XDB-S", Price = 690000.00m, Stock = 10, ReservedStock = 0, ProductId = 34, ColorId = 35, SizeId = 2 },
                new ProductVariant { Id = 167, SKU = "QJNN-XDB-M", Price = 690000.00m, Stock = 15, ReservedStock = 0, ProductId = 34, ColorId = 35, SizeId = 3 },
                new ProductVariant { Id = 168, SKU = "QJNN-XDB-L", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 34, ColorId = 35, SizeId = 4 },
                new ProductVariant { Id = 169, SKU = "QJNNU-DEN-S", Price = 650000.00m, Stock = 15, ReservedStock = 0, ProductId = 35, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 170, SKU = "QJNNU-DEN-M", Price = 650000.00m, Stock = 18, ReservedStock = 0, ProductId = 35, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 171, SKU = "QJNNU-DEN-L", Price = 650000.00m, Stock = 14, ReservedStock = 0, ProductId = 35, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 172, SKU = "QJNNU-XND-S", Price = 650000.00m, Stock = 12, ReservedStock = 0, ProductId = 35, ColorId = 36, SizeId = 2 },
                new ProductVariant { Id = 173, SKU = "QJNNU-XND-M", Price = 650000.00m, Stock = 15, ReservedStock = 0, ProductId = 35, ColorId = 36, SizeId = 3 },
                new ProductVariant { Id = 174, SKU = "QJNNU-XND-L", Price = 650000.00m, Stock = 12, ReservedStock = 0, ProductId = 35, ColorId = 36, SizeId = 4 },
                new ProductVariant { Id = 175, SKU = "QJNRG-XND-S", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 36, ColorId = 36, SizeId = 2 },
                new ProductVariant { Id = 176, SKU = "QJNRG-XND-M", Price = 690000.00m, Stock = 15, ReservedStock = 0, ProductId = 36, ColorId = 36, SizeId = 3 },
                new ProductVariant { Id = 177, SKU = "QJNRG-XND-L", Price = 690000.00m, Stock = 10, ReservedStock = 0, ProductId = 36, ColorId = 36, SizeId = 4 },
                new ProductVariant { Id = 178, SKU = "QJNRG-XNV-S", Price = 690000.00m, Stock = 10, ReservedStock = 0, ProductId = 36, ColorId = 37, SizeId = 2 },
                new ProductVariant { Id = 179, SKU = "QJNRG-XNV-M", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 36, ColorId = 37, SizeId = 3 },
                new ProductVariant { Id = 180, SKU = "QJNRG-XNV-L", Price = 690000.00m, Stock = 9, ReservedStock = 0, ProductId = 36, ColorId = 37, SizeId = 4 },
                new ProductVariant { Id = 181, SKU = "QJGN-DEN-S", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 37, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 182, SKU = "QJGN-DEN-M", Price = 490000.00m, Stock = 20, ReservedStock = 0, ProductId = 37, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 183, SKU = "QJGN-DEN-L", Price = 490000.00m, Stock = 16, ReservedStock = 0, ProductId = 37, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 184, SKU = "QJGN-XAM-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 37, ColorId = 5, SizeId = 2 },
                new ProductVariant { Id = 185, SKU = "QJGN-XAM-M", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 37, ColorId = 5, SizeId = 3 },
                new ProductVariant { Id = 186, SKU = "QJGN-XAM-L", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 37, ColorId = 5, SizeId = 4 },
                new ProductVariant { Id = 187, SKU = "QOSMN-NAU-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 38, ColorId = 14, SizeId = 2 },
                new ProductVariant { Id = 188, SKU = "QOSMN-NAU-M", Price = 590000.00m, Stock = 12, ReservedStock = 0, ProductId = 38, ColorId = 14, SizeId = 3 },
                new ProductVariant { Id = 189, SKU = "QOSMN-NAU-L", Price = 590000.00m, Stock = 9, ReservedStock = 0, ProductId = 38, ColorId = 14, SizeId = 4 },
                new ProductVariant { Id = 190, SKU = "QOSMN-XKK-S", Price = 590000.00m, Stock = 8, ReservedStock = 0, ProductId = 38, ColorId = 33, SizeId = 2 },
                new ProductVariant { Id = 191, SKU = "QOSMN-XKK-M", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 38, ColorId = 33, SizeId = 3 },
                new ProductVariant { Id = 192, SKU = "QOSMN-XKK-L", Price = 590000.00m, Stock = 8, ReservedStock = 0, ProductId = 38, ColorId = 33, SizeId = 4 },
                new ProductVariant { Id = 193, SKU = "QSJN-DEN-S", Price = 450000.00m, Stock = 15, ReservedStock = 0, ProductId = 39, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 194, SKU = "QSJN-DEN-M", Price = 450000.00m, Stock = 20, ReservedStock = 0, ProductId = 39, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 195, SKU = "QSJN-DEN-L", Price = 450000.00m, Stock = 16, ReservedStock = 0, ProductId = 39, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 196, SKU = "QSJN-XNL-S", Price = 450000.00m, Stock = 12, ReservedStock = 0, ProductId = 39, ColorId = 31, SizeId = 2 },
                new ProductVariant { Id = 197, SKU = "QSJN-XNL-M", Price = 450000.00m, Stock = 15, ReservedStock = 0, ProductId = 39, ColorId = 31, SizeId = 3 },
                new ProductVariant { Id = 198, SKU = "QSJN-XNL-L", Price = 450000.00m, Stock = 12, ReservedStock = 0, ProductId = 39, ColorId = 31, SizeId = 4 },
                new ProductVariant { Id = 199, SKU = "QSNN-DEN-S", Price = 390000.00m, Stock = 18, ReservedStock = 0, ProductId = 40, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 200, SKU = "QSNN-DEN-M", Price = 390000.00m, Stock = 22, ReservedStock = 0, ProductId = 40, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 201, SKU = "QSNN-DEN-L", Price = 390000.00m, Stock = 18, ReservedStock = 0, ProductId = 40, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 202, SKU = "QSNN-XAM-S", Price = 390000.00m, Stock = 14, ReservedStock = 0, ProductId = 40, ColorId = 5, SizeId = 2 },
                new ProductVariant { Id = 203, SKU = "QSNN-XAM-M", Price = 390000.00m, Stock = 18, ReservedStock = 0, ProductId = 40, ColorId = 5, SizeId = 3 },
                new ProductVariant { Id = 204, SKU = "QSNN-XAM-L", Price = 390000.00m, Stock = 14, ReservedStock = 0, ProductId = 40, ColorId = 5, SizeId = 4 },
                new ProductVariant { Id = 205, SKU = "QSTHN-KHK-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 41, ColorId = 12, SizeId = 2 },
                new ProductVariant { Id = 206, SKU = "QSTHN-KHK-M", Price = 490000.00m, Stock = 16, ReservedStock = 0, ProductId = 41, ColorId = 12, SizeId = 3 },
                new ProductVariant { Id = 207, SKU = "QSTHN-KHK-L", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 41, ColorId = 12, SizeId = 4 },
                new ProductVariant { Id = 208, SKU = "QSTHN-KHK-XL", Price = 490000.00m, Stock = 8, ReservedStock = 0, ProductId = 41, ColorId = 12, SizeId = 5 },
                new ProductVariant { Id = 209, SKU = "QTN-DEN-S", Price = 690000.00m, Stock = 10, ReservedStock = 0, ProductId = 42, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 210, SKU = "QTN-DEN-M", Price = 690000.00m, Stock = 14, ReservedStock = 0, ProductId = 42, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 211, SKU = "QTN-DEN-L", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 42, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 212, SKU = "QTN-DEN-XL", Price = 690000.00m, Stock = 8, ReservedStock = 0, ProductId = 42, ColorId = 1, SizeId = 5 },
                new ProductVariant { Id = 213, SKU = "SMCNN-TRG-S", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 43, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 214, SKU = "SMCNN-TRG-M", Price = 690000.00m, Stock = 16, ReservedStock = 0, ProductId = 43, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 215, SKU = "SMCNN-TRG-L", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 43, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 216, SKU = "SMCNN-DEN-S", Price = 690000.00m, Stock = 10, ReservedStock = 0, ProductId = 43, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 217, SKU = "SMCNN-DEN-M", Price = 690000.00m, Stock = 14, ReservedStock = 0, ProductId = 43, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 218, SKU = "SMCNN-DEN-L", Price = 690000.00m, Stock = 10, ReservedStock = 0, ProductId = 43, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 219, SKU = "SMCNN-XDB-S", Price = 690000.00m, Stock = 8, ReservedStock = 0, ProductId = 43, ColorId = 35, SizeId = 2 },
                new ProductVariant { Id = 220, SKU = "SMCNN-XDB-M", Price = 690000.00m, Stock = 12, ReservedStock = 0, ProductId = 43, ColorId = 35, SizeId = 3 },
                new ProductVariant { Id = 221, SKU = "SMCNN-XDB-L", Price = 690000.00m, Stock = 9, ReservedStock = 0, ProductId = 43, ColorId = 35, SizeId = 4 },
                new ProductVariant { Id = 222, SKU = "SMCTN-DO-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 44, ColorId = 18, SizeId = 2 },
                new ProductVariant { Id = 223, SKU = "SMCTN-DO-M", Price = 590000.00m, Stock = 14, ReservedStock = 0, ProductId = 44, ColorId = 18, SizeId = 3 },
                new ProductVariant { Id = 224, SKU = "SMCTN-DO-L", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 44, ColorId = 18, SizeId = 4 },
                new ProductVariant { Id = 225, SKU = "SMCTN-XDB-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 44, ColorId = 35, SizeId = 2 },
                new ProductVariant { Id = 226, SKU = "SMCTN-XDB-M", Price = 590000.00m, Stock = 14, ReservedStock = 0, ProductId = 44, ColorId = 35, SizeId = 3 },
                new ProductVariant { Id = 227, SKU = "SMCTN-XDB-L", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 44, ColorId = 35, SizeId = 4 },
                new ProductVariant { Id = 228, SKU = "SMJN-XDB-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 45, ColorId = 35, SizeId = 2 },
                new ProductVariant { Id = 229, SKU = "SMJN-XDB-M", Price = 590000.00m, Stock = 14, ReservedStock = 0, ProductId = 45, ColorId = 35, SizeId = 3 },
                new ProductVariant { Id = 230, SKU = "SMJN-XDB-L", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 45, ColorId = 35, SizeId = 4 },
                new ProductVariant { Id = 231, SKU = "SWTNU-HONG-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 46, ColorId = 23, SizeId = 2 },
                new ProductVariant { Id = 232, SKU = "SWTNU-HONG-M", Price = 590000.00m, Stock = 14, ReservedStock = 0, ProductId = 46, ColorId = 23, SizeId = 3 },
                new ProductVariant { Id = 233, SKU = "SWTNU-HONG-L", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 46, ColorId = 23, SizeId = 4 },
                new ProductVariant { Id = 234, SKU = "SWTNU-TRG-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 46, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 235, SKU = "SWTNU-TRG-M", Price = 590000.00m, Stock = 12, ReservedStock = 0, ProductId = 46, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 236, SKU = "SWTNU-TRG-L", Price = 590000.00m, Stock = 9, ReservedStock = 0, ProductId = 46, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 237, SKU = "SWTNU-XAM-S", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 46, ColorId = 5, SizeId = 2 },
                new ProductVariant { Id = 238, SKU = "SWTNU-XAM-M", Price = 590000.00m, Stock = 12, ReservedStock = 0, ProductId = 46, ColorId = 5, SizeId = 3 },
                new ProductVariant { Id = 239, SKU = "SWTNU-XAM-L", Price = 590000.00m, Stock = 9, ReservedStock = 0, ProductId = 46, ColorId = 5, SizeId = 4 },
                new ProductVariant { Id = 240, SKU = "TSN-DEN-S", Price = 290000.00m, Stock = 20, ReservedStock = 0, ProductId = 47, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 241, SKU = "TSN-DEN-M", Price = 290000.00m, Stock = 25, ReservedStock = 0, ProductId = 47, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 242, SKU = "TSN-DEN-L", Price = 290000.00m, Stock = 20, ReservedStock = 0, ProductId = 47, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 243, SKU = "TSN-KEM-S", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 47, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 244, SKU = "TSN-KEM-M", Price = 290000.00m, Stock = 20, ReservedStock = 0, ProductId = 47, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 245, SKU = "TSN-KEM-L", Price = 290000.00m, Stock = 15, ReservedStock = 0, ProductId = 47, ColorId = 8, SizeId = 4 },
                new ProductVariant { Id = 246, SKU = "V2DXN-NAU-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 48, ColorId = 14, SizeId = 2 },
                new ProductVariant { Id = 247, SKU = "V2DXN-NAU-M", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 48, ColorId = 14, SizeId = 3 },
                new ProductVariant { Id = 248, SKU = "V2DXN-NAU-L", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 48, ColorId = 14, SizeId = 4 },
                new ProductVariant { Id = 249, SKU = "V2DXN-VNG-S", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 48, ColorId = 28, SizeId = 2 },
                new ProductVariant { Id = 250, SKU = "V2DXN-VNG-M", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 48, ColorId = 28, SizeId = 3 },
                new ProductVariant { Id = 251, SKU = "V2DXN-VNG-L", Price = 490000.00m, Stock = 9, ReservedStock = 0, ProductId = 48, ColorId = 28, SizeId = 4 },
                new ProductVariant { Id = 252, SKU = "V2DXN-XND-S", Price = 490000.00m, Stock = 8, ReservedStock = 0, ProductId = 48, ColorId = 36, SizeId = 2 },
                new ProductVariant { Id = 253, SKU = "V2DXN-XND-M", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 48, ColorId = 36, SizeId = 3 },
                new ProductVariant { Id = 254, SKU = "V2DXN-XND-L", Price = 490000.00m, Stock = 9, ReservedStock = 0, ProductId = 48, ColorId = 36, SizeId = 4 },
                new ProductVariant { Id = 255, SKU = "VCFNU-DEN-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 49, ColorId = 1, SizeId = 2 },
                new ProductVariant { Id = 256, SKU = "VCFNU-DEN-M", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 49, ColorId = 1, SizeId = 3 },
                new ProductVariant { Id = 257, SKU = "VCFNU-DEN-L", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 49, ColorId = 1, SizeId = 4 },
                new ProductVariant { Id = 258, SKU = "VCFNU-NAU-S", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 49, ColorId = 14, SizeId = 2 },
                new ProductVariant { Id = 259, SKU = "VCFNU-NAU-M", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 49, ColorId = 14, SizeId = 3 },
                new ProductVariant { Id = 260, SKU = "VCFNU-NAU-L", Price = 490000.00m, Stock = 9, ReservedStock = 0, ProductId = 49, ColorId = 14, SizeId = 4 },
                new ProductVariant { Id = 261, SKU = "VDRCNU-TRG-S", Price = 390000.00m, Stock = 15, ReservedStock = 0, ProductId = 50, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 262, SKU = "VDRCNU-TRG-M", Price = 390000.00m, Stock = 18, ReservedStock = 0, ProductId = 50, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 263, SKU = "VDRCNU-TRG-L", Price = 390000.00m, Stock = 14, ReservedStock = 0, ProductId = 50, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 264, SKU = "VMDN-TRANG-S", Price = 490000.00m, Stock = 12, ReservedStock = 0, ProductId = 51, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 265, SKU = "VMDN-TRANG-M", Price = 490000.00m, Stock = 15, ReservedStock = 0, ProductId = 51, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 266, SKU = "VMDN-TRANG-L", Price = 490000.00m, Stock = 10, ReservedStock = 0, ProductId = 51, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 267, SKU = "VSTN-TIM-S", Price = 500000.00m, Stock = 10, ReservedStock = 0, ProductId = 52, ColorId = 40, SizeId = 2 },
                new ProductVariant { Id = 268, SKU = "VSTN-TIM-M", Price = 550000.00m, Stock = 14, ReservedStock = 0, ProductId = 52, ColorId = 40, SizeId = 3 },
                new ProductVariant { Id = 269, SKU = "VSTN-TIM-L", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 52, ColorId = 40, SizeId = 4 },
                new ProductVariant { Id = 270, SKU = "VSMNU-TRG-S", Price = 500000.00m, Stock = 12, ReservedStock = 0, ProductId = 53, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 271, SKU = "VSMNU-TRG-M", Price = 550000.00m, Stock = 15, ReservedStock = 0, ProductId = 53, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 272, SKU = "VSMNU-TRG-L", Price = 590000.00m, Stock = 10, ReservedStock = 0, ProductId = 53, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 273, SKU = "VSMNU-XND-S", Price = 500000.00m, Stock = 8, ReservedStock = 0, ProductId = 53, ColorId = 36, SizeId = 2 },
                new ProductVariant { Id = 274, SKU = "VSMNU-XND-M", Price = 550000.00m, Stock = 12, ReservedStock = 0, ProductId = 53, ColorId = 36, SizeId = 3 },
                new ProductVariant { Id = 275, SKU = "VSMNU-XND-L", Price = 590000.00m, Stock = 9, ReservedStock = 0, ProductId = 53, ColorId = 36, SizeId = 4 },
                new ProductVariant { Id = 276, SKU = "VSMTLNU-TRG-S", Price = 650000.00m, Stock = 10, ReservedStock = 0, ProductId = 54, ColorId = 2, SizeId = 2 },
                new ProductVariant { Id = 277, SKU = "VSMTLNU-TRG-M", Price = 650000.00m, Stock = 14, ReservedStock = 0, ProductId = 54, ColorId = 2, SizeId = 3 },
                new ProductVariant { Id = 278, SKU = "VSMTLNU-TRG-L", Price = 650000.00m, Stock = 10, ReservedStock = 0, ProductId = 54, ColorId = 2, SizeId = 4 },
                new ProductVariant { Id = 279, SKU = "VSMTLNU-XND-S", Price = 650000.00m, Stock = 8, ReservedStock = 0, ProductId = 54, ColorId = 36, SizeId = 2 },
                new ProductVariant { Id = 280, SKU = "VSMTLNU-XND-M", Price = 650000.00m, Stock = 10, ReservedStock = 0, ProductId = 54, ColorId = 36, SizeId = 3 },
                new ProductVariant { Id = 281, SKU = "VSMTLNU-XND-L", Price = 650000.00m, Stock = 8, ReservedStock = 0, ProductId = 54, ColorId = 36, SizeId = 4 },
                new ProductVariant { Id = 282, SKU = "VTNNU-KEM-S", Price = 350000.00m, Stock = 12, ReservedStock = 0, ProductId = 55, ColorId = 8, SizeId = 2 },
                new ProductVariant { Id = 283, SKU = "VTNNU-KEM-M", Price = 400000.00m, Stock = 15, ReservedStock = 0, ProductId = 55, ColorId = 8, SizeId = 3 },
                new ProductVariant { Id = 284, SKU = "VTNNU-KEM-L", Price = 450000.00m, Stock = 10, ReservedStock = 0, ProductId = 55, ColorId = 8, SizeId = 4 }
            );

            // ProductImage: 146 rows
            mb.Entity<ProductImage>().HasData(
                new ProductImage { Id = 1, FileName = "ao3lo-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 1, ColorId = 1 },
                new ProductImage { Id = 2, FileName = "ao3lo-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 1, ColorId = 1 },
                new ProductImage { Id = 3, FileName = "ao3lo-kem.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 1, ColorId = 8 },
                new ProductImage { Id = 4, FileName = "ao3lonu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 2, ColorId = 1 },
                new ProductImage { Id = 5, FileName = "ao3lonu-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 2, ColorId = 1 },
                new ProductImage { Id = 7, FileName = "ao3lonu-trang.avif", IsPrimary = false, DisplayOrder = 3, ProductId = 2, ColorId = 2 },
                new ProductImage { Id = 8, FileName = "aogile-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 3, ColorId = 1 },
                new ProductImage { Id = 9, FileName = "aogile.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 3, ColorId = 1 },
                new ProductImage { Id = 10, FileName = "aogilenu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 4, ColorId = 1 },
                new ProductImage { Id = 11, FileName = "aogilenu-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 4, ColorId = 1 },
                new ProductImage { Id = 12, FileName = "aogilenu-bedam.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 4, ColorId = 11 },
                new ProductImage { Id = 13, FileName = "aogilenu-kem.avif", IsPrimary = false, DisplayOrder = 3, ProductId = 4, ColorId = 8 },
                new ProductImage { Id = 14, FileName = "aoblazerdenimnu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 5, ColorId = 35 },
                new ProductImage { Id = 15, FileName = "aoblazerdenimnu.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 5, ColorId = 35 },
                new ProductImage { Id = 16, FileName = "aobomber-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 6, ColorId = 1 },
                new ProductImage { Id = 17, FileName = "aobomber.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 6, ColorId = 1 },
                new ProductImage { Id = 18, FileName = "aokhoaccottonnu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 7, ColorId = 14 },
                new ProductImage { Id = 19, FileName = "aokhoaccottonnu.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 7, ColorId = 14 },
                new ProductImage { Id = 20, FileName = "aokhoacdenimnu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 8, ColorId = 35 },
                new ProductImage { Id = 21, FileName = "aokhoacdenimnu.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 8, ColorId = 35 },
                new ProductImage { Id = 22, FileName = "aocodo-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 9, ColorId = 14 },
                new ProductImage { Id = 23, FileName = "aocodo-nau.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 9, ColorId = 14 },
                new ProductImage { Id = 24, FileName = "aocodo-trang.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 9, ColorId = 2 },
                new ProductImage { Id = 25, FileName = "aokhoaclinen-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 10, ColorId = 8 },
                new ProductImage { Id = 26, FileName = "aokhoaclinen-kem.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 10, ColorId = 8 },
                new ProductImage { Id = 27, FileName = "aokhoaclinen-kemdam.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 10, ColorId = 12 },
                new ProductImage { Id = 28, FileName = "aokhoacthethaonu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 11, ColorId = 2 },
                new ProductImage { Id = 29, FileName = "aokhoacthethaonu.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 11, ColorId = 2 },
                new ProductImage { Id = 30, FileName = "aokhoactuxedo-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 12, ColorId = 1 },
                new ProductImage { Id = 31, FileName = "aokhoactuxedo.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 12, ColorId = 1 },
                new ProductImage { Id = 32, FileName = "aokieucodiemnhun-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 13, ColorId = 2 },
                new ProductImage { Id = 33, FileName = "aokieucodiemnhun.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 13, ColorId = 3 },
                new ProductImage { Id = 34, FileName = "aopintuckedblouse-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 14, ColorId = 26 },
                new ProductImage { Id = 35, FileName = "aopintuckedblouse.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 14, ColorId = 26 },
                new ProductImage { Id = 36, FileName = "aosomichiffon-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 15, ColorId = 33 },
                new ProductImage { Id = 37, FileName = "aosomico2venu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 16, ColorId = 9 },
                new ProductImage { Id = 38, FileName = "aosomico2venu-be.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 16, ColorId = 9 },
                new ProductImage { Id = 39, FileName = "aosomico2venu-tim.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 16, ColorId = 40 },
                new ProductImage { Id = 40, FileName = "aosomikhoac-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 17, ColorId = 1 },
                new ProductImage { Id = 41, FileName = "aosomikhoac.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 17, ColorId = 1 },
                new ProductImage { Id = 42, FileName = "aosomilinennu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 18, ColorId = 2 },
                new ProductImage { Id = 43, FileName = "aosomilinennu-be.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 18, ColorId = 9 },
                new ProductImage { Id = 44, FileName = "aosomilinennu-trang.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 18, ColorId = 2 },
                new ProductImage { Id = 45, FileName = "aosomilinennu-xanhduong.avif", IsPrimary = false, DisplayOrder = 3, ProductId = 18, ColorId = 36 },
                new ProductImage { Id = 46, FileName = "aosominubuocday-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 19, ColorId = 25 },
                new ProductImage { Id = 47, FileName = "aosominubuocday.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 19, ColorId = 25 },
                new ProductImage { Id = 48, FileName = "aothunnu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 20, ColorId = 8 },
                new ProductImage { Id = 49, FileName = "aothunnu-kem.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 20, ColorId = 8 },
                new ProductImage { Id = 50, FileName = "aothunnu-vang.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 20, ColorId = 28 },
                new ProductImage { Id = 51, FileName = "aotrenchcoatnu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 21, ColorId = 9 },
                new ProductImage { Id = 52, FileName = "aotrenchcoatnu.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 21, ColorId = 9 },
                new ProductImage { Id = 53, FileName = "chanvaydapcheo-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 22, ColorId = 1 },
                new ProductImage { Id = 54, FileName = "chanvaydapcheo.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 22, ColorId = 1 },
                new ProductImage { Id = 55, FileName = "chanvayjersey-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 23, ColorId = 14 },
                new ProductImage { Id = 56, FileName = "chanvayjersey-nau.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 23, ColorId = 14 },
                new ProductImage { Id = 57, FileName = "chanvayjersey-xanholive.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 23, ColorId = 32 },
                new ProductImage { Id = 58, FileName = "chanvaymaxi-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 24, ColorId = 1 },
                new ProductImage { Id = 59, FileName = "chanvaymaxi.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 24, ColorId = 1 },
                new ProductImage { Id = 60, FileName = "chanvayxepru-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 25, ColorId = 19 },
                new ProductImage { Id = 61, FileName = "chanvayxepru.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 25, ColorId = 19 },
                new ProductImage { Id = 62, FileName = "hoddiekhongtay-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 26, ColorId = 1 },
                new ProductImage { Id = 63, FileName = "hoddiekhongtay-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 26, ColorId = 1 },
                new ProductImage { Id = 64, FileName = "hoddiekhongtay-xam.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 26, ColorId = 5 },
                new ProductImage { Id = 65, FileName = "hoodie-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 27, ColorId = 1 },
                new ProductImage { Id = 66, FileName = "hoodie-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 27, ColorId = 1 },
                new ProductImage { Id = 67, FileName = "hoodie-hong.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 27, ColorId = 23 },
                new ProductImage { Id = 68, FileName = "hoodie-nau.avif", IsPrimary = false, DisplayOrder = 3, ProductId = 27, ColorId = 14 },
                new ProductImage { Id = 69, FileName = "hoodiekhoakeo-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 28, ColorId = 5 },
                new ProductImage { Id = 70, FileName = "hoodiekhoakeo.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 28, ColorId = 5 },
                new ProductImage { Id = 71, FileName = "jumper-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 29, ColorId = 1 },
                new ProductImage { Id = 72, FileName = "jumper.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 29, ColorId = 1 },
                new ProductImage { Id = 73, FileName = "polo-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 30, ColorId = 35 },
                new ProductImage { Id = 74, FileName = "polo-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 30, ColorId = 1 },
                new ProductImage { Id = 75, FileName = "polo-trang.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 30, ColorId = 2 },
                new ProductImage { Id = 76, FileName = "polo-xanhbien.avif", IsPrimary = false, DisplayOrder = 3, ProductId = 30, ColorId = 35 },
                new ProductImage { Id = 77, FileName = "quandaicapchun-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 31, ColorId = 1 },
                new ProductImage { Id = 78, FileName = "quandaicapchun-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 31, ColorId = 1 },
                new ProductImage { Id = 79, FileName = "quandaicapchun-nau.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 31, ColorId = 14 },
                new ProductImage { Id = 80, FileName = "quandaidayrut-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 32, ColorId = 2 },
                new ProductImage { Id = 81, FileName = "quandaidayrut.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 32, ColorId = 2 },
                new ProductImage { Id = 82, FileName = "quandaisatin-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 33, ColorId = 1 },
                new ProductImage { Id = 83, FileName = "quandaisatin-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 33, ColorId = 1 },
                new ProductImage { Id = 84, FileName = "quandaisatin-kem.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 33, ColorId = 8 },
                new ProductImage { Id = 85, FileName = "quanjean-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 34, ColorId = 1 },
                new ProductImage { Id = 86, FileName = "quanjean-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 34, ColorId = 1 },
                new ProductImage { Id = 87, FileName = "quanjean-xanhbien.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 34, ColorId = 35 },
                new ProductImage { Id = 88, FileName = "quanjeannu-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 35, ColorId = 36 },
                new ProductImage { Id = 89, FileName = "quanjeannu-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 35, ColorId = 1 },
                new ProductImage { Id = 90, FileName = "quanjeannu-xanhduong.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 35, ColorId = 36 },
                new ProductImage { Id = 91, FileName = "quanjeannurachgoi-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 36, ColorId = 36 },
                new ProductImage { Id = 92, FileName = "quanjeannurachgoi-xanhduong.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 36, ColorId = 36 },
                new ProductImage { Id = 93, FileName = "quanjeannurachgoi-xanhduongdam.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 36, ColorId = 37 },
                new ProductImage { Id = 94, FileName = "quanjogger-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 37, ColorId = 1 },
                new ProductImage { Id = 95, FileName = "quanjogger-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 37, ColorId = 1 },
                new ProductImage { Id = 96, FileName = "quanjogger-xam.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 37, ColorId = 5 },
                new ProductImage { Id = 97, FileName = "quanongsuongmuslin-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 38, ColorId = 1 },
                new ProductImage { Id = 98, FileName = "quanongsuongmuslin-nau.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 38, ColorId = 1 },
                new ProductImage { Id = 99, FileName = "quanongsuongmuslin-xanhkaki.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 38, ColorId = 33 },
                new ProductImage { Id = 100, FileName = "quanshortjean-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 39, ColorId = 35 },
                new ProductImage { Id = 101, FileName = "quanshortjean-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 39, ColorId = 1 },
                new ProductImage { Id = 102, FileName = "quanshortjean-xanh.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 39, ColorId = 35 },
                new ProductImage { Id = 103, FileName = "quanshortni-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 40, ColorId = 5 },
                new ProductImage { Id = 104, FileName = "quanshortni-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 40, ColorId = 1 },
                new ProductImage { Id = 105, FileName = "quanshortni-xam.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 40, ColorId = 5 },
                new ProductImage { Id = 106, FileName = "quanshorttuihop-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 41, ColorId = 12 },
                new ProductImage { Id = 107, FileName = "quanshorttuihop.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 41, ColorId = 12 },
                new ProductImage { Id = 108, FileName = "quantay-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 42, ColorId = 12 },
                new ProductImage { Id = 109, FileName = "quantay.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 42, ColorId = 12 },
                new ProductImage { Id = 110, FileName = "somichongnhan-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 43, ColorId = 2 },
                new ProductImage { Id = 111, FileName = "somichongnhan-trang.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 43, ColorId = 2 },
                new ProductImage { Id = 112, FileName = "somichongnhan-den.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 43, ColorId = 1 },
                new ProductImage { Id = 113, FileName = "somichongnhan-xanhbien.avif", IsPrimary = false, DisplayOrder = 3, ProductId = 43, ColorId = 35 },
                new ProductImage { Id = 114, FileName = "somicotton-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 44, ColorId = 35 },
                new ProductImage { Id = 115, FileName = "somicotton-do.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 44, ColorId = 18 },
                new ProductImage { Id = 116, FileName = "somicotton-xanhbien.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 44, ColorId = 35 },
                new ProductImage { Id = 117, FileName = "somijean-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 45, ColorId = 35 },
                new ProductImage { Id = 118, FileName = "somijean.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 45, ColorId = 35 },
                new ProductImage { Id = 119, FileName = "sweater-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 46, ColorId = 23 },
                new ProductImage { Id = 120, FileName = "sweater-hong.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 46, ColorId = 23 },
                new ProductImage { Id = 121, FileName = "sweater-trang.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 46, ColorId = 2 },
                new ProductImage { Id = 122, FileName = "sweater-xam.avif", IsPrimary = false, DisplayOrder = 3, ProductId = 46, ColorId = 5 },
                new ProductImage { Id = 123, FileName = "tshirt-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 47, ColorId = 8 },
                new ProductImage { Id = 124, FileName = "tshirt-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 47, ColorId = 1 },
                new ProductImage { Id = 125, FileName = "tshirt-kem.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 47, ColorId = 8 },
                new ProductImage { Id = 126, FileName = "vay2dayxepnep-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 48, ColorId = 14 },
                new ProductImage { Id = 127, FileName = "vay2dayxepnep-nau.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 48, ColorId = 14 },
                new ProductImage { Id = 128, FileName = "vay2dayxepnep-vang.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 48, ColorId = 28 },
                new ProductImage { Id = 129, FileName = "vay2dayxepnep-xanhduong.avif", IsPrimary = false, DisplayOrder = 3, ProductId = 48, ColorId = 36 },
                new ProductImage { Id = 130, FileName = "vaychiffon-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 49, ColorId = 1 },
                new ProductImage { Id = 131, FileName = "vaychiffon-den.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 49, ColorId = 1 },
                new ProductImage { Id = 132, FileName = "vaychiffon-nau.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 49, ColorId = 14 },
                new ProductImage { Id = 133, FileName = "vaydayrutcotton-main.jpg", IsPrimary = true, DisplayOrder = 0, ProductId = 50, ColorId = 2 },
                new ProductImage { Id = 134, FileName = "vaydayrutcotton.jpg", IsPrimary = false, DisplayOrder = 1, ProductId = 50, ColorId = 2 },
                new ProductImage { Id = 135, FileName = "vaymidi-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 51, ColorId = 3 },
                new ProductImage { Id = 136, FileName = "vaymidi.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 51, ColorId = 3 },
                new ProductImage { Id = 137, FileName = "vaysatin-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 52, ColorId = 40 },
                new ProductImage { Id = 138, FileName = "vaysatin.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 52, ColorId = 40 },
                new ProductImage { Id = 139, FileName = "vaysomi-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 53, ColorId = 36 },
                new ProductImage { Id = 140, FileName = "vaysomi-trang.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 53, ColorId = 2 },
                new ProductImage { Id = 141, FileName = "vaysomi-xanhduong.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 53, ColorId = 36 },
                new ProductImage { Id = 142, FileName = "vaysomicothatlung-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 54, ColorId = 2 },
                new ProductImage { Id = 143, FileName = "vaysomicothatlung-trang.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 54, ColorId = 2 },
                new ProductImage { Id = 144, FileName = "vaysomicothatlung-xanhduong.avif", IsPrimary = false, DisplayOrder = 2, ProductId = 54, ColorId = 36 },
                new ProductImage { Id = 145, FileName = "vaytunic-main.avif", IsPrimary = true, DisplayOrder = 0, ProductId = 55, ColorId = 24 },
                new ProductImage { Id = 146, FileName = "vaytunic.avif", IsPrimary = false, DisplayOrder = 1, ProductId = 55, ColorId = 24 },
                new ProductImage { Id = 203, FileName = "ao3lonu-be.avif", IsPrimary = false, DisplayOrder = 0, ProductId = 2, ColorId = 9 }
            );

            // Cart: 2 rows

            // CartItem: 0 row

            // Order: 0 row

            // OrderDetail: 0 row

            // Wishlist: 0 rows — skipped

            // Review: 1 rows
            mb.Entity<Review>().HasData(
                new Review { Id = 1, Rating = 5, Comment = "", ImageUrl = null, SizeAccuracy = 3, CreatedAt = new DateTime(2026, 5, 18, 3, 53, 39, 324, DateTimeKind.Utc), UserId = 1, ProductId = 53 }
            );
        }
    }
}
