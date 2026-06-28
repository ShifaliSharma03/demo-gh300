using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using EmployeeApi.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Controllers with model validation
builder.Services.AddControllers();

// EF Core + SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                     ?? "Data Source=employees.db"));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Employee API",
        Version     = "v1",
        Description = "A production-ready CRUD API for managing employees.",
        Contact     = new OpenApiContact { Name = "API Support", Email = "support@example.com" }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Swagger UI (always enabled so the API is explorable)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API v1");
    c.RoutePrefix = string.Empty; // serve at root
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }

