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
    public class SalaryAdvancesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public SalaryAdvancesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/SalaryAdvances
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10; // Số bản ghi trên mỗi trang

            var query = _context.SalaryAdvances
                .Include(e => e.IdeNavigation)  // Include Department
                 // Include Qualification
                .OrderBy(c => c.Idsa);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Idsa);
            }

            var salaryAdvance = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(salaryAdvance);
        }

        // GET: Admins/SalaryAdvances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryAdvance = await _context.SalaryAdvances
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IddNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdqNavigation)
                .FirstOrDefaultAsync(m => m.Idsa == id);
            if (salaryAdvance == null)
            {
                return NotFound();
            }

            return View(salaryAdvance);
        }

        // GET: Admins/SalaryAdvances/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/SalaryAdvances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idsa,Ide,Money,Status,Month,Year")] SalaryAdvance salaryAdvance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salaryAdvance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryAdvance.Ide);
            return View(salaryAdvance);
        }

        // GET: Admins/SalaryAdvances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryAdvance = await _context.SalaryAdvances
                .Include(s => s.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Idsa == id);
            if (salaryAdvance == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryAdvance.Ide);
            return View(salaryAdvance);
        }

        // POST: Admins/SalaryAdvances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idsa,Ide,Money,Status,Month,Year")] SalaryAdvance salaryAdvance)
        {
            if (id != salaryAdvance.Idsa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salaryAdvance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryAdvanceExists(salaryAdvance.Idsa))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryAdvance.Ide);
            return View(salaryAdvance);
        }

        // GET: Admins/SalaryAdvances/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var salaryadvance = await _context.SalaryAdvances.FindAsync(id);
            if (salaryadvance == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.SalaryAdvances.Remove(salaryadvance);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công!" }); // Trả về JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }
        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            var salaryadvance = _context.SalaryAdvances.Find(id);
            if (salaryadvance == null)
            {
                return NotFound();
            }

            salaryadvance.Status = !(salaryadvance.Status ?? false); // Nếu null, mặc định là false rồi đảo trạng thái
            _context.SaveChanges();

            return Json(salaryadvance.Status);
        }

        private bool SalaryAdvanceExists(int id)
        {
            return _context.SalaryAdvances.Any(e => e.Idsa == id);
        }
        public IActionResult ExportToExcel()
        {
            var salaryAdvanceList = _context.SalaryAdvances
                .Select(c => new
                {
                    TenNhanSu = c.IdeNavigation.Name,
                    SoTien = c.Money,
                    Thang = c.Month,
                    Nam = c.Year,
                    TrangThai = c.Status == true ? "Đã duyệt" : "Chờ duyệt"
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachUngLuong");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "Tên Nhân Sự";
                worksheet.Cell(1, 2).Value = "Số Tiền";
                worksheet.Cell(1, 3).Value = "Tháng";
                worksheet.Cell(1, 4).Value = "Năm";
                worksheet.Cell(1, 5).Value = "Trạng Thái";

                // Định dạng tiêu đề
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Đổ dữ liệu vào Excel
                int row = 2;
                foreach (var item in salaryAdvanceList)
                {
                    worksheet.Cell(row, 1).Value = item.TenNhanSu;
                    worksheet.Cell(row, 2).Value = item.SoTien;
                    worksheet.Cell(row, 3).Value = item.Thang;
                    worksheet.Cell(row, 4).Value = item.Nam;
                    worksheet.Cell(row, 5).Value = item.TrangThai;
                    row++;
                }

                // Tự động căn chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Tạo MemoryStream nhưng KHÔNG đóng sớm
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = "DanhSachUngLuong.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }
    }
}
