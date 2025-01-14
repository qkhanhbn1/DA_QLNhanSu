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
    public class SalaryAdvancesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public SalaryAdvancesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/SalaryAdvances
        public async Task<IActionResult> Index()
        {
            var daQlNhanvienContext = _context.SalaryAdvances.Include(s => s.IdeNavigation);
            return View(await daQlNhanvienContext.ToListAsync());
        }

        // GET: Admins/SalaryAdvances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryAdvance = await _context.SalaryAdvances
                .Include(s => s.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Idsa == id);
            if (salaryAdvance == null)
            {
                return NotFound();
            }

            return View(salaryAdvance);
        }

        // GET: Admins/SalaryAdvances/Create
        public IActionResult Create()
        {
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide");
            return View();
        }

        // POST: Admins/SalaryAdvances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idsa,Ide,Money,Status,Date")] SalaryAdvance salaryAdvance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salaryAdvance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryAdvance.Ide);
            return View(salaryAdvance);
        }

        // GET: Admins/SalaryAdvances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryAdvance = await _context.SalaryAdvances.FindAsync(id);
            if (salaryAdvance == null)
            {
                return NotFound();
            }
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryAdvance.Ide);
            return View(salaryAdvance);
        }

        // POST: Admins/SalaryAdvances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idsa,Ide,Money,Status,Date")] SalaryAdvance salaryAdvance)
        {
            if (id != salaryAdvance.Idsa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salaryAdvance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryAdvanceExists(salaryAdvance.Idsa))
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
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", salaryAdvance.Ide);
            return View(salaryAdvance);
        }

        // GET: Admins/SalaryAdvances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryAdvance = await _context.SalaryAdvances
                .Include(s => s.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Idsa == id);
            if (salaryAdvance == null)
            {
                return NotFound();
            }

            return View(salaryAdvance);
        }

        // POST: Admins/SalaryAdvances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salaryAdvance = await _context.SalaryAdvances.FindAsync(id);
            if (salaryAdvance != null)
            {
                _context.SalaryAdvances.Remove(salaryAdvance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryAdvanceExists(int id)
        {
            return _context.SalaryAdvances.Any(e => e.Idsa == id);
        }
    }
}
