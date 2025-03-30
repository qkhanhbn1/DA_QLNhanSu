using DA_QLNhanSu.Models;
using Microsoft.EntityFrameworkCore;

namespace DA_QLNhanSu
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var connectionString = builder.Configuration.GetConnectionString("AppConnectionString");
            builder.Services.AddDbContext<DaQlNhanvienContext>(x => x.UseSqlServer(connectionString));

            // Cấu hình sử dụng session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "DATotNghiep.Session";
            });

            // Đăng ký dịch vụ cho HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

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

            

            // Sử dụng session đã khai báo ở trên
            app.UseSession();

            app.Use(async (context, next) =>
            {
                if (string.IsNullOrEmpty(context.Session.GetString("AdminLogin")) &&
                    context.Request.Cookies.ContainsKey("UserEmail"))
                {
                    // Nếu Session mất nhưng Cookie vẫn còn, thì khôi phục lại
                    context.Session.SetString("AdminLogin", context.Request.Cookies["UserEmail"]);
                    context.Session.SetInt32("UserRole", int.Parse(context.Request.Cookies["UserRole"]));
                }

                await next();
            });

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}",
                defaults: new { area = "Admins" });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
