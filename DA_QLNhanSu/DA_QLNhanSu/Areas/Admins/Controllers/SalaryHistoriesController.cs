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
using DocumentFormat.OpenXml.InkML;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class SalaryHistoriesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public SalaryHistoriesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/SalaryHistories
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 15; // Số bản ghi trên mỗi trang

            var query = _context.SalaryHistories
                .Include(e => e.IdeNavigation)
                .OrderBy(c => c.Id); // Sắp xếp theo Id để đảm bảo thứ tự trong DB

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var salaryHistories = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(salaryHistories);
        }

        // GET: Admins/SalaryHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryHistory = await _context.SalaryHistories
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IddNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdqNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salaryHistory == null)
            {
                return NotFound();
            }

            return View(salaryHistory);
        }

        // GET: Admins/SalaryHistories/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/SalaryHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,Salary,EffectiveDate,Note,IdAppendix")] SalaryHistory salaryHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salaryHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryHistory.Ide);
            return View(salaryHistory);
        }

        // GET: Admins/SalaryHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryHistory = await _context.SalaryHistories.FindAsync(id);
            if (salaryHistory == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryHistory.Ide);
            return View(salaryHistory);
        }

        // POST: Admins/SalaryHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,Salary,EffectiveDate,Note,IdAppendix")] SalaryHistory salaryHistory)
        {
            if (id != salaryHistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salaryHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryHistoryExists(salaryHistory.Id))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", salaryHistory.Ide);
            return View(salaryHistory);
        }

        // GET: Admins/SalaryHistories/Delete/5
        

        private bool SalaryHistoryExists(int id)
        {
            return _context.SalaryHistories.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var salaryHistory = await _context.SalaryHistories.FindAsync(id);
            if (salaryHistory == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.SalaryHistories.Remove(salaryHistory);
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
        var salaryHistoryList = _context.SalaryHistories
            .Select(s => new
            {
                TenNhanSu = s.IdeNavigation.Name,
                Luong = s.Salary,
                NgayApDung = s.EffectiveDate.HasValue ? s.EffectiveDate.Value.ToString("dd/MM/yyyy") : "",
                GhiChu = s.Note ?? ""
            })
            .ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("LichSuLuong");

            // Tiêu đề cột
            worksheet.Cell(1, 1).Value = "Tên Nhân Sự";
            worksheet.Cell(1, 2).Value = "Lương";
            worksheet.Cell(1, 3).Value = "Ngày Áp Dụng";
            worksheet.Cell(1, 4).Value = "Ghi Chú";

            // Định dạng tiêu đề
            var headerRange = worksheet.Range("A1:D1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Đổ dữ liệu vào Excel
            int row = 2;
            foreach (var item in salaryHistoryList)
            {
                worksheet.Cell(row, 1).Value = item.TenNhanSu;
                worksheet.Cell(row, 2).Value = item.Luong;
                worksheet.Cell(row, 3).Value = item.NgayApDung;
                worksheet.Cell(row, 4).Value = item.GhiChu;
                row++;
            }

            // Tự động căn chỉnh cột
            worksheet.Columns().AdjustToContents();

            // Xuất file Excel
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string fileName = "LichSuLuong.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(stream, contentType, fileName);
            }
            

        }
        
    }
}
