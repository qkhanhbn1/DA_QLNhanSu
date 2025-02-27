using DA_QLNhanSu.Models;
using Microsoft.AspNetCore.Mvc;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    //[Area("Admins")]
    public class DashboardController : BaseController
    {
        private readonly DaQlNhanvienContext _context;
        public DashboardController(DaQlNhanvienContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Sử dụng context của DbContext để lấy số lượng
            var employeeCount = _context.Employees.Count();
            var contractCount = _context.Contracts.Count();
            var rewardCount = _context.Rewards.Count();
            var disciplineCount = _context.Disciplines.Count();

            // Truyền dữ liệu qua ViewData
            ViewData["EmployeeCount"] = employeeCount;
            ViewData["ContractCount"] = contractCount;
            ViewData["RewardCount"] = rewardCount;
            ViewData["DisciplineCount"] = disciplineCount;
            return View();
        }
    }
}
