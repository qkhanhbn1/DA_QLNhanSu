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
    public class EmployeeAllowancesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public EmployeeAllowancesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/EmployeeAllowances
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10; // Số bản ghi trên mỗi trang

            var query = _context.EmployeeAllowances
                .Include(e => e.IdeNavigation)  // Include Department
                .Include(e => e.IdAllowancesNavigation)          // Include Position
                 // Include Qualification
                .OrderBy(c => c.Id);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var employeeAllowance = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAllowance = await _context.EmployeeAllowances
                .Include(e => e.IdAllowancesNavigation)
                .Include(e => e.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeAllowance == null)
            {
                return NotFound();
            }

            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Create
        public IActionResult Create()
        {
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Name");
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/EmployeeAllowances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,IdAllowances,Money")] EmployeeAllowance employeeAllowance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeAllowance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Name", employeeAllowance.IdAllowances);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", employeeAllowance.Ide);
            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAllowance = await _context.EmployeeAllowances
                .Include(e => e.IdAllowancesNavigation)
                .Include(e => e.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeAllowance == null)
            {
                return NotFound();
            }
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Name", employeeAllowance.IdAllowances);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", employeeAllowance.Ide);
            return View(employeeAllowance);
        }

        // POST: Admins/EmployeeAllowances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,IdAllowances,Money")] EmployeeAllowance employeeAllowance)
        {
            if (id != employeeAllowance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeAllowance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeAllowanceExists(employeeAllowance.Id))
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
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Name", employeeAllowance.IdAllowances);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", employeeAllowance.Ide);
            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Delete/5
        
        private bool EmployeeAllowanceExists(int id)
        {
            return _context.EmployeeAllowances.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var employeeAllowance = await _context.EmployeeAllowances.FindAsync(id);
            if (employeeAllowance == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.EmployeeAllowances.Remove(employeeAllowance);
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
            var allowanceList = _context.EmployeeAllowances
                .Select(c => new
                {
                    TenNhanSu = c.IdeNavigation.Name,
                    SoTien = c.Money,
                    LoaiPhuCap = c.IdAllowancesNavigation.Name
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachPhuCap");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "Tên Nhân Sự";
                worksheet.Cell(1, 2).Value = "Số Tiền";
                worksheet.Cell(1, 3).Value = "Loại Phụ Cấp";

                // Định dạng tiêu đề
                var headerRange = worksheet.Range("A1:C1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Đổ dữ liệu vào Excel
                int row = 2;
                foreach (var item in allowanceList)
                {
                    worksheet.Cell(row, 1).Value = item.TenNhanSu;
                    worksheet.Cell(row, 2).Value = item.SoTien;
                    worksheet.Cell(row, 3).Value = item.LoaiPhuCap;
                    row++;
                }

                // Tự động căn chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Tạo MemoryStream nhưng KHÔNG đóng sớm
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = "DanhSachPhuCap.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }
    }
}
