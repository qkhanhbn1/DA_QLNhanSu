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
using System.Globalization;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class ContractsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public ContractsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/Contracts
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 15; // Số bản ghi trên mỗi trang

            var query = _context.Contracts
                .Include(e => e.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .OrderBy(c => c.Id); // Sắp xếp theo Id để đảm bảo thứ tự trong DB

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var contracts = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(contracts);
        }


        // GET: Admins/Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Admins/Contracts/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            ViewData["Idp"] = new SelectList(_context.Positions, "Idp", "Name");
            return View();
        }

        // POST: Admins/Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codecontract,Ide,SigningDate,ReleaseDate,ExpirationDate,Content,Status")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", contract.Ide);
            
            return View(contract);
        }

        // GET: Admins/Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.IdeNavigation) // Load dữ liệu từ bảng Employees
                .FirstOrDefaultAsync(c => c.Id == id); // Tránh dùng FindAsync, vì nó không hỗ trợ Include

            if (contract == null)
            {
                return NotFound();
            }

            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", contract.Ide);

            return View(contract);
        }


        // POST: Admins/Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codecontract,Ide,SigningDate,ReleaseDate,ExpirationDate,Content,Status")] Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", contract.Ide);
            
            return View(contract);
        }

        // GET: Admins/Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Admins/Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }

        //update stautus
        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            var contract = _context.Contracts.Find(id);
            if (contract == null)
            {
                return NotFound();
            }

            contract.Status = !(contract.Status ?? false); // Nếu null, mặc định là false rồi đảo trạng thái
            _context.SaveChanges();

            return Json(contract.Status);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.Contracts.Remove(contract);
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
            var contracts = _context.Contracts
            .Select(c => new
            {
            TenNhanSu = c.IdeNavigation.Name,
            MaNhanSu = c.IdeNavigation.Code,
            ChucVu = c.IdeNavigation.IdpNavigation.Name,
            MoTa = c.Content,
            NgayBatDau = c.ReleaseDate.HasValue ? c.ReleaseDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
            NgayHetHan = c.ExpirationDate.HasValue ? c.ExpirationDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
            TrangThai = c.Status == true ? "Đã ký" : "Hết hạn"
            })
            .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Hợp đồng nhân sự");
                var currentRow = 1;

                // Tiêu đề cột
                worksheet.Cell(currentRow, 1).Value = "STT";
                worksheet.Cell(currentRow, 2).Value = "Tên nhân sự";
                worksheet.Cell(currentRow, 3).Value = "Mã nhân sự";
                worksheet.Cell(currentRow, 4).Value = "Chức vụ";
                worksheet.Cell(currentRow, 5).Value = "Mô tả";
                worksheet.Cell(currentRow, 6).Value = "Ngày bắt đầu";
                worksheet.Cell(currentRow, 7).Value = "Ngày hết hạn";
                worksheet.Cell(currentRow, 8).Value = "Trạng thái";

                // Định dạng tiêu đề
                for (int i = 1; i <= 8; i++)
                {
                    worksheet.Cell(currentRow, i).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, i).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    worksheet.Cell(currentRow, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Dữ liệu
                int stt = 1;
                foreach (var contract in contracts)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = stt++;
                    worksheet.Cell(currentRow, 2).Value = contract.TenNhanSu;
                    worksheet.Cell(currentRow, 3).Value = contract.MaNhanSu;
                    worksheet.Cell(currentRow, 4).Value = contract.ChucVu;
                    worksheet.Cell(currentRow, 5).Value = contract.MoTa;
                    worksheet.Cell(currentRow, 6).Value = contract.NgayBatDau;
                    worksheet.Cell(currentRow, 7).Value = contract.NgayHetHan;
                    worksheet.Cell(currentRow, 8).Value = contract.TrangThai;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachHopDong.xlsx");
                }
            }
        }
    }
}
