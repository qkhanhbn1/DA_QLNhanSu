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
                 .Include(s => s.IdeNavigation)
        .Include(s => s.IdPositionNavigation)
        .Include(s => s.IdEmployeeallowanceNavigation)
        .Include(s => s.IdOvertimeNavigation)
        .Include(s => s.IdSalaryadvanceNavigation)
                .OrderBy(c => c.Ide);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Ide);
            }

            var salaryCalculation = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(salaryCalculation);
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
                .Include(s => s.IdOvertimeNavigation)
                .Include(s => s.IdPositionNavigation)
                .Include(s => s.IdSalaryadvanceNavigation)
                .Include(s => s.IdTimesheetNavigation)
                .Include(s => s.IdeNavigation)
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
            ViewData["IdOvertime"] = new SelectList(_context.Overtimes, "Ido", "OvertimePay");
            ViewData["IdPosition"] = new SelectList(_context.Positions, "Idp", "Name");
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Money");
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Workday");
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");

            return View();
        }

        // POST: Admins/SalaryCalculations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ids,Ide,IdPosition,IdOvertime,IdEmployeeallowance,IdTimesheet,IdSalaryadvance,Month,Year,Workday,TotalSalary,Date")] SalaryCalculation salaryCalculation)
        {
            if (ModelState.IsValid)
            {
                // Tính lại TotalSalary (tránh phụ thuộc vào dữ liệu từ form)
                var position = await _context.Positions.FindAsync(salaryCalculation.IdPosition);
                var overtime = await _context.Overtimes.FindAsync(salaryCalculation.IdOvertime);
                var employeeAllowance = await _context.EmployeeAllowances.FindAsync(salaryCalculation.IdEmployeeallowance);
                var salaryAdvance = await _context.SalaryAdvances.FindAsync(salaryCalculation.IdSalaryadvance);

                decimal dailyWage = position?.DailyWage ?? 0;
                int workdays = salaryCalculation.Workday ?? 0;
                decimal overtimePay = overtime?.OvertimePay ?? 0;
                decimal allowances = employeeAllowance?.Money ?? 0;
                decimal salaryAdvances = salaryAdvance?.Money ?? 0;

                salaryCalculation.TotalSalary = (dailyWage * workdays) + overtimePay + allowances - salaryAdvances;

                salaryCalculation.Date = DateTime.Now;
                _context.Add(salaryCalculation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu Model không hợp lệ, giữ lại các giá trị đã chọn trong dropdown
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Money", salaryCalculation.IdEmployeeallowance);
            ViewData["IdOvertime"] = new SelectList(_context.Overtimes, "Ido", "OvertimePay", salaryCalculation.IdOvertime);
            ViewData["IdPosition"] = new SelectList(_context.Positions, "Idp", "Name", salaryCalculation.IdPosition);
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Money", salaryCalculation.IdSalaryadvance);
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
                .Include(s => s.IdEmployeeallowanceNavigation)
                .Include(s => s.IdOvertimeNavigation)
                .Include(s => s.IdPositionNavigation)
                .Include(s => s.IdSalaryadvanceNavigation)
                .Include(s => s.IdTimesheetNavigation)
                .Include(s => s.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Ids == id);
            if (salaryCalculation == null)
            {
                return NotFound();
            }
            ViewData["IdEmployeeallowance"] = new SelectList(_context.EmployeeAllowances, "Id", "Money", salaryCalculation.IdEmployeeallowance);
            ViewData["IdOvertime"] = new SelectList(_context.Overtimes, "Ido", "OvertimePay", salaryCalculation.IdOvertime);
            ViewData["IdPosition"] = new SelectList(_context.Positions, "Idp", "Name", salaryCalculation.IdPosition);
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Money", salaryCalculation.IdSalaryadvance);
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Workday", salaryCalculation.IdTimesheet);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryCalculation.Ide);
            return View(salaryCalculation);
        }

        // POST: Admins/SalaryCalculations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ids,Ide,IdPosition,IdOvertime,IdEmployeeallowance,IdTimesheet,IdSalaryadvance,Month,Year,Workday,TotalSalary,Date")] SalaryCalculation salaryCalculation)
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
            ViewData["IdOvertime"] = new SelectList(_context.Overtimes, "Ido", "OvertimePay", salaryCalculation.IdOvertime);
            ViewData["IdPosition"] = new SelectList(_context.Positions, "Idp", "Name", salaryCalculation.IdPosition);
            ViewData["IdSalaryadvance"] = new SelectList(_context.SalaryAdvances, "Idsa", "Money", salaryCalculation.IdSalaryadvance);
            ViewData["IdTimesheet"] = new SelectList(_context.TimeSheets, "Id", "Workday", salaryCalculation.IdTimesheet);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryCalculation.Ide);
            return View(salaryCalculation);
        }

        // GET: Admins/SalaryCalculations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryCalculation = await _context.SalaryCalculations
                .Include(s => s.IdEmployeeallowanceNavigation)
                .Include(s => s.IdOvertimeNavigation)
                .Include(s => s.IdPositionNavigation)
                .Include(s => s.IdSalaryadvanceNavigation)
                .Include(s => s.IdTimesheetNavigation)
                .Include(s => s.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Ids == id);
            if (salaryCalculation == null)
            {
                return NotFound();
            }

            return View(salaryCalculation);
        }

        // POST: Admins/SalaryCalculations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salaryCalculation = await _context.SalaryCalculations.FindAsync(id);
            if (salaryCalculation != null)
            {
                _context.SalaryCalculations.Remove(salaryCalculation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryCalculationExists(int id)
        {
            return _context.SalaryCalculations.Any(e => e.Ids == id);
        }

        //tinh luong
        [HttpGet]
        public async Task<IActionResult> GetData(int id, string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return BadRequest("Type is required.");
            }

            switch (type.ToLower())
            {
                case "dailywage":
                    var position = await _context.Positions.FindAsync(id);
                    if (position == null) return NotFound();
                    return Json(new { value = position.DailyWage });

                case "overtimepay":
                    var overtime = await _context.Overtimes.FindAsync(id);
                    if (overtime == null) return NotFound();
                    return Json(new { value = overtime.OvertimePay });

                case "allowancemoney":
                    var allowance = await _context.EmployeeAllowances.FindAsync(id);
                    if (allowance == null) return NotFound();
                    return Json(new { value = allowance.Money });

                case "advancemoney":
                    var salaryAdvance = await _context.SalaryAdvances.FindAsync(id);
                    if (salaryAdvance == null) return NotFound();
                    return Json(new { value = salaryAdvance.Money });

                default:
                    return BadRequest("Invalid type.");
            }
        }
        //tinh workday
        [HttpGet]
        public async Task<IActionResult> GetWorkday(int ide, int month, int year)
        {
            var timeSheet = await _context.TimeSheets
                .FirstOrDefaultAsync(t => t.Ide == ide && t.Month == month && t.Year == year);

            if (timeSheet == null)
            {
                return NotFound(new { message = "TimeSheet not found for the given criteria." });
            }

            return Json(new { workday = timeSheet.Workday });
        }
        //lay dữ liệu từ Ide nhân viên
        [HttpGet]
        public JsonResult GetPositionAndDailyWage(int ide)
        {
            // Lấy IdPosition từ Employee theo Ide
            var employee = _context.Employees
                .Where(e => e.Ide == ide)
                .FirstOrDefault();

            // Lấy DailyWage từ bảng Position dựa trên IdPosition
            var dailyWage = employee != null && employee.Idp.HasValue
                ? _context.Positions
                    .Where(p => p.Idp == employee.Idp)
                    .Select(p => p.DailyWage)
                    .FirstOrDefault()
                : (decimal?)null;

            return Json(new { idPosition = employee?.Idp, dailyWage });
        }
        [HttpGet]
        public JsonResult GetEmployeeDetails(int ide)
        {
            // Lấy IdPosition từ bảng Employee
            var employee = _context.Employees
                .Where(e => e.Ide == ide)
                .FirstOrDefault();

            // Nếu không tìm thấy employee, trả về kết quả mặc định
            if (employee == null)
            {
                return Json(new
                {
                    dailyWage = 0,
                    overtimePay = 0,
                    allowanceMoney = 0,
                    advanceMoney = 0
                });
            }
            // Lấy IdPosition từ bảng Employee
            var idPosition = employee.Idp;
            // Lấy DailyWage từ bảng Position theo IdPosition (Idp)
            var position = _context.Positions
                .Where(p => p.Idp == employee.Idp)
                .FirstOrDefault();
            var dailyWage = position?.DailyWage ?? 0;

            // Lấy OvertimePay từ bảng Overtime theo Ide
            var overtime = _context.Overtimes
                .Where(o => o.Ide == ide)
                .FirstOrDefault();
            var overtimePay = overtime?.OvertimePay ?? 0;

            // Lấy Money từ bảng EmployeeAllowance theo Ide
            var employeeAllowance = _context.EmployeeAllowances
                .Where(ea => ea.Ide == ide)
                .FirstOrDefault();
            var allowanceMoney = employeeAllowance?.Money ?? 0;

            // Lấy Money từ bảng SalaryAdvance theo Ide
            var salaryAdvance = _context.SalaryAdvances
                .Where(sa => sa.Ide == ide)
                .FirstOrDefault();
            var advanceMoney = salaryAdvance?.Money ?? 0;

            // Trả về kết quả
            return Json(new
            {
                idPosition,
                dailyWage,
                overtimePay,
                allowanceMoney,
                advanceMoney
            });
        }
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
        public IActionResult ExportToExcel()
        {
            var salaryList = _context.SalaryCalculations
                .Select(s => new
                {
                    TenNhanSu = s.IdeNavigation.Name,
                    Thang = s.Month,
                    Nam = s.Year,
                    SoNgayLam = s.Workday,
                    PhuCap = s.IdEmployeeallowanceNavigation != null ? s.IdEmployeeallowanceNavigation.Money : 0,
                    TangCa = s.IdOvertimeNavigation != null ? s.IdOvertimeNavigation.OvertimePay : 0,
                    UngLuong = s.IdSalaryadvanceNavigation != null ? s.IdSalaryadvanceNavigation.Money : 0,
                    TongTien = s.TotalSalary
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("BangLuongDinhKy");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "Tên Nhân Sự";
                worksheet.Cell(1, 2).Value = "Tháng";
                worksheet.Cell(1, 3).Value = "Năm";
                worksheet.Cell(1, 4).Value = "Số Ngày Làm";
                worksheet.Cell(1, 5).Value = "Phụ Cấp";
                worksheet.Cell(1, 6).Value = "Tăng Ca";
                worksheet.Cell(1, 7).Value = "Ứng Lương";
                worksheet.Cell(1, 8).Value = "Tổng Tiền";

                // Định dạng tiêu đề
                var headerRange = worksheet.Range("A1:H1");
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
                    worksheet.Cell(row, 4).Value = item.SoNgayLam;
                    worksheet.Cell(row, 5).Value = item.PhuCap;
                    worksheet.Cell(row, 6).Value = item.TangCa;
                    worksheet.Cell(row, 7).Value = item.UngLuong;
                    worksheet.Cell(row, 8).Value = item.TongTien;
                    row++;
                }

                // Tự động căn chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Tạo MemoryStream nhưng KHÔNG đóng sớm
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = "BangLuongDinhKy.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }

    }
}
