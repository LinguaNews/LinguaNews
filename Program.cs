using LinguaNews.Config;
using LinguaNews.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddRazorPages();
builder.Services.Configure<ParkWhizOptions>(builder.Configuration.GetSection("ParkWhiz"));

// Register HttpClient with options-aware configuration
builder.Services.AddHttpClient<IParkWhizService, ParkWhizService>()
    .ConfigureHttpClient((sp, client) =>
    {
        var opts = sp.GetRequiredService<IOptions<ParkWhizOptions>>().Value;
        if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
        {
            client.BaseAddress = new Uri(opts.BaseUrl);
        }

        // Attach API key as a Bearer token if provided. Adjust if ParkWhiz requires a different header or query param.
        if (!string.IsNullOrWhiteSpace(opts.ApiKey))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", opts.ApiKey);
        }

        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    });

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

app.MapRazorPages();

app.Run();


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
