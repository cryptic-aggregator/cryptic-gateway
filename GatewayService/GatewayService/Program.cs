using GatewayService.Repositories;
using GatewayService.Services;

var builder = WebApplication.CreateBuilder(args);

// Підключаємо appsettings.json, якщо потрібно
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Отримуємо connectionString
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing!");
}

// Отримуємо секретний ключ JWT
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT secret is missing!");
}

// Реєструємо репозиторій та сервіс
builder.Services.AddScoped<UserRepository>(provider => new UserRepository(connectionString));
builder.Services.AddScoped<UserService>(provider =>
    new UserService(provider.GetRequiredService<UserRepository>(), jwtSecret));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
