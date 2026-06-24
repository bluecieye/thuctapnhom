

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;

using Microsoft.OpenApi.Models;

using BaseCore.Common;

using BaseCore.Repository;

using BaseCore.Repository.EFCore;

using BaseCore.Services;

using BaseCore.Services.Authen;

using System.Text;

// ════════════════════════════════════════════════════════════
// KHỞI TẠO BUILDER
// ════════════════════════════════════════════════════════════
var builder = WebApplication.CreateBuilder(args);

// ════════════════════════════════════════════════════════════
// CẤU HÌNH SWAGGER
// ════════════════════════════════════════════════════════════
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {

        
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

        
        
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BaseCore Fashion API", Version = "v1" });

    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,              
        Description = "Bearer JWT token",
        Name = "Authorization",                     
        Type = SecuritySchemeType.Http,             
        BearerFormat = "JWT",
        Scheme = "bearer"                           
    });

    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ════════════════════════════════════════════════════════════
// CẤU HÌNH CORS
// ════════════════════════════════════════════════════════════
builder.Services.AddCors(options =>
{

    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ════════════════════════════════════════════════════════════
// CẤU HÌNH DBCONTEXT
// ════════════════════════════════════════════════════════════
builder.Services.AddDbContext<MySqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDb")));

// ════════════════════════════════════════════════════════════
// ĐĂNG KÝ DỊCH VỤ (DI)
// ════════════════════════════════════════════════════════════
builder.Services.AddScoped<IUserRepositoryEF, UserRepositoryEF>();
builder.Services.AddScoped<IProductRepositoryEF, ProductRepositoryEF>();
builder.Services.AddScoped<IProductVariantRepositoryEF, ProductVariantRepositoryEF>();
builder.Services.AddScoped<ICategoryRepositoryEF, CategoryRepositoryEF>();
builder.Services.AddScoped<IOrderRepositoryEF, OrderRepositoryEF>();
builder.Services.AddScoped<ICartRepositoryEF, CartRepositoryEF>();
builder.Services.AddScoped<ICouponRepositoryEF, CouponRepositoryEF>();
builder.Services.AddScoped<IReviewRepositoryEF, ReviewRepositoryEF>();
builder.Services.AddScoped<IAddressRepositoryEF, AddressRepositoryEF>();
builder.Services.AddScoped<IWishlistRepositoryEF, WishlistRepositoryEF>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IShippingService, ShippingService>();

// ── Email: bind section "Email" → EmailSettings (singleton) + EmailService ──
var emailSettings = builder.Configuration.GetSection("Email").Get<EmailSettings>() ?? new EmailSettings();
builder.Services.AddSingleton(emailSettings);
builder.Services.AddScoped<IEmailService, EmailService>();

// ════════════════════════════════════════════════════════════
// CẤU HÌNH AUTHENTICATION/JWT
// ════════════════════════════════════════════════════════════
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"]
    ?? "YourSecretKeyForAuthenticationShouldBeLongEnough");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(x =>
{
    
    x.RequireHttpsMetadata = false;
    
    x.SaveToken = true;
    
    x.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = false,
        ValidateAudience = false
        
    };
});

// ════════════════════════════════════════════════════════════
// BUILD APP & MIDDLEWARE PIPELINE
// ════════════════════════════════════════════════════════════
var app = builder.Build();

// ════════════════════════════════════════════════════════════
// MIGRATE DB & SEED ADMIN
// ════════════════════════════════════════════════════════════

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MySqlDbContext>();

    db.Database.Migrate();

    if (!db.Users.Any(u => u.Username == "admin"))
    {
        
        var hash = TokenHelper.HashPassword("admin", out string salt);
        db.Users.Add(new BaseCore.Entities.User
        {
            Username = "admin",
            Email = "admin@basecore.com",
            Phone = "",
            PasswordHash = hash,
            Salt = salt,
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
        Console.WriteLine("✅ Admin account seeded (admin / admin)");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();        
    app.UseSwaggerUI();      
}

// ════════════════════════════════════════════════════════════
// STATIC FILES & MEDIA
// ════════════════════════════════════════════════════════════
var mediaRoot = app.Configuration["Media:Root"] ?? "..\\Media";

if (!Path.IsPathRooted(mediaRoot))
    mediaRoot = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, mediaRoot));

Directory.CreateDirectory(Path.Combine(mediaRoot, "products"));

var contentTypes = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
contentTypes.Mappings[".avif"] = "image/avif";    
contentTypes.Mappings[".webp"] = "image/webp";    

app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(mediaRoot),
    RequestPath = "/images",
    ContentTypeProvider = contentTypes
});
Console.WriteLine($"📁 /images → {mediaRoot}");

// ════════════════════════════════════════════════════════════
// MAP CONTROLLERS & RUN
// ════════════════════════════════════════════════════════════
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("BaseCore Fashion API running on port 5001");

app.Run();
