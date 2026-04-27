using Microsoft.EntityFrameworkCore;
using BaseCore.Entities;

namespace BaseCore.Repository
{
    /// <summary>
    /// Entity Framework Core DbContext for MySQL
    /// Used for teaching EF Core concepts (Bài 10)
    /// </summary>
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options)
        {
        }

        // DbSet for each entity
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                //entity.HasKey(e => e.Guid);
                entity.Property(e => e.UserName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.HasIndex(e => e.UserName).IsUnique();
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(200);
                entity.Property(e => e.Brand).HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(50);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.OriginalPrice).HasPrecision(18, 2);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.IsNew).HasDefaultValue(false);

                // Relationship with Category
                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.ShippingAddress).HasMaxLength(500);
            });

            // Configure OrderDetail entity
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

                // Relationships
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Outerwear", Slug = "outerwear", Description = "Layered essentials for all seasons." },
                new Category { Id = 2, Name = "Tops", Slug = "tops", Description = "Everyday shirts, hoodies and tees." },
                new Category { Id = 3, Name = "Footwear", Slug = "footwear", Description = "Sneakers and boots with modern silhouettes." },
                new Category { Id = 4, Name = "Accessories", Slug = "accessories", Description = "Minimal accessories to complete the look." },
                new Category { Id = 5, Name = "Denim", Slug = "denim", Description = "Premium denim pieces built for comfort." }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Relaxed Blazer",
                    Slug = "relaxed-blazer",
                    Brand = "FormAsh",
                    Gender = "Unisex",
                    Price = 1450000m,
                    OriginalPrice = 1850000m,
                    IsNew = true,
                    Stock = 16,
                    CategoryId = 1,
                    Description = "A relaxed-fit blazer with clean lines and comfortable layering for street-ready styling.",
                    ImageUrl = "https://images.unsplash.com/photo-1594938298603-c8148c4b4545?auto=format&fit=crop&w=900&q=80"
                },
                new Product
                {
                    Id = 2,
                    Name = "Vintage Graphic Tee",
                    Slug = "vintage-graphic-tee",
                    Brand = "Urban Chill",
                    Gender = "Unisex",
                    Price = 420000m,
                    OriginalPrice = 550000m,
                    IsNew = true,
                    Stock = 38,
                    CategoryId = 2,
                    Description = "Soft cotton tee with a subtle retro print and a relaxed silhouette.",
                    ImageUrl = "https://images.unsplash.com/photo-1576566588028-4147f3842f27?auto=format&fit=crop&w=900&q=80"
                },
                new Product
                {
                    Id = 3,
                    Name = "Classic Sneakers",
                    Slug = "classic-sneakers",
                    Brand = "StreetStep",
                    Gender = "Unisex",
                    Price = 980000m,
                    OriginalPrice = 1080000m,
                    IsNew = false,
                    Stock = 24,
                    CategoryId = 3,
                    Description = "Minimal sneakers built with durable leather and cushioning for all-day comfort.",
                    ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=900&q=80"
                },
                new Product
                {
                    Id = 4,
                    Name = "Denim Jacket",
                    Slug = "denim-jacket",
                    Brand = "BlueAura",
                    Gender = "Unisex",
                    Price = 1320000m,
                    OriginalPrice = 1490000m,
                    IsNew = false,
                    Stock = 12,
                    CategoryId = 5,
                    Description = "A classic denim jacket with a soft washed finish and durable stitching.",
                    ImageUrl = "https://images.unsplash.com/photo-1523381210434-271e8be8a52f?auto=format&fit=crop&w=900&q=80"
                },
                new Product
                {
                    Id = 5,
                    Name = "Woven Tote Bag",
                    Slug = "woven-tote-bag",
                    Brand = "LoomLyfe",
                    Gender = "Unisex",
                    Price = 520000m,
                    OriginalPrice = 620000m,
                    IsNew = false,
                    Stock = 45,
                    CategoryId = 4,
                    Description = "A spacious woven tote that combines casual style with everyday function.",
                    ImageUrl = "https://images.unsplash.com/photo-1548036328-c9fa89d128fa?auto=format&fit=crop&w=900&q=80"
                },
                new Product
                {
                    Id = 6,
                    Name = "Wide Leg Jeans",
                    Slug = "wide-leg-jeans",
                    Brand = "Denim House",
                    Gender = "Unisex",
                    Price = 960000m,
                    OriginalPrice = 1100000m,
                    IsNew = false,
                    Stock = 30,
                    CategoryId = 5,
                    Description = "Relaxed wide-leg jeans with a high-rise waist for a modern silhouette.",
                    ImageUrl = "https://images.unsplash.com/photo-1541099649105-f69ad21f3246?auto=format&fit=crop&w=900&q=80"
                },
                new Product
                {
                    Id = 7,
                    Name = "Utility Trench Coat",
                    Slug = "utility-trench-coat",
                    Brand = "Rainform",
                    Gender = "Unisex",
                    Price = 1850000m,
                    OriginalPrice = 2250000m,
                    IsNew = true,
                    Stock = 20,
                    CategoryId = 1,
                    Description = "Oversized trench coat with water-resistant finish and modern utility details.",
                    ImageUrl = "https://images.unsplash.com/photo-1539533018447-63fcce2678e3?auto=format&fit=crop&w=900&q=80"
                },
                new Product
                {
                    Id = 8,
                    Name = "Premium Sweatshirt",
                    Slug = "premium-sweatshirt",
                    Brand = "CalmThread",
                    Gender = "Unisex",
                    Price = 650000m,
                    OriginalPrice = 780000m,
                    IsNew = false,
                    Stock = 27,
                    CategoryId = 2,
                    Description = "Crewneck sweatshirt with premium fleece lining and a clean logo-free look.",
                    ImageUrl = "https://images.unsplash.com/photo-1556821840-3a63f95609a7?auto=format&fit=crop&w=900&q=80"
                },
                new Product
                {
                    Id = 9,
                    Name = "High-Top Sneakers",
                    Slug = "high-top-sneakers",
                    Brand = "Pace",
                    Gender = "Unisex",
                    Price = 890000m,
                    OriginalPrice = 920000m,
                    IsNew = false,
                    Stock = 18,
                    CategoryId = 3,
                    Description = "High-top canvas sneakers with bold stitching and versatile street styling.",
                    ImageUrl = "https://images.unsplash.com/photo-1491553895911-0055eca6402d?auto=format&fit=crop&w=900&q=80"
                },
                new Product
                {
                    Id = 10,
                    Name = "Leather Belt",
                    Slug = "leather-belt",
                    Brand = "BuckleCo",
                    Gender = "Unisex",
                    Price = 360000m,
                    OriginalPrice = 420000m,
                    IsNew = false,
                    Stock = 60,
                    CategoryId = 4,
                    Description = "Minimal leather belt with matte hardware, designed to pair with denim and dress pants.",
                    ImageUrl = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?auto=format&fit=crop&w=900&q=80"
                }
            );

            // Note: Users are managed by AuthService.
            // User seed data is handled by the application layer.
        }
    }
}
