using LinguaNews.Data;
using LinguaNews.Models;
using LinguaNews.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Services to the container

// Register the DbContext to use an in-memory database
builder.Services.AddDbContext<LinguaNewsDbContext>(options =>
    options.UseInMemoryDatabase("LinguaNewsDb"));

// Register the mock services from your ArticleSnapshot.cs file
builder.Services.AddScoped<IArticleExtractionService, MockArticleExtractionService>();
builder.Services.AddScoped<ITranslationService, SmartTranslationService>();

builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

// DATA SEEDER (Injecting the Common Words)
// This runs every time the app starts to populate the In-Memory DB
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<LinguaNewsDbContext>();

	// Initialize the In-Memory Database
	context.Database.EnsureCreated();

	// Add some sample common words to test the caching
	if (!context.CommonWords.Any())
	{
		context.CommonWords.AddRange(
		    new CommonWord { OriginalWord = "Hello", LanguageCode = "ES", Translation = "Hola" },
		    new CommonWord { OriginalWord = "World", LanguageCode = "ES", Translation = "Mundo" },
		    new CommonWord { OriginalWord = "House", LanguageCode = "ES", Translation = "Casa" },
		    new CommonWord { OriginalWord = "Time", LanguageCode = "FR", Translation = "Temps" },
		    new CommonWord { OriginalWord = "Year", LanguageCode = "DE", Translation = "Jahr" }
		// To be expanded exhaustively later to 1000+ words via a JSON file load
		);
		context.SaveChanges();
	}
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.Run();