using DA_QLNhanSu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetTotalSalary()
        {
            var totalSalaries = await _context.SalaryCalculations
                .GroupBy(s => new { s.Year, s.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalSalary = g.Sum(s => s.TotalSalary)
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            if (!totalSalaries.Any())
            {
                return Json(new { error = "No data found" });
            }

            return Json(totalSalaries);
        }
    
    }
}
