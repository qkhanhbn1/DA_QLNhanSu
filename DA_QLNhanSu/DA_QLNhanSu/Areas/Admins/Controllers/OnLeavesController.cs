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
    public class OnLeavesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public OnLeavesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/OnLeaves
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10; // Số bản ghi trên mỗi trang

            var query = _context.OnLeaves
                .Include(e => e.IdeNavigation)  // Include Department
                 // Include Qualification
                .OrderBy(c => c.Id);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var onLeave = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(onLeave);
        }

        // GET: Admins/OnLeaves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var onLeave = await _context.OnLeaves
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IddNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .Include(o => o.IdeNavigation)
                .ThenInclude(e => e.IdqNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (onLeave == null)
            {
                return NotFound();
            }

            return View(onLeave);
        }

        // GET: Admins/OnLeaves/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/OnLeaves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,RequestDate,ReleaseDate,ExpirationDate,Content,Status")] OnLeave onLeave)
        {
            if (ModelState.IsValid)
            {
                _context.Add(onLeave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", onLeave.Ide);
            return View(onLeave);
        }

        // GET: Admins/OnLeaves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var onLeave = await _context.OnLeaves
                .Include(l => l.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation) 
                .FirstOrDefaultAsync(m => m.Id == id);
            if (onLeave == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", onLeave.Ide);
            return View(onLeave);
        }

        // POST: Admins/OnLeaves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,RequestDate,ReleaseDate,ExpirationDate,Content,Status")] OnLeave onLeave)
        {
            if (id != onLeave.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(onLeave);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OnLeaveExists(onLeave.Id))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", onLeave.Ide);
            return View(onLeave);
        }

      
        private bool OnLeaveExists(int id)
        {
            return _context.OnLeaves.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var onleave = await _context.OnLeaves.FindAsync(id);
            if (onleave == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.OnLeaves.Remove(onleave);
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
            var onleave = _context.OnLeaves.Find(id);
            if (onleave == null)
            {
                return NotFound();
            }

            onleave.Status = !(onleave.Status ?? false); // Nếu null, mặc định là false rồi đảo trạng thái
            _context.SaveChanges();

            return Json(onleave.Status);
        }
        public IActionResult ExportToExcel()
        {
            var onLeaveList = _context.OnLeaves
                .Select(c => new
                {
                    TenNhanSu = c.IdeNavigation.Name,
                    NgayNghi = c.ReleaseDate,
                    NgayTroLai = c.ExpirationDate,
                    LyDo = c.Content,
                    TrangThai = c.Status == true ? "Đã duyệt" : "Chờ duyệt"
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachNghiPhep");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "Tên Nhân Sự";
                worksheet.Cell(1, 2).Value = "Ngày Nghỉ";
                worksheet.Cell(1, 3).Value = "Ngày Trở Lại";
                worksheet.Cell(1, 4).Value = "Lý Do";
                worksheet.Cell(1, 5).Value = "Trạng Thái";

                // Định dạng tiêu đề
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Đổ dữ liệu vào Excel
                int row = 2;
                foreach (var item in onLeaveList)
                {
                    worksheet.Cell(row, 1).Value = item.TenNhanSu;
                    worksheet.Cell(row, 2).Value = item.NgayNghi?.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 3).Value = item.NgayTroLai?.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 4).Value = item.LyDo;
                    worksheet.Cell(row, 5).Value = item.TrangThai;
                    row++;
                }

                // Tự động căn chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Tạo MemoryStream nhưng KHÔNG đóng sớm
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = "DanhSachNghiPhep.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }
    }
}
