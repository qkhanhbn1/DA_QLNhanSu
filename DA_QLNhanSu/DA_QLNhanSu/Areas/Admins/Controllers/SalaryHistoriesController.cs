using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA_QLNhanSu.Models;

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
        public async Task<IActionResult> Index()
        {
            var daQlNhanvienContext = _context.SalaryHistories.Include(s => s.IdeNavigation);
            return View(await daQlNhanvienContext.ToListAsync());
        }

        // GET: Admins/SalaryHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryHistory = await _context.SalaryHistories
                .Include(s => s.IdeNavigation)
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide");
            return View();
        }

        // POST: Admins/SalaryHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,Salary,EffectiveDate,Note")] SalaryHistory salaryHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salaryHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryHistory.Ide);
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryHistory.Ide);
            return View(salaryHistory);
        }

        // POST: Admins/SalaryHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,Salary,EffectiveDate,Note")] SalaryHistory salaryHistory)
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryHistory.Ide);
            return View(salaryHistory);
        }

        // GET: Admins/SalaryHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryHistory = await _context.SalaryHistories
                .Include(s => s.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salaryHistory == null)
            {
                return NotFound();
            }

            return View(salaryHistory);
        }

        // POST: Admins/SalaryHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salaryHistory = await _context.SalaryHistories.FindAsync(id);
            if (salaryHistory != null)
            {
                _context.SalaryHistories.Remove(salaryHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryHistoryExists(int id)
        {
            return _context.SalaryHistories.Any(e => e.Id == id);
        }
    }
}
