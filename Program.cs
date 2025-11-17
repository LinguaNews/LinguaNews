var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

<<<<<<< HEAD
=======
//Binding API keys to config class for data feed
builder.Services.Configure<NewsApiAiOptions>(
    builder.Configuration.GetSection("NewsApiAi"));


>>>>>>> parent of 35a031d (Fixed appsettings error and handled reclassification of controller attributes to align with NEWSDATA.io JSON schema)
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
