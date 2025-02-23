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
    public class CareerDevelopmentsController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public CareerDevelopmentsController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/CareerDevelopments
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 15; // Số bản ghi trên mỗi trang

            var query = _context.CareerDevelopments
                .Include(e => e.IdeNavigation)
                .ThenInclude(e => e.IdpNavigation)
                .OrderBy(c => c.Id); // Sắp xếp theo Id để đảm bảo thứ tự trong DB

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Id);
            }

            var careerdevelopment = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(careerdevelopment);
        }

        // GET: Admins/CareerDevelopments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careerDevelopment = await _context.CareerDevelopments
                .Include(c => c.IdeNavigation)
                .ThenInclude(e => e.IddNavigation)
                .Include(c => c.IdeNavigation)
                .ThenInclude(e => e.IdqNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (careerDevelopment == null)
            {
                return NotFound();
            }

            return View(careerDevelopment);
        }

        // GET: Admins/CareerDevelopments/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name");
            return View();
        }

        // POST: Admins/CareerDevelopments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,Type,Fromrole,Torole,Status,Date")] CareerDevelopment careerDevelopment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(careerDevelopment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", careerDevelopment.Ide);
            return View(careerDevelopment);
        }

        // GET: Admins/CareerDevelopments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careerDevelopment = await _context.CareerDevelopments
                .Include(cd => cd.IdeNavigation) // Load dữ liệu ảnh
                .FirstOrDefaultAsync(cd => cd.Id == id);

            if (careerDevelopment == null)
            {
                return NotFound();
            }

            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", careerDevelopment.Ide);
            return View(careerDevelopment);
        }


        // POST: Admins/CareerDevelopments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,Type,Fromrole,Torole,Status,Date")] CareerDevelopment careerDevelopment)
        {
            if (id != careerDevelopment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(careerDevelopment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CareerDevelopmentExists(careerDevelopment.Id))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Name", careerDevelopment.Ide);
            return View(careerDevelopment);
        }

        // GET: Admins/CareerDevelopments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careerDevelopment = await _context.CareerDevelopments
                .Include(c => c.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (careerDevelopment == null)
            {
                return NotFound();
            }

            return View(careerDevelopment);
        }

        // POST: Admins/CareerDevelopments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var careerDevelopment = await _context.CareerDevelopments.FindAsync(id);
            if (careerDevelopment != null)
            {
                _context.CareerDevelopments.Remove(careerDevelopment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CareerDevelopmentExists(int id)
        {
            return _context.CareerDevelopments.Any(e => e.Id == id);
        }
        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            var careerdevelopment = _context.CareerDevelopments.Find(id);
            if (careerdevelopment == null)
            {
                return NotFound();
            }

            careerdevelopment.Status = !(careerdevelopment.Status ?? false); // Nếu null, mặc định là false rồi đảo trạng thái
            _context.SaveChanges();

            return Json(careerdevelopment.Status);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var careerdevelopment = await _context.CareerDevelopments.FindAsync(id);
            if (careerdevelopment == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy
            }

            try
            {
                _context.CareerDevelopments.Remove(careerdevelopment);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công!" }); // Trả về JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }
    }
}
