using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA_QLNhanSu.Models;
using X.PagedList;
using ClosedXML.Excel;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class SalaryCalculationsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public SalaryCalculationsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/SalaryCalculations
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10; // Số bản ghi trên mỗi trang

            var query = _context.SalaryCalculations
                .Include(e => e.IdeNavigation)
                .Include(s => s.IdEmployeeallowanceNavigation)
                .Include(s => s.IdSalaryadvanceNavigation)
                .Include(s => s.IdSalaryhistoryNavigation)
                .Include(s => s.IdTimesheetNavigation)
                .Include(s => s.IdeNavigation)
                .OrderBy(c => c.Ids); // Sắp xếp theo Id để đảm bảo thứ tự trong DB

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Ids);
            }

            var salarycalculation = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(salarycalculation);
        }

        // GET: Admins/SalaryCalculations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryCalculation = await _context.SalaryCalculations
                .Include(s => s.IdEmployeeallowanceNavigation)
                .Include(s => s.IdSalaryadvanceNavigation)
                .Include(s => s.IdSalaryhistoryNavigation)
                .Include(s => s.IdTimesheetNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IddNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdqNavigation)
                .FirstOrDefaultAsync(m => m.Ids == id);
            if (salaryCalculation == null)
            {
                return NotFound();
            }

            return View(salaryCalculation);
        }

        // GET: Admins/SalaryCalculations/Create
        public IActionResult Create()
        {
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Money");
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Money");
            ViewData["IdSalaryhistory"] = new SelectList(_context.SalaryHistories, "Id", "Salary");
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Workday");
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/SalaryCalculations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ids,Ide,IdSalaryhistory,IdEmployeeallowance,IdTimesheet,IdSalaryadvance,Month,Year,TotalSalary,Date")] SalaryCalculation salaryCalculation)
        {
            if (ModelState.IsValid)
            {
                // Nếu Date không có giá trị, đặt ngày hiện tại
                salaryCalculation.Date ??= DateOnly.FromDateTime(DateTime.Now);

                // 1️⃣ Lấy số ngày làm từ bảng TimeSheet
                var timeSheet = await _context.TimeSheets
                    .FirstOrDefaultAsync(t => t.Ide == salaryCalculation.Ide && t.Month == salaryCalculation.Month && t.Year == salaryCalculation.Year);

                if (timeSheet == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy bảng chấm công phù hợp!");
                    return View(salaryCalculation);
                }
                salaryCalculation.IdTimesheet = timeSheet.Id;

                // 2️⃣ Lấy lương cứng cao nhất từ SalaryHistory
                var salaryHistory = await _context.SalaryHistories
                    .Where(s => s.Ide == salaryCalculation.Ide)
                    .OrderByDescending(s => s.Salary)
                    .FirstOrDefaultAsync();

                if (salaryHistory == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy thông tin lương cứng của nhân viên!");
                    return View(salaryCalculation);
                }
                salaryCalculation.IdSalaryhistory = salaryHistory.Id;

                // 3️⃣ Lấy phụ cấp từ EmployeeAllowance
                var employeeAllowance = await _context.EmployeeAllowances
                    .FirstOrDefaultAsync(e => e.Ide == salaryCalculation.Ide);

                salaryCalculation.IdEmployeeallowance = employeeAllowance?.Id;

                // 4️⃣ Lấy ứng lương theo tháng/năm từ SalaryAdvance
                var salaryAdvance = await _context.SalaryAdvances
                    .FirstOrDefaultAsync(s => s.Ide == salaryCalculation.Ide && s.Month == salaryCalculation.Month && s.Year == salaryCalculation.Year);

                salaryCalculation.IdSalaryadvance = salaryAdvance?.Idsa;

                // 5️⃣ Tính tổng tiền lương theo công thức: 
                //    (số ngày làm * (lương cứng/26) + phụ cấp - ứng lương)
                decimal salary = salaryHistory?.Salary ?? 0; // Nếu không có lương cứng, gán giá trị mặc định là 0

                decimal workdays = timeSheet.Workday ?? 0;
                decimal allowance = employeeAllowance?.Money ?? 0;
                decimal advanceSalary = salaryAdvance?.Money ?? 0;

                salaryCalculation.TotalSalary = Math.Round((workdays * (salary / 26)) + allowance - advanceSalary, 2);

                // 6️⃣ Lưu vào DB
                _context.Add(salaryCalculation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, hiển thị lại form với dữ liệu hợp lệ
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Money", salaryCalculation.IdEmployeeallowance);
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Money", salaryCalculation.IdSalaryadvance);
            ViewData["IdSalaryhistory"] = new SelectList(_context.SalaryHistories, "Id", "Salary", salaryCalculation.IdSalaryhistory);
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Workday", salaryCalculation.IdTimesheet);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryCalculation.Ide);

            return View(salaryCalculation);
        }


        // GET: Admins/SalaryCalculations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryCalculation = await _context.SalaryCalculations
    .Include(s => s.IdSalaryadvanceNavigation)
    .Include(s => s.IdeNavigation)
    .FirstOrDefaultAsync(s => s.Ids == id);
            if (salaryCalculation == null)
            {
                return NotFound();
            }
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Money", salaryCalculation.IdEmployeeallowance);
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Money", salaryCalculation.IdSalaryadvance);
            ViewData["IdSalaryhistory"] = new SelectList(_context.SalaryHistories, "Id", "Salary", salaryCalculation.IdSalaryhistory);
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Workday", salaryCalculation.IdTimesheet);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryCalculation.Ide);
            return View(salaryCalculation);
        }

        // POST: Admins/SalaryCalculations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ids,Ide,IdSalaryhistory,IdEmployeeallowance,IdTimesheet,IdSalaryadvance,Month,Year,TotalSalary,Date")] SalaryCalculation salaryCalculation)
        {
            if (id != salaryCalculation.Ids)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salaryCalculation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryCalculationExists(salaryCalculation.Ids))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Money", salaryCalculation.IdEmployeeallowance);
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Money", salaryCalculation.IdSalaryadvance);
            ViewData["IdSalaryhistory"] = new SelectList(_context.SalaryHistories, "Id", "Salary", salaryCalculation.IdSalaryhistory);
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Workday", salaryCalculation.IdTimesheet);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryCalculation.Ide);
            return View(salaryCalculation);
        }

        // GET: Admins/SalaryCalculations/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var salaryCalculation = await _context.SalaryCalculations.FindAsync(id);
            if (salaryCalculation == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.SalaryCalculations.Remove(salaryCalculation);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công!" }); // Trả về JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }

        private bool SalaryCalculationExists(int id)
        {
            return _context.SalaryCalculations.Any(e => e.Ids == id);
        }
        [HttpGet]
        public async Task<IActionResult> GetWorkday(int ide, int month, int year)
        {
            var workday = await _context.TimeSheets
                .Where(t => t.Ide == ide && t.Month == month && t.Year == year)
                .Select(t => t.Workday)
                .FirstOrDefaultAsync();

            if (workday == null)
            {
                return Json(new { workday = 0, message = "Không tìm thấy dữ liệu" });
            }

            return Json(new { workday = workday });
        }
        [HttpGet]
        public async Task<IActionResult> GetMaxSalary(int ide)
        {
            var maxSalary = await _context.SalaryHistories
                .Where(s => s.Ide == ide)
                .OrderByDescending(s => s.Salary) // Lấy lương lớn nhất
                .Select(s => s.Salary)
                .FirstOrDefaultAsync();

            return Json(new { maxSalary = maxSalary });
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeAllowance(int ide)
        {
            var allowance = await _context.EmployeeAllowances
                .Where(e => e.Ide == ide)
                .Select(e => e.Money)
                .FirstOrDefaultAsync();

            return Json(new { allowance = allowance });
        }
        [HttpGet]
        public async Task<IActionResult> GetSalaryAdvance(int ide, int month, int year)
        {
            var advanceSalary = await _context.SalaryAdvances
                .Where(s => s.Ide == ide && s.Month == month && s.Year == year)
                .Select(s => s.Money)
                .FirstOrDefaultAsync();

            return Json(new { advanceSalary = advanceSalary });
        }
        [HttpGet]
        public async Task<IActionResult> CalculateTotalSalary(int ide, int month, int year)
        {
            // Lấy lương cứng cao nhất của nhân viên
            decimal salary = await _context.SalaryHistories
                .Where(s => s.Ide == ide)
                .OrderByDescending(s => s.Salary)
                .Select(s => s.Salary)
                .FirstOrDefaultAsync() ?? 0;

            // Lấy số ngày làm từ bảng TimeSheet
            int workdays = await _context.TimeSheets
                .Where(t => t.Ide == ide && t.Month == month && t.Year == year)
                .Select(t => t.Workday)
                .FirstOrDefaultAsync() ?? 0;

            // Lấy phụ cấp từ bảng EmployeeAllowance
            decimal allowance = await _context.EmployeeAllowances
                .Where(e => e.Ide == ide)
                .Select(e => e.Money)
                .FirstOrDefaultAsync() ?? 0;

            // Lấy tiền ứng lương từ bảng SalaryAdvance
            decimal advanceSalary = await _context.SalaryAdvances
                .Where(s => s.Ide == ide && s.Month == month && s.Year == year)
                .Select(s => s.Money)
                .FirstOrDefaultAsync() ?? 0;

            // Kiểm tra giá trị tránh lỗi chia cho 0
            decimal dailySalary = (salary > 0) ? (salary / 26) : 0;
            decimal totalSalary = (workdays * dailySalary) + allowance - advanceSalary;

            return Json(new { totalSalary = totalSalary });
        }
        public IActionResult ExportToExcel()
        {
            var salaryList = _context.SalaryCalculations
                .Select(s => new
                {
                    TenNhanSu = s.IdeNavigation.Name,
                    Thang = s.Month,
                    Nam = s.Year,
                    NgayCong = s.IdTimesheetNavigation.Workday,
                    LuongCung = s.IdSalaryhistoryNavigation.Salary,
                    PhuCap = s.IdEmployeeallowanceNavigation.Money,
                    UngLuong = s.IdSalaryadvanceNavigation != null ? s.IdSalaryadvanceNavigation.Money : 0,
                    TongTien = s.TotalSalary,
                    NgayTao = s.Date.HasValue ? s.Date.Value.ToString("dd/MM/yyyy") : ""
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("BangTinhLuong");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "Tên Nhân Sự";
                worksheet.Cell(1, 2).Value = "Tháng";
                worksheet.Cell(1, 3).Value = "Năm";
                worksheet.Cell(1, 4).Value = "Ngày Công";
                worksheet.Cell(1, 5).Value = "Lương Cứng";
                worksheet.Cell(1, 6).Value = "Phụ Cấp";
                worksheet.Cell(1, 7).Value = "Ứng Lương";
                worksheet.Cell(1, 8).Value = "Tổng Tiền";
                worksheet.Cell(1, 9).Value = "Ngày Tạo";

                // Định dạng tiêu đề
                var headerRange = worksheet.Range("A1:I1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Đổ dữ liệu vào Excel
                int row = 2;
                foreach (var item in salaryList)
                {
                    worksheet.Cell(row, 1).Value = item.TenNhanSu;
                    worksheet.Cell(row, 2).Value = item.Thang;
                    worksheet.Cell(row, 3).Value = item.Nam;
                    worksheet.Cell(row, 4).Value = item.NgayCong;
                    worksheet.Cell(row, 5).Value = item.LuongCung;
                    worksheet.Cell(row, 6).Value = item.PhuCap;
                    worksheet.Cell(row, 7).Value = item.UngLuong;
                    worksheet.Cell(row, 8).Value = item.TongTien;
                    worksheet.Cell(row, 9).Value = item.NgayTao;
                    row++;
                }

                // Tự động căn chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Xuất file Excel
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = "BangTinhLuong.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }

        }


    }
}
