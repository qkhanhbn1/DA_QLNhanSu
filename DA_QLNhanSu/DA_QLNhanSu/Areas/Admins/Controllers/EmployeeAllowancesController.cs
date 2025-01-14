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
    public class EmployeeAllowancesController : Controller
    {
        private readonly DaQlNhanvienContext _context;

        public EmployeeAllowancesController(DaQlNhanvienContext context)
        {
            _context = context;
        }

        // GET: Admins/EmployeeAllowances
        public async Task<IActionResult> Index()
        {
            var daQlNhanvienContext = _context.EmployeeAllowances.Include(e => e.IdAllowancesNavigation).Include(e => e.IdeNavigation);
            return View(await daQlNhanvienContext.ToListAsync());
        }

        // GET: Admins/EmployeeAllowances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAllowance = await _context.EmployeeAllowances
                .Include(e => e.IdAllowancesNavigation)
                .Include(e => e.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeAllowance == null)
            {
                return NotFound();
            }

            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Create
        public IActionResult Create()
        {
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Ida");
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide");
            return View();
        }

        // POST: Admins/EmployeeAllowances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ide,IdAllowances,Money")] EmployeeAllowance employeeAllowance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeAllowance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Ida", employeeAllowance.IdAllowances);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", employeeAllowance.Ide);
            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAllowance = await _context.EmployeeAllowances.FindAsync(id);
            if (employeeAllowance == null)
            {
                return NotFound();
            }
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Ida", employeeAllowance.IdAllowances);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", employeeAllowance.Ide);
            return View(employeeAllowance);
        }

        // POST: Admins/EmployeeAllowances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ide,IdAllowances,Money")] EmployeeAllowance employeeAllowance)
        {
            if (id != employeeAllowance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeAllowance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeAllowanceExists(employeeAllowance.Id))
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
            ViewData["IdAllowances"] = new SelectList(_context.Allowances, "Ida", "Ida", employeeAllowance.IdAllowances);
            ViewData["Ide"] = new SelectList(_context.Employees, "Ide", "Ide", employeeAllowance.Ide);
            return View(employeeAllowance);
        }

        // GET: Admins/EmployeeAllowances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeAllowance = await _context.EmployeeAllowances
                .Include(e => e.IdAllowancesNavigation)
                .Include(e => e.IdeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeAllowance == null)
            {
                return NotFound();
            }

            return View(employeeAllowance);
        }

        // POST: Admins/EmployeeAllowances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeAllowance = await _context.EmployeeAllowances.FindAsync(id);
            if (employeeAllowance != null)
            {
                _context.EmployeeAllowances.Remove(employeeAllowance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeAllowanceExists(int id)
        {
            return _context.EmployeeAllowances.Any(e => e.Id == id);
        }
    }
}
