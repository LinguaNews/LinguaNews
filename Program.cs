using LinguaNews.Options;
using LinguaNews.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
// Registering NewsData Ingest Service
builder.Services.AddHttpClient<INewsDataIngestService, NewsDataIngestService>(); // Due to Service registration, need specific build call for service class
builder.Services.Configure<NewsDataOptions>(
    builder.Configuration.GetSection("NewsData")); // Configuration for external service class
// Registering Translation Service (DeepL)
builder.Services.AddHttpClient<ITranslationService, DeepLTranslationService>();
builder.Services.Configure<DeepLOptions>(
    builder.Configuration.GetSection("DeepL"));

// Adding Controllers with JSON options to handle reference cycles (how to read nested JSON!!)
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