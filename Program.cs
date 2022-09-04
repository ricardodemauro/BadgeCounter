using BadgeCounters.Db;
using BadgeCounters.Endpoints;
using BadgeCounters.HandlebarsTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "secrets.json"), optional: true);

builder.Host.UseSerilog();

builder.Services.AddTransient<HandlebarsProvider>();

var basePath = Path.Combine(builder.Environment.WebRootPath, "templates");
var fileProvider = new PhysicalFileProvider(basePath, ExclusionFilters.Hidden);

builder.Services.AddSingleton<IFileProvider>(x => fileProvider);

// Replace with your connection string.
var connectionString = builder.Configuration.GetConnectionString("Store");

var serverVersion = ServerVersion.AutoDetect(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
{
    opts.UseMySql(connectionString, serverVersion)
    // The following three options help with debugging, but should
    // be changed or removed for production.
    .LogTo(o => Log.Information(o), LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors();
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapHandlebars("/profiles");

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await db.Database.EnsureCreatedAsync();

await db.Database.MigrateAsync();

app.Run();
