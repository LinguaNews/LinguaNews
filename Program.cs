using LinguaNews.Options;
using LinguaNews.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<INewsDataIngestService, NewsDataIngestService>(); // Due to Service registration, need specific build call for service class
builder.Services.Configure<NewsDataOptions>(
    builder.Configuration.GetSection("NewsData")); // Configuration for external service class
// Long story short, we are injecting all of our ingestion / API call / filtering logic via this Service class
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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