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
    public class OvertimesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public OvertimesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/Overtimes
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            int limit = 5; // Số bản ghi trên mỗi trang

            var query = _context.Overtimes
                .Include(e => e.IdeNavigation)
                // Include Qualification
                .OrderBy(c => c.Ido);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.IdeNavigation.Name.Contains(name)).OrderBy(c => c.Ido);
            }

            var overtime = await query.ToPagedListAsync(page, limit);

            ViewBag.keyword = name;
            return View(overtime);
        }

        // GET: Admins/Overtimes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtime = await _context.Overtimes
                .Include(o => o.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Ido == id);
            if (overtime == null)
            {
                return NotFound();
            }

            return View(overtime);
        }

        // GET: Admins/Overtimes/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide");
            return View();
        }

        // POST: Admins/Overtimes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ido,Ide,WorkingHours,SalaryCoeficient,HourlyWage,OvertimePay,Date")] Overtime overtime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(overtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", overtime.Ide);
            return View(overtime);
        }

        // GET: Admins/Overtimes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtime = await _context.Overtimes.FindAsync(id);
            if (overtime == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", overtime.Ide);
            return View(overtime);
        }

        // POST: Admins/Overtimes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ido,Ide,WorkingHours,SalaryCoeficient,HourlyWage,OvertimePay,Date")] Overtime overtime)
        {
            if (id != overtime.Ido)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(overtime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OvertimeExists(overtime.Ido))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", overtime.Ide);
            return View(overtime);
        }

        // GET: Admins/Overtimes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtime = await _context.Overtimes
                .Include(o => o.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Ido == id);
            if (overtime == null)
            {
                return NotFound();
            }

            return View(overtime);
        }

        // POST: Admins/Overtimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var overtime = await _context.Overtimes.FindAsync(id);
            if (overtime != null)
            {
                _context.Overtimes.Remove(overtime);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OvertimeExists(int id)
        {
            return _context.Overtimes.Any(e => e.Ido == id);
        }
    }
}
