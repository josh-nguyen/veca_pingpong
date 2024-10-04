using pingpong.Services;
using Microsoft.EntityFrameworkCore;
using pingpong.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// var connection = String.Empty;
var connection = "Server=tcp:vecapingpong.database.windows.net,1433;Initial Catalog=vecapingpong;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";";

// if (builder.Environment.IsDevelopment())
// {
//     builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
//     connection = builder.Configuration.GetConnectionString("DefaultConnection");
// }
// else
// {
//     connection = Environment.GetEnvironmentVariable("DefaultConnection");
// }

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Player}/{action=Index}/{id?}");

app.Run();