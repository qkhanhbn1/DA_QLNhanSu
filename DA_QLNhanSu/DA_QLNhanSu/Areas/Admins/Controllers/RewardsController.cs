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
    public class RewardsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public RewardsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/Rewards
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10; // Số bản ghi trên mỗi trang

            var query = _context.Rewards
                .Include(e => e.IdeNavigation)  // Include Department
                 // Include Qualification
                .OrderBy(c => c.Id);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var reward = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(reward);
        }

        // GET: Admins/Rewards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reward = await _context.Rewards
                .Include(r => r.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reward == null)
            {
                return NotFound();
            }

            return View(reward);
        }

        // GET: Admins/Rewards/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/Rewards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,RewardDate,Content,RewardGift,Status")] Reward reward)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", reward.Ide);
            return View(reward);
        }

        // GET: Admins/Rewards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reward = await _context.Rewards
                .Include(r => r.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reward == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", reward.Ide);
            return View(reward);
        }

        // POST: Admins/Rewards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,RewardDate,Content,RewardGift,Status")] Reward reward)
        {
            if (id != reward.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RewardExists(reward.Id))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", reward.Ide);
            return View(reward);
        }

        // GET: Admins/Rewards/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            if (reward == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.Rewards.Remove(reward);
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
            var reward = _context.Rewards.Find(id);
            if (reward == null)
            {
                return NotFound();
            }

            reward.Status = !(reward.Status ?? false); // Nếu null, mặc định là false rồi đảo trạng thái
            _context.SaveChanges();

            return Json(reward.Status);
        }
        private bool RewardExists(int id)
        {
            return _context.Rewards.Any(e => e.Id == id);
        }
        public IActionResult ExportToExcel()
        {
            var rewardList = _context.Rewards
                .Select(r => new
                {
                    TenNhanSu = r.IdeNavigation.Name,
                    NgayThuong = r.RewardDate.HasValue ? r.RewardDate.Value.ToString("dd/MM/yyyy") : "",
                    NoiDung = r.Content,
                    PhanThuong = r.RewardGift,
                    TrangThai = r.Status == true ? "Đã duyệt" : "Chờ duyệt"
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachKhenThuong");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "Tên Nhân Sự";
                worksheet.Cell(1, 2).Value = "Ngày Thưởng";
                worksheet.Cell(1, 3).Value = "Nội Dung";
                worksheet.Cell(1, 4).Value = "Phần Thưởng (VNĐ)";
                worksheet.Cell(1, 5).Value = "Trạng Thái";

                // Định dạng tiêu đề
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Đổ dữ liệu vào Excel
                int row = 2;
                foreach (var item in rewardList)
                {
                    worksheet.Cell(row, 1).Value = item.TenNhanSu;
                    worksheet.Cell(row, 2).Value = item.NgayThuong;
                    worksheet.Cell(row, 3).Value = item.NoiDung;
                    worksheet.Cell(row, 4).Value = item.PhanThuong;
                    worksheet.Cell(row, 5).Value = item.TrangThai;
                    row++;
                }

                // Tự động căn chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Tạo MemoryStream nhưng KHÔNG đóng sớm
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = "DanhSachKhenThuong.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }
    }
}
