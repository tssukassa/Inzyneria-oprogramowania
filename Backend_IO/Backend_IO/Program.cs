using Microsoft.EntityFrameworkCore;
using Backend_IO.Data;
using Backend_IO.Services;  // Добавь это пространство имен для AuthService

var builder = WebApplication.CreateBuilder(args);

// Настройка подключения к базе данных SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=aviationcompany.db"));

builder.Services.AddScoped<AuthService>();  // Это строка добавит AuthService в DI контейнер

// Добавление сервисов для Controllers
builder.Services.AddControllers();

// Добавление поддержки Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Настройка Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
