using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebServer.Data;
using WebServer.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WebServerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebServerContext") ?? throw new InvalidOperationException("Connection string 'WebServerContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.RequireHttpsMetadata = false;
    option.SaveToken = true;
    option.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWTParams:Audience"],
        ValidIssuer = builder.Configuration["JWTParams:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTParams:SecretKey"]))
    };
});

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(50);
});
 
builder.Services.AddSingleton<IDictionary<string, string>>(opt => new Dictionary<string, string>());

FirebaseApp.Create(new AppOptions()
{
    //Credential = GoogleCredential.FromAccessToken(google_token),
    Credential = GoogleCredential.FromFile("Firebase-pk.json"),
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow All", builder =>
    {
        builder
        //the host of the react
          .WithOrigins("Http://localhost:3000")
           .AllowAnyMethod()
           .AllowAnyHeader().AllowCredentials();
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
app.UseCors("Allow All");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Comments}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/hubs/chathub");
app.Run();
