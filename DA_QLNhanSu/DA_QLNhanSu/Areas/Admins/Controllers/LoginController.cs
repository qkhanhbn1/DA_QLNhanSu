using Microsoft.AspNetCore.Http;
using DA_QLNhanSu.Areas.Admins.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class LoginController : Controller
    {
        [HttpGet]// get, hiển thị form để nhập dữ liệu
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost] // POST -> khi submit form
        public IActionResult Index(Models.Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);// trả về trạng thái lỗi
            }
            //su li logic tai day

            //lưu session khi đăng nhập thành công
            HttpContext.Session.SetString("AdminLogin", model.Email);
            return RedirectToAction("Index","Dashboard");

        }
        [HttpGet]// thoát đăng nhập, huỷ session
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLogin"); // huỷ session với key AdminLogin đã lưu trước đó

            return RedirectToAction("Index");
        }
    }
}