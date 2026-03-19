using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MvcMovieContext");

// On Azure App Service, store SQLite DB in HOME\data
if (!builder.Environment.IsDevelopment())
{
    var home = Environment.GetEnvironmentVariable("HOME");

    if (!string.IsNullOrEmpty(home))
    {
        var dataDir = Path.Combine(home, "data");
        Directory.CreateDirectory(dataDir);

        var dbPath = Path.Combine(dataDir, "MvcMovie.db");
        connectionString = $"Data Source={dbPath}";
    }
}

builder.Services.AddDbContext<MvcMovieContext>(options =>
    options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();