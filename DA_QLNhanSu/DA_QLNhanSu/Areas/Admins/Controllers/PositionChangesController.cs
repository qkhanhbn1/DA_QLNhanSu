using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA_QLNhanSu.Models;
using X.PagedList;

namespace DA_QLNhanSu.Areas.Admins.Controllers
{
    [Area("Admins")]
    public class PositionChangesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public PositionChangesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/PositionChanges
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 15; // Số bản ghi trên mỗi trang

            var query = _context.PositionChanges
                .Include(e => e.IdeNavigation)
                .OrderBy(c => c.Id); // Sắp xếp theo Id để đảm bảo thứ tự trong DB

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var positionChanges = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(positionChanges);
        }

        // GET: Admins/PositionChanges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionChange = await _context.PositionChanges
                .Include(p => p.IdAppendixNavigation)
                .Include(p => p.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (positionChange == null)
            {
                return NotFound();
            }

            return View(positionChange);
        }

        // GET: Admins/PositionChanges/Create
        public IActionResult Create()
        {
            ViewData["IdAppendix"] = new SelectList(_context.ContractAppendices, "Id", "Name");
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/PositionChanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,Type,Fromrole,Torole,Status,Date,IdAppendix")] PositionChange positionChange)
        {
            if (ModelState.IsValid)
            {
                _context.Add(positionChange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAppendix"] = new SelectList(_context.ContractAppendices, "Id", "Name", positionChange.IdAppendix);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", positionChange.Ide);
            return View(positionChange);
        }

        // GET: Admins/PositionChanges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionChange = await _context.PositionChanges
                .Include(l => l.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation) // Chức vụ
                                                   // Phòng ban
                .FirstOrDefaultAsync(m => m.Id == id);
            if (positionChange == null)
            {
                return NotFound();
            }
            ViewBag.Ide = new SelectList(_context.Employees, "Ide", "Name", positionChange.Ide);
            ViewBag.IdAppendix = new SelectList(_context.ContractAppendices, "Id", "Id", positionChange.IdAppendix);

            return View(positionChange);
        }

        // POST: Admins/PositionChanges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,Type,Fromrole,Torole,Status,Date,IdAppendix")] PositionChange positionChange)
        {
            if (id != positionChange.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(positionChange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionChangeExists(positionChange.Id))
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
            ViewBag.Ide = new SelectList(_context.Employees, "Ide", "Name", positionChange.Ide);
            ViewBag.IdAppendix = new SelectList(_context.ContractAppendices, "Id", "Id", positionChange.IdAppendix);

            return View(positionChange);
        }

        // GET: Admins/PositionChanges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var positionChange = await _context.PositionChanges
                .Include(p => p.IdAppendixNavigation)
                .Include(p => p.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (positionChange == null)
            {
                return NotFound();
            }

            return View(positionChange);
        }

        // POST: Admins/PositionChanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var positionChange = await _context.PositionChanges.FindAsync(id);
            if (positionChange != null)
            {
                _context.PositionChanges.Remove(positionChange);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionChangeExists(int id)
        {
            return _context.PositionChanges.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var positionChange = await _context.PositionChanges.FindAsync(id);
            if (positionChange == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.PositionChanges.Remove(positionChange);
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
            var positionChange = _context.PositionChanges.Find(id);
            if (positionChange == null)
            {
                return NotFound();
            }

            positionChange.Status = !(positionChange.Status ?? false); // Nếu null, mặc định là false rồi đảo trạng thái
            _context.SaveChanges();

            return Json(positionChange.Status);
        }
    }
}
