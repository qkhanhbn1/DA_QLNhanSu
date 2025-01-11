using Microsoft.AspNetCore.Mvc;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
