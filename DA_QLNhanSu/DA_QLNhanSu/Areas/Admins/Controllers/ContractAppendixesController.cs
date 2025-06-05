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
    public class ContractAppendixesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public ContractAppendixesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/ContractAppendixes
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 15; // Số bản ghi trên mỗi trang

            var query = _context.ContractAppendices
                .Include(e => e.IdContractNavigation)
                .OrderBy(c => c.Id); // Sắp xếp theo Id để đảm bảo thứ tự trong DB

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdContractNavigation.Codecontract.Contains(name)).OrderBy(c => c.Id);
            }

            var contractAppendices = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(contractAppendices);
        }

        // GET: Admins/ContractAppendixes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractAppendix = await _context.ContractAppendices
                .Include(c => c.IdContractNavigation)
                    .ThenInclude(cn => cn.IdeNavigation) // Bao gồm thông tin nhân viên
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contractAppendix == null)
            {
                return NotFound();
            }

            return View(contractAppendix);
        }


        // GET: Admins/ContractAppendixes/Create
        public IActionResult Create()
        {
            ViewData["IdContract"] = new SelectList(_context.Contracts, "Id", "Id");
            return View();
        }

        // POST: Admins/ContractAppendixes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdContract,SigningDate,TypeAppendix,Content")] ContractAppendix contractAppendix)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contractAppendix);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdContract"] = new SelectList(_context.Contracts, "Id", "Id", contractAppendix.IdContract);
            return View(contractAppendix);
        }

        // GET: Admins/ContractAppendixes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractAppendix = await _context.ContractAppendices.FindAsync(id);
            if (contractAppendix == null)
            {
                return NotFound();
            }
            ViewData["IdContract"] = new SelectList(_context.Contracts, "Id", "Codecontract", contractAppendix.IdContract);
            return View(contractAppendix);
        }

        // POST: Admins/ContractAppendixes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdContract,SigningDate,TypeAppendix,Content")] ContractAppendix contractAppendix)
        {
            if (id != contractAppendix.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contractAppendix);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractAppendixExists(contractAppendix.Id))
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
            ViewData["IdContract"] = new SelectList(_context.Contracts, "Id", "Codecontract", contractAppendix.IdContract);
            return View(contractAppendix);
        }

        // GET: Admins/ContractAppendixes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contractAppendix = await _context.ContractAppendices
                .Include(c => c.IdContractNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contractAppendix == null)
            {
                return NotFound();
            }

            return View(contractAppendix);
        }

        // POST: Admins/ContractAppendixes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contractAppendix = await _context.ContractAppendices.FindAsync(id);
            if (contractAppendix != null)
            {
                _context.ContractAppendices.Remove(contractAppendix);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractAppendixExists(int id)
        {
            return _context.ContractAppendices.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var contractAppendix = await _context.ContractAppendices.FindAsync(id);
            if (contractAppendix == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.ContractAppendices.Remove(contractAppendix);
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
            var data = _context.ContractAppendices
                .Select(c => new
                {
                    MaPhuLuc = c.Id,
                    MaHopDong = c.IdContractNavigation.Codecontract,
                    LoaiPhuLuc = c.TypeAppendix,
                    NoiDung = c.Content,
                    NgayKy = c.SigningDate
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachPhuLucHopDong");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "Mã phụ lục";
                worksheet.Cell(1, 2).Value = "Mã hợp đồng";
                worksheet.Cell(1, 3).Value = "Loại phụ lục";
                worksheet.Cell(1, 4).Value = "Nội dung";
                worksheet.Cell(1, 5).Value = "Ngày ký";

                // Định dạng tiêu đề
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Đổ dữ liệu vào Excel
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.MaPhuLuc;
                    worksheet.Cell(row, 2).Value = item.MaHopDong;
                    worksheet.Cell(row, 3).Value = item.LoaiPhuLuc;
                    worksheet.Cell(row, 4).Value = item.NoiDung;
                    worksheet.Cell(row, 5).Value = item.NgayKy?.ToString("dd/MM/yyyy");
                    row++;
                }

                // Tự động căn chỉnh cột
                worksheet.Columns().AdjustToContents();

                // Tạo MemoryStream
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                string fileName = "DanhSachPhuLucHopDong.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }

    }
}
