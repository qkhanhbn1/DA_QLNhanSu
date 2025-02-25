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
using System.IO;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class EmployeesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public EmployeesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/Employees
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 12; // Số bản ghi trên mỗi trang

            var query = _context.Employees
                .Include(e => e.IddNavigation)  // Include Department
                .Include(e => e.IdpNavigation)          // Include Position
                .Include(e => e.IdqNavigation) // Include Qualification
                .OrderBy(c => c.Ide);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name)).OrderBy(c => c.Ide);
            }

            var employee = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(employee);
        }

        // GET: Admins/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.IddNavigation)
                .Include(e => e.IdpNavigation)
                .Include(e => e.IdqNavigation)
                .Include(e => e.Contracts)
                .Include(e => e.Insurances)
                .Include(e => e.OnLeaves)
                .Include(e => e.Disciplines)
                .Include(e => e.Rewards)
                .Include(e => e.LeaveJobs)
                .FirstOrDefaultAsync(m => m.Ide == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Admins/Employees/Create
        public IActionResult Create()
        {
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Name");
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name");
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Name");
            return View();
        }

        // POST: Admins/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ide,Name,Code,Gender,Birthday,Email,Phone,Cccd,Address,Image,Idd,Idp,Idq,Marry,Status")] Employee employee)
        {
            try
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 && files[0].Length > 0)
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\employee", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        employee.Image = "/Images/employee/" + FileName;
                    }
                }
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
            }
            catch (Exception ex)
            {
                ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Name", employee.Idd);
                ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name", employee.Idp);
                ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Name", employee.Idq);
                ViewBag.error = ex.Message;
                return View(employee);
            }
        }

        // GET: Admins/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Name", employee.Idd);
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name", employee.Idp);
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Name", employee.Idq);
            return View(employee);
        }

        // POST: Admins/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ide,Name,Code,Gender,Birthday,Email,Phone,Cccd,Address,Image,Idd,Idp,Idq,Marry,Status")] Employee employee)
        {
            if (id != employee.Ide)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    if (files.Count() > 0 && files[0].Length > 0)
                    {
                        var file = files[0];
                        var FileName = file.FileName;

                        // Tạo đường dẫn lưu tệp vào một thư mục chung
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\employee", FileName);

                        // Tạo thư mục nếu chưa tồn tại
                        Directory.CreateDirectory(Path.GetDirectoryName(path));

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            employee.Image = $"/Images/employee/{FileName}";
                        }
                    }
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Ide))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return View(employee);
                    }
                }
                
            }
            // Đưa ra lỗi của modelState để bạn có thể debug
            var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
            // Xem danh sách lỗi trong debug console hoặc log
            foreach (var errorMessage in errorMessages)
            {
                Console.WriteLine(errorMessage);
            }
            ViewData["Idd"] = new SelectList(_context.Departments, "Idd", "Name", employee.Idd);
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name", employee.Idp);
            ViewData["Idq"] = new SelectList(_context.Qualifications, "Idq", "Name", employee.Idq);
            return View(employee);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Ide == id);
        }
        public IActionResult ExportToExcel()
        {
            var employees = _context.Employees
                .Include(e => e.IddNavigation)
                .Include(e => e.IdpNavigation)
                .Include(e => e.IdqNavigation)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employees");
                int currentRow = 1;

                // Tạo tiêu đề cột
                worksheet.Cell(currentRow, 1).Value = "Mã nhân sự";
                worksheet.Cell(currentRow, 2).Value = "Họ và tên";
                worksheet.Cell(currentRow, 3).Value = "Phòng ban";
                worksheet.Cell(currentRow, 4).Value = "Chức vụ";
                worksheet.Cell(currentRow, 5).Value = "Trình độ";
                worksheet.Cell(currentRow, 6).Value = "Ngày sinh";

                // Định dạng tiêu đề
                var headerRange = worksheet.Range(currentRow, 1, currentRow, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Điền dữ liệu
                foreach (var emp in employees)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = emp.Code;
                    worksheet.Cell(currentRow, 2).Value = emp.Name;
                    worksheet.Cell(currentRow, 3).Value = emp.IddNavigation?.Name;
                    worksheet.Cell(currentRow, 4).Value = emp.IdpNavigation?.Name;
                    worksheet.Cell(currentRow, 5).Value = emp.IdqNavigation?.Name;
                    worksheet.Cell(currentRow, 6).Value = emp.Birthday?.ToString("dd/MM/yyyy");
                }

                // Xuất file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachNhanVien.xlsx");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công!" }); // Trả về JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }

    }
}
