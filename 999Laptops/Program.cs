using Microsoft.EntityFrameworkCore;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Services;
using LaptopEcommerceAndMore.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ĐĂNG KÝ SQL SERVER ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
var mvcBuilder = builder.Services.AddControllersWithViews();
mvcBuilder.AddViewComponentsAsServices();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Account/Cart");
});
// --- 2. THAY THẾ IN-MEMORY BẰNG SQL SERVICE ---
builder.Services.AddScoped<IDataService, DataService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHeaderBadgeService, HeaderBadgeService>();
builder.Services.AddScoped<ICompareService, CompareService>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
       options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");
app.MapControllers();
app.MapRazorPages();

app.Run();
