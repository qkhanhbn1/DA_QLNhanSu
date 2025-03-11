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

            var pass = model.Password;
            var dataLogin = _context.Accounts
            .Where(x => x.Email == model.Email && x.Password == pass)
            .Select(x => new { x.Email, x.Idrole }) // Chỉ lấy Email & IdRole
            .FirstOrDefault();

            if (dataLogin != null)
            {
                HttpContext.Session.SetString("AdminLogin", dataLogin.Email);
                HttpContext.Session.SetInt32("UserRole", dataLogin.Idrole ?? 2); // Mặc định là nhân viên (2)

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