using DA_QLNhanSu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class LoginController : Controller
    {
        public DaQlNhanvienContext _context;

        public LoginController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        [HttpGet]// get, hiển thị form để nhập dữ liệu
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Models.Login model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Dữ liệu không hợp lệ.";
                return View(model);
            }
            var pass = model.Password.Trim();
            var dataLogin = _context.Accounts
            .Where(x => x.Email == model.Email)
            .Select(x => new
            {
            x.Email,
            x.Password,
            x.Idrole,
            EmployeeId = _context.Employees
            .Where(e => e.Email == model.Email) // Lấy nhân viên đúng với Email đăng nhập
            .Select(e => e.Ide)
            .FirstOrDefault()
            })
            .FirstOrDefault();

            if (dataLogin != null && dataLogin.Password != null && dataLogin.Password == pass)
            {
                HttpContext.Session.SetString("AdminLogin", dataLogin.Email);
                HttpContext.Session.SetInt32("UserRole", dataLogin.Idrole ?? 2);
                HttpContext.Session.SetInt32("UserId", dataLogin.EmployeeId); // 👉 Lưu ID nhân viên vào session
                
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7), // Cookie tồn tại 7 ngày
                    HttpOnly = true,
                    IsEssential = true
                };

                Response.Cookies.Append("UserEmail", dataLogin.Email, cookieOptions);
                Response.Cookies.Append("UserRole", (dataLogin.Idrole ?? 2).ToString(), cookieOptions);
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.ErrorMessage = "Email hoặc mật khẩu không đúng!";
            return View(model);
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa toàn bộ session khi đăng xuất
            return RedirectToAction("Index");
        }
    }
}