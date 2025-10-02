using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TechHiveAPI.Data;
using TechHiveAPI.Middleware;
using TechHiveAPI.Repositories;
using TechHiveAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure web host for Azure App Service
// var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
// builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Add services to the container
builder.Services.AddControllers();

// Configure EF Core with In-Memory Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("UserDb"));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<DataSeedService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TechHive User Management API",
        Version = "v1",
        Description = "A .NET 9.0 ASP.NET Core Web API for managing users with clean architecture principles."
    });

    // Add XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Add security definition for Bearer token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'TechHive2024SecureToken' in the text input below.\n\nExample: TechHive2024SecureToken",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"   // <-- Lowercase "bearer" for OpenAPI specification compliance
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed the database on startup
using (var scope = app.Services.CreateScope())
{
    var dataSeedService = scope.ServiceProvider.GetRequiredService<DataSeedService>();
    await dataSeedService.SeedDataAsync();
}

// Configure the HTTP request pipeline - CRITICAL: Order matters!
app.UseMiddleware<ErrorHandlingMiddleware>();      // 1. Catches all exceptions first
app.UseDefaultFiles(); // 2. Looks for index.html by default in wwwroot
app.UseStaticFiles();  // 3. Serves static files from wwwroot
app.UseMiddleware<AuthenticationMiddleware>();     // 4. Validates token
app.UseMiddleware<LoggingMiddleware>();            // 5. Logs request/response

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Make Program class accessible to tests
public partial class Program { }
