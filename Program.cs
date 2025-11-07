using BookingHomestay.Data;
using BookingHomestay.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddViewOptions(options =>
{
    options.HtmlHelperOptions.ClientValidationEnabled = true;
});

// Đăng ký IEmailService
builder.Services.AddScoped<IEmailService, EmailService>();
// Thêm dòng bạn yêu cầu: Đăng ký IEmailService với AddTransient
builder.Services.AddTransient<IEmailService, EmailService>();

// Đăng ký IPaymentService
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Cấu hình Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Cấu hình Authorization
builder.Services.AddAuthorization();

// Thêm dòng bạn yêu cầu: Đăng ký logging
builder.Services.AddLogging();

// Get connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

try
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}
catch (Exception ex)
{
    Console.WriteLine($"Lỗi kết nối cơ sở dữ liệu: {ex.Message}");
    throw;
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();