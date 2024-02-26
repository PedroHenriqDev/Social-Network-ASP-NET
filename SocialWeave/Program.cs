using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SocialWeave.Data;
using SocialWeave.Models.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SocialWeave.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ProfilePictureService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<AmountOfPostsHelper>();
builder.Services.AddScoped<NotificationHelper>();
builder.Services.AddScoped<NotificationHelperActionFilter>();
builder.Services.AddScoped<GenerateTrendingPostsService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Social.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
    });

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});

var app = builder.Build();

// Create the directory for profile pictures
string profilePicturesDirectory = Path.Combine(app.Environment.ContentRootPath, "profile-pictures");
if (!Directory.Exists(profilePicturesDirectory))
{
    Directory.CreateDirectory(profilePicturesDirectory);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(profilePicturesDirectory),
    RequestPath = "/profile-pictures"
});

app.Run();
