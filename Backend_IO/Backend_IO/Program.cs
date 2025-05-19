/*
 * Program.cs (Minimal API setup for Backend_IO)
 * 
 * This file configures and runs the ASP.NET Core Web API application.
 * It includes setup for:
 * - Entity Framework Core with SQLite databases
 * - Dependency injection for services
 * - JWT-based authentication
 * - Swagger/OpenAPI documentation with JWT support
 */

// Required namespaces
using Microsoft.EntityFrameworkCore;
using Backend_IO.Data;
using Backend_IO.Services; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext for main application database (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=aviationcompany.db"));

// Configure DbContext for bank database (SQLite)
builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlite("Data Source=bank.db"));

// Register AuthService for dependency injection (scoped lifetime)
builder.Services.AddScoped<AuthService>();

// Add controllers support
builder.Services.AddControllers();

// Load JWT settings from configuration (appsettings.json or environment)
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

// Configure JWT authentication middleware
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Disable HTTPS requirement for dev/testing
    options.SaveToken = true;             // Save the token in the authentication properties
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true
    };
});

// Enable API explorer for Swagger/OpenAPI generation
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI documentation generation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Backend_IO", Version = "v1" });

    // Add JWT Bearer security definition for Swagger UI
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter the token in the following format: Bearer {your token}"
    });

    // Add global security requirement so JWT is required on all endpoints (unless otherwise specified)
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();

// Use Swagger middleware in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add authentication and authorization middleware to the request pipeline
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Start the application
app.Run();
