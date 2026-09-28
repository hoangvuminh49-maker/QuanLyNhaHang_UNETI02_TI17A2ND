using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang.Data;

var builder = WebApplication.CreateBuilder(args);

// ===== Đăng ký MVC =====
builder.Services.AddControllersWithViews();

// ===== Đăng ký EF Core + SQL Server =====
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== Đăng ký Session (dùng để lưu thông tin đăng nhập / phân quyền) =====
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ===== Cấu hình HTTP request pipeline =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Phải đặt trước UseAuthorization và trước khi Controller dùng Session

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
