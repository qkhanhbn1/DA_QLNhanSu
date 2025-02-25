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
    public class DisciplinesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public DisciplinesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/Disciplines
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 10; // Số bản ghi trên mỗi trang

            var query = _context.Disciplines
                .Include(e => e.IdeNavigation)  // Include Department
                                                // Include Qualification
                .OrderBy(c => c.Id);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var discipline = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(discipline);
        }

        // GET: Admins/Disciplines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _context.Disciplines
                .Include(d => d.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discipline == null)
            {
                return NotFound();
            }

            return View(discipline);
        }

        // GET: Admins/Disciplines/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/Disciplines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,DisciplineDate,Content,Punishment,Status")] Discipline discipline)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discipline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", discipline.Ide);
            return View(discipline);
        }

        // GET: Admins/Disciplines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discipline = await _context.Disciplines
                .Include(d => d.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discipline == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", discipline.Ide);
            return View(discipline);
        }

        // POST: Admins/Disciplines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,DisciplineDate,Content,Punishment,Status")] Discipline discipline)
        {
            if (id != discipline.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discipline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisciplineExists(discipline.Id))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", discipline.Ide);
            return View(discipline);
        }

        // GET: Admins/Disciplines/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var discipline = await _context.Disciplines.FindAsync(id);
            if (discipline == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.Disciplines.Remove(discipline);
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
            var discipline = _context.Disciplines.Find(id);
            if (discipline == null)
            {
                return NotFound();
            }

            discipline.Status = !(discipline.Status ?? false); // Nếu null, mặc định là false rồi đảo trạng thái
            _context.SaveChanges();

            return Json(discipline.Status);
        }

        private bool DisciplineExists(int id)
        {
            return _context.Disciplines.Any(e => e.Id == id);
        }
    }
}
