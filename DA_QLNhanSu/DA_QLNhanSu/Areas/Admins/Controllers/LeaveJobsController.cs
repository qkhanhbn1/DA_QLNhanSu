using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA_QLNhanSu.Models;
using X.PagedList;
using DocumentFormat.OpenXml.Office2010.Excel;
using ClosedXML.Excel;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class LeaveJobsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public LeaveJobsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/LeaveJobs
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 15; // Số bản ghi trên mỗi trang

            var query = _context.LeaveJobs
                .Include(e => e.IdeNavigation)
                .OrderBy(c => c.Id); // Sắp xếp theo Id để đảm bảo thứ tự trong DB

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var leavejob = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(leavejob);
        }
        // GET: Admins/LeaveJobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveJob = await _context.LeaveJobs
                .Include(l => l.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .Include(c => c.IdeNavigation)
                .ThenInclude(e => e.IddNavigation)
                .Include(c => c.IdeNavigation)
                .ThenInclude(e => e.IdqNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveJob == null)
            {
                return NotFound();
            }

            return View(leaveJob);
        }

        // GET: Admins/LeaveJobs/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/LeaveJobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,Status,Date,ReasonLeave,TypeTermination")] LeaveJob leaveJob)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(leaveJob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", leaveJob.Ide);
            return View(leaveJob);
        }

        // GET: Admins/LeaveJobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveJob = await _context.LeaveJobs
                .Include(l => l.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation) // Chức vụ
                 // Phòng ban
                .FirstOrDefaultAsync(m => m.Id == id);

            if (leaveJob == null)
            {
                return NotFound();
            }

            // Sửa lại lấy danh sách từ bảng Employees
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", leaveJob.Ide);

            return View(leaveJob);
        }


        // POST: Admins/LeaveJobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,Status,Date,ReasonLeave,TypeTermination")] LeaveJob leaveJob)
        {
            if (id != leaveJob.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveJob);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveJobExists(leaveJob.Id))
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
            ViewData["Ide"] = new SelectList(_context.Positions, "Ide", "Name", leaveJob.Ide);
            return View(leaveJob);
        }

        // GET: Admins/LeaveJobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveJob = await _context.LeaveJobs
                .Include(l => l.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveJob == null)
            {
                return NotFound();
            }

            return View(leaveJob);
        }

        // POST: Admins/LeaveJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveJob = await _context.LeaveJobs.FindAsync(id);
            if (leaveJob != null)
            {
                _context.LeaveJobs.Remove(leaveJob);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveJobExists(int id)
        {
            return _context.LeaveJobs.Any(e => e.Id == id);
        }
        //update stautus
        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            var leavejob = _context.LeaveJobs.Find(id);
            if (leavejob == null)
            {
                return NotFound();
            }

            leavejob.Status = !(leavejob.Status ?? false); // Nếu null, mặc định là false rồi đảo trạng thái
            _context.SaveChanges();

            return Json(leavejob.Status);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var leavejob = await _context.LeaveJobs.FindAsync(id);
            if (leavejob == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.LeaveJobs.Remove(leavejob);
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
            var leaveJobs = _context.LeaveJobs
                .Select(c => new
                {
                    TenNhanSu = c.IdeNavigation.Name,
                    LyDoNghiViec = c.ReasonLeave,
                    HinhThucNghiViec = c.TypeTermination,
                    NgayNghiViec = c.Date,
                    TrangThai = c.Status == true ? "Đã duyệt" : "Chờ duyệt"
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachNghiViec");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "Tên Nhân Sự";
                worksheet.Cell(1, 2).Value = "Lý Do Nghỉ Việc";
                worksheet.Cell(1, 3).Value = "Hình Thức Nghỉ Việc";
                worksheet.Cell(1, 4).Value = "Ngày Nghỉ Việc";
                worksheet.Cell(1, 5).Value = "Trạng Thái";

                // Định dạng tiêu đề
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Đổ dữ liệu vào Excel
                int row = 2;
                foreach (var item in leaveJobs)
                {
                    worksheet.Cell(row, 1).Value = item.TenNhanSu;
                    worksheet.Cell(row, 2).Value = item.LyDoNghiViec;
                    worksheet.Cell(row, 3).Value = item.HinhThucNghiViec;
                    worksheet.Cell(row, 4).Value = item.NgayNghiViec?.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 5).Value = item.TrangThai;
                    row++;
                }

                // Tự động căn chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Tạo MemoryStream nhưng không đóng sớm
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = "DanhSachDonNghiViec.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }

    }
}
