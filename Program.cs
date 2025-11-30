using LinguaNews.Options;
using LinguaNews.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<INewsDataIngestService, NewsDataIngestService>(); // Due to Service registration, need specific build call for service class
builder.Services.Configure<NewsDataOptions>(
    builder.Configuration.GetSection("NewsData")); // Configuration for external service class
builder.Services.AddControllers()
.AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
 });
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

// Database context registration
// 1. Get the string by its specific name
var connectionString = builder.Configuration.GetConnectionString("LinguaNewsDbContext")
    ?? throw new InvalidOperationException("Connection string 'LinguaNewsDbContext' not found.");

// 2. Register the Context
builder.Services.AddDbContext<LinguaNews.Data.LinguaNewsDbContext>(options =>
    options.UseSqlite(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Mapping endpoints
app.MapRazorPages();

app.MapControllers();

app.Run();