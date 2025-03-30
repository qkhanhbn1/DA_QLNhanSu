using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userLogin = session.GetString("AdminLogin");
            var userRole = context.HttpContext.Session.GetInt32("UserRole") ?? 2; // 2 là mặc định nhân viên

            var controllerName = context.RouteData.Values["controller"]?.ToString();

            if (string.IsNullOrEmpty(userLogin))
            {
                // Chưa đăng nhập -> Chuyển về trang Login
                context.Result = new RedirectToRouteResult(new { area = "Admins", controller = "Login", action = "Index" });
            }
            else
            {
                // Nếu không phải Admin nhưng cố vào các trang cần quyền Admin -> Chặn
                if (userRole != 1 && (controllerName == "Accounts" || controllerName == "SalaryCalculations"))
                {
                    context.Result = new RedirectToActionResult("Index", "Dashboard", new { area = "Admins" });
                }

            }
            base.OnActionExecuting(context);
        }
    }
}
