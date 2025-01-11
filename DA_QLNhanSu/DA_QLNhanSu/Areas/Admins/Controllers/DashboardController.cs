using Microsoft.AspNetCore.Mvc;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    //[Area("Admins")]
    public class DashboardController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
