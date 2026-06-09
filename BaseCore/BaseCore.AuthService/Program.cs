

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using BaseCore.Repository;
using BaseCore.Repository.EFCore;
using BaseCore.Services.Authen;
using System.Text;

// ════════════════════════════════════════════════════════════
// CẤU HÌNH BUILDER
// ════════════════════════════════════════════════════════════
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {

        
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();

// ════════════════════════════════════════════════════════════
// CẤU HÌNH CORS
// ════════════════════════════════════════════════════════════
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ════════════════════════════════════════════════════════════
// SWAGGER & MVC
// ════════════════════════════════════════════════════════════
builder.Services.AddSwaggerGen(c =>
{
    
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BaseCore Auth Service",
        Version = "v1",
        Description = "Authentication microservice — login, register, users, roles."
    });

    
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
            System.Array.Empty<string>()
        }
    });
});

// ════════════════════════════════════════════════════════════
// KẾT NỐI DATABASE
// ════════════════════════════════════════════════════════════
builder.Services.AddDbContext<MySqlDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDb")));

// ════════════════════════════════════════════════════════════
// ĐĂNG KÝ SERVICE (DEPENDENCY INJECTION)
// ════════════════════════════════════════════════════════════
builder.Services.AddScoped<IUserRepositoryEF, UserRepositoryEF>();
builder.Services.AddScoped<IUserService, UserService>();

// ════════════════════════════════════════════════════════════
// JWT & AUTHENTICATION
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

var app = builder.Build();

// ════════════════════════════════════════════════════════════
// MIDDLEWARE PIPELINE
// ════════════════════════════════════════════════════════════
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// ════════════════════════════════════════════════════════════
// MAP CONTROLLERS & RUN
// ════════════════════════════════════════════════════════════
System.Console.WriteLine("BaseCore Auth Service running on port 5002");

app.Run();
