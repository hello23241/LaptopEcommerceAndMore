using Microsoft.EntityFrameworkCore;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Services;
using LaptopEcommerceAndMore.Interfaces;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

var builder = WebApplication.CreateBuilder(args);

// --- 0. CẤU HÌNH AZURE KEY VAULT (CHỈ CHẠY TRÊN CLOUD) ---
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUri = new Uri("https://laptop-ecom-vault-2026.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

// --- 1. REGISTER SQL SERVER (SỬA ĐỂ ĐỌC TRỰC TIẾP KEY MÃ HÓA) ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings--DefaultConnection"]
                       ?? builder.Configuration["ConnectionStrings:DefaultConnection"]));

// [Giữ nguyên đoạn Register Services của bạn...]
var mvcBuilder = builder.Services.AddControllersWithViews();
mvcBuilder.AddViewComponentsAsServices();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Cart");
});

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
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

        options.Cookie.Path = "/";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
});
var app = builder.Build();

// --- 3. SEED DATA INITIALIZATION ---
/*
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var message = ex.InnerException?.Message ?? ex.Message;
        Console.WriteLine("SEED ERROR: " + message);
        // Lưu ý: Trên Cloud, nếu Seed lỗi có thể làm App không Start được. 
        // Hãy đảm bảo Migration đã được chạy thành công trước đó.
    }var builder = WebApplication
}
*/
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.MapControllers();
app.MapRazorPages();

app.Run();