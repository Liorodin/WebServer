using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebServer.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WebServerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebServerContext") ?? throw new InvalidOperationException("Connection string 'WebServerContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(15);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow All", builder =>
    {
        builder
          .AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();

    });
});

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
app.UseCors("Allow All");
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Comments}/{action=Index}/{id?}");


app.Run();
