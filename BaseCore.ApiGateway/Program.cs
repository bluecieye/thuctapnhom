

using Ocelot.DependencyInjection;
using Ocelot.Middleware;

// ════════════════════════════════════════════════════════════
// CẤU HÌNH BUILDER
// ════════════════════════════════════════════════════════════
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// ════════════════════════════════════════════════════════════
// SWAGGER & MVC
// ════════════════════════════════════════════════════════════
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ════════════════════════════════════════════════════════════
// CẤU HÌNH CORS
// ════════════════════════════════════════════════════════════
builder.Services.AddCors(options =>
{

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ════════════════════════════════════════════════════════════
// YARP REVERSE PROXY
// ════════════════════════════════════════════════════════════
builder.Services.AddOcelot();

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

await app.UseOcelot();

Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║              BaseCore API Gateway                            ║
║══════════════════════════════════════════════════════════════║
║  Gateway:        http://localhost:5000                       ║
║  User Service:   http://localhost:5003                       ║
║  Product Service: http://localhost:5001                      ║
║  Order Service:  http://localhost:5002                       ║
╚══════════════════════════════════════════════════════════════╝
");

// ════════════════════════════════════════════════════════════
// KHỞI ĐỘNG ỨNG DỤNG
// ════════════════════════════════════════════════════════════
app.Run();
